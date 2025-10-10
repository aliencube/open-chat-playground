using OpenChat.PlaygroundApp.Configurations;
using OpenChat.PlaygroundApp.Connectors;

namespace OpenChat.PlaygroundApp.Extensions;

/// <summary>
/// This represents the extension entity for handling AppSettings configuration.
/// </summary>
public static class AppSettingsExtensions
{
    /// <summary>
    /// Configures and adds AppSettings to the service collection with model name populated.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> instance.</param>
    /// <param name="configuration">The <see cref="IConfiguration"/> instance.</param>
    /// <param name="settings">The <see cref="AppSettings"/> instance.</param>
    /// <returns>Returns the modified <see cref="IServiceCollection"/> instance.</returns>
    public static IServiceCollection AddAppSettings(this IServiceCollection services, IConfiguration configuration, AppSettings settings)
    {
        ConfigureModelName(configuration, settings);
        services.AddSingleton(settings);
        
        return services;
    }
    
    /// <summary>
    /// Configures the model name in AppSettings based on the connector type.
    /// </summary>
    /// <param name="configuration">The <see cref="IConfiguration"/> instance.</param>
    /// <param name="settings">The <see cref="AppSettings"/> instance.</param>
    private static void ConfigureModelName(IConfiguration configuration, AppSettings settings)
    { 
        string? modelFromSettings = settings.ConnectorType switch
        {
            ConnectorType.AzureAIFoundry => settings.AzureAIFoundry?.DeploymentName,
            ConnectorType.FoundryLocal => settings.FoundryLocal?.Alias,
            ConnectorType.GitHubModels => settings.GitHubModels?.Model,
            ConnectorType.OpenAI => settings.OpenAI?.Model,
            ConnectorType.HuggingFace => settings.HuggingFace?.Model,
            ConnectorType.Anthropic => settings.Anthropic?.Model,
            ConnectorType.LG => settings.LG?.Model,
            _ => throw new ArgumentException($"Unsupported ConnectorType: {settings.ConnectorType}")
        };

        var section = configuration.GetSection(settings.ConnectorType.ToString());
        
        settings.Model = modelFromSettings ?? settings.ConnectorType switch
        {
            ConnectorType.AzureAIFoundry => section.GetValue<string>("DeploymentName"),
            ConnectorType.FoundryLocal => section.GetValue<string>("Alias"),
            _ => section.GetValue<string>("Model")
        };
    }
}