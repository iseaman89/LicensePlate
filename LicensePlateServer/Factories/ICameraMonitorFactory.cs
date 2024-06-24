using LicensePlateDataShared.Models;
using LicensePlateServer.Services;

namespace LicensePlateServer.Factories;

public interface ICameraMonitorFactory
{
    /// <summary>
    /// Creates an instance of <see cref="ICameraMonitor"/>.
    /// </summary>
    /// <param name="licensePlateService">The license plate recognition service.</param>
    /// <param name="loggerFactory">The logger factory.</param>
    /// <param name="camera">The camera object.</param>
    /// <returns>An instance of <see cref="ICameraMonitor"/>.</returns>
    public ICameraMonitor Create(ILicensePlateService licensePlateService, ILoggerFactory loggerFactory, Camera camera);
}