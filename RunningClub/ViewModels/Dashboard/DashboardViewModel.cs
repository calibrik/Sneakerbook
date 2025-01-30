using RunningClub.Models;

namespace RunningClub.ViewModels;

public class DashboardViewModel
{
    public string FName { get; set; }
    public string Email { get; set; }
    public string Username { get; set; }
    public string LName { get; set; }
    public float Mileage { get; set; }
    public List<Club> Clubs { get; set; } = new List<Club>();
    public List<Race> Races { get; set; } = new List<Race>();
    public List<Club> AdminClubs { get; set; } = new List<Club>();
    public List<Race> AdminRaces { get; set; } = new List<Race>();

    public DashboardViewModel(AppUser appUser)
    {
        FName = appUser.FName;
        LName = appUser.LName;
        Mileage = appUser.Mileage;
        Email = appUser.Email;
        Username = appUser.UserName;
    }
}