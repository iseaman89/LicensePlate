using LicensePlateServer.UI.Services;
using LicensePlateServer.UI.Services.Base;
using Microsoft.AspNetCore.Components;

namespace LicensePlateServer.UI.Pages.Cameras;

public partial class Update
{ 
        [Inject] private ICameraService _cameraService { get; set; }
        [Inject] private NavigationManager _navManager { get; set; }
        
        [Parameter] public int Id { get; set; }
    
        private CameraUpdateDto _camera = new();

        protected override async Task OnInitializedAsync()
        {
                var response = await _cameraService.GetCameraForUpdate(Id);
                if (response.Success)
                {
                        _camera = response.Data;
                }
        }

        private async Task HandleUpdateCamera()
        {
                var response = await _cameraService.EditCamera(Id, _camera);
                if (response.Success)
                {
                        BackToList();
                }
        }

        private void BackToList()
        {
                _navManager.NavigateTo("/cameras/");
        }
}