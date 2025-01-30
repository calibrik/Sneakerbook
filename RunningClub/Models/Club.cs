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
    public required Address Address { get; set; }
    public required string AdminId { get; set; }
    public AppUser? Admin { get; set; }
    public List<MemberClub> Members { get; set; }=new();
    public required ClubCategory Category { get; set; }
}