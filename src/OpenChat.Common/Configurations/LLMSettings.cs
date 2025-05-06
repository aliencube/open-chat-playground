using System.Text.Json.Serialization;

namespace OpenChat.Common.Configurations;

public class LLMSettings
{
    public string Provider { get; set; } = "undefined";

    [JsonIgnore]
    public LLMProviderType ProviderType
    {
        get => Enum.TryParse<LLMProviderType>(Provider, ignoreCase: true, out var result)
               ? result
               : Provider!.Equals("hface", StringComparison.InvariantCultureIgnoreCase)
                   ? LLMProviderType.HuggingFace
                   : LLMProviderType.Undefined;
    }
}
