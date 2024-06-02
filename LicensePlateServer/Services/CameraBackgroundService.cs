using LicensePlateServer.Data;
using LicensePlateServer.Factories;

namespace LicensePlateServer.Services;

public class CameraBackgroundService : BackgroundService
{
    private readonly ILogger<CameraBackgroundService> _logger;
    private readonly IServiceScopeFactory _serviceProvider;
    private readonly List<ICameraCapture> _cameraCaptures = new List<ICameraCapture>();

    public CameraBackgroundService(ILogger<CameraBackgroundService> logger, IServiceScopeFactory serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Camera Background Service is starting.");

        stoppingToken.Register(() => 
            _logger.LogInformation("Camera Background Service is stopping."));

        using (var scope = _serviceProvider.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<LicensePlateDbContext>();
            //var recognitionService = scope.ServiceProvider.GetRequiredService<LicensePlateRecognition>();
            var loggerFactory = scope.ServiceProvider.GetRequiredService<ILoggerFactory>();
            var cameraService = scope.ServiceProvider.GetRequiredService<ICameraService>();
            var factory = scope.ServiceProvider.GetRequiredService<ICameraCaptureFactory>();

            var cameras = cameraService.GetAllCameras();

            foreach (var camera in cameras)
            {
                var cameraLogger = loggerFactory.CreateLogger<CameraCapture>();
                var cameraCapture = factory.Create(camera);
                _cameraCaptures.Add(cameraCapture);
                _ = cameraCapture.StartCaptureAsync(stoppingToken);
            }
        }

        await Task.Delay(Timeout.Infinite, stoppingToken);
    }

    public override Task StopAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Camera Background Service is stopping.");

        foreach (var cameraCapture in _cameraCaptures)
        {
            cameraCapture.StopCapture();
        }

        return Task.CompletedTask;
    }
}
