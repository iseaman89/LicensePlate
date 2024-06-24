using LicensePlateClient.Services.Base;
using MudBlazor;

namespace LicensePlateClient.Pages.LicensePlates;

public partial class Index
{
    private IEnumerable<LicensePlateReadOnlyDto> _licensePlates;
    private IEnumerable<LicensePlateReadOnlyDto> _data;
    private Response<List<LicensePlateReadOnlyDto>> _response = new Response<List<LicensePlateReadOnlyDto>> { Success = true };
    private MudTable<LicensePlateReadOnlyDto> _table;
    private int _totalItems;
    private string _searchString = null;
    
    private async Task<TableData<LicensePlateReadOnlyDto>> ServerReload(TableState state)
    {
        var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
        var user = authState.User;

        _response = await _licensePlateService.GetLicensePlates();
        if (_response.Success)
        {
            _data = _response.Data;
        }
        await Task.Delay(300);
        _data = _data.Where(element =>
        {
            if (string.IsNullOrWhiteSpace(_searchString))
                return true;
            if (element.CameraName.ToString().Contains(_searchString, StringComparison.OrdinalIgnoreCase))
            if (element.Date.ToString().Contains(_searchString, StringComparison.OrdinalIgnoreCase))
                return true;
            if (element.Time.ToString().Contains(_searchString, StringComparison.OrdinalIgnoreCase))
                return true;
            if (element.PlateNumber.Contains(_searchString, StringComparison.OrdinalIgnoreCase))
                return true;
            return false;
        }).ToArray();
        _totalItems = _data.Count();
        switch (state.SortLabel)
        {
            case "name_field":
                _data = _data.OrderByDirection(state.SortDirection, o => o.CameraName);
                break;
            case "date_field":
                _data = _data.OrderByDirection(state.SortDirection, o => o.Date);
                break;
            case "time_field":
                _data = _data.OrderByDirection(state.SortDirection, o => o.Time);
                break;
            case "plate_field":
                _data = _data.OrderByDirection(state.SortDirection, o => o.PlateNumber);
                break;
        }

        _licensePlates = _data.Skip(state.Page * state.PageSize).Take(state.PageSize).ToArray();
        return new TableData<LicensePlateReadOnlyDto>() {TotalItems = _totalItems, Items = _licensePlates};
    }

    private void OnSearch(string text)
    {
        _searchString = text;
        _table.ReloadServerData();
    }
    
    private string GetImagePath(string imageName)
    {
        return $"Images/Plates/{imageName}";
    }
}