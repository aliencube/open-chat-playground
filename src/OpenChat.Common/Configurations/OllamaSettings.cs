namespace OpenChat.Common.Configurations;

public class OllamaSettings
{
    public string ImageTag { get; set; } = "0.6.8";
    public bool UseGPU { get; set; }
    public string DeploymentName { get; set; } = string.Empty;
    public string ModelName { get; set; } = string.Empty;
}
