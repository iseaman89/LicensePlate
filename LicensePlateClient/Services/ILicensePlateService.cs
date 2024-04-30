using LicensePlateClient.Services.Base;

namespace LicensePlateClient.Services;

public interface ILicensePlateService
{
    Task<Response<List<LicensePlateReadOnlyDto>>> GetLicensePlates();
    Task<Response<LicensePlateReadOnlyDto>> GetLicensePlate(int id);
    Task<Response<LicensePlateUpdateDto>> GetLicensePlateForUpdate(int id);
    Task<Response<int>> CreateLicensePlate(LicensePlateCreateDto licensePlate);
    Task<Response<int>> EditLicensePlate(int id, LicensePlateUpdateDto licensePlate);
    Task<Response<int>> Delete(int id);

}