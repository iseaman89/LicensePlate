namespace LicensePlateServer.Models.Users;

public class AuthResponse
{
    public string UserId { get; set; }
    public string Token { get; set; }
    public string UserName { get; set; }
}