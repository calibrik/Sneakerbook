using RunningClub.Models;

namespace RunningClub.ViewModels;

public class DashboardViewModel
{
    public string Id {get; set;}
    public string FName { get; set; }
    public string Email { get; set; }
    public string Username { get; set; }
    public string LName { get; set; }
    public double Mileage { get; set; }
    public List<Club> Clubs { get; set; } = new List<Club>();
    public List<Race> Races { get; set; } = new List<Race>();

    public DashboardViewModel(AppUser appUser)
    {
        Id = appUser.Id;
        FName = appUser.FName;
        LName = appUser.LName;
        Mileage = appUser.Mileage;
        Email = appUser.Email;
        Username = appUser.UserName;
    }
}