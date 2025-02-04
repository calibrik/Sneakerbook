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
    public required Address Address { get; set; }
    public required string AdminId { get; set; }
    public AppUser? Admin { get; set; }
    public List<MemberRace> Members { get; set; }=new List<MemberRace>();
    public required int MaxMembersNumber { get; set; }
    public required int ClubId { get; set; }
    public Club? Club { get; set; }
    public required RaceCategory Category { get; set; }
}