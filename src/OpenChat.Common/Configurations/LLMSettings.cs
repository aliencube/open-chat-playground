using System.Text.Json.Serialization;

namespace OpenChat.Common.Configurations;

/// <summary>
/// This represents the entity for the LLM settings in the 'appsettings.json' file.
/// </summary>
public class LLMSettings
{
    /// <summary>
    /// Gets or sets the LLM provider name.
    /// </summary>
    /// <value></value>
    public string Provider { get; set; } = "undefined";

    /// <summary>
    /// Gets the LLM provider type, which is READ-ONLY.
    /// </summary>
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
