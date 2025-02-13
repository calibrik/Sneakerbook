using RunningClub.Models;

namespace RunningClub.ViewModels;

public class DetailRaceViewModel
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string Image { get; set; }
    public Address Address { get; set; }
    public AppUser Admin { get; set; }
    public int MaxMembersNumber { get; set; }
    public Club Club { get; set; }
    public RaceCategory Category { get; set; }
    public DateTime StartDate { get; set; }
    public bool IsJoined { get; set; }
    public bool IsAdmin { get; set; }
    
    public List<AppUser> Members { get; set; }


    public DetailRaceViewModel(Race race)
    {
        Id = race.Id;
        Title = race.Title;
        Description = race.Description;
        Image = race.Image;
        Address = race.Address;
        Admin = race.Admin;
        MaxMembersNumber = race.MaxMembersNumber;
        Category = race.Category;
        Club = race.Club;
        StartDate = race.StartDate.ToLocalTime();
    }
}