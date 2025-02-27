using RunningClub.Models;

namespace RunningClub.ViewModels.DashboardApi;

public class DashboardApiClubViewModel
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string Image { get; set; }
    public string Category { get; set; }
    public string AdminLink { get; set; }
    public string AdminUsername { get; set; }
    public string ClubLink { get; set; }

    public DashboardApiClubViewModel(Club club)
    {
        Id = club.Id;
        Title = club.Title;
        Description = club.Description;
        Image = club.Image;
        Category = club.Category.ToString();
        AdminUsername=club.Admin.UserName;
    }
}