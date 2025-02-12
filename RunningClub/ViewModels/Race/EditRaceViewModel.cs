using System.ComponentModel.DataAnnotations;
using RunningClub.Models;

namespace RunningClub.ViewModels;

public class EditRaceViewModel
{
    public int Id { get; set; }
    [Required(ErrorMessage = "Title is required")]
    [Display(Name = "Title")]
    public string Title { get; set; }
    [Display(Name = "Description")]
    public string? Description { get; set; }
    public bool IsImageChanged { get; set; }
    public IFormFile? Image { get; set; }
    public Address Address { get; set; }
    [Required(ErrorMessage = "Category is required")]
    [Display(Name = "Category")]
    public RaceCategory Category { get; set; }
    [Required(ErrorMessage = "Max Members Number is required")]
    [Display(Name = "Max Members Number")]
    [Range(1,20,ErrorMessage = "Max Members Number must be between 1 and 20")]
    public int MaxMembersNumber { get; set; }
}