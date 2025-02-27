using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RunningClub.Misc;
using RunningClub.Models;
using RunningClub.Repository;
using RunningClub.ViewModels;
using RunningClub.ViewModels.DashboardApi;

namespace RunningClub.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
public class DashboardApiController:ControllerBase
{
    private readonly DashboardRepository _dashboardRepo;
    private readonly RaceRepository _raceRepo;
    private readonly UserManager<AppUser> _userManager;

    public DashboardApiController(DashboardRepository dashboardRepo, UserManager<AppUser> userManager, RaceRepository raceRepo)
    {
        _dashboardRepo = dashboardRepo;
        _userManager = userManager;
        _raceRepo = raceRepo;
    }

    [HttpGet("")]
    public async Task<IActionResult> GetUserInfo([FromQuery] string userId)
    {
        AppUser? user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return NotFound(new { message = "User not found." });
        DashboardApiUserViewModel model = new DashboardApiUserViewModel(user)
        {
            IsAdmin = await _userManager.IsInRoleAsync(user, "Admin"),
            IsSelf = User.Identity.IsAuthenticated && User.GetUserId() == userId,
            ChangePasswordLink = Url.Action("ChangePassword", "Dashboard"),
            EditProfileLink = Url.Action("Edit", "Dashboard"),
        };
        return Ok(model);
    }

    [HttpGet("")]
    public async Task<IActionResult> GetUsersUpcomingRaces([FromQuery] string userId)
    {
        List<Race> races = await _dashboardRepo.GetUserUpcomingRacesAsyncRO(userId);
        List<RaceApiViewModel> model = new List<RaceApiViewModel>();
        foreach (Race race in races)
        {
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
    public async Task<IActionResult> GetUsersAdminRaces([FromQuery] string userId)
    {
        List<Race> races = await _dashboardRepo.GetUserAdminRacesAsyncRO(userId);
        List<RaceApiViewModel> model = new List<RaceApiViewModel>();
        foreach (Race race in races)
        {
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
    public async Task<IActionResult> GetUsersCompletedRaces([FromQuery] string userId)
    {
        List<Race> races = await _dashboardRepo.GetUserCompletedRacesAsyncRO(userId);
        List<RaceApiViewModel> model = new List<RaceApiViewModel>();
        foreach (Race race in races)
        {
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
    public async Task<IActionResult> GetUserClubs([FromQuery] string userId)
    {
        List<Club> clubs = await _dashboardRepo.GetUserClubsAsyncRO(userId);
        List<DashboardApiClubViewModel> model = new List<DashboardApiClubViewModel>();
        foreach (Club club in clubs)
        {
            model.Add(new DashboardApiClubViewModel(club)
            {
                ClubLink = Url.Action("Detail", "Club", new { clubId = club.Id }),
                AdminLink = Url.Action("Index", "Dashboard", new { userId = club.AdminId }),
            });
        }
        return Ok(model);
    }
    
    [HttpGet("")]
    public async Task<IActionResult> GetUserAdminClubs([FromQuery] string userId)
    {
        List<Club> clubs = await _dashboardRepo.GetUserAdminClubsAsyncRO(userId);
        List<DashboardApiClubViewModel> model = new List<DashboardApiClubViewModel>();
        foreach (Club club in clubs)
        {
            model.Add(new DashboardApiClubViewModel(club)
            {
                ClubLink = Url.Action("Detail", "Club", new { clubId = club.Id }),
                AdminLink = Url.Action("Index", "Dashboard", new { userId = club.AdminId }),
            });
        }
        return Ok(model);
    }
}