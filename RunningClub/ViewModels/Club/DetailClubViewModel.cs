using RunningClub.Models;

namespace RunningClub.ViewModels;

public class DetailClubViewModel
{
    public int Id { get; set; }
    public string Title { get; set; }
    public bool IsAdmin { get; set; } = false;

    public DetailClubViewModel(Club club)
    {
        Id = club.Id;
        Title = club.Title;
    }
}