using LicensePlateClient.Services.Base;

namespace LicensePlateClient.Services.Authentication;

public interface IAuthenticationService
{
    Task<bool> AuthenticateAsync(LoginUserDto loginModel);
    public Task Logout();
}