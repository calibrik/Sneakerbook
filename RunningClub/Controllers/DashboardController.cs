using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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
        DashboardViewModel model=new DashboardViewModel
        {
            Id = userId
        };
        return View(model);
    }

    public async Task<IActionResult> Edit()
    {
        if (!User.Identity.IsAuthenticated)
            return RedirectToAction("Login", "Account");
        AppUser? user = await _userManager.GetUserAsync(User);
        EditProfileViewModel model = new EditProfileViewModel(user);
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(EditProfileViewModel model)
    {
        if (!User.Identity.IsAuthenticated)
            return RedirectToAction("Login", "Account");
        if (!ModelState.IsValid)
            return View(model);
        AppUser? user = await _userManager.GetUserAsync(User);
        AppUser? checkUser = await _userManager.FindByEmailAsync(model.Email);
        if (checkUser != null && checkUser.Id != user.Id)
        {
            ModelState.AddModelError("Email", "Email address already exists");
        }
        checkUser =  await _userManager.FindByNameAsync(model.UserName);
        if (checkUser != null && checkUser.Id != user.Id)
        {
            ModelState.AddModelError("UserName", "Username already exists");
        }
        if (!ModelState.IsValid)
            return View(model);
        user.FName = model.FName;
        user.LName = model.LName;
        user.Email = model.Email;
        user.UserName = model.UserName;
        await _userManager.UpdateAsync(user);
        return RedirectToAction("Index", "Dashboard",new {userId=user.Id});
    }

    public IActionResult ChangePassword()
    {
        if (!User.Identity.IsAuthenticated)
            return RedirectToAction("Login", "Account");
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
    {
        if (!User.Identity.IsAuthenticated)
            return RedirectToAction("Login", "Account");
        if (!ModelState.IsValid)
            return View(model);
        if (model.NewPassword != model.ConfirmNewPassword)
        {
            ModelState.AddModelError("ConfirmNewPassword", "Does not match with new password");
            return View(model);
        }

        if (model.NewPassword == model.OldPassword)
        {
            ModelState.AddModelError("NewPassword", "New password is same as old password");
            return View(model);
        }
        AppUser? user = await _userManager.GetUserAsync(User);
        IdentityResult result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
        if (!result.Succeeded)
        {    
            if (result.Errors.First().Code.Contains("Password"))
                ModelState.AddModelError("ChangePasswordAttempt", result.Errors.First().Description);
            else
                ModelState.AddModelError("ChangePasswordAttempt", "Unknown error occured.");
            return View(model);
        }
        return RedirectToAction("Index", "Dashboard", new {userId=user.Id});
    }
    public async Task<IActionResult> MyClubs(string userId)
    {
        DashboardViewModel model = new DashboardViewModel()
        {
            Id = userId
        };
        return View(model);
    }

    public async Task<IActionResult> ManageClubs(string userId)
    {
        DashboardViewModel model = new DashboardViewModel()
        {
            Id = userId
        };
        return View(model);
    }
    public async Task<IActionResult> ManageRaces(string userId)
    {
        DashboardViewModel model = new DashboardViewModel()
        {
            Id = userId
        };
        return View(model);
    }

    public async Task<IActionResult> CompletedRaces(string userId)
    {
        DashboardViewModel model = new DashboardViewModel()
        {
            Id = userId
        };
        return View(model);
    }
}