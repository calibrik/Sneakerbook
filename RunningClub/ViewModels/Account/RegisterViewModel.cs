using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace RunningClub.ViewModels;

public class RegisterViewModel
{
    [Required(ErrorMessage = "Username is required")]
    [DisplayName("Username")]
    [StringLength(30,ErrorMessage = "Username cannot be longer than 30 characters.")]
    public string UserName { get; set; }
    [Required(ErrorMessage = "Password is required")]
    [DataType(DataType.Password)]
    [DisplayName("Password")]
    public string Password { get; set; }
    [Required(ErrorMessage = "Password confirmation is required")]
    [DataType(DataType.Password)]
    [DisplayName("Confirm Password")]
    public string ConfirmPassword { get; set; }
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
}