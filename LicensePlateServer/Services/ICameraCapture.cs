namespace LicensePlateServer.Services;

public interface ICameraCapture
{
    Task StartCaptureAsync(CancellationToken stoppingToken);
    void StopCapture();
}