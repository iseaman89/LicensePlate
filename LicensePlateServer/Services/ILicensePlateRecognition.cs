using SixLabors.ImageSharp;

namespace LicensePlateServer.Services;

public interface ILicensePlateRecognition
{
    void ProcessFrame(Image frame);
}