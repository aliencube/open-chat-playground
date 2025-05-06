namespace OpenChat.Common.Configurations;

/// <summary>
/// This represents the entity for the OpenAI settings in the 'appsettings.json' file.
/// </summary>
public class OpenAISettings
{
    /// <summary>
    /// Gets or sets the deployment name for the OpenAI model. Default is 'gpt-4o'.
    /// </summary>
    public string DeploymentName { get; set; } = "gpt-4o";

    /// <summary>
    /// Gets or sets the connection string for the OpenAI model.
    /// </summary>
    public string ConnectionString { get; set; } = string.Empty;
}
