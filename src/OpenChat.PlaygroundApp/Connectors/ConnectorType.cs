using System.Text.Json.Serialization;

namespace OpenChat.PlaygroundApp.Connectors;

/// <summary>
/// This specifies the type of connector to use.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ConnectorType
{
    /// <summary>
    /// Identifies the unknown connector type.
    /// </summary>
    Unknown,

    /// <summary>
    /// Identifies the Amazon Bedrock connector type.
    /// </summary>
    AmazonBedrock,

    /// <summary>
    /// Identifies the Azure AI Foundry connector type.
    /// </summary>
    AzureAIFoundry,

    /// <summary>
    /// Identifies the GitHub Models connector type.
    /// </summary>
    GitHubModels,

    /// <summary>
    /// Identifies the Google Vertex AI connector type.
    /// </summary>
    GoogleVertexAI,

    /// <summary>
    /// Identifies the Docker Model Runner connector type.
    /// </summary>
    DockerModelRunner,

    /// <summary>
    /// Identifies the Foundry Local connector type.
    /// </summary>
    FoundryLocal,

    /// <summary>
    /// Identifies the Hugging Face connector type.
    /// </summary>
    HuggingFace,

    /// <summary>
    /// Identifies the Ollama connector type.
    /// </summary>
    Ollama,

    /// <summary>
    /// Identifies the Anthropic connector type.
    /// </summary>
    Anthropic,

    /// <summary>
    /// Identifies the LG connector type.
    /// </summary>
    LG,

    /// <summary>
    /// Identifies the Naver connector type.
    /// </summary>
    Naver,

    /// <summary>
    /// Identifies the OpenAI connector type.
    /// </summary>
    OpenAI,

    /// <summary>
    /// Identifies the Upstage connector type.
    /// </summary>
    Upstage,
}