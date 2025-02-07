using RunningClub.Models;

namespace RunningClub.ViewModels;

public class DetailClubViewModel
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string Image { get; set; }
    public Address Address { get; set; }
    public AppUser Admin { get; set; }
    public ClubCategory Category { get; set; }
    public bool IsJoined { get; set; }
    public bool IsAdmin { get; set; }
    public List<Race> Races { get; set; }
    public List<AppUser> Members { get; set; }
    public HashSet<int> JoinedRaces { get; set; } = new();

    public DetailClubViewModel(Club club)
    {
        Id = club.Id;
        Title = club.Title;
        Description = club.Description;
        Image = club.Image;
        Address = club.Address;
        Admin = club.Admin;
        Category = club.Category;
    }
}