using LicensePlateServer.UI.Services;
using LicensePlateServer.UI.Services.Base;
using Microsoft.AspNetCore.Components;

namespace LicensePlateServer.UI.Pages.LicensePlates;

public partial class Create
{
    [Inject] private ILicensePlateService _licensePlateService { get; set; }
    [Inject] private NavigationManager _navManager { get; set; }
    
    private LicensePlateCreateDto _licensePlates = new LicensePlateCreateDto();

    private async Task HandleCreateLicensePlate()
    {
        _licensePlates.Date = DateTime.Now;
        _licensePlates.Time = DateTime.Now.TimeOfDay;
        var response = await _licensePlateService.CreateLicensePlate(_licensePlates);
        if (response.Success)
        {
            BackToList();
        }
    }

    private void BackToList()
    {
        _navManager.NavigateTo("/license-plates/");
    }
}