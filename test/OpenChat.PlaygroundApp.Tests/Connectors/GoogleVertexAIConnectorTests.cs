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
    [Theory]
    [InlineData(typeof(LanguageModelConnector), typeof(GoogleVertexAIConnector), true)]
    [InlineData(typeof(GoogleVertexAIConnector), typeof(LanguageModelConnector), false)]
    public void Given_BaseType_Then_It_Should_Be_AssignableFrom_DerivedType(Type baseType, Type derivedType, bool expected)
    {
        // Act
        var result = baseType.IsAssignableFrom(derivedType);

        // Assert
        result.ShouldBe(expected);
    }

    [Trait("Category", "UnitTest")]
    [Fact]
    public void Given_Null_Settings_When_Instantiated_Then_It_Should_Throw()
    {
        // Act
        Action action = () => new GoogleVertexAIConnector(null!);

        // Assert
        action.ShouldThrow<ArgumentNullException>()
              .Message.ShouldContain("settings");
    }

    [Trait("Category", "UnitTest")]
    [Fact]
    public void Given_Settings_When_Instantiated_Then_It_Should_Return()
    {
        // Arrange
        var settings = BuildAppSettings();

        // Act
        var result = new GoogleVertexAIConnector(settings);

        // Assert
        result.ShouldNotBeNull();
    }

    [Trait("Category", "UnitTest")]
    [Fact]
    public void Given_Settings_Is_Null_When_EnsureLanguageModelSettingsValid_Invoked_Then_It_Should_Throw()
    {
        // Arrange
        var appSettings = new AppSettings
        {
            ConnectorType = ConnectorType.GoogleVertexAI,
            GoogleVertexAI = null
        };
        var connector = new GoogleVertexAIConnector(appSettings);

        // Act
        Action action = () => connector.EnsureLanguageModelSettingsValid();

        // Assert
        action.ShouldThrow<InvalidOperationException>()
              .Message.ShouldContain("GoogleVertexAI");
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData(null, typeof(InvalidOperationException), "GoogleVertexAI:ApiKey")]
    [InlineData("", typeof(InvalidOperationException), "GoogleVertexAI:ApiKey")]
    [InlineData("   ", typeof(InvalidOperationException), "GoogleVertexAI:ApiKey")]
    [InlineData("\t\r\n", typeof(InvalidOperationException), "GoogleVertexAI:ApiKey")]
    public void Given_Invalid_ApiKey_When_EnsureLanguageModelSettingsValid_Invoked_Then_It_Should_Throw(string? apiKey, Type expectedType, string expectedMessage)
    {
        // Arrange
        var appSettings = BuildAppSettings(apiKey: apiKey);
        var connector = new GoogleVertexAIConnector(appSettings);

        // Act
        Action action = () => connector.EnsureLanguageModelSettingsValid();

        // Assert
        action.ShouldThrow(expectedType)
              .Message.ShouldContain(expectedMessage);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData(null, typeof(InvalidOperationException), "GoogleVertexAI:Model")]
    [InlineData("", typeof(InvalidOperationException), "GoogleVertexAI:Model")]
    [InlineData("   ", typeof(InvalidOperationException), "GoogleVertexAI:Model")]
    [InlineData("\t\r\n", typeof(InvalidOperationException), "GoogleVertexAI:Model")]
    public void Given_Invalid_Model_When_EnsureLanguageModelSettingsValid_Invoked_Then_It_Should_Throw(string? model, Type expectedType, string expectedMessage)
    {
        // Arrange
        var appSettings = BuildAppSettings(apiKey: "valid-key", model: model);
        var connector = new GoogleVertexAIConnector(appSettings);

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
        var connector = new GoogleVertexAIConnector(appSettings);

        // Act
        var result = connector.EnsureLanguageModelSettingsValid();

        // Assert
        result.ShouldBeTrue();
    }

    [Trait("Category", "UnitTest")]
    [Fact]
    public void Given_Settings_Is_Null_When_GetChatClientAsync_Invoked_Then_It_Should_Throw()
    {
        // Arrange
        var appSettings = new AppSettings {
            ConnectorType = ConnectorType.GoogleVertexAI,
            GoogleVertexAI = null
        };
        var connector = new GoogleVertexAIConnector(appSettings);

        // Act
        Func<Task> func = async () => await connector.GetChatClientAsync();

        // Assert
        func.ShouldThrow<NullReferenceException>()
            .Message.ShouldContain("Object reference not set to an instance of an object");
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData(null, typeof(NullReferenceException), "Object reference not set to an instance of an object.")]
    [InlineData("", typeof(ArgumentException), "apiKey")]
    [InlineData("   ", typeof(ArgumentException), "apiKey")]
    [InlineData("\t\r\n", typeof(ArgumentException), "apiKey")]
    public async Task Given_Invalid_ApiKey_When_GetChatClientAsync_Invoked_Then_It_Should_Throw(string? apiKey, Type expected, string message)
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
    [InlineData(null, typeof(NullReferenceException), "Object reference not set to an instance of an object.")]
    public async Task Given_Invalid_Model_When_GetChatClientAsync_Invoked_Then_It_Should_Throw(string? model, Type expected, string message)
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
    public async Task Given_Valid_Settings_When_GetChatClientAsync_Invoked_Then_It_Should_Return_ChatClient()
    {
        // Arrange
        var settings = BuildAppSettings();
        var connector = new GoogleVertexAIConnector(settings);

        // Act
        var client = await connector.GetChatClientAsync();

        // Assert
        client.ShouldNotBeNull();
        client.ShouldBeAssignableTo<IChatClient>();
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    
    [InlineData(null, null, typeof(InvalidOperationException),"GoogleVertexAI")]
    [InlineData(null, Model, typeof(InvalidOperationException),"GoogleVertexAI")]
    [InlineData("", Model, typeof(InvalidOperationException),"GoogleVertexAI")]
    [InlineData("   ", Model, typeof(InvalidOperationException),"GoogleVertexAI")]
    [InlineData("\t\r\n", Model, typeof(InvalidOperationException),"GoogleVertexAI")]
    [InlineData(ApiKey, null, typeof(InvalidOperationException),"GoogleVertexAI")]
    public void Given_Invalid_Settings_When_CreateChatClientAsync_Invoked_Then_It_Should_Throw(string? apiKey, string? model, Type expected, string expectedMessage)
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
        func.ShouldThrow(expected)
            .Message.ShouldContain(expectedMessage);
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
}
