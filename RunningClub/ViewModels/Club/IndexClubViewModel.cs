using RunningClub.Models;

namespace RunningClub.ViewModels;

public class IndexClubViewModel
{
    public class IndexClubModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public ClubCategory Category { get; set; }
        public AppUser Admin { get; set; }
        public string Image { get; set; }
        public int MemberCount { get; set; }

        public IndexClubModel(Club club)
        {
            Id = club.Id;
            Title = club.Title;
            Category = club.Category;
            Admin = club.Admin;
            Image = club.Image;
        }
    }

    public List<IndexClubModel> Clubs { get; set; } = new();
    public HashSet<int> JoinedClubs { get; set; } = new();
}