using LicensePlateDataShared.Models;
using LicensePlateServer.Data;

namespace LicensePlateServer.Repositories;

public class CameraRepository : GenericRepository<Camera>, ICameraRepository
{
    public CameraRepository(LicensePlateDbContext context) : base(context)
    {
    }
}