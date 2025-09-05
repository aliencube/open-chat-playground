using OpenChat.PlaygroundApp.Abstractions;
using OpenChat.PlaygroundApp.Configurations;

namespace OpenChat.PlaygroundApp.Tests.Abstractions;

public class LanguageModelSettingsTests
{
	[Trait("Category", "UnitTest")]
    [Theory]
    [InlineData(typeof(AmazonBedrockSettings))]
    [InlineData(typeof(AzureAIFoundrySettings))]
    [InlineData(typeof(GitHubModelsSettings))]
    [InlineData(typeof(GoogleVertexAISettings))]
    // [InlineData(typeof(DockerModelRunnerSettings))]
    [InlineData(typeof(FoundryLocalSettings))]
    [InlineData(typeof(HuggingFaceSettings))]
    [InlineData(typeof(OllamaSettings))]
    [InlineData(typeof(AnthropicSettings))]
    [InlineData(typeof(LGSettings))]
    // [InlineData(typeof(NaverSettings))]
    [InlineData(typeof(OpenAISettings))]
    [InlineData(typeof(UpstageSettings))]
    public void Given_Concrete_Settings_When_Checking_Inheritance_Then_Should_Inherit_From_LanguageModelSettings(Type type)
    {
        // Act
        var isSubclass = type.IsSubclassOf(typeof(LanguageModelSettings));
        
        // Assert
        isSubclass.ShouldBeTrue();
    }
}