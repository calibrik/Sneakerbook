using System.Reflection;
using Microsoft.AspNetCore.Identity;

namespace RunningClub.Misc;

public class Seed
{
    public static async Task Initialize(IServiceProvider serviceProvider)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        string[] roleNames = typeof(UserRoles).GetFields(BindingFlags.Public | BindingFlags.Static).Select(x => x.GetValue(null).ToString()).ToArray();

        foreach (var roleName in roleNames)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }
    }
}