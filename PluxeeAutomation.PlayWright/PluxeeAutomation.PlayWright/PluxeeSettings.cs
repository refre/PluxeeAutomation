namespace PluxeeAutomation.PlayWright;

public class PluxeeSettings
{
    public string BrowserExecutablePath { get; set; } = string.Empty;
    public string PluxeeUrl { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public DateTime? CustomDate { get; set; }
}
