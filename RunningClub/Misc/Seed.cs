using System.Reflection;
using Microsoft.AspNetCore.Identity;
using RunningClub.Models;

namespace RunningClub.Misc;

public class Seed
{
    public static async Task InitializeRoles(RoleManager<IdentityRole> roleManager)
    {
        string[] roleNames = typeof(UserRoles).GetFields(BindingFlags.Public | BindingFlags.Static).Select(x => x.GetValue(null).ToString()).ToArray();

        foreach (string roleName in roleNames)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }
    }

    public static async Task InitializeUsers(RoleManager<IdentityRole> roleManager,
        UserManager<AppUser> userManager)
    {
        string adminEmail = "test@gmail.com";
        string userEmail = "johndoe@gmail.com";
        string password = "12345Aa!";
        AppUser? admin= await userManager.FindByEmailAsync(adminEmail);
        if (admin == null)
        {
            admin = new AppUser
            {
                UserName = "calibrik",
                Email = adminEmail,
                EmailConfirmed = true,
                FName = "Matt",
                LName = "Maslov"
            };
            IdentityResult res= await userManager.CreateAsync(admin, password);
            if (res.Succeeded)
            {
                res = await userManager.AddToRoleAsync(admin, UserRoles.Admin);
            }
        }
        AppUser? user=await userManager.FindByEmailAsync(userEmail);
        if (user == null)
        {
            user = new AppUser
            {
                UserName = "johndoe",
                Email = userEmail,
                EmailConfirmed = true,
                FName = "John",
                LName = "Doe"
            };
            IdentityResult res= await userManager.CreateAsync(user, password);
            if (res.Succeeded)
            {
                res = await userManager.AddToRoleAsync(user, UserRoles.User);
            }
        }
    }
}