using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RunningClub.Models;
using RunningClub.ViewModels;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace RunningClub.Controllers;

public class AccountController:Controller
{
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly AppDbContext _db;

    public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, AppDbContext db)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _db = db;
    }
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel loginViewModel)
    {
        if (!ModelState.IsValid)
            return View(loginViewModel);
        AppUser? user = await _userManager.FindByEmailAsync(loginViewModel.Email);
        if (user == null)
        {
            ModelState.AddModelError("LoginAttempt", "Invalid email or password");
            return View(loginViewModel);
        }
        bool passwordCheck = await _userManager.CheckPasswordAsync(user, loginViewModel.Password);
        if (!passwordCheck)
        {
            ModelState.AddModelError("LoginAttempt", "Invalid email or password");
            return View(loginViewModel);
        }
        SignInResult result = await _signInManager.PasswordSignInAsync(user, loginViewModel.Password, loginViewModel.RememberMe, false);
        if (result.Succeeded)
            return RedirectToAction("Index", "Club");
        ModelState.AddModelError("LoginAttempt", "Unknown error occured.");
        return View(loginViewModel);
    }

    public IActionResult Register()
    {
        return View();
    }
}