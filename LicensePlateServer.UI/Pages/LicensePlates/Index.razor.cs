using LicensePlateServer.UI.Services;
using LicensePlateServer.UI.Services.Base;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace LicensePlateServer.UI.Pages.LicensePlates;

public partial class Index
{ 
        [Inject] private ILicensePlateService _licensePlateService { get; set; }
        [Inject] private IJSRuntime _js { get; set; }
        
        private List<LicensePlateReadOnlyDto> _licensePlates;
        private Response<List<LicensePlateReadOnlyDto>> _response = new Response<List<LicensePlateReadOnlyDto>>(){Success = true};

        protected override async Task OnInitializedAsync()
        {
                _response = await _licensePlateService.GetLicensePlates();

                if (_response.Success)
                {
                        _licensePlates = _response.Data;
                }
        }

        private async Task Delete(int id)
        {
                var licensePlate = _licensePlates.Find(lp => lp.Id == id);
                var confirm = await _js.InvokeAsync<bool>("confirm", $"Are you sure that you want delete license plate {licensePlate.PlateNumber}?");
                if (confirm)
                {
                        var response = await _licensePlateService.Delete(id);
                        if (response.Success)
                        {
                                await OnInitializedAsync();
                        }
                }
        }
}