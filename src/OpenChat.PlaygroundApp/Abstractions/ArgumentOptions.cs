using OpenChat.PlaygroundApp.Configurations;
using OpenChat.PlaygroundApp.Connectors;
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
        // Azure AI Foundry
        (ConnectorType.AzureAIFoundry, "--endpoint", false),
        (ConnectorType.AzureAIFoundry, "--api-key", false),
        (ConnectorType.AzureAIFoundry, "--deployment-name", false),
        // GitHub Models
        (ConnectorType.GitHubModels, "--endpoint", false),
        (ConnectorType.GitHubModels, "--token", false),
        (ConnectorType.GitHubModels, "--model", false),
        // Google Vertex AI
        (ConnectorType.GoogleVertexAI, "--api-key", false),
        (ConnectorType.GoogleVertexAI, "--model", false),
        // Docker Model Runner
        // Foundry Local
        (ConnectorType.FoundryLocal, "--alias", false),
        // Hugging Face
        (ConnectorType.HuggingFace, "--base-url", false),
        (ConnectorType.HuggingFace, "--model", false),
        // Ollama
        (ConnectorType.Ollama, "--base-url", false),
        (ConnectorType.Ollama, "--model", false),
        // Anthropic
        (ConnectorType.Anthropic, "--api-key", false),
        (ConnectorType.Anthropic, "--model", false),
        // LG
        // Naver
        // OpenAI
        (ConnectorType.OpenAI, "--api-key", false),
        (ConnectorType.OpenAI, "--model", false),
        // Upstage
        (ConnectorType.Upstage, "--base-url", false),
        (ConnectorType.Upstage, "--api-key", false),
        (ConnectorType.Upstage, "--model", false)
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
        var connectorType = Enum.TryParse<ConnectorType>(config["ConnectorType"], ignoreCase: true, out var result) ? result : ConnectorType.Unknown;
        for (var i = 0; i < args.Length; i++)
        {
            if (string.Equals(args[i], "--connector-type", StringComparison.InvariantCultureIgnoreCase) ||
                string.Equals(args[i], "-c", StringComparison.InvariantCultureIgnoreCase))
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
                case "--connector-type":
                case "-c":
                    if (i + 1 < args.Length)
                    {
                        if (Enum.TryParse<ConnectorType>(args[++i], ignoreCase: true, out var result))
                        {
                            options.ConnectorType = result;
                        }
                    }
                    break;

                case "--help":
                case "-h":
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
            // case AmazonBedrockArgumentOptions amazonBedrock:
            //     break;

            case AzureAIFoundryArgumentOptions azureAIFoundry:
                settings.AzureAIFoundry ??= new AzureAIFoundrySettings();
                settings.AzureAIFoundry.Endpoint = azureAIFoundry.Endpoint ?? settings.AzureAIFoundry.Endpoint;
                settings.AzureAIFoundry.ApiKey = azureAIFoundry.ApiKey ?? settings.AzureAIFoundry.ApiKey;
                settings.AzureAIFoundry.DeploymentName = azureAIFoundry.DeploymentName ?? settings.AzureAIFoundry.DeploymentName;
                break;

            case GitHubModelsArgumentOptions github:
                settings.GitHubModels ??= new GitHubModelsSettings();
                settings.GitHubModels.Endpoint = github.Endpoint ?? settings.GitHubModels.Endpoint;
                settings.GitHubModels.Token = github.Token ?? settings.GitHubModels.Token;
                settings.GitHubModels.Model = github.Model ?? settings.GitHubModels.Model;
                break;
            
            case GoogleVertexAIArgumentOptions googleVertexAI:
                settings.GoogleVertexAI ??= new GoogleVertexAISettings();
                settings.GoogleVertexAI.ApiKey = googleVertexAI.ApiKey ?? settings.GoogleVertexAI.ApiKey;
                settings.GoogleVertexAI.Model = googleVertexAI.Model ?? settings.GoogleVertexAI.Model;
                break;

            // case DockerModelRunnerArgumentOptions dockerModelRunner:
            //     break;

            case FoundryLocalArgumentOptions foundryLocal:
                settings.FoundryLocal ??= new FoundryLocalSettings();
                settings.FoundryLocal.Alias = foundryLocal.Alias ?? settings.FoundryLocal.Alias;
                break;

            case HuggingFaceArgumentOptions huggingFace:
                settings.HuggingFace ??= new HuggingFaceSettings();
                settings.HuggingFace.BaseUrl = huggingFace.BaseUrl ?? settings.HuggingFace.BaseUrl;
                settings.HuggingFace.Model = huggingFace.Model ?? settings.HuggingFace.Model;
                break;
            
            case OllamaArgumentOptions ollama:
                settings.Ollama ??= new OllamaSettings();
                settings.Ollama.BaseUrl = ollama.BaseUrl ?? settings.Ollama.BaseUrl;
                settings.Ollama.Model = ollama.Model ?? settings.Ollama.Model;
                break;

            case AnthropicArgumentOptions anthropic:
                settings.Anthropic ??= new AnthropicSettings();
                settings.Anthropic.ApiKey = anthropic.ApiKey ?? settings.Anthropic.ApiKey;
                settings.Anthropic.Model = anthropic.Model ?? settings.Anthropic.Model;
                break;

            // case LGArgumentOptions lg:
            //     break;

            // case NaverArgumentOptions naver:
            //     break;

            case OpenAIArgumentOptions openai:
                settings.OpenAI ??= new OpenAISettings();
                settings.OpenAI.ApiKey = openai.ApiKey ?? settings.OpenAI.ApiKey;
                settings.OpenAI.Model = openai.Model ?? settings.OpenAI.Model;
                break;

            case UpstageArgumentOptions upstage:
                settings.Upstage ??= new UpstageSettings();
                settings.Upstage.BaseUrl = upstage.BaseUrl ?? settings.Upstage.BaseUrl;
                settings.Upstage.ApiKey = upstage.ApiKey ?? settings.Upstage.ApiKey;
                settings.Upstage.Model = upstage.Model ?? settings.Upstage.Model;
                break;

            default:
                break;
        }

        settings.ConnectorType = options.ConnectorType;
        settings.Help = options.ShouldDisplayHelp(optionsType);

        return settings;
    }

    /// <summary>
    /// Displays the help information for the command line arguments.
    /// </summary>
    public static void DisplayHelp()
    {
        var foregroundColor = Console.ForegroundColor;

        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("OpenChat Playground");
        Console.ForegroundColor = foregroundColor;

        Console.WriteLine("Usage: [options]");
        Console.WriteLine();
        Console.WriteLine("Options:");
        Console.WriteLine("  --connector-type|-c  The connector type. Supporting connectors are:");
        Console.WriteLine("                       - AmazonBedrock, AzureAIFoundry, GitHubModels, GoogleVertexAI");
        Console.WriteLine("                       - DockerModelRunner, FoundryLocal, HuggingFace, Ollama");
        Console.WriteLine("                       - Anthropic, LG, Naver, OpenAI, Upstage");
        Console.WriteLine();
        DisplayHelpForAmazonBedrock();
        DisplayHelpForAzureAIFoundry();
        DisplayHelpForGitHubModels();
        DisplayHelpForGoogleVertexAI();
        DisplayHelpForDockerModelRunner();
        DisplayHelpForFoundryLocal();
        DisplayHelpForHuggingFace();
        DisplayHelpForOllama();
        DisplayHelpForAnthropic();
        DisplayHelpForLG();
        DisplayHelpForNaver();
        DisplayHelpForOpenAI();
        DisplayHelpForUpstage();
        Console.WriteLine("  --help|-h            Show this help message.");
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

    private static void DisplayHelpForAmazonBedrock()
    {
        var foregroundColor = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Magenta;
        Console.WriteLine("  ** Amazon Bedrock: **");
        Console.ForegroundColor = foregroundColor;

        Console.WriteLine("  TBD");
        Console.WriteLine();
    }

    private static void DisplayHelpForAzureAIFoundry()
    {
        var foregroundColor = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Magenta;
        Console.WriteLine("  ** Azure AI Foundry: **");
        Console.ForegroundColor = foregroundColor;

        Console.WriteLine("  --endpoint           The Azure AI Foundry endpoint.");
        Console.WriteLine("  --api-key            The Azure AI Foundry API key.");
        Console.WriteLine("  --deployment-name    The deployment name. Default to 'gpt-4o-mini'");
        Console.WriteLine();
    }

    private static void DisplayHelpForGitHubModels()
    {
        var foregroundColor = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Magenta;
        Console.WriteLine("  ** GitHub Models: **");
        Console.ForegroundColor = foregroundColor;

        Console.WriteLine("  --endpoint           The endpoint URL. Default to 'https://models.github.ai/inference'");
        Console.WriteLine("  --token              The GitHub PAT.");
        Console.WriteLine("  --model              The model name. Default to 'openai/gpt-4o-mini'");
        Console.WriteLine();
    }

    private static void DisplayHelpForGoogleVertexAI()
    {
        var foregroundColor = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Magenta;
        Console.WriteLine("  ** Google Vertex AI: **");
        Console.ForegroundColor = foregroundColor;

        Console.WriteLine("  TBD");
        Console.WriteLine();
    }

    private static void DisplayHelpForDockerModelRunner()
    {
        var foregroundColor = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.DarkYellow;
        Console.WriteLine("  ** Docker Model Runner: **");
        Console.ForegroundColor = foregroundColor;

        Console.WriteLine("  TBD");
        Console.WriteLine();
    }

    private static void DisplayHelpForFoundryLocal()
    {
        var foregroundColor = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.DarkYellow;
        Console.WriteLine("  ** Foundry Local: **");
        Console.ForegroundColor = foregroundColor;

        Console.WriteLine("  TBD");
        Console.WriteLine();
    }

    private static void DisplayHelpForHuggingFace()
    {
        var foregroundColor = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.DarkYellow;
        Console.WriteLine("  ** Hugging Face: **");
        Console.ForegroundColor = foregroundColor;

        Console.WriteLine("  --base-url           The endpoint URL. Default to 'http://localhost:11434'");
        Console.WriteLine("  --model              The model name. Default to 'hf.co/google/gemma-3-1b-pt-qat-q4_0-gguf'");
        Console.WriteLine();
    }

    private static void DisplayHelpForOllama()
    {
        var foregroundColor = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.DarkYellow;
        Console.WriteLine("  ** Ollama: **");
        Console.ForegroundColor = foregroundColor;

        Console.WriteLine("  --base-url           The baseURL. Default to 'http://localhost:11434'");
        Console.WriteLine("  --model              The model name. Default to 'llama3.2'");
        Console.WriteLine();
    }

    private static void DisplayHelpForAnthropic()
    {
        var foregroundColor = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.DarkGreen;
        Console.WriteLine("  ** Anthropic: **");
        Console.ForegroundColor = foregroundColor;

        Console.WriteLine("  --api-key            The Anthropic API key.");
        Console.WriteLine("  --model              The Anthropic model name. Default to 'claude-sonnet-4-0'");
        Console.WriteLine();
    }

    private static void DisplayHelpForLG()
    {
        var foregroundColor = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.DarkGreen;
        Console.WriteLine("  ** LG: **");
        Console.ForegroundColor = foregroundColor;

        Console.WriteLine("  TBD");
        Console.WriteLine();
    }

    private static void DisplayHelpForNaver()
    {
        var foregroundColor = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.DarkGreen;
        Console.WriteLine("  ** Naver: **");
        Console.ForegroundColor = foregroundColor;

        Console.WriteLine("  TBD");
        Console.WriteLine();
    }

    private static void DisplayHelpForOpenAI()
    {
        var foregroundColor = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.DarkGreen;
        Console.WriteLine("  ** OpenAI: **");
        Console.ForegroundColor = foregroundColor;

        Console.WriteLine("  --api-key            The OpenAI API key. (Env: OPENAI_API_KEY)");
        Console.WriteLine("  --model              The OpenAI model name. Default to 'gpt-4.1-mini'");
        Console.WriteLine();
    }

    private static void DisplayHelpForUpstage()
    {
        var foregroundColor = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.DarkGreen;
        Console.WriteLine("  ** Upstage: **");
        Console.ForegroundColor = foregroundColor;

        Console.WriteLine("  --base-url           The base URL for Upstage API. Default to 'https://api.upstage.ai/v1/solar'");
        Console.WriteLine("  --api-key            The Upstage API key.");
        Console.WriteLine("  --model              The model name. Default to 'solar-mini'");
        Console.WriteLine();
    }
}
