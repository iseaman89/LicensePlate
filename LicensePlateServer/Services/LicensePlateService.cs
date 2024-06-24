using LicensePlateDataShared.Models;
using LicensePlateServer.Data;

namespace LicensePlateServer.Services;

public class LicensePlateService : ILicensePlateService
{
    private readonly IServiceScopeFactory _serviceProvider;

    public LicensePlateService(IServiceScopeFactory serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }
  
    
    public void SavePlateToDatabase(LicensePlate licensePlate)
    {
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<LicensePlateDbContext>();
        var existingRecord = context.LicensePlates
            .Where(p => p.PlateNumber == licensePlate.PlateNumber)
            .Where(p => p.Date == licensePlate.Date)
            .OrderByDescending(p => p.Time)
            .FirstOrDefault();
        
        if (existingRecord != null && 
            (TimeOnly.FromDateTime(DateTime.Now) - existingRecord.Time).TotalMinutes < 5) return;
        
        context.LicensePlates.Add(licensePlate);
        context.SaveChanges();
    }
}