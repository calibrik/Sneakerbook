using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace RunningClub.Models;

public class AppUser : IdentityUser
{
    public required string FName { get; set; }
    public required string LName { get; set; }
    public double Mileage { get; set; }
    public List<MemberClub> Clubs { get; set; } = new();
    public List<MemberRace> Races { get; set; } = new();

    public List<Race> AdminRaces { get; set; } = new();
    public List<Club> AdminClubs { get; set; } = new();
}