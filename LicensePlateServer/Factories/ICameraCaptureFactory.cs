using LicensePlateDataShared.Models;
using LicensePlateServer.Services;

namespace LicensePlateServer.Factories;

public interface ICameraCaptureFactory
{
    /// <summary>
    /// Creates an instance of <see cref="ICameraCapture"/>.
    /// </summary>
    /// <param name="licensePlateRecognition">The license plate recognition service.</param>
    /// <param name="loggerFactory">The logger factory.</param>
    /// <param name="camera">The camera object.</param>
    /// <returns>An instance of <see cref="ICameraCapture"/>.</returns>
    public ICameraCapture Create(ILicensePlateRecognition licensePlateRecognition, ILoggerFactory loggerFactory, Camera camera);
}