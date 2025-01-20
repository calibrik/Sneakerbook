using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace RunningClub.Models;

public class AppUser: IdentityUser
{
    public required string FName { get; set; }
    public required string LName { get; set; }
    public int? AddressId { get; set; }
    public Address? Address { get; set; }
    public float Mileage { get; set; }
    public ICollection<Club>? Clubs { get; set; }
    public ICollection<Race>? Races { get; set; }
}