using LicensePlateServer.UI.Services;
using LicensePlateServer.UI.Services.Base;
using Microsoft.AspNetCore.Components;

namespace LicensePlateServer.UI.Pages.LicensePlates;

public partial class Update
{ 
        [Inject] private ILicensePlateService _licensePlateService { get; set; }
        [Inject] private NavigationManager _navManager { get; set; }
        
        [Parameter] public int Id { get; set; }
    
        private LicensePlateUpdateDto _licensePlate = new();

        protected override async Task OnInitializedAsync()
        {
                var response = await _licensePlateService.GetLicensePlateForUpdate(Id);
                if (response.Success)
                {
                        _licensePlate = response.Data;
                }
        }

        private async Task HandleUpdateLicensePlate()
        {
                var response = await _licensePlateService.EditLicensePlate(Id, _licensePlate);
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