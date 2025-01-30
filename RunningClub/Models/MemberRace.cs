

namespace RunningClub.Models;

public class MemberRace
{
    public required string MemberId { get; set; }
    public AppUser? Member { get; set; }
    public required int RaceId { get; set; }
    public Race? Race { get; set; }
}