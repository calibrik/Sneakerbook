﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RunningClub.Misc;
using RunningClub.Models;
using RunningClub.Repository;
using RunningClub.Services;
using RunningClub.ViewModels.ClubApi;

namespace RunningClub.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
public class ClubApiController : ControllerBase
{
    private readonly ClubRepository _clubRepo;
    private readonly RaceRepository _raceRepo;
    private readonly UserManager<AppUser> _userManager;
    private readonly PhotoService _photoService;

    public ClubApiController(ClubRepository clubRepo, RaceRepository raceRepo, UserManager<AppUser> userManager, PhotoService photoService)
    {
        _clubRepo = clubRepo;
        _raceRepo = raceRepo;
        _userManager = userManager;
        _photoService = photoService;
    }
    
    [HttpPost("")]
    public async Task<IActionResult> Delete([FromBody] int clubId)
    {
        if (!User.Identity.IsAuthenticated)
            return Unauthorized(new { message = "Please log in/sign up first" });
        if(!await _clubRepo.IsUserAdminInClubAsync(User.GetUserId(), clubId) && !User.IsInRole("Admin"))
            return Unauthorized(new { message = "You do not have permission for this action" });
        Club? club = await _clubRepo.GetClubByIdAsyncRO(clubId);
        if (club == null)
            return NotFound(new { message = "Club is not found" });
        List<string> raceImages = await _raceRepo.GetClubRacesImagePublicIDsAsyncRO(clubId);
        foreach (string imageId in raceImages)
        {
            await _photoService.DeletePhotoFromCloudinaryAsync(imageId);
        }
        await _photoService.DeletePhotoFromCloudinaryAsync(club.ImagePublicId);
        await _clubRepo.DeleteClub(club);
        return Ok(new { message = "Club successfully deleted" });
    }
    
    [HttpPost("")]
    public async Task<IActionResult> KickMember([FromBody] KickClubMemberViewModel model)
    {
        if (!User.Identity.IsAuthenticated)
            return Unauthorized(new { message = "Please log in/sign up first" });
        if(!await _clubRepo.IsUserAdminInClubAsync(User.GetUserId(), model.ClubId) && !User.IsInRole("Admin"))
            return Unauthorized(new { message = "You do not have permission for this action" });
        if (await _clubRepo.GetClubByIdAsyncRO(model.ClubId) == null)
            return NotFound(new { message = "Club is not found" });
        if (!await _clubRepo.IsUserMemberInClubAsync(model.UserId, model.ClubId))
            return NotFound(new { message = "User is not found in this club" });
        await _clubRepo.RemoveUserFromClubAsync(model.UserId, model.ClubId);
        AppUser? user = await _userManager.FindByIdAsync(model.UserId);
        return Ok(new { message = $"{user.UserName} successfully kicked",memberCount=await _clubRepo.GetClubMemberCountAsyncRO(model.ClubId)});
    }

    [HttpPost("")]
    public async Task<IActionResult> Join([FromBody] int clubId)
    {
        if (!User.Identity.IsAuthenticated)
            return Unauthorized(new { message = "Please log in/sign up first" });
        if (await _clubRepo.GetClubByIdAsyncRO(clubId) == null)
            return NotFound(new { message = "Club is not found" });
        if (await _clubRepo.IsUserMemberInClubAsync(User.GetUserId(), clubId))
            return BadRequest(new { message = "You are already in this club" });
        await _clubRepo.AddUserToClubAsync(User.GetUserId(), clubId);
        AppUser? user = await _userManager.GetUserAsync(User);
        JoinApiViewModel model = new JoinApiViewModel()
        {
            Id = User.GetUserId(),
            UserName = user.UserName,
            LinkToDashboard = Url.Action("Index", "Dashboard", new { userId = User.GetUserId() }),
        };
        return Ok(new { message = "You successfully joined",model,memberCount=await _clubRepo.GetClubMemberCountAsyncRO(clubId) });
    }

    [HttpPost("")]
    public async Task<IActionResult> Leave([FromBody] int clubId)
    {
        if (!User.Identity.IsAuthenticated)
            return Unauthorized(new { message = "Please log in/sign up first" });
        if (await _clubRepo.GetClubByIdAsyncRO(clubId) == null)
            return NotFound(new { message = "Club is not found" });
        if (!await _clubRepo.IsUserMemberInClubAsync(User.GetUserId(), clubId))
            return BadRequest(new { message = "You aren't in this club" });
        if (await _clubRepo.IsUserAdminInClubAsync(User.GetUserId(), clubId))
            return BadRequest(new { message = "Admin can't leave club" });
        await _clubRepo.RemoveUserFromClubAsync(User.GetUserId(), clubId);
        return Ok(new { message = "You successfully left",memberCount=await _clubRepo.GetClubMemberCountAsyncRO(clubId) });
    }

