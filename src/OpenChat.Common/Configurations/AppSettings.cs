using System.Text.Json.Serialization;

using Microsoft.Extensions.Configuration;

namespace OpenChat.Common.Configurations;

public class AppSettings
{
    public LLMSettings LLM { get; set; } = new LLMSettings();
    public OpenAISettings OpenAI { get; set; } = new OpenAISettings();
    public OllamaSettings Ollama { get; set; } = new OllamaSettings();
    public bool Help { get; set; }

    public static AppSettings Parse(IConfiguration config, string[] args)
    {
        var settings = new AppSettings();
        config.Bind(settings);

        settings.OpenAI.ConnectionString = config["ConnectionStrings:openai"] ?? string.Empty;

        if (args.Length < 2)
        {
            if (args.Length == 1 && args[0] == "--help")
            {
                settings.Help = true;

                return settings;
            }

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
                        settings.LLM.Provider = Enum.TryParse<LLMProviderType>(args[++i], ignoreCase: true, out var result) ? result : LLMProviderType.OpenAI;
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

                case "--ollama-use-huggingface-model":
                    if (i < args.Length - 1)
                    {
                        settings.Ollama.UseHuggingFaceModel = bool.TryParse(args[++i], out var result) && result;
                    }
                    break;

                case "--ollama-deployment":
                    if (i < args.Length - 1)
                    {
                        settings.Ollama.DeploymentName = args[++i];
                    }
                    break;

                case "--ollama-model":
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

        return settings;
    }
}

public class LLMSettings
{
    public LLMProviderType Provider { get; set; }
}

public class OpenAISettings
{
    public string DeploymentName { get; set; } = "gpt-4o";
    public string? ConnectionString { get; set; }
}

public class OllamaSettings
{
    public string ImageTag { get; set; } = "0.6.8";
    public bool UseGPU { get; set; }
    public bool UseHuggingFaceModel { get; set; }
    public string DeploymentName { get; set; } = "ollama";
    public string ModelName { get; set; } = "qwen3";
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum LLMProviderType
{
    [JsonStringEnumMemberName("openai")]
    OpenAI,
    [JsonStringEnumMemberName("ollama")]
    Ollama,
    [JsonStringEnumMemberName("hface")]
    HuggingFace
}