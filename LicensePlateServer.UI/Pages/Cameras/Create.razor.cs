using LicensePlateServer.UI.Services;
using LicensePlateServer.UI.Services.Base;
using Microsoft.AspNetCore.Components;

namespace LicensePlateServer.UI.Pages.Cameras;

public partial class Create
{
    [Inject] public ICameraService CameraService { get; set; }
    [Inject] public NavigationManager NavManager { get; set; }
    
    private CameraCreateDto _camera = new CameraCreateDto();

    private async Task HandleCreateCamera()
    {
        var response = await CameraService.CreateCamera(_camera);
        if (response.Success)
        {
            BackToList();
        }
    }

    private void BackToList()
    {
        NavManager.NavigateTo("/cameras/");
    }
}