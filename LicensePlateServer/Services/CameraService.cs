using LicensePlateDataShared.Models;
using LicensePlateServer.Data;

namespace LicensePlateServer.Services;

public class CameraService : ICameraService
{
    private readonly LicensePlateDbContext _context;

    public CameraService(LicensePlateDbContext context)
    {
        _context = context;
    }

    public IEnumerable<Camera> GetAllCameras()
    {
        return _context.Cameras.ToList();
    }
}