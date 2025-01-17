using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace RunningClub.ViewModels;

public class LoginViewModel
{
    [DisplayName("Email address")]
    [Required(ErrorMessage = "Email is required")]
    public string Email { get; set; }
    [DisplayName("Password")]
    [DataType(DataType.Password)]
    [Required(ErrorMessage = "Password is required")]
    public string Password { get; set; }
    [DisplayName("Remember me")]
    public bool RememberMe { get; set; }
}