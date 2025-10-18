using Microsoft.Extensions.AI;

using OpenChat.PlaygroundApp.Abstractions;
using OpenChat.PlaygroundApp.Configurations;
using OpenChat.PlaygroundApp.Connectors;

namespace OpenChat.PlaygroundApp.Tests.Connectors;

public class GoogleVertexAIConnectorTests
{
    private const string ApiKey = "AIzaSyA1234567890abcdefgHIJKLMNOpqrstuv";
    private const string Model = "test-model";
    private static AppSettings BuildAppSettings(string? apiKey = ApiKey, string? model = Model)
    {
        return new AppSettings
        {
            ConnectorType = ConnectorType.GoogleVertexAI,
            GoogleVertexAI = new GoogleVertexAISettings
            {
                ApiKey = apiKey,
                Model = model
            }
        };
    }

    [Trait("Category", "UnitTest")]
    [Fact]
    public void Given_Settings_Is_Null_When_EnsureLanguageModelSettingsValid_Invoked_Then_It_Should_Throw()
    {
        // Arrange
        var appSettings = new AppSettings { ConnectorType = ConnectorType.GoogleVertexAI, GoogleVertexAI = null };
        var connector = new GoogleVertexAIConnector(appSettings);

        // Act
        Action action = () => connector.EnsureLanguageModelSettingsValid();

        // Assert
        action.ShouldThrow<InvalidOperationException>()
              .Message.ShouldContain("GoogleVertexAI");
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData(null, "GoogleVertexAI:ApiKey")]
    [InlineData("", "GoogleVertexAI:ApiKey")]
    [InlineData("   ", "GoogleVertexAI:ApiKey")]
    public void Given_Invalid_ApiKey_When_EnsureLanguageModelSettingsValid_Invoked_Then_It_Should_Throw(string? apiKey, string expectedMessage)
    {
        // Arrange
        var appSettings = BuildAppSettings(apiKey: apiKey);
        var connector = new GoogleVertexAIConnector(appSettings);

        // Act
        Action action = () => connector.EnsureLanguageModelSettingsValid();

        // Assert
        action.ShouldThrow<InvalidOperationException>()
              .Message.ShouldContain(expectedMessage);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData(null, "GoogleVertexAI:Model")]
    [InlineData("", "GoogleVertexAI:Model")]
    [InlineData("   ", "GoogleVertexAI:Model")]
    public void Given_Invalid_Model_When_EnsureLanguageModelSettingsValid_Invoked_Then_It_Should_Throw(string? model, string expectedMessage)
    {
        // Arrange
        var appSettings = BuildAppSettings(apiKey: "valid-key", model: model);
        var connector = new GoogleVertexAIConnector(appSettings);

        // Act
        Action action = () => connector.EnsureLanguageModelSettingsValid();

        // Assert
        action.ShouldThrow<InvalidOperationException>()
              .Message.ShouldContain(expectedMessage);
    }

    [Trait("Category", "UnitTest")]
    [Fact]
    public void Given_Valid_Settings_When_EnsureLanguageModelSettingsValid_Invoked_Then_It_Should_Return_True()
    {
        // Arrange
        var appSettings = BuildAppSettings();
        var connector = new GoogleVertexAIConnector(appSettings);

        // Act
        var result = connector.EnsureLanguageModelSettingsValid();

        // Assert
        result.ShouldBeTrue();
    }

    [Trait("Category", "UnitTest")]
    [Fact]
    public async Task Given_Valid_Settings_When_GetChatClient_Invoked_Then_It_Should_Return_ChatClient()
    {
        var settings = BuildAppSettings();
        var connector = new GoogleVertexAIConnector(settings);

        var client = await connector.GetChatClientAsync();

        client.ShouldNotBeNull();
    }

    [Trait("Category", "UnitTest")]
    [Fact]
    public void Given_Settings_Is_Null_When_GetChatClientAsync_Invoked_Then_It_Should_Throw()
    {
        // Arrange
        var appSettings = new AppSettings { ConnectorType = ConnectorType.GoogleVertexAI, GoogleVertexAI = null };
        var connector = new GoogleVertexAIConnector(appSettings);

        // Act
        Func<Task> func = async () => await connector.GetChatClientAsync();

        // Assert
        func.ShouldThrow<InvalidOperationException>();
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData(null, typeof(InvalidOperationException), "GoogleVertexAI:ApiKey")]
    [InlineData("", typeof(ArgumentException), "key")]
    public async Task Given_Missing_ApiKey_When_GetChatClient_Invoked_Then_It_Should_Throw(string? apiKey, Type expected, string message)
    {
        // Arrange
        var settings = BuildAppSettings(apiKey: apiKey);
        var connector = new GoogleVertexAIConnector(settings);

        // Act
        Func<Task> func = async () => await connector.GetChatClientAsync();

        // Assert
        func.ShouldThrow(expected)
            .Message.ShouldContain(message);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData(null, typeof(InvalidOperationException), "model")]
    public async Task Given_Missing_Model_When_GetChatClient_Invoked_Then_It_Should_Throw(string? model, Type expected, string message)
    {
        // Arrange
        var settings = BuildAppSettings(model: model);
        var connector = new GoogleVertexAIConnector(settings);

        // Act
        Func<Task> func = async () => await connector.GetChatClientAsync();

        // Assert
        func.ShouldThrow(expected)
            .Message.ShouldContain(message);
    }

    [Trait("Category", "UnitTest")]
    [Fact]
    public async Task Given_Valid_Settings_When_CreateChatClientAsync_Invoked_Then_It_Should_Return_ChatClient()
    {
        // Arrange
        var settings = BuildAppSettings();

        // Act
        var result = await LanguageModelConnector.CreateChatClientAsync(settings);
        
        // Assert
        result.ShouldNotBeNull();
        result.ShouldBeAssignableTo<IChatClient>();
    }

    [Trait("Category", "UnitTest")]
    [Fact]
    public void Given_Invalid_Settings_When_CreateChatClientAsync_Invoked_Then_It_Should_Throw()
    {
        // Arrange
        var settings = new AppSettings
        {
            ConnectorType = ConnectorType.GoogleVertexAI,
            GoogleVertexAI = new GoogleVertexAISettings
            {
                ApiKey = null,
                Model = "test-model"
            }
        };

        // Act
        Func<Task> func = async () => await LanguageModelConnector.CreateChatClientAsync(settings);

        // Assert
        func.ShouldThrow<InvalidOperationException>();
    }
}
