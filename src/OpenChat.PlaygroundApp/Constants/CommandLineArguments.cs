namespace OpenChat.PlaygroundApp.Constants;

/// <summary>
/// Contains all command-line argument strings used throughout the application.
/// </summary>
public static class CommandLineArguments
{
    /// <summary>
    /// General command-line arguments.
    /// </summary>
    public const string ConnectorType = "--connector-type";
    public const string ConnectorTypeShort = "-c";
    public const string Help = "--help";
    public const string HelpShort = "-h";

    /// <summary>
    /// Amazon Bedrock command-line arguments.
    /// </summary>
    public static class AmazonBedrock
    {
        public const string AccessKeyId = "--access-key-id";
        public const string SecretAccessKey = "--secret-access-key";
        public const string Region = "--region";
        public const string ModelId = "--model-id";
    }

    /// <summary>
    /// Azure AI Foundry command-line arguments.
    /// </summary>
    public static class AzureAIFoundry
    {
        public const string Endpoint = "--endpoint";
        public const string ApiKey = "--api-key";
        public const string DeploymentName = "--deployment-name";
    }

    /// <summary>
    /// GitHub Models command-line arguments.
    /// </summary>
    public static class GitHubModels
    {
        public const string Endpoint = "--endpoint";
        public const string Token = "--token";
        public const string Model = "--model";
    }

    /// <summary>
    /// Google Vertex AI command-line arguments.
    /// </summary>
    public static class GoogleVertexAI
    {
        public const string ApiKey = "--api-key";
        public const string Model = "--model";
    }

    /// <summary>
    /// Docker Model Runner command-line arguments.
    /// </summary>
    public static class DockerModelRunner
    {
        public const string BaseUrl = "--base-url";
        public const string Model = "--model";
    }

    /// <summary>
    /// Foundry Local command-line arguments.
    /// </summary>
    public static class FoundryLocal
    {
        public const string Alias = "--alias";
    }

    /// <summary>
    /// Hugging Face command-line arguments.
    /// </summary>
    public static class HuggingFace
    {
        public const string BaseUrl = "--base-url";
        public const string Model = "--model";
    }

    /// <summary>
    /// Ollama command-line arguments.
    /// </summary>
    public static class Ollama
    {
        public const string BaseUrl = "--base-url";
        public const string Model = "--model";
    }

    /// <summary>
    /// Anthropic command-line arguments.
    /// </summary>
    public static class Anthropic
    {
        public const string ApiKey = "--api-key";
        public const string Model = "--model";
    }

    /// <summary>
    /// Naver command-line arguments.
    /// </summary>
    public static class Naver
    {
    }

    /// <summary>
    /// LG command-line arguments.
    /// </summary>
    public static class LG
    {
        public const string BaseUrl = "--base-url";
        public const string Model = "--model";
    }

    /// <summary>
    /// OpenAI command-line arguments.
    /// </summary>
    public static class OpenAI
    {
        public const string ApiKey = "--api-key";
        public const string Model = "--model";
    }

    /// <summary>
    /// Upstage command-line arguments.
    /// </summary>
    public static class Upstage
    {
        public const string BaseUrl = "--base-url";
        public const string ApiKey = "--api-key";
        public const string Model = "--model";
    }
}
