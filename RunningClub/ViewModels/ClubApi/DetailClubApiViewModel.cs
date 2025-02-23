using RunningClub.Models;

namespace RunningClub.ViewModels.ClubApi;

public class DetailRaceClubApiUser
{
    public string Username { get; set; }
    public string Link { get; set; }
    public bool IsAdmin { get; set; }
    public string Id { get; set; }
}
public class DetailClubApiViewModel
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string Image { get; set; }
    public Address Address { get; set; }
    public string AdminUsername { get; set; }
    public string AdminId { get; set; }
    public string AdminLink { get; set; }
    public string Category { get; set; }
    public bool IsJoined { get; set; } = false;
    public bool IsAdmin { get; set; }= false;
    public List<DetailRaceClubApiUser> Members { get; set; }= new();
    
    public DetailClubApiViewModel(Club club)
    {
        Id = club.Id;
        Title = club.Title;
        Description = club.Description;
        Image = club.Image;
        Address = club.Address;
        AdminId = club.AdminId;
        Category = club.Category.ToString();
    }
}