using System.ComponentModel.DataAnnotations;

namespace RunningClub.Models;

public class Address
{
    [Key]
    public required int Id { get; set; }
    public required string Street { get; set; }
    public required string City { get; set; }
    public required string Country { get; set; }
}