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
    public async Task<IActionResult> Index(string userId)
    {
        AppUser? user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return RedirectToAction("Index", "Home");
        DashboardViewModel model=new DashboardViewModel(user)
        {
            Races = await _dashboardRepository.GetUserRacesAsyncRO(userId)
        };
        return View(model);
    }

    public async Task<IActionResult> MyClubs(string userId)
    {
        AppUser? user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return RedirectToAction("Index", "Home");
        DashboardViewModel model = new DashboardViewModel(user)
        {
            Clubs = await _dashboardRepository.GetUserClubsAsyncRO(userId)
        };
        return View(model);
    }

    public async Task<IActionResult> ManageClubs(string userId)
    {
        AppUser? user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return RedirectToAction("Index", "Home");
        DashboardViewModel model = new DashboardViewModel(user)
        {
            AdminClubs = await _dashboardRepository.GetUserAdminClubsAsyncRO(userId)
        };
        return View(model);
    }
    public async Task<IActionResult> ManageRaces(string userId)
    {
        AppUser? user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return RedirectToAction("Index", "Home");
        DashboardViewModel model = new DashboardViewModel(user)
        {
            AdminRaces = await _dashboardRepository.GetUserAdminRacesAsyncRO(userId)
        };
        return View(model);
    }
}