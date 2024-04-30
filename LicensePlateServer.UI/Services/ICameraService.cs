using LicensePlateServer.UI.Services.Base;

namespace LicensePlateServer.UI.Services;

public interface ICameraService
{
    Task<Response<List<CameraReadOnlyDto>>> GetCameras();
    Task<Response<CameraReadOnlyDto>> GetCamera(int id);
    Task<Response<CameraUpdateDto>> GetCameraForUpdate(int id);
    Task<Response<int>> CreateCamera(CameraCreateDto camera);
    Task<Response<int>> EditCamera(int id, CameraUpdateDto camera);
    Task<Response<int>> Delete(int id);

}