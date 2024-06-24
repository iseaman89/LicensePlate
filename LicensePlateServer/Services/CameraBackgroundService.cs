using LicensePlateServer.Data;
using LicensePlateServer.Factories;

namespace LicensePlateServer.Services;

public class CameraBackgroundService : BackgroundService
{
    private readonly ILogger<CameraBackgroundService> _logger;
    private readonly IServiceScopeFactory _serviceProvider;
    private readonly List<ICameraMonitor> _cameraCaptures = new List<ICameraMonitor>();

    public CameraBackgroundService(ILogger<CameraBackgroundService> logger, IServiceScopeFactory serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    /// <summary>
    /// This method is called when the <see cref="IHostedService"/> starts.
    /// </summary>
    /// <param name="stoppingToken">A <see cref="CancellationToken"/> that indicates when the service should stop.</param>
    /// <returns>A <see cref="Task"/> that represents the background operation.</returns>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Camera Background Service is starting.");

        stoppingToken.Register(() => 
            _logger.LogInformation("Camera Background Service is stopping."));

        using (var scope = _serviceProvider.CreateScope())
        {
            var recognitionService = scope.ServiceProvider.GetRequiredService<ILicensePlateService>();
            var loggerFactory = scope.ServiceProvider.GetRequiredService<ILoggerFactory>();
            var cameraService = scope.ServiceProvider.GetRequiredService<ICameraService>();
            var factory = scope.ServiceProvider.GetRequiredService<ICameraMonitorFactory>();

            var cameras = cameraService.GetAllCameras();

            foreach (var camera in cameras)
            {
                var cameraCapture = factory.Create(recognitionService, loggerFactory, camera);
                _cameraCaptures.Add(cameraCapture);
                _ = cameraCapture.StartMonitorCameraAsync(stoppingToken);
            }
        }

        await Task.Delay(Timeout.Infinite, stoppingToken);
    }

    /// <summary>
    /// This method is called when the <see cref="IHostedService"/> stops.
    /// </summary>
    /// <param name="stoppingToken">A <see cref="CancellationToken"/> that indicates when the service should stop.</param>
    /// <returns>A <see cref="Task"/> that represents the stopping operation.</returns>
    public override Task StopAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Camera Background Service is stopping.");

        foreach (var cameraCapture in _cameraCaptures)
        {
            cameraCapture.StopMonitorCamera();
        }

        return Task.CompletedTask;
    }
}
