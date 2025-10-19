using OpenChat.PlaygroundApp.Configurations;
using OpenChat.PlaygroundApp.Connectors;
using OpenChat.PlaygroundApp.Constants;
using OpenChat.PlaygroundApp.Options;

namespace OpenChat.PlaygroundApp.Abstractions;

/// <summary>
/// This represents the base argument options settings entity for all arguments options to inherit.
/// </summary>
public abstract class ArgumentOptions
{
    private static readonly (ConnectorType ConnectorType, string Argument, bool IsSwitch)[] arguments =
    [
        // Amazon Bedrock
        (ConnectorType.AmazonBedrock, ArgumentOptionConstants.AmazonBedrock.AccessKeyId, false),
        (ConnectorType.AmazonBedrock, ArgumentOptionConstants.AmazonBedrock.SecretAccessKey, false),
        (ConnectorType.AmazonBedrock, ArgumentOptionConstants.AmazonBedrock.Region, false),
        (ConnectorType.AmazonBedrock, ArgumentOptionConstants.AmazonBedrock.ModelId, false),
        // Azure AI Foundry
        (ConnectorType.AzureAIFoundry, ArgumentOptionConstants.AzureAIFoundry.Endpoint, false),
        (ConnectorType.AzureAIFoundry, ArgumentOptionConstants.AzureAIFoundry.ApiKey, false),
        (ConnectorType.AzureAIFoundry, ArgumentOptionConstants.AzureAIFoundry.DeploymentName, false),
        // GitHub Models
        (ConnectorType.GitHubModels, ArgumentOptionConstants.GitHubModels.Endpoint, false),
        (ConnectorType.GitHubModels, ArgumentOptionConstants.GitHubModels.Token, false),
        (ConnectorType.GitHubModels, ArgumentOptionConstants.GitHubModels.Model, false),
        // Google Vertex AI
        (ConnectorType.GoogleVertexAI, ArgumentOptionConstants.GoogleVertexAI.ApiKey, false),
        (ConnectorType.GoogleVertexAI, ArgumentOptionConstants.GoogleVertexAI.Model, false),
        // Docker Model Runner
        // Foundry Local
        (ConnectorType.FoundryLocal, ArgumentOptionConstants.FoundryLocal.Alias, false),
        // Hugging Face
        (ConnectorType.HuggingFace, ArgumentOptionConstants.HuggingFace.BaseUrl, false),
        (ConnectorType.HuggingFace, ArgumentOptionConstants.HuggingFace.Model, false),
        // Ollama
        (ConnectorType.Ollama, ArgumentOptionConstants.Ollama.BaseUrl, false),
        (ConnectorType.Ollama, ArgumentOptionConstants.Ollama.Model, false),
        // Anthropic
        (ConnectorType.Anthropic, ArgumentOptionConstants.Anthropic.ApiKey, false),
        (ConnectorType.Anthropic, ArgumentOptionConstants.Anthropic.Model, false),
        // LG
        (ConnectorType.LG, ArgumentOptionConstants.LG.BaseUrl, false),
        (ConnectorType.LG, ArgumentOptionConstants.LG.Model, false),
        // Naver
        // OpenAI
        (ConnectorType.OpenAI, ArgumentOptionConstants.OpenAI.ApiKey, false),
        (ConnectorType.OpenAI, ArgumentOptionConstants.OpenAI.Model, false),
        // Upstage
        (ConnectorType.Upstage, ArgumentOptionConstants.Upstage.BaseUrl, false),
        (ConnectorType.Upstage, ArgumentOptionConstants.Upstage.ApiKey, false),
        (ConnectorType.Upstage, ArgumentOptionConstants.Upstage.Model, false)
    ];

    /// <summary>
    /// Gets or sets the connector type to use.
    /// </summary>
    public ConnectorType ConnectorType { get; set; }

    /// <summary>
    /// Gets or sets the value indicating whether to display help information or not.
    /// </summary>
    public bool Help { get; set; }

