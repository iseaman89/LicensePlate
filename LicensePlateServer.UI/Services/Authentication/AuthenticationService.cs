using Blazored.LocalStorage;
using LicensePlateServer.UI.Providers;
using LicensePlateServer.UI.Services.Base;
using Microsoft.AspNetCore.Components.Authorization;

namespace LicensePlateServer.UI.Services.Authentication;

public class AuthenticationService : IAuthenticationService
{
    private readonly Client _httpClient;
    private readonly ILocalStorageService _localStorageService;
    private readonly AuthenticationStateProvider _authenticationStateProvider;
    
    public AuthenticationService(Client httpClient, ILocalStorageService localStorageService, AuthenticationStateProvider authenticationStateProvider)
    {
        _httpClient = httpClient;
        _localStorageService = localStorageService;
        _authenticationStateProvider = authenticationStateProvider;
    }
    
    public async Task<bool> AuthenticateAsync(LoginUserDto loginModel)
    {
        var response = await _httpClient.LoginAsync(loginModel);

        await _localStorageService.SetItemAsync("accessToken", response.Token);

        await ((ApiAuthenticationStateProvider)_authenticationStateProvider).LoggedIn();

        return true;
    }

    public async Task Logout()
    {
        await ((ApiAuthenticationStateProvider)_authenticationStateProvider).LoggedOut();
    }
}