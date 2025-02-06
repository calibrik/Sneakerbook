using System.ComponentModel.DataAnnotations;

namespace RunningClub.Models;

public class Address
{
    [Required(ErrorMessage = "Street is required")]
    [Display(Name = "Street")]
    public required string Street { get; set; }
    [Required(ErrorMessage = "City is required")]
    [Display(Name = "City")]
    public required string City { get; set; }
    [Required(ErrorMessage = "Country is required")]
    [Display(Name = "Country")]
    public required string Country { get; set; }
}