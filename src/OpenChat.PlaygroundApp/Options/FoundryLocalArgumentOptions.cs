using OpenChat.PlaygroundApp.Abstractions;
using OpenChat.PlaygroundApp.Configurations;
using OpenChat.PlaygroundApp.Constants;

namespace OpenChat.PlaygroundApp.Options;

/// <summary>
/// This represents the argument options entity for Foundry Local.
/// </summary>
public class FoundryLocalArgumentOptions : ArgumentOptions
{
    /// <summary>
    /// Gets or sets the alias of Foundry Local.
    /// </summary>
    public string? Alias { get; set; }

    /// <summary>
    /// Gets or sets the Endpoint of FoundryLocal.
    /// </summary>
    public string? Endpoint { get; set; }

    /// <summary>
    /// Gets or sets the model ID of FoundryLocal.
    /// </summary>
    public string? ModelId { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to disable the automatic FoundryLocal manager and use a manually configured endpoint.
    /// </summary>
    public bool DisableFoundryLocalManager { get; set; }

    /// <inheritdoc/>
    protected override void ParseOptions(IConfiguration config, string[] args)
    {
        var settings = new AppSettings();
        config.Bind(settings);

        var foundryLocal = settings.FoundryLocal;

        this.Alias ??= foundryLocal?.Alias;
        this.Endpoint ??= foundryLocal?.Endpoint;
        this.ModelId ??= foundryLocal?.ModelId;
        this.DisableFoundryLocalManager = foundryLocal?.DisableFoundryLocalManager ?? false;

        for (var i = 0; i < args.Length; i++)
        {
            switch (args[i])
            {
                case ArgumentOptionConstants.FoundryLocal.Alias:
                    if (i + 1 < args.Length)
                    {
                        this.Alias = args[++i];
                    }
                    break;

                case ArgumentOptionConstants.FoundryLocal.Endpoint:
                    if (i + 1 < args.Length)
                    {
                        this.Endpoint = args[++i];
                    }
                    break;

                case ArgumentOptionConstants.FoundryLocal.ModelId:
                    if (i + 1 < args.Length)
                    {
                        this.ModelId = args[++i];
                    }
                    break;

                case ArgumentOptionConstants.FoundryLocal.DisableFoundryLocalManager:
                    this.DisableFoundryLocalManager = true;
                    break;

                default:
                    break;
            }
        }
    }
}