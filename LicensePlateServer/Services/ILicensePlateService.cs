using LicensePlateDataShared.Models;
using SixLabors.ImageSharp;

namespace LicensePlateServer.Services;

public interface ILicensePlateService
{
    void SavePlateToDatabase(LicensePlate licensePlate);
}