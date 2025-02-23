using RunningClub.Models;

namespace RunningClub.ViewModels;

public class DetailRaceViewModel
{
    public int Id { get; set; }
    public string Title { get; set; }
    public bool IsAdmin { get; set; }
    public bool IsCompleted { get; set; }


    public DetailRaceViewModel(Race race)
    {
        Id = race.Id;
        IsCompleted = race.IsCompleted;
        Title = race.Title;
    }
}