    [HttpGet("")]
    public async Task<IActionResult> GetCompletedRacesForClub([FromQuery] int clubId)
    {
        if (await _clubRepo.GetClubByIdAsyncRO(clubId) == null)
            return NotFound(new { message = "Club is not found" });
        List<Race> races=await _raceRepo.GetClubCompletedRacesAsyncRO(clubId);
        HashSet<int> joinedRaces = new();
        if (User.Identity.IsAuthenticated)
            joinedRaces=await _raceRepo.GetUserRacesIdsInClubAsyncRO(User.GetUserId(),clubId);
        List<DetailClubApiRaceViewModel> model = new List<DetailClubApiRaceViewModel>();
        foreach (Race race in races)
        {
            model.Add(new DetailClubApiRaceViewModel(race)
            {
                MemberCount = await _raceRepo.GetRaceMemberCountAsyncRO(race.Id),
                IsJoined = joinedRaces.Contains(race.Id),
                Link = Url.Action("Detail", "Race", new { raceId = race.Id }),
            });
        }
        return Ok(model);
    }
    [HttpGet("")]
    public async Task<IActionResult> GetUpcomingRacesForClub([FromQuery] int clubId)
    {
        if (await _clubRepo.GetClubByIdAsyncRO(clubId) == null)
            return NotFound(new { message = "Club is not found" });
        List<Race> races=await _raceRepo.GetClubUpcomingRacesAsyncRO(clubId);
        HashSet<int> joinedRaces = new();
        if (User.Identity.IsAuthenticated)
            joinedRaces=await _raceRepo.GetUserRacesIdsInClubAsyncRO(User.GetUserId(),clubId);
        List<DetailClubApiRaceViewModel> model = new List<DetailClubApiRaceViewModel>();
        foreach (Race race in races)
        {
            model.Add(new DetailClubApiRaceViewModel(race)
            {
                MemberCount = await _raceRepo.GetRaceMemberCountAsyncRO(race.Id),
                IsJoined = joinedRaces.Contains(race.Id),
                Link = Url.Action("Detail", "Race", new { raceId = race.Id }),
            });
        }
        return Ok(model);
    }

    [HttpGet("")]
    public async Task<IActionResult> GetClubs()
    {
        List<Club> clubs = await _clubRepo.GetClubsAsyncRO();
        HashSet<int> joinedClubs = new();
        if (User.Identity.IsAuthenticated)
            joinedClubs = await _clubRepo.GetUserClubsIdsAsyncRO(User.GetUserId());
        IndexClubApiViewModel model = new IndexClubApiViewModel();
        foreach (Club club in clubs)
        {
            model.Clubs.Add(new IndexClubApiViewModel.IndexClubModel(club)
            {
                IsJoined = joinedClubs.Contains(club.Id),
                MemberCount = await _clubRepo.GetClubMemberCountAsyncRO(club.Id),
                Link = Url.Action("Detail", "Club", new { clubId = club.Id }),
                AdminLink = Url.Action("Index", "Dashboard", new { userId = club.Admin.Id }),
                AdminUsername = club.Admin.UserName,
            });
        }
        return Ok(model);
    }

    [HttpGet("")]
    public async Task<IActionResult> GetClub([FromQuery] int clubId)
    {
        Club? club = await _clubRepo.GetClubByIdAsyncRO(clubId);
        if (club==null)
            return NotFound(new { message = "Club is not found" });
        List<AppUser> members = await _clubRepo.GetUsersInClubAsyncRO(clubId);
        DetailClubApiViewModel model = new DetailClubApiViewModel(club)
        {
            AdminUsername = club.Admin.UserName,
            AdminLink = Url.Action("Index", "Dashboard", new { userId = club.Admin.Id }),
        };
        foreach (AppUser member in members)
        {
            model.Members.Add(new DetailRaceClubApiUser()
            {
                Id = member.Id,
                IsAdmin = member.Id == club.Admin.Id,
                Link = Url.Action("Index", "Dashboard", new { userId = member.Id }),
                Username = member.UserName,
            });
        }
        if (User.Identity.IsAuthenticated)
        {
            model.IsAdmin = User.GetUserId()==club.AdminId;
            model.IsJoined = await _clubRepo.IsUserMemberInClubAsync(User.GetUserId(), clubId);
        }
        return Ok(model);
    }
}