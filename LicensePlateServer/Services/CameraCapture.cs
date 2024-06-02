using System.Diagnostics;
using System.Drawing;
using System.Net;
using Emgu.CV;
using Emgu.CV.OCR;
using Emgu.CV.Models;
using LicensePlateDataShared.Models;
using LicensePlateDataShared.Static;
using Yolov8Net;
using SixLabors.ImageSharp;
using LicensePlateServer.Data;
using LicensePlateServer.Statics;
using NuGet.Protocol;
using SixLabors.ImageSharp.Processing;
using Tesseract;
using Image = SixLabors.ImageSharp.Image;
using ImageConverter = LicensePlateServer.Statics.ImageConverter;
using Pix = Tesseract.Pix;
using Rectangle = System.Drawing.Rectangle;
using RectangleF = System.Drawing.RectangleF;

namespace LicensePlateServer.Services;

public class CameraCapture : ICameraCapture
{
    //private readonly LicensePlateRecognition _recognitionService;
    private readonly LicensePlateDbContext _context;
    private readonly ILogger<CameraCapture> _logger;
    private readonly Camera _camera;
    private readonly int _checkIntervalSeconds;
    private bool _isRunning;

    public CameraCapture(LicensePlateDbContext context, ILogger<CameraCapture> logger, Camera camera, int checkIntervalSeconds = 10)
    {
        //_recognitionService = recognitionService;
        _context = context;
        _logger = logger;
        _camera = camera;
        _checkIntervalSeconds = checkIntervalSeconds;
    }
    
    public async Task StartCaptureAsync(CancellationToken stoppingToken)
    {
        var ocrEngine = InitializeOcrEngine();
        using var predictorLicensePlate = InitializeYoloPredictor(Paths.YoloLicensePlatePath);
        using var predictorYolo = InitializeYoloPredictor(Paths.Yolo8Path);
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
                ProcessFrame(frame, predictorYolo, predictorLicensePlate, ocrEngine);
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

    private void SavePlateToDatabase(string plate)
    {
        var existingRecord = _context.LicensePlates
            .Where(p => p.PlateNumber == plate)
            .OrderByDescending(p => p.Time)
            .FirstOrDefault();
        
        if (existingRecord != null && (TimeOnly.FromDateTime(DateTime.Now) - existingRecord.Time).TotalMinutes < 5) return;
        
        var record = new LicensePlate{Date = DateOnly.FromDateTime(DateTime.Now), Time = TimeOnly.FromDateTime(DateTime.Now), PlateNumber = plate};
        _context.LicensePlates.Add(record);
        _context.SaveChanges();
    }
    
    private TesseractEngine InitializeOcrEngine()
    {
        var ocrEngine = new TesseractEngine(Paths.DataPath, "eng", EngineMode.Default);
        ocrEngine.SetVariable("tessedit_char_whitelist", "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ");
        return ocrEngine;
    }
    
    private IPredictor InitializeYoloPredictor(string modelPath)
    {
        return YoloV8Predictor.Create(modelPath);
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
    
    private void ProcessFrame(Image frame, IPredictor predictorYolo, IPredictor predictorLicensePlate, TesseractEngine ocrEngine)
    {
        var predictions = predictorYolo.Predict(frame);
        foreach (var predict in predictions)
        {
            if (predict.Label?.Name != "car") continue;
            var car = predict.Rectangle;
            var imgSharp = frame.Clone(ctx => ctx.Crop(ImageConverter.ConvertRectangleFToRectangleImageSharp(car)));
            var predictionPlates = predictorLicensePlate.Predict(imgSharp);
            foreach (var predictPlate in predictionPlates)
            {
                var test = ImageConverter.ImageSharpToBitmap(imgSharp);
                var test1 = test.ToMat();
                var plateRectangle = predictPlate.Rectangle;
                var plateTesseract = ImageConverter.ConvertRectangleFToRectangle(plateRectangle);
                var plateImageLP = ImageConverter.ConvertBitmapToPix(test); //new Mat(test1, plateTesseract);

                //ocrEngine.Process(plateImageLP);
                //ocrEngine.Recognize();
                using (var plate = ocrEngine.Process(plateImageLP))
                {
                    SavePlateToDatabase(plate.GetText());
                }
            }
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
