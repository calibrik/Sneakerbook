using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace RunningClub.ViewModels;

public class UploadPhotoInRegisterViewModel
{
    [DisplayName("Upload your profile photo")]
    [DataType(DataType.Upload)]
    public IFormFile Photo { get; set; }
}