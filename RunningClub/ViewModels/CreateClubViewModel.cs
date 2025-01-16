using RunningClub.Models;

namespace RunningClub.ViewModels;

public class CreateClubViewModel
{
    public required string Title { get; set; }
    public string? Description { get; set; }
    public required IFormFile Image { get; set; }
    public required Address Address { get; set; }
    public required ClubCategory Category { get; set; }
}