using System.ComponentModel.DataAnnotations;
using RunningClub.Models;

namespace RunningClub.ViewModels;

public class CreateRaceViewModel
{
    [Required(ErrorMessage = "Title is required")]
    [Display(Name = "Title")]
    public string Title { get; set; }
    public string? Description { get; set; }
    [Required(ErrorMessage = "Image is required")]
    [Display(Name = "Image")]
    public IFormFile Image { get; set; }
    public Address Address { get; set; }
    [Required(ErrorMessage = "Category is required")]
    [Display(Name = "Category")]
    public RaceCategory Category { get; set; }
    [Required]
    public int ClubId { get; set; }
}