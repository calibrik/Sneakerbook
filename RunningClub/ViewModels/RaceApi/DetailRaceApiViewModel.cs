using RunningClub.Models;
using RunningClub.ViewModels.ClubApi;

namespace RunningClub.ViewModels;

public class DetailRaceApiViewModel
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string Image { get; set; }
    public Address Address { get; set; }
    public DateTime StartDate { get; set; }
    public double Length { get; set; }
    public bool IsCompleted { get; set; } = false;
    public string AdminUsername { get; set; }
    public string AdminLink { get; set; }
    public int MaxMembersNumber { get; set; }
    public string ClubTitle { get; set; }
    public string ClubLink { get; set; }
    public string Category { get; set; }
    public bool IsJoined { get; set; } = false;
    public bool IsAdmin { get; set; } = false;
    public List<DetailRaceClubApiUser> Members { get; set; } = new();

    public DetailRaceApiViewModel(Race race)
    {
        Id = race.Id;
        Title = race.Title;
        Description = race.Description;
        Image = race.Image;
        Address = race.Address;
        StartDate = race.StartDate;
        Length = race.Length;
        IsCompleted = race.IsCompleted;
        MaxMembersNumber = race.MaxMembersNumber;
        Category = race.Category.ToString();
    }
}