    /// <summary>
    /// Verifies the connector type from the configuration and command line arguments.
    /// </summary>
    /// <param name="config"><see cref="IConfiguration"/> instance.</param>
    /// <param name="args">List of arguments from the command line.</param>
    /// <returns>The verified <see cref="ConnectorType"/> value.</returns>
    public static ConnectorType VerifyConnectorType(IConfiguration config, string[] args)
    {
        var connectorType = Enum.TryParse<ConnectorType>(config[AppSettingConstants.ConnectorType], ignoreCase: true, out var result) ? result : ConnectorType.Unknown;
        for (var i = 0; i < args.Length; i++)
        {
            if (string.Equals(args[i], ArgumentOptionConstants.ConnectorType, StringComparison.InvariantCultureIgnoreCase) ||
                string.Equals(args[i], ArgumentOptionConstants.ConnectorTypeInShort, StringComparison.InvariantCultureIgnoreCase))
            {
                if (i + 1 < args.Length && Enum.TryParse<ConnectorType>(args[i + 1], ignoreCase: true, out result))
                {
                    connectorType = result;
                }
                break;
            }
        }

        return connectorType;
    }

    /// <summary>
    /// Parses the command line arguments into the specified options type.
    /// </summary>
    /// <param name="config"><see cref="IConfiguration"/> instance.</param>
    /// <param name="args">List of arguments from the command line.</param>
    /// <returns>The parsed options.</returns>
    public static AppSettings Parse(IConfiguration config, string[] args)
    {
        var settings = new AppSettings();
        config.Bind(settings);

        var connectorType = VerifyConnectorType(config, args);
        if (connectorType == ConnectorType.Unknown)
        {
            settings.ConnectorType = connectorType;
            settings.Help = true;

            return settings;
        }

        var expectedName = connectorType + "ArgumentOptions";

        var assembly = typeof(ArgumentOptions).Assembly;
        var optionsType = assembly?.GetTypes()
                                   .SingleOrDefault(t => typeof(ArgumentOptions).IsAssignableFrom(t) &&
                                                         string.Equals(t.Name, expectedName, StringComparison.InvariantCultureIgnoreCase))
                          ?? throw new InvalidOperationException($"Options type '{expectedName}' not found for connector type '{connectorType}'.");

        var options = (ArgumentOptions)Activator.CreateInstance(optionsType)!;
        options.ConnectorType = connectorType;

        options.ParseOptions(config, args);

        for (var i = 0; i < args.Length; i++)
        {
            switch (args[i])
            {
                case ArgumentOptionConstants.ConnectorType:
                case ArgumentOptionConstants.ConnectorTypeInShort:
                    if (i + 1 < args.Length)
                    {
                        if (Enum.TryParse<ConnectorType>(args[++i], ignoreCase: true, out var result))
                        {
                            options.ConnectorType = result;
                        }
                    }
                    break;

                case ArgumentOptionConstants.Help:
                case ArgumentOptionConstants.HelpInShort:
                    options.Help = true;
                    break;

                default:
                    var argument = arguments.SingleOrDefault(p => p.ConnectorType == connectorType &&
                                                                  p.Argument.Equals(args[i], StringComparison.InvariantCultureIgnoreCase));
                    if (argument == default)
                    {
                        options.Help = true;
                    }
                    else if (argument.IsSwitch == false)
                    {
                        i++;
                    }
                    break;
            }
        }

        switch (options)
        {
            case AmazonBedrockArgumentOptions amazonBedrock:
                settings.AmazonBedrock ??= new AmazonBedrockSettings();
                settings.AmazonBedrock.AccessKeyId = amazonBedrock.AccessKeyId ?? settings.AmazonBedrock.AccessKeyId;
                settings.AmazonBedrock.SecretAccessKey = amazonBedrock.SecretAccessKey ?? settings.AmazonBedrock.SecretAccessKey;
                settings.AmazonBedrock.Region = amazonBedrock.Region ?? settings.AmazonBedrock.Region;
                settings.AmazonBedrock.ModelId = amazonBedrock.ModelId ?? settings.AmazonBedrock.ModelId;

                settings.Model = amazonBedrock.ModelId ?? settings.AmazonBedrock.ModelId;
                break;

            case AzureAIFoundryArgumentOptions azureAIFoundry:
                settings.AzureAIFoundry ??= new AzureAIFoundrySettings();
                settings.AzureAIFoundry.Endpoint = azureAIFoundry.Endpoint ?? settings.AzureAIFoundry.Endpoint;
                settings.AzureAIFoundry.ApiKey = azureAIFoundry.ApiKey ?? settings.AzureAIFoundry.ApiKey;
                settings.AzureAIFoundry.DeploymentName = azureAIFoundry.DeploymentName ?? settings.AzureAIFoundry.DeploymentName;

                settings.Model = azureAIFoundry.DeploymentName ?? settings.AzureAIFoundry.DeploymentName;
                break;

            case GitHubModelsArgumentOptions github:
                settings.GitHubModels ??= new GitHubModelsSettings();
                settings.GitHubModels.Endpoint = github.Endpoint ?? settings.GitHubModels.Endpoint;
                settings.GitHubModels.Token = github.Token ?? settings.GitHubModels.Token;
                settings.GitHubModels.Model = github.Model ?? settings.GitHubModels.Model;

                settings.Model = github.Model ?? settings.GitHubModels.Model;
                break;

            case GoogleVertexAIArgumentOptions googleVertexAI:
                settings.GoogleVertexAI ??= new GoogleVertexAISettings();
                settings.GoogleVertexAI.ApiKey = googleVertexAI.ApiKey ?? settings.GoogleVertexAI.ApiKey;
                settings.GoogleVertexAI.Model = googleVertexAI.Model ?? settings.GoogleVertexAI.Model;

                settings.Model = googleVertexAI.Model ?? settings.GoogleVertexAI.Model;
                break;

            // case DockerModelRunnerArgumentOptions dockerModelRunner:
            //     break;

            case FoundryLocalArgumentOptions foundryLocal:
                settings.FoundryLocal ??= new FoundryLocalSettings();
                settings.FoundryLocal.Alias = foundryLocal.Alias ?? settings.FoundryLocal.Alias;

                settings.Model = foundryLocal.Alias ?? settings.FoundryLocal.Alias;
                break;

            case HuggingFaceArgumentOptions huggingFace:
                settings.HuggingFace ??= new HuggingFaceSettings();
                settings.HuggingFace.BaseUrl = huggingFace.BaseUrl ?? settings.HuggingFace.BaseUrl;
                settings.HuggingFace.Model = huggingFace.Model ?? settings.HuggingFace.Model;

                settings.Model = huggingFace.Model ?? settings.HuggingFace.Model;
                break;

            case OllamaArgumentOptions ollama:
                settings.Ollama ??= new OllamaSettings();
                settings.Ollama.BaseUrl = ollama.BaseUrl ?? settings.Ollama.BaseUrl;
                settings.Ollama.Model = ollama.Model ?? settings.Ollama.Model;

                settings.Model = ollama.Model ?? settings.Ollama.Model;
                break;

            case AnthropicArgumentOptions anthropic:
                settings.Anthropic ??= new AnthropicSettings();
                settings.Anthropic.ApiKey = anthropic.ApiKey ?? settings.Anthropic.ApiKey;
                settings.Anthropic.Model = anthropic.Model ?? settings.Anthropic.Model;

                settings.Model = anthropic.Model ?? settings.Anthropic.Model;
                break;

            case LGArgumentOptions lg:
                settings.LG ??= new LGSettings();
                settings.LG.BaseUrl = lg.BaseUrl ?? settings.LG.BaseUrl;
                settings.LG.Model = lg.Model ?? settings.LG.Model;

                settings.Model = lg.Model ?? settings.LG.Model;
                break;

            // case NaverArgumentOptions naver:
            //     break;

            case OpenAIArgumentOptions openai:
                settings.OpenAI ??= new OpenAISettings();
                settings.OpenAI.ApiKey = openai.ApiKey ?? settings.OpenAI.ApiKey;
                settings.OpenAI.Model = openai.Model ?? settings.OpenAI.Model;

                settings.Model = openai.Model ?? settings.OpenAI.Model;
                break;

            case UpstageArgumentOptions upstage:
                settings.Upstage ??= new UpstageSettings();
                settings.Upstage.BaseUrl = upstage.BaseUrl ?? settings.Upstage.BaseUrl;
                settings.Upstage.ApiKey = upstage.ApiKey ?? settings.Upstage.ApiKey;
                settings.Upstage.Model = upstage.Model ?? settings.Upstage.Model;

                settings.Model = upstage.Model ?? settings.Upstage.Model;
                break;

            default:
                break;
        }

        settings.ConnectorType = options.ConnectorType;
        settings.Help = options.ShouldDisplayHelp(optionsType);

        return settings;
    }

    /// <summary>
    /// Parses the command line arguments into the specified options type.
    /// </summary>
    /// <param name="config"><see cref="IConfiguration"/> instance.</param>
    /// <param name="args">List of arguments from the command line.</param>
    protected virtual void ParseOptions(IConfiguration config, string[] args)
    {
        return;
    }

    /// <summary>
    /// Determines whether to display help information based on the options provided.
    /// </summary>
    /// <param name="type">The type of the options to parse into.</param>
    /// <returns></returns>
    protected virtual bool ShouldDisplayHelp(Type type)
    {
        return this.ConnectorType == ConnectorType.Unknown || this.Help;
    }
}
