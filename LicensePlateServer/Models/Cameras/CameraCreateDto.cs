using System.ComponentModel.DataAnnotations;

namespace LicensePlateServer.Models.Cameras;

public class CameraCreateDto
{
    [Required]
    public string IpAddress { get; set; }
    [Required]
    public string Name { get; set; }
    [Required]
    public string Login { get; set; }
    [Required]
    public string Password { get; set; }
}