using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace RunningClub.ViewModels;

public class ChangePasswordViewModel
{
    [Required(ErrorMessage = "Old password is required")]
    [DataType(DataType.Password)]
    [DisplayName("Old password")]
    public string OldPassword { get; set; }
    [Required(ErrorMessage = "New password is required")]
    [DataType(DataType.Password)]
    [DisplayName("New password")]
    public string NewPassword { get; set; }
    [Required(ErrorMessage = "Password confirmation is required")]
    [DataType(DataType.Password)]
    [DisplayName("Confirm new password")]
    public string ConfirmNewPassword { get; set; }
}