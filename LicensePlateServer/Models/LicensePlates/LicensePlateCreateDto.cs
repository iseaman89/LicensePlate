using System.ComponentModel.DataAnnotations;

namespace LicensePlateServer.Models.LicensePlates;

public class LicensePlateCreateDto
{
    [Required]
    public DateOnly Date { get; set; }
    [Required]
    public TimeOnly Time { get; set; }
    [Required] 
    public string? CameraName { get; set; }
    [Required]
    public string? PlateNumber { get; set; }
    [Required]
    public string? Image { get; set; }
}