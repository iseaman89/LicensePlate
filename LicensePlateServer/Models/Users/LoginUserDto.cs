using System.ComponentModel.DataAnnotations;

namespace LicensePlateServer.Models.Users;

public class LoginUserDto
{
    [Required]
    public string UserName { get; set; }

    [Required]
    public string Password { get; set; }
}