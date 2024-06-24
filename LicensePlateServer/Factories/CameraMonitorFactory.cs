using LicensePlateDataShared.Models;
using LicensePlateServer.Services;

namespace LicensePlateServer.Factories;

public class CameraMonitorFactory : ICameraMonitorFactory
{
    public ICameraMonitor Create(ILicensePlateService licensePlateService, ILoggerFactory loggerFactory, Camera camera)
    {
        var logger = loggerFactory.CreateLogger<CameraMonitor>();
        return new CameraMonitor(licensePlateService, logger, camera);
    }
}