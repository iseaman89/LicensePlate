using AutoMapper;
using Blazored.LocalStorage;
using LicensePlateServer.UI.Services.Base;

namespace LicensePlateServer.UI.Services;

public class CameraService : BaseHttpService, ICameraService
{
    private readonly Client _httpClient;
    private readonly IMapper _mapper;
    
    public CameraService(Client httpClient, ILocalStorageService localStorageService, IMapper mapper) : base(httpClient, localStorageService)
    {
        _httpClient = httpClient;
        _mapper = mapper;
    }

    public async Task<Response<List<CameraReadOnlyDto>>> GetCameras()
    {
        Response<List<CameraReadOnlyDto>> response;

        try
        {
            await GetBearerToken();
            var data = await _httpClient.CameraAllAsync();
            response = new Response<List<CameraReadOnlyDto>>()
            {
                Data = data.ToList(),
                Success = true
            };
        }
        catch (ApiException ex)
        {
            response = ConvertApiExceptions<List<CameraReadOnlyDto>>(ex);
        }

        return response;
    }

    public async Task<Response<CameraReadOnlyDto>> GetCamera(int id)
    {
        Response<CameraReadOnlyDto> response;

        try
        {
            await GetBearerToken();
            var data = await _httpClient.CameraGETAsync(id);
            response = new Response<CameraReadOnlyDto>()
            {
                Data = data,
                Success = true
            };
        }
        catch (ApiException ex)
        {
            response = ConvertApiExceptions<CameraReadOnlyDto>(ex);
        }

        return response;
    }

    public async Task<Response<CameraUpdateDto>> GetCameraForUpdate(int id)
    {
        Response<CameraUpdateDto> response;

        try
        {
            await GetBearerToken();
            var data = await _httpClient.CameraGETAsync(id);
            response = new Response<CameraUpdateDto>()
            {
                Data = _mapper.Map<CameraUpdateDto>(data),
                Success = true
            };
        }
        catch (ApiException ex)
        {
            response = ConvertApiExceptions<CameraUpdateDto>(ex);
        }

        return response;
    }

    public async Task<Response<int>> CreateCamera(CameraCreateDto camera)
    {
        Response<int> response = new Response<int>() {Success = true};

        try
        {
            await GetBearerToken(); 
            await _httpClient.CameraPOSTAsync(camera);
        }
        catch (ApiException ex)
        {
            response = ConvertApiExceptions<int>(ex);
        }

        return response;
    }

    public async Task<Response<int>> EditCamera(int id, CameraUpdateDto camera)
    {
        Response<int> response = new Response<int>() {Success = true};

        try
        {
            await GetBearerToken(); 
            await _httpClient.CameraPUTAsync(id, camera);
        }
        catch (ApiException ex)
        {
            response = ConvertApiExceptions<int>(ex);
        }

        return response;
    }

    public async Task<Response<int>> Delete(int id)
    {
        Response<int> response = new Response<int>() {Success = true};

        try
        {
            await GetBearerToken(); 
            await _httpClient.CameraDELETEAsync(id);
        }
        catch (ApiException ex)
        {
            response = ConvertApiExceptions<int>(ex);
        }

        return response;
    }
}