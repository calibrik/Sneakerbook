using System.Reflection;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RunningClub.Models;
using RunningClub.Repository;

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

    public static async Task InitializeClubs(UserManager<AppUser> userManager,ClubRepository clubRepository,int clubAmount)
    {
        string adminEmail = "test@gmail.com";
        AppUser? admin = await userManager.FindByEmailAsync(adminEmail);
        if (admin == null)
            return;
        if (await clubRepository.IsAnythingInTable())
            return;
        List<Club> clubsToAdd = new List<Club>();
        for (int i = 1; i <= clubAmount; i++)
        {
            clubsToAdd.Add(
                new Club()
                {
                    Title = $"Club {i}",
                    Description = $"Cool club {i}",
                    Image = "/uploads/defaultImage.jpg",
                    Address = new Address()
                    {
                        City = "Sydney",
                        Country = "Australia",
                        Street = "2 Somewhere Av.",
                    },
                    AdminId = admin.Id,
                    Category = (ClubCategory)((i + 1) % 5),
                });
        };
        await clubRepository.AddManyClubs(clubsToAdd);
        foreach (Club club in clubsToAdd)
        {
            await clubRepository.AddUserToClubAsync(admin.Id, club.Id);
        }
    }

    public async static Task InitializeRaces(UserManager<AppUser> userManager, RaceRepository racesRepository,ClubRepository clubRepository,int raceAmount)
    {
        string adminEmail = "test@gmail.com";
        AppUser? admin = await userManager.FindByEmailAsync(adminEmail);
        if (admin == null)
            return;
        if (await racesRepository.IsAnythingInTable())
            return;
        List<Club> clubs = await clubRepository.GetClubsAsyncRO();
        List<Race> racesToAdd = new List<Race>();
        for (int i = 1; i <= raceAmount; i++)
        {
            RaceCategory raceCategory = (RaceCategory)((i + 1) % 4);
            double length;
            switch (raceCategory)
            {
                case RaceCategory.Sprint:
                {
                    length=Math.Round(Random.Shared.NextDouble()*4.9+0.1,1);
                    break;
                }
                case RaceCategory.Marathon:
                {
                    length=Math.Round(Random.Shared.NextDouble()*20.9+21.1,1);
                    break;
                }
                case RaceCategory.HalfMarathon:
                {
                    length=Math.Round(Random.Shared.NextDouble()*15.9+5.1,1);
                    break;
                }
                case RaceCategory.Ultramarathon:
                {
                    length=Math.Round(Random.Shared.NextDouble()*37.9+42.1,1);
                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException();
            }
            racesToAdd.Add(
                new Race()
                {
                    Title = $"Race {i}",
                    Description = $"Cool race {i}",
                    Image = "/uploads/defaultImage.jpg",
                    Address = new Address()
                    {
                        City = "Sydney",
                        Country = "Australia",
                        Street = "2 Somewhere Av.",
                    },
                    AdminId = admin.Id,
                    Category = raceCategory,
                    Length = length,
                    ClubId = clubs[(i-1)%clubs.Count].Id,
                    MaxMembersNumber = 10,
                    StartDate = DateTime.UtcNow.AddHours(Random.Shared.Next(1,15)),
                    
                });
        }
        await racesRepository.AddManyRacesAsync(racesToAdd);
        foreach (Race race in racesToAdd)
        {
            await racesRepository.AddUserToRaceAsync(admin.Id, race.Id);
        }
    }
}