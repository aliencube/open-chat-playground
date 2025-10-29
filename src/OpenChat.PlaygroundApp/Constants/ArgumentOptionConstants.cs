namespace OpenChat.PlaygroundApp.Constants;

/// <summary>
/// This represents the constant entity for command-line arguments to reference.
/// </summary>
public static class ArgumentOptionConstants
{
    /// <summary>
    /// Defines the constant for '--connector-type'.
    /// </summary>
    public const string ConnectorType = "--connector-type";

    /// <summary>
    /// Defines the constant for '-c'.
    /// </summary>
    public const string ConnectorTypeInShort = "-c";

    /// <summary>
    /// Defines the constant for '--help'.
    /// </summary>
    public const string Help = "--help";

    /// <summary>
    /// Defines the constant for '-h'.
    /// </summary>
    public const string HelpInShort = "-h";

    /// <summary>
    /// This represents the command-line argument constants for Amazon Bedrock.
    /// </summary>
    public static class AmazonBedrock
    {
        /// <summary>
        /// Defines the constant for '--access-key-id'.
        /// </summary>
        public const string AccessKeyId = "--access-key-id";

        /// <summary>
        /// Defines the constant for '--secret-access-key'.
        /// </summary>
        public const string SecretAccessKey = "--secret-access-key";

        /// <summary>
        /// Defines the constant for '--region'.
        /// </summary>
        public const string Region = "--region";

        /// <summary>
        /// Defines the constant for '--model-id'.
        /// </summary>
        public const string ModelId = "--model-id";
    }

    /// <summary>
    /// This represents the command-line argument constants for Azure AI Foundry.
    /// </summary>
    public static class AzureAIFoundry
    {
        /// <summary>
        /// Defines the constant for '--endpoint'.
        /// </summary>
        public const string Endpoint = "--endpoint";

        /// <summary>
        /// Defines the constant for '--api-key'.
        /// </summary>
        public const string ApiKey = "--api-key";

        /// <summary>
        /// Defines the constant for '--deployment-name'.
        /// </summary>
        public const string DeploymentName = "--deployment-name";
    }

    /// <summary>
    /// This represents the command-line argument constants for GitHub Models.
    /// </summary>
    public static class GitHubModels
    {
        /// <summary>
        /// Defines the constant for '--endpoint'.
        /// </summary>
        public const string Endpoint = "--endpoint";

        /// <summary>
        /// Defines the constant for '--token'.
        /// </summary>
        public const string Token = "--token";

        /// <summary>
        /// Defines the constant for '--model'.
        /// </summary>
        public const string Model = "--model";
    }

    /// <summary>
    /// This represents the command-line argument constants for Google Vertex AI.
    /// </summary>
    public static class GoogleVertexAI
    {
        /// <summary>
        /// Defines the constant for '--api-key'.
        /// </summary>
        public const string ApiKey = "--api-key";

        /// <summary>
        /// Defines the constant for '--model'.
        /// </summary>
        public const string Model = "--model";
    }

    /// <summary>
    /// This represents the command-line argument constants for Docker Model Runner.
    /// </summary>
    public static class DockerModelRunner
    {
        /// <summary>
        /// Defines the constant for '--base-url'.
        /// </summary>
        public const string BaseUrl = "--base-url";

        /// <summary>
        /// Defines the constant for '--model'.
        /// </summary>
        public const string Model = "--model";
    }

    /// <summary>
    /// This represents the command-line argument constants for Foundry Local.
    /// </summary>
    public static class FoundryLocal
    {
        /// <summary>
        /// Defines the constant for '--base-url'.
        /// </summary>
        public const string BaseUrl = "--base-url";

        /// <summary>
        /// Defines the constant for '--alias'.
        /// </summary>
        public const string Alias = "--alias";

        /// <summary>
        /// Defines the constant for '--model'.
        /// </summary>
        public const string Model = "--model";

        /// <summary>
        /// Defines the constant for '--disable-foundry-local-manager'.
        /// </summary>
        public const string DisableFoundryLocalManager = "--disable-foundry-local-manager";

        /// <summary>
        /// Defines the constant for '--disable-flm'.
        /// </summary>
        public const string DisableFoundryLocalManagerInShort = "--disable-flm";
    }

    /// <summary>
    /// This represents the command-line argument constants for Hugging Face.
    /// </summary>
    public static class HuggingFace
    {
        /// <summary>
        /// Defines the constant for '--base-url'.
        /// </summary>
        public const string BaseUrl = "--base-url";

        /// <summary>
        /// Defines the constant for '--model'.
        /// </summary>
        public const string Model = "--model";
    }

    /// <summary>
    /// This represents the command-line argument constants for Ollama.
    /// </summary>
    public static class Ollama
    {
        /// <summary>
        /// Defines the constant for '--base-url'.
        /// </summary>
        public const string BaseUrl = "--base-url";

        /// <summary>
        /// Defines the constant for '--model'.
        /// </summary>
        public const string Model = "--model";
    }

    /// <summary>
    /// This represents the command-line argument constants for Anthropic.
    /// </summary>
    public static class Anthropic
    {
        /// <summary>
        /// Defines the constant for '--api-key'.
        /// </summary>
        public const string ApiKey = "--api-key";

        /// <summary>
        /// Defines the constant for '--model'.
        /// </summary>
        public const string Model = "--model";

        /// <summary>
        /// Defines the constant for '--max-tokens'.
        /// </summary>
        public const string MaxTokens = "--max-tokens";
    }

    /// <summary>
    /// This represents the command-line argument constants for LG.
    /// </summary>
    public static class LG
    {
        /// <summary>
        /// Defines the constant for '--base-url'.
        /// </summary>
        public const string BaseUrl = "--base-url";

        /// <summary>
        /// Defines the constant for '--model'.
        /// </summary>
        public const string Model = "--model";
    }

    /// <summary>
    /// This represents the command-line argument constants for Naver.
    /// </summary>
    public static class Naver
    {
    }

    /// <summary>
    /// This represents the command-line argument constants for OpenAI.
    /// </summary>
    public static class OpenAI
    {
        /// <summary>
        /// Defines the constant for '--api-key'.
        /// </summary>
        public const string ApiKey = "--api-key";

        /// <summary>
        /// Defines the constant for '--model'.
        /// </summary>
        public const string Model = "--model";
    }

    /// <summary>
    /// This represents the command-line argument constants for Upstage.
    /// </summary>
    public static class Upstage
    {
        /// <summary>
        /// Defines the constant for '--base-url'.
        /// </summary>
        public const string BaseUrl = "--base-url";

        /// <summary>
        /// Defines the constant for '--api-key'.
        /// </summary>
        public const string ApiKey = "--api-key";

        /// <summary>
        /// Defines the constant for '--model'.
        /// </summary>
        public const string Model = "--model";
    }
}
