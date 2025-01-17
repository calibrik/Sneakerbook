using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RunningClub.Models;

public enum RaceCategory
{
    Marathon,
    Ultra,
    FiveK,
    TenK,
    HalfMarathon
}

public class Race
{
    [Key]
    public int Id { get; set; }
    public required string Title { get; set; }
    public string? Description { get; set; }
    public required string Image { get; set; }
    [ForeignKey("Address")]
    public int AddressId { get; set; }
    public required Address Address { get; set; }
    [ForeignKey("AppUser")]
    public string? AppUserId { get; set; }
    public AppUser? AppUser { get; set; }
    [ForeignKey("Club")]
    public int? ClubId { get; set; }
    public required RaceCategory Category { get; set; }
}