using System.ComponentModel.DataAnnotations;

namespace LicensePlateServer.Models.LicensePlates;

public class LicensePlateUpdateDto : BaseDto
{
    [Required]
    public DateOnly Date { get; set; }
    [Required]
    public TimeOnly Time { get; set; }
    [Required]
    public string? PlateNumber { get; set; }
    public string? Image { get; set; }
}