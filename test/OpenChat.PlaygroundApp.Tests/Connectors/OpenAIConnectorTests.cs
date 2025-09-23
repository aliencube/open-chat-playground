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
        var appSettings = new AppSettings { ConnectorType = ConnectorType.OpenAI, OpenAI = null };
        var connector = new OpenAIConnector(appSettings);

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
        var appSettings = BuildAppSettings(apiKey: apiKey);
        var connector = new OpenAIConnector(appSettings);

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
        var appSettings = BuildAppSettings();
        var connector = new OpenAIConnector(appSettings);

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
        var appSettings = BuildAppSettings(apiKey: "valid-key", model: model);
        var connector = new OpenAIConnector(appSettings);

        // Act
        Action action = () => connector.EnsureLanguageModelSettingsValid();

        // Assert
        action.ShouldThrow(expectedType)
              .Message.ShouldContain(expectedMessage);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData(null, null)]
    [InlineData("", "")]
    public void Given_Both_ApiKey_And_Model_Invalid_When_EnsureLanguageModelSettingsValid_Invoked_Then_It_Should_Throw_ApiKey_First(string? apiKey, string? model)
    {
        // Arrange
        var appSettings = BuildAppSettings(apiKey: apiKey, model: model);
        var connector = new OpenAIConnector(appSettings);

        // Act
        Action action = () => connector.EnsureLanguageModelSettingsValid();

        // Assert
        if (apiKey is null)
        {
            action.ShouldThrow<NullReferenceException>();
        }
        else
        {
            // ApiKey가 먼저 검증되므로 ApiKey 에러가 먼저 발생해야 함
            action.ShouldThrow<InvalidOperationException>()
                  .Message.ShouldContain("OpenAI:ApiKey");
        }
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
    public void Given_Settings_Is_Null_When_GetChatClientAsync_Invoked_Then_It_Should_Throw()
    {
        // Arrange
        var appSettings = new AppSettings { ConnectorType = ConnectorType.OpenAI, OpenAI = null };
        var connector = new OpenAIConnector(appSettings);

        // Act
        Func<Task> func = async () => await connector.GetChatClientAsync();

        // Assert
        func.ShouldThrow<InvalidOperationException>()
            .Message.ShouldContain("Missing configuration: OpenAI.");
    }
    
    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData(null, typeof(InvalidOperationException), "OpenAI:ApiKey")]
    [InlineData("", typeof(ArgumentException), "key")]
    public void Given_Missing_ApiKey_When_GetChatClientAsync_Invoked_Then_It_Should_Throw(string? apiKey, Type expected, string message)
    {
        // Arrange
        var settings = BuildAppSettings(apiKey: apiKey);
        var connector = new OpenAIConnector(settings);

        // Act
        Func<Task> func = async () => await connector.GetChatClientAsync();

        // Assert
        func.ShouldThrow(expected)
            .Message.ShouldContain(message);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData(null, typeof(ArgumentNullException), "model")]
    [InlineData("", typeof(ArgumentException), "model")]
    public void Given_Missing_Model_When_GetChatClientAsync_Invoked_Then_It_Should_Throw(string? model, Type expected, string message)
    {
        // Arrange
        var settings = BuildAppSettings(model: model);
        var connector = new OpenAIConnector(settings);

        // Act
        Func<Task> func = async () => await connector.GetChatClientAsync();

        // Assert
        func.ShouldThrow(expected)
            .Message.ShouldContain(message);
    }

    [Trait("Category", "UnitTest")]
    [Fact]
    public void Given_Valid_Format_Settings_When_GetChatClientAsync_Invoked_Then_It_Should_Pass()
    {
        // Arrange
        var settings = BuildAppSettings(apiKey: "sk-test-key-with-proper-format", model: "gpt-3.5-turbo");
        var connector = new OpenAIConnector(settings);

        // Act
        Func<Task> func = async () => await connector.GetChatClientAsync();
        
        // Assert
        func.ShouldNotThrow();
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
            ConnectorType = ConnectorType.OpenAI,
            OpenAI = new OpenAISettings
            {
                ApiKey = null,
                Model = "test-model"
            }
        };

        // Act
        Func<Task> func = async () => await LanguageModelConnector.CreateChatClientAsync(settings);

        // Assert
        func.ShouldThrow<NullReferenceException>();
    }
}