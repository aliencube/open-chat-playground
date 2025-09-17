using Microsoft.Extensions.AI;
using OpenChat.PlaygroundApp.Abstractions;
using OpenChat.PlaygroundApp.Configurations;
using OpenChat.PlaygroundApp.Connectors;

namespace OpenChat.PlaygroundApp.Tests.Connectors;

public class OllamaConnectorTests
{
    private const string BaseUrl = "https://test.ollama";
	private const string Model = "test-model";
    private static AppSettings BuildAppSettings(string? baseUrl = BaseUrl, string? model = Model)
    {
        return new AppSettings
        {
            ConnectorType = ConnectorType.Ollama,
            Ollama = new OllamaSettings
            {
                BaseUrl = baseUrl,
                Model = model
            }
        };
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData(typeof(LanguageModelConnector), typeof(OllamaConnector), true)]
    [InlineData(typeof(OllamaConnector), typeof(LanguageModelConnector), false)]
    public void Given_BaseType_Then_It_Should_Be_AssignableFrom_DerivedType(Type baseType, Type derivedType, bool expected)
    {
        // Act
        var result = baseType.IsAssignableFrom(derivedType);

        // Assert
        result.ShouldBe(expected);
    }

    [Trait("Category", "UnitTest")]
    [Fact]
    public void Given_Settings_Is_Null_When_EnsureLanguageModelSettingsValid_Invoked_Then_It_Should_Throw()
    {
        // Arrange
        var appSettings = new AppSettings { ConnectorType = ConnectorType.Ollama, Ollama = null };
        var connector = new OllamaConnector(appSettings);

        // Act
        Action action = () => connector.EnsureLanguageModelSettingsValid();

        // Assert
        action.ShouldThrow<InvalidOperationException>()
            .Message.ShouldContain("Ollama");
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData(null, typeof(NullReferenceException), "Object reference not set to an instance of an object")]
    [InlineData("", typeof(InvalidOperationException), "Ollama:BaseUrl")]
    [InlineData("   ", typeof(InvalidOperationException), "Ollama:BaseUrl")]
    public void Given_Invalid_BaseUrl_When_EnsureLanguageModelSettingsValid_Invoked_Then_It_Should_Throw(string? baseUrl, Type expectedType, string expectedMessage)
    {
        // Arrange
        var appSettings = BuildAppSettings(baseUrl: baseUrl);
        var connector = new OllamaConnector(appSettings);

        // Act
        Action action = () => connector.EnsureLanguageModelSettingsValid();

        // Assert
        action.ShouldThrow(expectedType)
            .Message.ShouldContain(expectedMessage);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData(null, typeof(NullReferenceException), "Object reference not set to an instance of an object")]
    [InlineData("", typeof(InvalidOperationException), "Ollama:Model")]
    [InlineData("   ", typeof(InvalidOperationException), "Ollama:Model")]
    public void Given_Invalid_Model_When_EnsureLanguageModelSettingsValid_Invoked_Then_It_Should_Throw(string? model, Type expectedType, string expectedMessage)
    {
        // Arrange
        var appSettings = BuildAppSettings(model: model);
        var connector = new OllamaConnector(appSettings);

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
        var connector = new OllamaConnector(appSettings);

        // Act
        var result = connector.EnsureLanguageModelSettingsValid();

        // Assert
        result.ShouldBeTrue();
    }

    [Trait("Category", "UnitTest")]
    [Fact]
    public async Task Given_Valid_Settings_When_GetChatClientAsync_Invoked_Then_It_Should_Return_ChatClient()
    {
        // Arrange
        var settings = BuildAppSettings();
        var connector = new OllamaConnector(settings);

        // Act
        var client = await connector.GetChatClientAsync();

        // Assert
        client.ShouldNotBeNull();
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData(null, typeof(ArgumentNullException), "null")]
    [InlineData("", typeof(UriFormatException), "empty")]
    public void Given_Missing_BaseUrl_When_GetChatClientAsync_Invoked_Then_It_Should_Throw(string? baseUrl, Type expected, string message)
    {
        // Arrange
        var settings = BuildAppSettings(baseUrl: baseUrl);
        var connector = new OllamaConnector(settings);

        // Act
        Func<Task> action = async () => await connector.GetChatClientAsync();

        // Assert
        action.ShouldThrow(expected)
            .Message.ShouldContain(message);
    }

    [Trait("Category", "UnitTest")]
    [Fact]
    public async Task Given_Valid_AppSettings_When_CreateChatClientAsync_Invoked_Then_It_Should_Return_ChatClient()
    {
        // Arrange
        var appSettings = BuildAppSettings();

        // Act
        var client = await LanguageModelConnector.CreateChatClientAsync(appSettings);

        // Assert
        client.ShouldNotBeNull();
        client.ShouldBeAssignableTo<IChatClient>();
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData(null, null, typeof(NullReferenceException))]
    [InlineData("", Model, typeof(InvalidOperationException))]
    [InlineData("   ", Model, typeof(InvalidOperationException))]
    [InlineData(BaseUrl, null, typeof(NullReferenceException))]
    [InlineData(BaseUrl, "", typeof(InvalidOperationException))]
    [InlineData(BaseUrl, "   ", typeof(InvalidOperationException))]
    public async Task Given_Invalid_Ollama_Settings_When_CreateChatClientAsync_Invoked_Then_It_Should_Throw(string? baseUrl, string? model, Type expectedExceptionType)
    {
        // Arrange
        var settings = BuildAppSettings(baseUrl: baseUrl, model: model);

        // Act
        Func<Task> action = async () => await LanguageModelConnector.CreateChatClientAsync(settings);

        // Assert
        await action.ShouldThrowAsync(expectedExceptionType);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData(ConnectorType.Unknown)]
    [InlineData(ConnectorType.AmazonBedrock)]
    [InlineData(ConnectorType.GoogleVertexAI)]
    public async Task Given_Unsupported_ConnectorType_When_CreateChatClientAsync_Invoked_Then_It_Should_Throw(ConnectorType connectorType)
    {
        // Arrange
        var appSettings = new AppSettings { ConnectorType = connectorType };

        // Act
        Func<Task> action = async () => await LanguageModelConnector.CreateChatClientAsync(appSettings);

        // Assert
        await action.ShouldThrowAsync<NotSupportedException>();
    }
}