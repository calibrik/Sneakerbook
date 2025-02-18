using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RunningClub.Misc;
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
        SignInResult result = await _signInManager.PasswordSignInAsync(user, loginViewModel.Password, loginViewModel.RememberMe, false);//TODO remember me doesn't work (it always remembers user and for unknown time amount)
        if (result.Succeeded)
            return RedirectToAction("Index", "Dashboard",new {userId = user.Id});
        ModelState.AddModelError("LoginAttempt", "Unknown error occured.");
        return View(loginViewModel);
    }

    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Register(RegisterViewModel registerViewModel)
    {
        if (!ModelState.IsValid)
            return View(registerViewModel);
        if (registerViewModel.Password != registerViewModel.ConfirmPassword)
        {
            ModelState.AddModelError("ConfirmPassword", "Passwords don't match");
            return View(registerViewModel);
        }
        AppUser? userExisting = await _userManager.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Email == registerViewModel.Email);
        if (userExisting != null)
        {
            ModelState.AddModelError("Email", "Email already exists");
            return View(registerViewModel);
        }
        userExisting = await _userManager.Users.AsNoTracking().FirstOrDefaultAsync(u=>u.UserName == registerViewModel.UserName);
        if (userExisting != null)
        {
            ModelState.AddModelError("Username", "Username already exists");
            return View(registerViewModel);
        }

        AppUser newUser = new AppUser
        {
            Email = registerViewModel.Email,
            UserName = registerViewModel.UserName,
            FName = registerViewModel.FName,
            LName = registerViewModel.LName
        };
        IdentityResult result = await _userManager.CreateAsync(newUser, registerViewModel.Password);
        if (!result.Succeeded)
        {    
            if (result.Errors.First().Code.Contains("Password"))
                ModelState.AddModelError("Password", result.Errors.First().Description);
            else
                ModelState.AddModelError("RegisterAttempt", "Unknown error occured.");
            return View(registerViewModel);
        }
        await _userManager.AddToRoleAsync(newUser, UserRoles.User);
        return RedirectToAction("Login", "Account");
    }
    
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }
}