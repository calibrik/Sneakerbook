using RunningClub.Models;

namespace RunningClub.ViewModels.DashboardApi;

public class DashboardApiUserViewModel
{
    public string FName { get; set; }
    public string LName { get; set; }
    public double Mileage { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public bool IsSelf { get; set; }
    public bool IsAdmin { get; set; }
    public string ChangePasswordLink { get; set; }
    public string EditProfileLink { get; set; }

    public DashboardApiUserViewModel(AppUser user)
    {
        Username = user.UserName;
        Email = user.Email;
        Mileage = user.Mileage;
        FName = user.FName;
        LName = user.LName;
    }
}