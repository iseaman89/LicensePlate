using LicensePlateDataShared.Models;
using LicensePlateServer.Data;

namespace LicensePlateServer.Repositories;

public class LicensePlateRepository : GenericRepository<LicensePlate>, ILicensePlateRepository
{
    public LicensePlateRepository(LicensePlateDbContext context) : base(context)
    {
    }
}