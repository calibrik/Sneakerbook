using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RunningClub.Models;

public enum RaceCategory
{
    Sprint,
    Marathon,
    HalfMarathon,
    Ultramarathon
}

public class Race
{
    [Key]
    public int Id { get; set; }
    public required string Title { get; set; }
    public string? Description { get; set; }
    public required string Image { get; set; }
    public required string ImagePublicId { get; set; }
    public required Address Address { get; set; }
    public required string AdminId { get; set; }
    public required DateTime StartDate { get; set; }
    //TODO Move conversion local time to frontend
    //TODO Skeleton loading of races and clubs lists and details
    public required double Length { get; set; }
    public bool IsCompleted { get; set; } = false;
    public AppUser? Admin { get; set; }
    public List<MemberRace> Members { get; set; }=new List<MemberRace>();
    public required int MaxMembersNumber { get; set; }
    public required int ClubId { get; set; }
    public Club? Club { get; set; }
    public required RaceCategory Category { get; set; }
}