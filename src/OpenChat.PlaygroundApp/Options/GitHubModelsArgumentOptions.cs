using OpenChat.PlaygroundApp.Abstractions;
using OpenChat.PlaygroundApp.Configurations;
using OpenChat.PlaygroundApp.Constants;

namespace OpenChat.PlaygroundApp.Options;

/// <summary>
/// This represents the argument options entity for GitHub Models.
/// </summary>
public class GitHubModelsArgumentOptions : ArgumentOptions
{
    /// <summary>
    /// Gets or sets the endpoint URL for GitHub Models API.
    /// </summary>
    public string? Endpoint { get; set; }

    /// <summary>
    /// Gets or sets the personal access token for GitHub Models.
    /// </summary>
    public string? Token { get; set; }

    /// <summary>
    /// Gets or sets the model name of GitHub Models.
    /// </summary>
    public string? Model { get; set; }

    /// <inheritdoc/>
    protected override void ParseOptions(IConfiguration config, string[] args)
    {
        var settings = new AppSettings();
        config.Bind(settings);

        var github = settings.GitHubModels;

        this.Endpoint ??= github?.Endpoint;
        this.Token ??= github?.Token;
        this.Model ??= github?.Model;

        for (var i = 0; i < args.Length; i++)
        {
            switch (args[i])
            {
                case var _ when args[i] == ArgumentOptionConstants.GitHubModels.Endpoint:
                    if (i + 1 < args.Length)
                    {
                        this.Endpoint = args[++i];
                    }
                    break;

                case var _ when args[i] == ArgumentOptionConstants.GitHubModels.Token:
                    if (i + 1 < args.Length)
                    {
                        this.Token = args[++i];
                    }
                    break;

                case var _ when args[i] == ArgumentOptionConstants.GitHubModels.Model:
                    if (i + 1 < args.Length)
                    {
                        this.Model = args[++i];
                    }
                    break;

                default:
                    break;
            }
        }
    }
}
