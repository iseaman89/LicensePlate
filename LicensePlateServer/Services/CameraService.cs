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

    /// <summary>
    /// Gets all cameras from the database.
    /// </summary>
    /// <returns>An enumerable collection of <see cref="Camera"/> objects.</returns>
    public IEnumerable<Camera> GetAllCameras()
    {
        return _context.Cameras.ToList();
    }
}