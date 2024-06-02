using LicensePlateDataShared.Models;
using LicensePlateServer.Data;
using LicensePlateServer.Services;

namespace LicensePlateServer.Factories;

public class CameraCaptureFactory : ICameraCaptureFactory
{
    //private readonly LicensePlateRecognition _licensePlateRecognition;
    private readonly LicensePlateDbContext _context;
    private readonly ILoggerFactory _loggerFactory;

    public CameraCaptureFactory(LicensePlateDbContext context, ILoggerFactory loggerFactory)
    {
        //_licensePlateRecognition = licensePlateRecognition;
        _context = context;
        _loggerFactory = loggerFactory;
    }

    public ICameraCapture Create(Camera camera)
    {
        var logger = _loggerFactory.CreateLogger<CameraCapture>();
        return new CameraCapture( _context, logger, camera);
    }
}