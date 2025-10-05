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

    /// <inheritdoc/>
    protected override void ParseOptions(IConfiguration config, string[] args)
    {
        var settings = new AppSettings();
        config.Bind(settings);

        var foundryLocal = settings.FoundryLocal;

        this.Alias ??= foundryLocal?.Alias;

        for (var i = 0; i < args.Length; i++)
        {
            switch (args[i])
            {
                case var _ when args[i] == ArgumentOptionConstants.FoundryLocal.Alias:
                    if (i + 1 < args.Length)
                    {
                        this.Alias = args[++i];
                    }
                    break;

                default:
                    break;
            }
        }
    }
}