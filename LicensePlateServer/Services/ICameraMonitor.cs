namespace LicensePlateServer.Services;

public interface ICameraMonitor
{
    Task StartMonitorCameraAsync(CancellationToken stoppingToken);
    void StopMonitorCamera();
}