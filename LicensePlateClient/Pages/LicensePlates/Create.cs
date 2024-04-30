using LicensePlateClient.Services;
using LicensePlateClient.Services.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;

namespace LicensePlateClient.Pages.LicensePlates;

public partial class Create
{
    [Inject] public ILicensePlateService LicensePlateService { get; set; }
    [Inject] public NavigationManager NavManager { get; set; }

    private LicensePlateCreateDto _licensePlates = new LicensePlateCreateDto();

    private async Task HandleCreateLicensePlate()
    {
        _licensePlates.Date = DateTime.Now;
        _licensePlates.Time = DateTime.Now.TimeOfDay;
        var response = await LicensePlateService.CreateLicensePlate(_licensePlates);
        if (response.Success)
        {
            BackToList();
        }
    }

    private void BackToList()
    {
        NavManager.NavigateTo("/license-plates/");
    }
}