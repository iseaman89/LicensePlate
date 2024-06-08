using System.Net;
using LicensePlateDataShared.Models;
using LicensePlateDataShared.Static;
using Image = SixLabors.ImageSharp.Image;

namespace LicensePlateServer.Services;

public class CameraCapture : ICameraCapture
{
    private readonly ILicensePlateRecognition _recognitionService;
    private readonly ILogger<CameraCapture> _logger;
    private readonly Camera _camera;
    private readonly int _checkIntervalSeconds;
    private bool _isRunning;

    public CameraCapture(ILicensePlateRecognition recognitionService, ILogger<CameraCapture> logger, Camera camera, int checkIntervalSeconds = 10)
    {
        _recognitionService = recognitionService;
        _logger = logger;
        _camera = camera;
        _checkIntervalSeconds = checkIntervalSeconds;
    }
    
    public async Task StartCaptureAsync(CancellationToken stoppingToken)
    {
        using var httpClientHandler = new HttpClientHandler { Credentials = new NetworkCredential(_camera.Login, _camera.Password) };
        using var httpClient = new HttpClient(httpClientHandler);

        _isRunning = true;
        while (_isRunning && !stoppingToken.IsCancellationRequested)
        {
            if (!IsCameraAvailable(httpClient))
            {
                _logger.LogInformation($"Camera {_camera.Name} in {_camera.IpAddress} is not available.");
                await Task.Delay(_checkIntervalSeconds * 1000, stoppingToken);
                continue;
            }

            //var frame = await GetCameraFrameAsync(httpClient);
            var frame = Image.Load(Paths.PicPath);
            if (frame != null)
            {
                _recognitionService.ProcessFrame(frame);
            }

            await Task.Delay(_checkIntervalSeconds * 1000, stoppingToken);
        }
        _logger.LogInformation($"Camera {_camera.Name} in {_camera.IpAddress} started capturing.");
    }
    
    public void StopCapture()
    {
        _isRunning = false;
        _logger.LogInformation($"Camera {_camera.Name} in {_camera.IpAddress} stopped capturing.");
    }
    
    private async Task<Image> GetCameraFrameAsync(HttpClient httpClient)
    {
        var response = await httpClient.GetAsync(Paths.CameraPath);
        if (response.StatusCode == HttpStatusCode.OK)
        {
            using var stream = await response.Content.ReadAsStreamAsync();
            return Image.Load(stream);
        }
        return null;
    }

    private bool IsCameraAvailable(HttpClient httpClient)
    {
        try
        {
            var response = httpClient.GetAsync("http://" + _camera.IpAddress + Paths.CameraPath).Result;
            return response.StatusCode == HttpStatusCode.OK;
        }
        catch (HttpRequestException)
        {
            return false;
        }
    }
}
