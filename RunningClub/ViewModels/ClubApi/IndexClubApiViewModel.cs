using RunningClub.Models;

namespace RunningClub.ViewModels.ClubApi;


public class IndexClubApiViewModel
{
    public class IndexClubModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Category { get; set; }
        public string Image { get; set; }
        public int MemberCount { get; set; }
        public bool IsJoined { get; set; }
        public string Link { get; set; }
        public string AdminLink { get; set; }
        public string AdminUsername { get; set; }

        public IndexClubModel(Club club)
        {
            Id = club.Id;
            Title = club.Title;
            Category = club.Category.ToString();
            Image = club.Image;
        }
    }

    public List<IndexClubModel> Clubs { get; set; } = new();
}