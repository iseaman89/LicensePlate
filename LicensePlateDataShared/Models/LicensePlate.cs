namespace LicensePlateDataShared.Models;

public class LicensePlate
{
    public int Id { get; set; }
    public DateOnly Date { get; set; }
    public TimeOnly Time { get; set; }
    public string CameraName { get; set; }
    public string PlateNumber { get; set; }
    public string Image { get; set; }
}