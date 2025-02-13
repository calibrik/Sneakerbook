using System.ComponentModel.DataAnnotations;
using RunningClub.Misc;
using RunningClub.Models;

namespace RunningClub.ViewModels;

public class CreateRaceViewModel
{
    [Required(ErrorMessage = "Title is required")]
    [Display(Name = "Title")]
    public string Title { get; set; }
    [Display(Name = "Description")]
    public string? Description { get; set; }
    [Required(ErrorMessage = "Image is required")]
    [Display(Name = "Image")]
    [DataType(DataType.Upload)]
    public IFormFile Image { get; set; }
    public Address Address { get; set; }
    [Required(ErrorMessage = "Category is required")]
    [Display(Name = "Category")]
    public RaceCategory Category { get; set; }
    [Required]
    public int ClubId { get; set; }
    [Required(ErrorMessage = "Max Members Number is required")]
    [Display(Name = "Max Members Number")]
    [Range(1,20,ErrorMessage = "Max Members Number must be between 1 and 20")]
    public int MaxMembersNumber { get; set; }
    [Required(ErrorMessage = "Start date is required")]
    [Display(Name = "Start Date")]
    [DataType(DataType.Date)]
    public DateTime StartDate { get; set; }
    [Required(ErrorMessage = "Start time is required")]
    [Display(Name = "Start Time")]
    [DataType(DataType.Time)]
    public DateTime StartTime { get; set; }
}