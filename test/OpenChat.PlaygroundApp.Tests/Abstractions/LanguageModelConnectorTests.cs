using OpenChat.PlaygroundApp.Abstractions;
using OpenChat.PlaygroundApp.Configurations;
using OpenChat.PlaygroundApp.Connectors;

namespace OpenChat.PlaygroundApp.Tests.Abstractions;

public class LanguageModelConnectorTests
{
    private static AppSettings BuildAppSettings(
        ConnectorType connectorType = ConnectorType.GitHubModels,
        string? endpoint = "https://models.github.ai/inference",
        string? token = "test-token",
        string? model = "openai/gpt-4o-mini")
    {
        return new AppSettings
        {
            ConnectorType = connectorType,
            GitHubModels = new GitHubModelsSettings
            {
                Endpoint = endpoint,
                Token = token,
                Model = model
            }
        };
    }

    [Trait("Category", "UnitTest")]
    [Fact]
    public async Task Given_GitHubModels_Settings_When_CreateChatClient_Invoked_Then_It_Should_Return_ChatClient()
    {
        // Arrange
        var settings = BuildAppSettings();

        // Act
        var client = await LanguageModelConnector.CreateChatClientAsync(settings);

        // Assert
        client.ShouldNotBeNull();
    }

    [Trait("Category", "UnitTest")]
    [Fact]
    public async Task Given_Invalid_Settings_When_CreateChatClientAsync_Invoked_Then_It_Should_Throw()
    {
        // Arrange
        var settings = BuildAppSettings(endpoint: null);

        // Act
        Func<Task> func = async () => await LanguageModelConnector.CreateChatClientAsync(settings);

        // Assert
        await func.ShouldThrowAsync<NullReferenceException>();
    }

    [Trait("Category", "UnitTest")]
    [Fact]
    public async Task Given_Null_Settings_When_CreateChatClient_Invoked_Then_It_Should_Throw()
    {
        // Arrange
        AppSettings settings = null!;

        // Act
        Func<Task> func = async () => await LanguageModelConnector.CreateChatClientAsync(settings);

        // Assert
        await func.ShouldThrowAsync<NullReferenceException>();
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData(ConnectorType.Unknown)]
    public async Task Given_Unsupported_ConnectorType_When_CreateChatClient_Invoked_Then_It_Should_Throw(ConnectorType connectorType)
    {
        // Arrange
        var settings = BuildAppSettings(connectorType: connectorType);

        // Act
        var ex = await Assert.ThrowsAsync<NotSupportedException>(() => LanguageModelConnector.CreateChatClientAsync(settings));

        // Assert
        ex.Message.ShouldContain($"Connector type '{connectorType}'");
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    // [InlineData(typeof(AmazonBedrockConnector))]
    // [InlineData(typeof(AzureAIFoundryConnector))]
    [InlineData(typeof(GitHubModelsConnector))]
    // [InlineData(typeof(GoogleVertexAIConnector))]
    // [InlineData(typeof(DockerModelRunnerConnector))]
    // [InlineData(typeof(FoundryLocalConnector))]
    [InlineData(typeof(HuggingFaceConnector))]
    // [InlineData(typeof(OllamaConnector))]
    // [InlineData(typeof(AnthropicConnector))]
    // [InlineData(typeof(LGConnector))]
    // [InlineData(typeof(NaverConnector))]
    [InlineData(typeof(OpenAIConnector))]
    // [InlineData(typeof(UpstageConnector))]
    public void Given_Concrete_Connectors_When_Checking_Inheritance_Then_Should_Inherit_From_LanguageModelConnector(Type derivedType)
    {
        // Arrange
        var baseType = typeof(LanguageModelConnector);

        // Act
        var result = baseType.IsAssignableFrom(derivedType);

        // Assert
        result.ShouldBeTrue();
    }
}