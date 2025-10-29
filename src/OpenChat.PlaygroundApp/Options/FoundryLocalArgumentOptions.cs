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
    /// Gets or sets the base URL of Foundry Local. If `DisableFoundryLocalManager` is set, this value must be provided.
    /// </summary>
    public string? BaseUrl { get; set; }

    /// <summary>
    /// Gets or sets either alias or model ID of Foundry Local.
    /// </summary>
    public string? AliasOrModel { get; set; }

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

        this.BaseUrl ??= foundryLocal?.BaseUrl;
        this.AliasOrModel ??= foundryLocal?.AliasOrModel;
        this.DisableFoundryLocalManager = foundryLocal?.DisableFoundryLocalManager ?? false;

        for (var i = 0; i < args.Length; i++)
        {
            switch (args[i])
            {
                case ArgumentOptionConstants.FoundryLocal.BaseUrl:
                    if (i + 1 < args.Length)
                    {
                        this.BaseUrl = args[++i];
                    }
                    break;

                case ArgumentOptionConstants.FoundryLocal.Alias:
                case ArgumentOptionConstants.FoundryLocal.Model:
                    if (i + 1 < args.Length)
                    {
                        this.AliasOrModel = args[++i];
                    }
                    break;

                case ArgumentOptionConstants.FoundryLocal.DisableFoundryLocalManager:
                case ArgumentOptionConstants.FoundryLocal.DisableFoundryLocalManagerInShort:
                    this.DisableFoundryLocalManager = true;
                    break;

                default:
                    break;
            }
        }
    }
}