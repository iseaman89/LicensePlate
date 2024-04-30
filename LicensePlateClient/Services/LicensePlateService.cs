using AutoMapper;
using Blazored.LocalStorage;
using LicensePlateClient.Services;
using LicensePlateClient.Services.Base;
using LicensePlateClient.Services.Base;

namespace LicensePlateClient.Services;

public class LicensePlateService : BaseHttpService, ILicensePlateService
{
    private readonly Client _httpClient;
    private readonly IMapper _mapper;
    
    public LicensePlateService(Client httpClient, ILocalStorageService localStorageService, IMapper mapper) : base(httpClient, localStorageService)
    {
        _httpClient = httpClient;
        _mapper = mapper;
    }

    public async Task<Response<List<LicensePlateReadOnlyDto>>> GetLicensePlates()
    {
        Response<List<LicensePlateReadOnlyDto>> response;

        try
        {
            await GetBearerToken();
            var data = await _httpClient.LicensePlateAllAsync();
            response = new Response<List<LicensePlateReadOnlyDto>>()
            {
                Data = data.ToList(),
                Success = true
            };
        }
        catch (ApiException ex)
        {
            response = ConvertApiExceptions<List<LicensePlateReadOnlyDto>>(ex);
        }

        return response;
    }

    public async Task<Response<LicensePlateReadOnlyDto>> GetLicensePlate(int id)
    {
        Response<LicensePlateReadOnlyDto> response;

        try
        {
            await GetBearerToken();
            var data = await _httpClient.LicensePlateGETAsync(id);
            response = new Response<LicensePlateReadOnlyDto>()
            {
                Data = data,
                Success = true
            };
        }
        catch (ApiException ex)
        {
            response = ConvertApiExceptions<LicensePlateReadOnlyDto>(ex);
        }

        return response;
    }

    public async Task<Response<LicensePlateUpdateDto>> GetLicensePlateForUpdate(int id)
    {
        Response<LicensePlateUpdateDto> response;

        try
        {
            await GetBearerToken();
            var data = await _httpClient.LicensePlateGETAsync(id);
            response = new Response<LicensePlateUpdateDto>()
            {
                Data = _mapper.Map<LicensePlateUpdateDto>(data),
                Success = true
            };
        }
        catch (ApiException ex)
        {
            response = ConvertApiExceptions<LicensePlateUpdateDto>(ex);
        }

        return response;
    }

    public async Task<Response<int>> CreateLicensePlate(LicensePlateCreateDto licensePlate)
    {
        Response<int> response = new Response<int>() {Success = true};

        try
        {
            await GetBearerToken(); 
            await _httpClient.LicensePlatePOSTAsync(licensePlate);
        }
        catch (ApiException ex)
        {
            response = ConvertApiExceptions<int>(ex);
        }

        return response;
    }

    public async Task<Response<int>> EditLicensePlate(int id, LicensePlateUpdateDto licensePlate)
    {
        Response<int> response = new Response<int>() {Success = true};

        try
        {
            await GetBearerToken(); 
            await _httpClient.LicensePlatePUTAsync(id, licensePlate);
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
            await _httpClient.LicensePlateDELETEAsync(id);
        }
        catch (ApiException ex)
        {
            response = ConvertApiExceptions<int>(ex);
        }

        return response;
    }
}