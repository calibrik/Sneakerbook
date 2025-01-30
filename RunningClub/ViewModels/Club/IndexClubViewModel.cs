using RunningClub.Models;

namespace RunningClub.ViewModels;

public class IndexClubViewModel
{
    public List<Club> Clubs { get; set; }
    public HashSet<int> JoinedClubs { get; set; } = new();
}