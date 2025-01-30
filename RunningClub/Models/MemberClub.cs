namespace RunningClub.Models;

public class MemberClub
{
    public required string MemberId { get; set; }
    public AppUser? Member { get; set; }
    public required int ClubId { get; set; }
    public Club? Club { get; set; }
}