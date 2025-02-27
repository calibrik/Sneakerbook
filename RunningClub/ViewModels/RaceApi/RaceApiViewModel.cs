using CloudinaryDotNet;
using RunningClub.Models;

namespace RunningClub.ViewModels;

public class RaceApiViewModel
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Category { get; set; }
    
    public string ClubLink { get; set; }
    public string ClubTitle { get; set; }
    public string Image { get; set; }
    public string? Description { get; set; }
    public int MemberCount { get; set; }
    public int MaxMemberCount { get; set; }
    public DateTime StartDate { get; set; }
    public double Length { get; set; }
    public string RaceLink { get; set; }

    public RaceApiViewModel(Race race)
    {
        Id = race.Id;
        Title = race.Title;
        Description = race.Description;
        Category = race.Category.ToString();
        ClubTitle=race.Club.Title;
        Image = race.Image;
        MaxMemberCount = race.MaxMembersNumber;
        StartDate = race.StartDate;
        Length = race.Length;
    }
}