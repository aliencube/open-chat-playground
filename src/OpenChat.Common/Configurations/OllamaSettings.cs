namespace OpenChat.Common.Configurations;

/// <summary>
/// This represents the entity for the Ollama settings in the 'appsettings.json' file.
/// </summary>
public class OllamaSettings
{
    /// <summary>
    /// Gets or sets the Ollama container image tag. Default is '0.6.8'.
    /// </summary>
    public string ImageTag { get; set; } = "0.6.8";

    /// <summary>
    /// Gets or sets the value indicating whether to use GPU or not.
    /// </summary>
    public bool UseGPU { get; set; }

    /// <summary>
    /// Gets or sets the deployment name for either Ollama or Hugging Face.
    /// </summary>
    public string DeploymentName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the model name for either Ollama or Hugging Face.
    /// Make sure to include '/' in the middle and 'GGUF' at the end of the model name, if you are using Hugging Face.
    /// </summary>
    public string ModelName { get; set; } = string.Empty;
}
