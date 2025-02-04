using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RunningClub.Misc;
using RunningClub.Models;
using RunningClub.Repository;
using RunningClub.ViewModels;

namespace RunningClub.Controllers;

public class DashboardController:Controller
{
    private readonly DashboardRepository _dashboardRepository;
    private readonly UserManager<AppUser> _userManager;

    public DashboardController(AppDbContext db, DashboardRepository dashboardRepository, UserManager<AppUser> userManager)
    {
        _dashboardRepository = dashboardRepository;
        _userManager = userManager;
    }
    public async Task<IActionResult> Index()
    {
        if (!User.Identity.IsAuthenticated)
            return RedirectToAction("Login", "Account");
        AppUser? user = await _userManager.GetUserAsync(User);
        DashboardViewModel model=new DashboardViewModel(user)
        {
            Races = await _dashboardRepository.GetUserRacesAsyncRO(User.GetUserId())
        };
        return View(model);
    }

    public async Task<IActionResult> MyClubs()
    {
        if (!User.Identity.IsAuthenticated)
            return RedirectToAction("Login", "Account");
        AppUser? user = await _userManager.GetUserAsync(User);
        DashboardViewModel model = new DashboardViewModel(user)
        {
            Clubs = await _dashboardRepository.GetUserClubsAsyncRO(User.GetUserId())
        };
        return View(model);
    }

    public async Task<IActionResult> ManageClubs()
    {
        if (!User.Identity.IsAuthenticated)
            return RedirectToAction("Login", "Account");
        AppUser? user = await _userManager.GetUserAsync(User);
        DashboardViewModel model = new DashboardViewModel(user)
        {
            AdminClubs = await _dashboardRepository.GetUserAdminClubsAsyncRO(User.GetUserId())
        };
        return View(model);
    }
    public async Task<IActionResult> ManageRaces()
    {
        if (!User.Identity.IsAuthenticated)
            return RedirectToAction("Login", "Account");
        AppUser? user = await _userManager.GetUserAsync(User);
        DashboardViewModel model = new DashboardViewModel(user)
        {
            AdminRaces = await _dashboardRepository.GetUserAdminRacesAsyncRO(User.GetUserId())
        };
        return View(model);
    }
}