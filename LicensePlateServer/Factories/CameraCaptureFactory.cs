using LicensePlateDataShared.Models;
using LicensePlateServer.Data;
using LicensePlateServer.Services;

namespace LicensePlateServer.Factories;

public class CameraCaptureFactory : ICameraCaptureFactory
{
    public ICameraCapture Create(ILicensePlateRecognition licensePlateRecognition, ILoggerFactory loggerFactory, Camera camera)
    {
        var logger = loggerFactory.CreateLogger<CameraCapture>();
        return new CameraCapture(licensePlateRecognition, logger, camera);
    }
}