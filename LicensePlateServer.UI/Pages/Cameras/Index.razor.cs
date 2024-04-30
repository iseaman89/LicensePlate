using LicensePlateServer.UI.Services;
using LicensePlateServer.UI.Services.Base;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace LicensePlateServer.UI.Pages.Cameras;

public partial class Index
{ 
        [Inject] private ICameraService _cameraService { get; set; }
        [Inject] private IJSRuntime _js { get; set; }
        
        private List<CameraReadOnlyDto> _cameras;
        private Response<List<CameraReadOnlyDto>> _response = new Response<List<CameraReadOnlyDto>>(){Success = true};

        protected override async Task OnInitializedAsync()
        {
                _response = await _cameraService.GetCameras();

                if (_response.Success)
                {
                        _cameras = _response.Data;
                }
        }

        private async Task Delete(int id)
        {
                var camera = _cameras.Find(lp => lp.Id == id);
                var confirm = await _js.InvokeAsync<bool>("confirm", $"Are you sure that you want delete camera {camera.Name}?");
                if (confirm)
                {
                        var response = await _cameraService.Delete(id);
                        if (response.Success)
                        {
                                await OnInitializedAsync();
                        }
                }
        }
}