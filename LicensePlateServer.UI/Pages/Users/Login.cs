using LicensePlateServer.UI.Services.Authentication;
using LicensePlateServer.UI.Services.Base;
using Microsoft.AspNetCore.Components;

namespace LicensePlateServer.UI.Pages.Users;

public partial class Login
{ 
    [Inject] private IAuthenticationService _authService { get; set; } 
    [Inject] private NavigationManager _navManager { get; set; }
    
    private LoginUserDto _loginModel = new LoginUserDto();
    private string _message = String.Empty;

    private async Task HandleLogin()
    {
        try
        {
            var response = await _authService.AuthenticateAsync(_loginModel);
            if (response)
            {
                _navManager.NavigateTo("/");
            }
        }
        catch (ApiException ex)
        {
            if (ex.StatusCode >= 200 && ex.StatusCode <= 299)
            {
                _navManager.NavigateTo("/");
            }
            _message = ex.Response;
        }
    }
}