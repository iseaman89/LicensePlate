using LicensePlateDataShared.Models;
using LicensePlateServer.Services;

namespace LicensePlateServer.Factories;

public interface ICameraCaptureFactory
{
    ICameraCapture Create(Camera camera);
}