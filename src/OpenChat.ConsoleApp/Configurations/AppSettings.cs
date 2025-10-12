namespace OpenChat.ConsoleApp.Configurations;

/// <summary>
/// This represents the app settings entity from appsettings.json.
/// </summary>
public partial class AppSettings
{
    /// <summary>
    /// Gets or sets the <see cref="ApiAppSettings"/> instance.
    /// </summary>
    public ApiAppSettings ApiApp { get; set; } = new();

    /// <summary>
    /// Gets or sets the value indicating whether to display help information or not.
    /// </summary>
    public bool Help { get; set; }
}

/// <summary>
/// This represents the API app settings entity from appsettings.json.
/// </summary>
public class ApiAppSettings
{
    /// <summary>
    /// Gets or sets the API endpoint.
    /// </summary>
    public string? Endpoint { get; set; }
}