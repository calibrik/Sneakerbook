using RunningClub.Misc;
using RunningClub.Models;

namespace RunningClub.ViewModels.ClubApi;

public class DetailClubApiRaceViewModel
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Category { get; set; }
    public string Image { get; set; }
    public string? Description { get; set; }
    public int MemberCount { get; set; }
    public int MaxMemberCount { get; set; }
    public DateTime StartDate { get; set; }
    public double Length { get; set; }
    public bool IsJoined { get; set; }
    public string Link { get; set; }

    public DetailClubApiRaceViewModel(Race race)
    {
        Id = race.Id;
        Title = race.Title;
        Description = race.Description;
        Category = race.Category.ToString();
        Image = race.Image;
        MaxMemberCount = race.MaxMembersNumber;
        StartDate = race.StartDate;
        Length = race.Length;
    }
}