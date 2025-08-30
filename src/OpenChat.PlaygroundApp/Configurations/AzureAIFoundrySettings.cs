using OpenChat.PlaygroundApp.Abstractions;

namespace OpenChat.PlaygroundApp.Configurations;

/// <inheritdoc/>
public partial class AppSettings
{
    /// <summary>
    /// Gets or sets the <see cref="AzureAIFoundrySettings"/> instance.
    /// </summary>
    public AzureAIFoundrySettings? AzureAIFoundry { get; set; }
}

/// <summary>
/// This represents the app settings entity for Azure AI Foundry.
/// </summary>
public class AzureAIFoundrySettings : LanguageModelSettings
{
    /// <summary>
    /// Gets or sets the endpoint URL of Azure AI Foundry API.
    /// </summary>
    public string? Endpoint { get; set; }

    /// <summary>
    /// Gets or sets the Azure AI Foundry API Access Token.
    /// </summary>
    public string? ApiKey { get; set; }

    /// <summary>
    /// Gets or sets the model name of Azure AI Foundry.
    /// </summary>
    public string? DeploymentName { get; set; }
}
