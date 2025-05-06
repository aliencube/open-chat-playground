using Microsoft.Extensions.Configuration;

namespace OpenChat.Common.Configurations;

/// <summary>
/// This represents the entity for the 'appsettings.json' file.
/// </summary>
public class AppSettings
{
    /// <summary>
    /// Gets or sets the <see cref="LLMSettings"/> instance.
    /// </summary>
    public LLMSettings LLM { get; set; } = new LLMSettings();

    /// <summary>
    /// Gets or sets the <see cref="OpenAISettings"/> instance.
    /// </summary>
    public OpenAISettings OpenAI { get; set; } = new OpenAISettings();

    /// <summary>
    /// Gets or sets the <see cref="OllamaSettings"/> instance.
    /// </summary>
    public OllamaSettings Ollama { get; set; } = new OllamaSettings();

    /// <summary>
    /// Gets or sets the value indicating whether to show help information or not.
    /// </summary>
    public bool Help { get; set; }

    /// <summary>
    /// Parses the command line arguments and configuration settings to create an instance of <see cref="AppSettings"/>.
    /// </summary>
    /// <param name="config">The <see cref="IConfiguration"/> instance.</param>
    /// <param name="args">The list of arguments passed from the command-line.</param>
    /// <returns>Returns the <see cref="AppSettings"/> instance.</returns>
    public static AppSettings Parse(IConfiguration config, string[] args)
    {
        var settings = new AppSettings();
        config.Bind(settings);

        settings.OpenAI.ConnectionString = config["ConnectionStrings:openai"] ?? string.Empty;

        if (args.Length == 0)
        {
            return settings;
        }

        if (args.Length == 1)
        {
            settings.Help = true;
            return settings;
        }

        for (var i = 0; i < args.Length; i++)
        {
            var arg = args[i];
            switch (arg)
            {
                case "--llm-provider":
                    if (i < args.Length - 1)
                    {
                        settings.LLM.Provider = args[++i];
                    }
                    break;

                case "--openai-deployment":
                    if (i < args.Length - 1)
                    {
                        settings.OpenAI.DeploymentName = args[++i];
                    }
                    break;

                case "--openai-connection-string":
                    if (i < args.Length - 1)
                    {
                        settings.OpenAI.ConnectionString = args[++i];
                    }
                    break;

                case "--ollama-image-tag":
                    if (i < args.Length - 1)
                    {
                        settings.Ollama.ImageTag = args[++i];
                    }
                    break;

                case "--ollama-use-gpu":
                    if (i < args.Length - 1)
                    {
                        settings.Ollama.UseGPU = bool.TryParse(args[++i], out var result) && result;
                    }
                    break;

                case "--ollama-deployment":
                case "--huggingface-deployment":
                    if (i < args.Length - 1)
                    {
                        settings.Ollama.DeploymentName = args[++i];
                    }
                    break;

                case "--ollama-model":
                case "--huggingface-model":
                    if (i < args.Length - 1)
                    {
                        settings.Ollama.ModelName = args[++i];
                    }
                    break;

                case "--help":
                default:
                    settings.Help = true;
                    break;
            }
        }

        if (settings.LLM.ProviderType == LLMProviderType.Ollama && string.IsNullOrWhiteSpace(settings.Ollama.DeploymentName))
        {
            settings.Ollama.DeploymentName = "llama";
        }
        if (settings.LLM.ProviderType == LLMProviderType.Ollama && string.IsNullOrWhiteSpace(settings.Ollama.ModelName))
        {
            settings.Ollama.ModelName = "llama3.2";
        }

        if (settings.LLM.ProviderType == LLMProviderType.HuggingFace && string.IsNullOrWhiteSpace(settings.Ollama.DeploymentName))
        {
            settings.Ollama.DeploymentName = "qwen3";
        }
        if (settings.LLM.ProviderType == LLMProviderType.HuggingFace && string.IsNullOrWhiteSpace(settings.Ollama.ModelName))
        {
            settings.Ollama.ModelName = "Qwen/Qwen3-14B-GGUF";
        }

        if (settings.LLM.ProviderType == LLMProviderType.HuggingFace && IsValidHuggingFaceModel(settings.Ollama.ModelName) != true)
        {
            settings.Help = true;
            return settings;
        }

        return settings;
    }

    private static bool IsValidHuggingFaceModel(string modelName)
    {
        return modelName.Contains('/') && modelName.EndsWith("GGUF", StringComparison.InvariantCultureIgnoreCase);
    }
}
