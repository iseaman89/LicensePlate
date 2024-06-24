namespace LicensePlateServer.Models.LicensePlates;

public class LicensePlateReadOnlyDto : BaseDto
{
    public DateOnly Date { get; set; }
    public TimeOnly Time { get; set; }
    public string? CameraName { get; set; }
    public string? PlateNumber { get; set; }
    public string? Image { get; set; }
}