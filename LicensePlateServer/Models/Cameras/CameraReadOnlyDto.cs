namespace LicensePlateServer.Models.Cameras;

public class CameraReadOnlyDto : BaseDto
{
    public string IpAddress { get; set; }
    public string Name { get; set; }
    public string Login { get; set; }
    public string Password { get; set; }
}