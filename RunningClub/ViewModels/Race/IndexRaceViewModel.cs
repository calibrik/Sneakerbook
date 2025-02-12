using RunningClub.Models;

namespace RunningClub.ViewModels;

public class IndexRaceViewModel
{
    public class IndexRaceModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public RaceCategory Category { get; set; }
        public Club Club { get; set; }
        public string Image { get; set; }
        public string? Description { get; set; }
        public int MemberCount { get; set; }
        public int MaxMemberCount { get; set; }

        public IndexRaceModel(Race race)
        {
            Id = race.Id;
            Title = race.Title;
            Description = race.Description;
            Category = race.Category;
            Club = race.Club;
            Image = race.Image;
            MaxMemberCount = race.MaxMembersNumber;
        }
    }

    public List<IndexRaceModel> Races { get; set; } = new();
    public HashSet<int> JoinedRaces { get; set; } = new();
}