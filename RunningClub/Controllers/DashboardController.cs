using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RunningClub.Models;
using RunningClub.Repository;

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
        user.Races = await _dashboardRepository.GetAllUserRacesAsync(user.Id);
        return View(user);
    }
}