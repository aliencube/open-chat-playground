using System.Text.Json.Serialization;

namespace OpenChat.Common.Configurations;

/// <summary>
/// This defines the LLM provider types.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum LLMProviderType
{
    /// <summary>
    /// Identifies undefined LLM provider type.
    /// </summary>
    [JsonStringEnumMemberName("undefined")]
    Undefined,

    /// <summary>
    /// Identifies the OpenAI LLM provider type.
    /// </summary>
    [JsonStringEnumMemberName("openai")]
    OpenAI,

    /// <summary>
    /// Identifies the Ollama LLM provider type.
    /// </summary>
    [JsonStringEnumMemberName("ollama")]
    Ollama,

    /// <summary>
    /// Identifies the Hugging Face LLM provider type.
    /// </summary>
    [JsonStringEnumMemberName("hface")]
    HuggingFace
}