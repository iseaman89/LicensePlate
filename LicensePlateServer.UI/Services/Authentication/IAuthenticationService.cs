using LicensePlateServer.UI.Services.Base;

namespace LicensePlateServer.UI.Services.Authentication;

public interface IAuthenticationService
{
    Task<bool> AuthenticateAsync(LoginUserDto loginModel);
    public Task Logout();
}