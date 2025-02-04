using RunningClub.Models;

namespace RunningClub.ViewModels;

public class CreateRaceViewModel
{
    public required string Title { get; set; }
    public string? Description { get; set; }
    public required IFormFile Image { get; set; }
    public required Address Address { get; set; }
    public required RaceCategory Category { get; set; }
    public required int ClubId { get; set; }
}