using OpenChat.PlaygroundApp.Abstractions;

namespace OpenChat.PlaygroundApp.Options;

/// <summary>
/// This represents the argument options entity for Azure AI Foundry.
/// </summary>
public class AzureAIFoundryArgumentOptions : ArgumentOptions
{
    /// <summary>
    /// Gets or sets the endpoint URL for Azure AI Foundry API.
    /// </summary>
    public string? Endpoint { get; set; }

    /// <summary>
    /// Gets or sets the personal access token for Azure AI Foundry.
    /// </summary>
    public string? ApiKey { get; set; }

    /// <summary>
    /// Gets or sets the model name of Azure AI Foundry.
    /// </summary>
    public string? DeploymentName { get; set; }
}
