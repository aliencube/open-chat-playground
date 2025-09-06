using OpenChat.PlaygroundApp.Abstractions;
using OpenChat.PlaygroundApp.Configurations;

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

    /// <inheritdoc/>
    protected override void ParseOptions(IConfiguration config, string[] args)
    {
        var settings = new AppSettings();
        config.Bind(settings);

        var azureAIFoundry = settings.AzureAIFoundry;

        this.Endpoint ??= azureAIFoundry?.Endpoint;
        this.ApiKey ??= azureAIFoundry?.ApiKey;
        this.DeploymentName ??= azureAIFoundry?.DeploymentName;

        for (var i = 0; i < args.Length; i++)
        {
            switch (args[i])
            {
                case "--endpoint":
                    if (i + 1 < args.Length)
                    {
                        this.Endpoint = args[++i];
                    }
                    break;

                case "--api-key":
                    if (i + 1 < args.Length)
                    {
                        this.ApiKey = args[++i];
                    }
                    break;

                case "--deployment-name":
                    if (i + 1 < args.Length)
                    {
                        this.DeploymentName = args[++i];
                    }
                    break;

                default:
                    break;
            }
        }
    }
}
