using RunningClub.Models;

namespace RunningClub.ViewModels;

public class EditRaceViewModel
{
    public required int Id { get; set; }
    public required string Title { get; set; }
    public string? Description { get; set; }
    public bool IsImageChanged { get; set; }
    public IFormFile? Image { get; set; }
    public required Address Address { get; set; }
    public required RaceCategory Category { get; set; }
}