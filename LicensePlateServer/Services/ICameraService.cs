using LicensePlateDataShared.Models;

namespace LicensePlateServer.Services;

public interface ICameraService
{
    IEnumerable<Camera> GetAllCameras();
}