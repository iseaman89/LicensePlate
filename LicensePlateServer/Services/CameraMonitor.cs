using System.Diagnostics;
using System.Net;
using LicensePlateDataShared.Models;
using LicensePlateDataShared.Static;
using System.Text.Json;
using Image = SixLabors.ImageSharp.Image;

namespace LicensePlateServer.Services;

public class CameraMonitor : ICameraMonitor
{
    private readonly ILicensePlateService _serviceService;
    private readonly ILogger<CameraMonitor> _logger;
    private readonly Camera _camera;
    private readonly int _checkIntervalSeconds;
    private Process? _pythonProcess;
    private bool _isCameraAvailable;

    public CameraMonitor(ILicensePlateService serviceService, ILogger<CameraMonitor> logger, Camera camera, int checkIntervalSeconds = 10)
    {
        _serviceService = serviceService;
        _logger = logger;
        _camera = camera;
        _checkIntervalSeconds = checkIntervalSeconds;
    }
    
    public async Task StartMonitorCameraAsync(CancellationToken stoppingToken)
    {
        using var httpClientHandler = new HttpClientHandler { Credentials = new NetworkCredential(_camera.Login, _camera.Password) };
        using var httpClient = new HttpClient(httpClientHandler);

        while (!stoppingToken.IsCancellationRequested)
        {
            _isCameraAvailable = IsCameraAvailable(httpClient);

            if (_isCameraAvailable && _pythonProcess == null)
            {
                StartPythonScript(_camera);
            }
            else if (!_isCameraAvailable && _pythonProcess != null)
            {
                StopPythonScript();
            }

            await Task.Delay(_checkIntervalSeconds * 1000, stoppingToken);
        }

        StopPythonScript();
    }
    
    /// <summary>
    /// Stops capturing images from the camera.
    /// </summary>
    public void StopMonitorCamera()
    {
        StopPythonScript();
        _logger.LogInformation($"Camera {_camera.Name} in {_camera.IpAddress} stopped capturing.");
    }
    
    private void StartPythonScript(Camera camera)
    {
        var psi = new ProcessStartInfo
        {
            FileName = Paths.PythonInterpreterPath,
            Arguments = $"{Paths.PythonScriptPath} {camera.IpAddress} {camera.Login} {camera.Password} {camera.Name}",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        _pythonProcess = new Process { StartInfo = psi };
        _pythonProcess.OutputDataReceived += (sender, e) =>
        {
            if (!string.IsNullOrEmpty(e.Data))
            {
                var licensePlate = JsonSerializer.Deserialize<LicensePlate>(e.Data);
                if (licensePlate != null) _serviceService.SavePlateToDatabase(licensePlate);
            }
        };
        _pythonProcess.ErrorDataReceived += (sender, e) =>
        {
            if (!string.IsNullOrEmpty(e.Data))
            {
                Console.WriteLine($"Python error: {e.Data}");
            }
        };

        _pythonProcess.Start();
        _pythonProcess.BeginOutputReadLine();
        _pythonProcess.BeginErrorReadLine();
    }

    private void StopPythonScript()
    {
        if (_pythonProcess != null && !_pythonProcess.HasExited)
        {
            _pythonProcess.Kill();
            _pythonProcess.WaitForExit();
            _pythonProcess = null;
        }
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
