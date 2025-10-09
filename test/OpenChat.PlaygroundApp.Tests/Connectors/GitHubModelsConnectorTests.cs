using Humanizer;

using OpenChat.PlaygroundApp.Configurations;
using OpenChat.PlaygroundApp.Connectors;

namespace OpenChat.PlaygroundApp.Tests.Connectors;

public class GitHubModelsConnectorTests
{
    private const string Endpoint = "https://models.github.ai/inference";
    private const string Token = "test-token";
    private const string Model = "openai/gpt-4o-mini";

    private static AppSettings BuildAppSettings(string? endpoint = Endpoint, string? token = Token, string? model = Model)
    {
        return new AppSettings
        {
            ConnectorType = ConnectorType.GitHubModels,
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
    public void Given_Settings_Is_Null_When_EnsureLanguageModelSettingsValid_Invoked_Then_It_Should_Throw()
    {
        // Arrange
        var settings = new AppSettings { ConnectorType = ConnectorType.GitHubModels, GitHubModels = null };
        var connector = new GitHubModelsConnector(settings);

        // Act
        Action action = () => connector.EnsureLanguageModelSettingsValid();

        // Assert
        action.ShouldThrow<InvalidOperationException>()
              .Message.ShouldContain("GitHubModels");
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData(null, typeof(NullReferenceException), "Object reference not set to an instance of an object")]
    [InlineData("", typeof(InvalidOperationException), "GitHubModels:Endpoint")]
    [InlineData("   ", typeof(InvalidOperationException), "GitHubModels:Endpoint")]
    public void Given_Invalid_Endpoint_When_EnsureLanguageModelSettingsValid_Invoked_Then_It_Should_Throw(string? endpoint, Type expectedType, string expectedMessage)
    {
        // Arrange
        var settings = BuildAppSettings(endpoint: endpoint);
        var connector = new GitHubModelsConnector(settings);

        // Act
        Action action = () => connector.EnsureLanguageModelSettingsValid();

        // Assert
        action.ShouldThrow(expectedType)
              .Message.ShouldContain(expectedMessage);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData(null, typeof(NullReferenceException), "Object reference not set to an instance of an object")]
    [InlineData("", typeof(InvalidOperationException), "GitHubModels:Token")]
    [InlineData("   ", typeof(InvalidOperationException), "GitHubModels:Token")]
    public void Given_Invalid_Token_When_EnsureLanguageModelSettingsValid_Invoked_Then_It_Should_Throw(string? token, Type expectedType, string expectedMessage)
    {
        // Arrange
        var settings = BuildAppSettings(token: token);
        var connector = new GitHubModelsConnector(settings);

        // Act
        Action action = () => connector.EnsureLanguageModelSettingsValid();

        // Assert
        action.ShouldThrow(expectedType)
              .Message.ShouldContain(expectedMessage);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData(null, typeof(NullReferenceException), "Object reference not set to an instance of an object")]
    [InlineData("", typeof(InvalidOperationException), "GitHubModels:Model")]
    [InlineData("   ", typeof(InvalidOperationException), "GitHubModels:Model")]
    public void Given_Invalid_Model_When_EnsureLanguageModelSettingsValid_Invoked_Then_It_Should_Throw(string? model, Type expectedType, string expectedMessage)
    {
        // Arrange
        var settings = BuildAppSettings(model: model);
        var connector = new GitHubModelsConnector(settings);

        // Act
        Action action = () => connector.EnsureLanguageModelSettingsValid();

        // Assert
        action.ShouldThrow(expectedType)
              .Message.ShouldContain(expectedMessage);
    }

    [Trait("Category", "UnitTest")]
    [Fact]
    public void Given_Valid_Settings_When_EnsureLanguageModelSettingsValid_Invoked_Then_It_Should_Return_True()
    {
        // Arrange
        var settings = BuildAppSettings();
        var connector = new GitHubModelsConnector(settings);

        // Act
        var result = connector.EnsureLanguageModelSettingsValid();

        // Assert
        result.ShouldBeTrue();
    }

    [Trait("Category", "UnitTest")]
    [Fact]
    public async Task Given_Valid_Settings_When_GetChatClient_Invoked_Then_It_Should_Return_ChatClient()
    {
        // Arrange
        var settings = BuildAppSettings();
        var connector = new GitHubModelsConnector(settings);

        // Act
        var client = await connector.GetChatClientAsync();

        // Assert
        client.ShouldNotBeNull();
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData(null, typeof(InvalidOperationException), "GitHubModels:Token")]
    [InlineData("", typeof(ArgumentException), "key")]
    public async Task Given_Missing_Token_When_GetChatClient_Invoked_Then_It_Should_Throw(string? token, Type expected, string message)
    {
        // Arrange
        var settings = BuildAppSettings(token: token);
        var connector = new GitHubModelsConnector(settings);

        // Act
        Func<Task> func = connector.GetChatClientAsync;

        // Assert
        var ex = await func.ShouldThrowAsync(expected);
        ex.Message.ShouldContain(message);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData(null, typeof(InvalidOperationException), "GitHubModels:Endpoint")]
    [InlineData("", typeof(UriFormatException), "empty")]
    public async Task Given_Missing_Endpoint_When_GetChatClient_Invoked_Then_It_Should_Throw(string? endpoint, Type expected, string message)
    {
        // Arrange
        var settings = BuildAppSettings(endpoint: endpoint);
        var connector = new GitHubModelsConnector(settings);

        // Act
        Func<Task> func = connector.GetChatClientAsync;

        // Assert
        var ex = await func.ShouldThrowAsync(expected);
        ex.Message.ShouldContain(message);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData(null, typeof(ArgumentNullException), "model")]
    [InlineData("", typeof(ArgumentException), "model")]
    public async Task Given_Missing_Model_When_GetChatClient_Invoked_Then_It_Should_Throw(string? model, Type expected, string message)
    {
        // Arrange
        var settings = BuildAppSettings(model: model);
        var connector = new GitHubModelsConnector(settings);

        // Act
        Func<Task> func = connector.GetChatClientAsync;

        // Assert
        var ex = await func.ShouldThrowAsync(expected);
        ex.Message.ShouldContain(message);
    }
}
