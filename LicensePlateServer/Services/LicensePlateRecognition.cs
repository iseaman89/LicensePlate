using System.Drawing;
using LicensePlateDataShared.Models;
using LicensePlateDataShared.Static;
using LicensePlateServer.Data;
using SixLabors.ImageSharp.Processing;
using Tesseract;
using Yolov8Net;
using Image = SixLabors.ImageSharp.Image;
using ImageConverter = LicensePlateServer.Statics.ImageConverter;
using ImageFormat = System.Drawing.Imaging.ImageFormat;

namespace LicensePlateServer.Services;

public class LicensePlateRecognition : ILicensePlateRecognition
{
    private readonly LicensePlateDbContext _context;
    private TesseractEngine _ocrEngine;
    private IPredictor _predictorLicensePlate;
    private IPredictor _predictorYolo;

    public LicensePlateRecognition(LicensePlateDbContext context)
    {
        _context = context;
        _ocrEngine = InitializeOcrEngine();
        _predictorLicensePlate = InitializeYoloPredictor(Paths.YoloLicensePlatePath);
        _predictorYolo = InitializeYoloPredictor(Paths.Yolo8Path);
    }
    
    private TesseractEngine InitializeOcrEngine()
    {
        return null;
        var ocrEngine = new TesseractEngine(Paths.DataPath, "eng", EngineMode.Default);
        ocrEngine.SetVariable("tessedit_char_whitelist", "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ");
        return ocrEngine;
    }
    
    private IPredictor InitializeYoloPredictor(string modelPath)
    {
        return null;
        return YoloV8Predictor.Create(modelPath);
    }
    
    public void ProcessFrame(Image frame)
    {
        var predictions = _predictorYolo.Predict(frame);
        foreach (var predict in predictions)
        {
            if (predict.Label?.Name != "car") continue;
            var carRectangle = predict.Rectangle;
            var carImage = frame.Clone(ctx => ctx.Crop(Statics.ImageConverter.ConvertRectangleFToRectangleImageSharp(carRectangle)));
            var predictionPlates = _predictorLicensePlate.Predict(carImage);
            foreach (var predictPlate in predictionPlates)
            {
                var carBitmap = Statics.ImageConverter.ImageSharpToBitmap(carImage);
                var plateRectangle = predictPlate.Rectangle;
                var platePix = ImageConverter.ConvertBitmapToPix(carBitmap);

                using (var plate = _ocrEngine.Process(platePix))
                {
                    SavePlateToDatabase(plate.GetText(), carBitmap);
                }
            }
        }
    }
    
    private void SavePlateToDatabase(string plate, Bitmap image)
    {
        var existingRecord = _context.LicensePlates
            .Where(p => p.PlateNumber == plate)
            .OrderByDescending(p => p.Time)
            .FirstOrDefault();
        
        if (existingRecord != null && (TimeOnly.FromDateTime(DateTime.Now) - existingRecord.Time).TotalMinutes < 5) return;

        var plateWithoutSpaces = plate.Replace(" ", String.Empty);
        
        image.Save(Paths.ImagePath + plateWithoutSpaces + ".jpeg", ImageFormat.Jpeg);
        
        var record = new LicensePlate
        {
            Date = DateOnly.FromDateTime(DateTime.Now),
            Time = TimeOnly.FromDateTime(DateTime.Now),
            PlateNumber = plate,
            Image = Paths.ImagePath + plateWithoutSpaces + ".jpeg"
        };
        _context.LicensePlates.Add(record);
        _context.SaveChanges();
    }
}