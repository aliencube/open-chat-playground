using System.Text.Json.Serialization;

namespace OpenChat.Common.Configurations;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum LLMProviderType
{
    [JsonStringEnumMemberName("undefined")]
    Undefined,
    [JsonStringEnumMemberName("openai")]
    OpenAI,
    [JsonStringEnumMemberName("ollama")]
    Ollama,
    [JsonStringEnumMemberName("hface")]
    HuggingFace
}