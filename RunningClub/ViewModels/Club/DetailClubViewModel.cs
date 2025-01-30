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