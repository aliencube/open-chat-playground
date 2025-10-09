using Microsoft.Extensions.AI;

using OpenChat.PlaygroundApp.Abstractions;
using OpenChat.PlaygroundApp.Configurations;
using OpenChat.PlaygroundApp.Connectors;

namespace OpenChat.PlaygroundApp.Tests.Connectors;

public class OpenAIConnectorTests
{
    private const string ApiKey = "test-api-key";
    private const string Model = "test-model";

    private static AppSettings BuildAppSettings(string? apiKey = ApiKey, string? model = Model)
    {
        return new AppSettings
        {
            ConnectorType = ConnectorType.OpenAI,
            OpenAI = new OpenAISettings
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
        var settings = new AppSettings { ConnectorType = ConnectorType.OpenAI, OpenAI = null };
        var connector = new OpenAIConnector(settings);

        // Act
        Action action = () => connector.EnsureLanguageModelSettingsValid();

        // Assert
        action.ShouldThrow<InvalidOperationException>()
              .Message.ShouldContain("Missing configuration: OpenAI.");
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData(null, typeof(NullReferenceException), "Object reference not set to an instance of an object")]
    [InlineData("", typeof(InvalidOperationException), "OpenAI:ApiKey")]
    [InlineData("   ", typeof(InvalidOperationException), "OpenAI:ApiKey")]
    [InlineData("\t\n\r", typeof(InvalidOperationException), "OpenAI:ApiKey")]
    public void Given_Invalid_ApiKey_When_EnsureLanguageModelSettingsValid_Invoked_Then_It_Should_Throw(string? apiKey, Type expectedType, string expectedMessage)
    {
        // Arrange
        var settings = BuildAppSettings(apiKey: apiKey);
        var connector = new OpenAIConnector(settings);

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
        var connector = new OpenAIConnector(settings);

        // Act
        var result = connector.EnsureLanguageModelSettingsValid();

        // Assert
        result.ShouldBeTrue();
    }
    
    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData(null, typeof(NullReferenceException), "Object reference not set to an instance of an object")]
    [InlineData("", typeof(InvalidOperationException), "OpenAI:Model")]
    [InlineData("   ", typeof(InvalidOperationException), "OpenAI:Model")]
    [InlineData("\t\n\r", typeof(InvalidOperationException), "OpenAI:Model")]
    public void Given_Invalid_Model_When_EnsureLanguageModelSettingsValid_Invoked_Then_It_Should_Throw(string? model, Type expectedType, string expectedMessage)
    {
        // Arrange
        var settings = BuildAppSettings(model: model);
        var connector = new OpenAIConnector(settings);

        // Act
        Action action = () => connector.EnsureLanguageModelSettingsValid();

        // Assert
        action.ShouldThrow(expectedType)
              .Message.ShouldContain(expectedMessage);
    }
    
    [Trait("Category", "UnitTest")]
    [Fact]
    public async Task Given_Valid_Settings_When_GetChatClientAsync_Invoked_Then_It_Should_Return_ChatClient()
    {
        // Arrange
        var settings = BuildAppSettings();
        var connector = new OpenAIConnector(settings);

        // Act
        var client = await connector.GetChatClientAsync();

        // Assert
        client.ShouldNotBeNull();
    }
    
    [Trait("Category", "UnitTest")]
    [Fact]
    public async Task Given_Settings_Is_Null_When_GetChatClientAsync_Invoked_Then_It_Should_Throw()
    {
        // Arrange
        var settings = new AppSettings { ConnectorType = ConnectorType.OpenAI, OpenAI = null! };
        var connector = new OpenAIConnector(settings);

        // Act
        Func<Task> func = connector.GetChatClientAsync;

        // Assert
        var ex = await func.ShouldThrowAsync<InvalidOperationException>();
        ex.Message.ShouldContain("Missing configuration: OpenAI:ApiKey.");
    }
    
    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData(null, typeof(InvalidOperationException), "OpenAI:ApiKey")]
    [InlineData("", typeof(ArgumentException), "key")]
    public async Task Given_Missing_ApiKey_When_GetChatClientAsync_Invoked_Then_It_Should_Throw(string? apiKey, Type expected, string message)
    {
        // Arrange
        var settings = BuildAppSettings(apiKey: apiKey);
        var connector = new OpenAIConnector(settings);

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
    public async Task Given_Missing_Model_When_GetChatClientAsync_Invoked_Then_It_Should_Throw(string? model, Type expected, string message)
    {
        // Arrange
        var settings = BuildAppSettings(model: model);
        var connector = new OpenAIConnector(settings);

        // Act
        Func<Task> func = connector.GetChatClientAsync;

        // Assert
        var ex = await func.ShouldThrowAsync(expected);
        ex.Message.ShouldContain(message);
    }

    [Trait("Category", "UnitTest")]
    [Fact]
    public async Task Given_Valid_Format_Settings_When_GetChatClientAsync_Invoked_Then_It_Should_Pass()
    {
        // Arrange
        var settings = BuildAppSettings();
        var connector = new OpenAIConnector(settings);

        // Act
        Func<Task> func = connector.GetChatClientAsync;
        
        // Assert
        await func.ShouldNotThrowAsync();
    }
}