using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace RunningClub.Models;

public class AppUser
{
    [Key]
    public int Id { get; set; }
    public Address? Address { get; set; }
    public float Mileage { get; set; }
    public ICollection<Club>? Clubs { get; set; }
    public ICollection<Race>? Races { get; set; }
}