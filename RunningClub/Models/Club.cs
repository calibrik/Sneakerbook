using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RunningClub.ViewModels;

namespace RunningClub.Models;

public enum ClubCategory
{
    Women,
    Men,
    Endurance,
    City,
    Trail
}

public class Club
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
    public int? AppUserId { get; set; }
    public AppUser? AppUser { get; set; }
    public required ClubCategory Category { get; set; }
}