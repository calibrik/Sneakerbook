using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RunningClub.Misc;
using RunningClub.Models;
using RunningClub.Repository;
using RunningClub.Services;
using RunningClub.ViewModels;
using RunningClub.ViewModels.ClubApi;

namespace RunningClub.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
public class RaceApiController : ControllerBase
{
    private readonly RaceRepository _raceRepo;
    private readonly ClubRepository _clubRepo;
    private readonly UserManager<AppUser> _userManager;
    private readonly PhotoService _photoService;

    public RaceApiController(RaceRepository raceRepo, ClubRepository clubRepo, UserManager<AppUser> userManager, PhotoService photoService)
    {
        _clubRepo = clubRepo;
        _raceRepo = raceRepo;
        _userManager = userManager;
        _photoService = photoService;
    }

    [HttpPost("")]
    public async Task<IActionResult> Delete([FromBody] int raceId)
    {
        if (!User.Identity.IsAuthenticated || !await _raceRepo.IsUserAdminInRaceAsync(User.GetUserId(), raceId) &&
            !User.IsInRole("Admin"))
            return Unauthorized(new { message = "You do not have permission for this action" });
        
        Race? race = await _raceRepo.GetRaceByIdAsync(raceId);
        if (race == null)
            return NotFound(new { message = "Race is not found" });
        
        if (race.IsCompleted)
            return BadRequest(new { message = "Race is already completed" });
        
        await _photoService.DeletePhotoFromCloudinaryAsync(race.ImagePublicId);
        await _raceRepo.DeleteRace(race);
        return Ok(new { message = "Race successfully deleted"});
    }
    [HttpPost("")]
    public async Task<IActionResult> Join([FromBody] int raceId)
    {
        if (!User.Identity.IsAuthenticated)
            return Unauthorized(new { message = "You do not have permission for this action" });
        Race? race = await _raceRepo.GetRaceByIdAsyncRO(raceId);
        if (race == null)
            return NotFound(new { message = "Race is not found" });
        
        if (race.IsCompleted)
            return BadRequest(new { message = "Race is already completed" });
        
        if (await _raceRepo.GetRaceMemberCountAsyncRO(raceId) >= race.MaxMembersNumber)
            return BadRequest(new { message = "Race is full" });
        
        if (!await _clubRepo.IsUserMemberInClubAsync(User.GetUserId(), race.ClubId))
            return Unauthorized(new { message = $"You need to be a member of club \"{race.Club.Title}\" to be able to join this race" });
        
        if (await _raceRepo.IsUserMemberInRaceAsync(User.GetUserId(), raceId))
            return BadRequest(new { message = "You are already in this race" });
        
        await _raceRepo.AddUserToRaceAsync(User.GetUserId(), raceId);
        AppUser? user=await _userManager.GetUserAsync(User);
        JoinApiViewModel model = new JoinApiViewModel()
        {
            Id = user.Id,
            UserName = user.UserName,
            LinkToDashboard = Url.Action("Index", "Dashboard", new { userId = User.GetUserId() }),
        };
        return Ok(new { message = "You successfully joined the race", model, memberCount=await _raceRepo.GetRaceMemberCountAsyncRO(raceId) });
    }
    [HttpPost("")]
    public async Task<IActionResult> Leave([FromBody] int raceId)
    {
        if (!User.Identity.IsAuthenticated)
            return Unauthorized(new { message = "You do not have permission for this action" });
        
        Race? race = await _raceRepo.GetRaceByIdAsyncRO(raceId);
        if (race == null)
            return NotFound(new { message = "Race is not found" });
        
        if (race.IsCompleted)
            return BadRequest(new { message = "Race is already completed" });
        
        if (!await _raceRepo.IsUserMemberInRaceAsync(User.GetUserId(), raceId))
            return BadRequest(new { message = "You are not in this race" });
        
        await _raceRepo.RemoveUserFromRaceAsync(User.GetUserId(), raceId);
        return Ok(new { message = "You successfully left the race", memberCount=await _raceRepo.GetRaceMemberCountAsyncRO(raceId) });
    }
    [HttpPost("")]
    public async Task<IActionResult> KickMember([FromBody] KickRaceMemberViewModel model)
    {
        if (!User.Identity.IsAuthenticated || !await _raceRepo.IsUserAdminInRaceAsync(User.GetUserId(), model.RaceId) ||
            !User.IsInRole("Admin"))
            return Unauthorized(new { message = "You do not have permission for this action" });
        
        Race? race = await _raceRepo.GetRaceByIdAsyncRO(model.RaceId);
        if (race == null)
            return NotFound(new { message = "Race is not found" });
        
        if (race.IsCompleted)
            return BadRequest(new { message = "Race is already completed" });
        
        if (!await _raceRepo.IsUserMemberInRaceAsync(model.UserId, model.RaceId))
            return NotFound(new { message = "User is not found in this club" });
        
        await _raceRepo.RemoveUserFromRaceAsync(model.UserId, model.RaceId);
        return Ok(new { message = "User successfully kicked", memberCount=await _raceRepo.GetRaceMemberCountAsyncRO(model.RaceId) });
    }

    [HttpGet("")]
    public async Task<IActionResult> GetRaces()
    {
        if (!User.Identity.IsAuthenticated)
            return Unauthorized(new { message = "You do not have permission for this action" });
        HashSet<int> joinedClubs=await _clubRepo.GetUserClubsIdsAsyncRO(User.GetUserId());
        List<Race> races=await _raceRepo.GetClubsUpcomingRacesAsyncRO(joinedClubs);
        HashSet<int> joinedRaces=await _raceRepo.GetUserUpcomingRacesIdsAsyncRO(User.GetUserId());
        List<RaceApiViewModel> model = new List<RaceApiViewModel>();
        foreach (Race race in races)
        {
            if (joinedRaces.Contains(race.Id))
                continue;
            model.Add(new RaceApiViewModel(race)
            {
                ClubLink = Url.Action("Detail","Club", new { clubId = race.ClubId }),
                MemberCount = await _raceRepo.GetRaceMemberCountAsyncRO(race.Id),
                RaceLink = Url.Action("Detail", "Race", new { raceId = race.Id }),
            });
        }
        return Ok(model);
    }

    [HttpGet("")]
    public async Task<IActionResult> GetRace([FromQuery] int raceId)
    {
        Race? race = await _raceRepo.GetRaceByIdAsyncRO(raceId);
        if (race == null)
            return NotFound(new { message = "Race is not found" });
        DetailRaceApiViewModel model = new DetailRaceApiViewModel(race)
        {
            ClubLink = Url.Action("Detail", "Club", new { clubId = race.ClubId }),
            ClubTitle = race.Club.Title,
            AdminLink = Url.Action("Index", "Dashboard", new { userId = race.AdminId }),
            AdminUsername = race.Admin.UserName,
        };
        List<AppUser> users=await _raceRepo.GetUsersInRaceAsyncRO(raceId);
        foreach (AppUser user in users)
        {
            model.Members.Add(new DetailRaceClubApiUser()
            {
                Id = user.Id,
                IsAdmin = user.Id == race.Admin.Id,
                Username = user.UserName,
            });
        }
        if (!User.Identity.IsAuthenticated)
            return Ok(model);
        model.IsJoined=await _raceRepo.IsUserMemberInRaceAsync(User.GetUserId(), raceId);
        model.IsAdmin =User.IsInRole(UserRoles.Admin) || User.GetUserId()==race.Admin.Id;
        return Ok(model);
    }
}