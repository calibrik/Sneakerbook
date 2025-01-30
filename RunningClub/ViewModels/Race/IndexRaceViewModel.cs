using RunningClub.Models;

namespace RunningClub.ViewModels;

public class IndexRaceViewModel
{
    public List<Race> Races { get; set; }
    public HashSet<int> JoinedRaces { get; set; } = new();
}