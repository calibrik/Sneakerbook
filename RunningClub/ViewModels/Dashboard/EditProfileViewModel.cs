using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using RunningClub.Models;

namespace RunningClub.ViewModels;

public class EditProfileViewModel
{
    [Required(ErrorMessage = "Username is required")]
    [DisplayName("Username")]
    [StringLength(30,ErrorMessage = "Username cannot be longer than 30 characters.")]
    public string UserName { get; set; }
    [Required(ErrorMessage = "Email is required")]
    [DisplayName("Email")]
    public string Email { get; set; }
    [Required(ErrorMessage = "First name is required")]
    [DisplayName("First name")]
    [StringLength(30,ErrorMessage = "First name cannot be longer than 30 characters.")]
    public string FName { get; set; }
    [Required(ErrorMessage = "Last name is required")]
    [DisplayName("Last name")]
    [StringLength(30,ErrorMessage = "Last name cannot be longer than 30 characters.")]
    public string LName { get; set; }

    public EditProfileViewModel()
    {
        UserName = default;
        Email = default;
        FName = default;
        LName = default;
    }
    public EditProfileViewModel(AppUser user)
    {
        UserName = user.UserName;
        Email = user.Email;
        FName = user.FName;
        LName = user.LName;
    }
}