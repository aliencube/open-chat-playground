using Microsoft.Extensions.AI;
using OpenChat.PlaygroundApp.Abstractions;
using OpenChat.PlaygroundApp.Configurations;
using OpenChat.PlaygroundApp.Connectors;

namespace OpenChat.PlaygroundApp.Tests.Connectors;

public class DockerModelRunnerConnectorTests
{
    private const string BaseUrl = "https://test.dockermodelrunner.co/api";
    private const string Model = "test-model";

    private static AppSettings BuildAppSettings(string? baseUrl = BaseUrl, string? model = Model)
    {
        return new AppSettings
        {
            ConnectorType = ConnectorType.DockerModelRunner,
            DockerModelRunner = new DockerModelRunnerSettings
            {
                BaseUrl = baseUrl,
                Model = model
            }
        };
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData(typeof(LanguageModelConnector), typeof(DockerModelRunnerConnector), true)]
    [InlineData(typeof(DockerModelRunnerConnector), typeof(LanguageModelConnector), false)]
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
        Action action = () => new DockerModelRunnerConnector(null!);

        // Assert
        action.ShouldThrow<ArgumentNullException>()
              .Message.ShouldContain("settings");
    }

    [Trait("Category", "UnitTest")]
    [Fact]
    public void Given_Settings_Is_Null_When_EnsureLanguageModelSettingsValid_Invoked_Then_It_Should_Throw()
    {
        // Arrange
        var settings = new AppSettings
        {
            ConnectorType = ConnectorType.DockerModelRunner,
            DockerModelRunner = null
        };
        var connector = new DockerModelRunnerConnector(settings);

        // Act
        Action action = () => connector.EnsureLanguageModelSettingsValid();

        // Assert
        action.ShouldThrow<InvalidOperationException>()
              .Message.ShouldContain("DockerModelRunner");
    }

    [Trait("Category", "UnitTest")]
    [Fact]
    public void Given_Settings_When_Instantiated_Then_It_Should_Return()
    {
        // Arrange
        var settings = BuildAppSettings();

        // Act
        var result = new DockerModelRunnerConnector(settings);

        // Assert
        result.ShouldNotBeNull();
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData(null, typeof(NullReferenceException), "Object reference not set to an instance of an object")]
    [InlineData("", typeof(InvalidOperationException), "DockerModelRunner:BaseUrl")]
    [InlineData("   ", typeof(InvalidOperationException), "DockerModelRunner:BaseUrl")]
    [InlineData("\t\n\r", typeof(InvalidOperationException), "DockerModelRunner:BaseUrl")]
    public void Given_Invalid_BaseUrl_When_EnsureLanguageModelSettingsValid_Invoked_Then_It_Should_Throw(string? baseUrl, Type expectedType, string expectedMessage)
    {
        // Arrange
        var settings = BuildAppSettings(baseUrl: baseUrl);
        var connector = new DockerModelRunnerConnector(settings);

        // Act
        Action action = () => connector.EnsureLanguageModelSettingsValid();

        // Assert
        action.ShouldThrow(expectedType)
              .Message.ShouldContain(expectedMessage);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData(null, typeof(NullReferenceException), "Object reference not set to an instance of an object")]
    [InlineData("", typeof(InvalidOperationException), "DockerModelRunner:Model")]
    [InlineData("   ", typeof(InvalidOperationException), "DockerModelRunner:Model")]
    [InlineData("\t\n\r", typeof(InvalidOperationException), "DockerModelRunner:Model")]
    public void Given_Invalid_Model_When_EnsureLanguageModelSettingsValid_Invoked_Then_It_Should_Throw(string? model, Type expectedType, string expectedMessage)
    {
        // Arrange
        var settings = BuildAppSettings(model: model);
        var connector = new DockerModelRunnerConnector(settings);

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
        var connector = new DockerModelRunnerConnector(settings);

        // Act
        var result = connector.EnsureLanguageModelSettingsValid();

        // Assert
        result.ShouldBeTrue();
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData(null, typeof(ArgumentNullException), "null")]
    [InlineData("", typeof(UriFormatException), "empty")]
    [InlineData("   ", typeof(UriFormatException), "Invalid URI: The format of the URI could not be determined.")]
    [InlineData("invalid-uri-format", typeof(UriFormatException), "Invalid URI: The format of the URI could not be determined.")]
    [InlineData("not-a-url", typeof(UriFormatException), "Invalid URI: The format of the URI could not be determined.")]
    public void Given_Invalid_BaseUrl_When_GetChatClient_Invoked_Then_It_Should_Throw(string? baseUrl, Type expectedType, string message)
    {
        // Arrange
        var settings = BuildAppSettings(baseUrl: baseUrl);
        var connector = new DockerModelRunnerConnector(settings);

        // Act
        Func<Task> func = async () => await connector.GetChatClientAsync();

        // Assert
        func.ShouldThrow(expectedType)
            .Message.ShouldContain(message);
    }

    [Trait("Category", "IntegrationTest")]
    [Trait("Category", "LLMRequired")]
    [Theory]
    [InlineData(null, typeof(OllamaSharp.Models.Exceptions.OllamaException), "invalid model name")]
    [InlineData("", typeof(OllamaSharp.Models.Exceptions.OllamaException), "invalid model name")]
    [InlineData("   ", typeof(OllamaSharp.Models.Exceptions.OllamaException), "invalid model name")]
    [InlineData("invalid-model-format", typeof(OllamaSharp.Models.Exceptions.ResponseError), "pull model manifest")]
    public void Given_Invalid_Model_When_GetChatClient_Invoked_Then_It_Should_Throw(string? model, Type expectedType, string message)
    {
        // Arrange
        var settings = BuildAppSettings(model: model);
        var connector = new DockerModelRunnerConnector(settings);

        // Act
        Func<Task> func = async () => await connector.GetChatClientAsync();

        // Assert
        func.ShouldThrow(expectedType)
            .Message.ShouldContain(message);
    }

    [Trait("Category", "IntegrationTest")]
    [Trait("Category", "LLMRequired")]
    [Fact]
    public async Task Given_Valid_Settings_When_GetChatClient_Invoked_Then_It_Should_Return_ChatClient()
    {
        // Arrange
        var settings = BuildAppSettings();
        var connector = new DockerModelRunnerConnector(settings);

        // Act
        var client = await connector.GetChatClientAsync();

        // Assert
        client.ShouldNotBeNull();
        client.ShouldBeAssignableTo<IChatClient>();
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData(null, null, typeof(NullReferenceException), "Object reference not set to an instance of an object")]
    [InlineData(null, Model, typeof(NullReferenceException), "Object reference not set to an instance of an object")]
    [InlineData("", Model, typeof(InvalidOperationException), "Missing configuration: DockerModelRunner:BaseUrl")]
    [InlineData("   ", Model, typeof(InvalidOperationException), "Missing configuration: DockerModelRunner:BaseUrl")]
    [InlineData(BaseUrl, null, typeof(NullReferenceException), "Object reference not set to an instance of an object")]
    [InlineData(BaseUrl, "", typeof(InvalidOperationException), "Missing configuration: DockerModelRunner:Model")]
    [InlineData(BaseUrl, "   ", typeof(InvalidOperationException), "Missing configuration: DockerModelRunner:Model")]
    public void Given_Invalid_Settings_When_CreateChatClientAsync_Invoked_Then_It_Should_Throw(string? baseUrl, string? model, Type expected, string message)
    {
        // Arrange
        var settings = BuildAppSettings(baseUrl: baseUrl, model: model);

        // Act
        Func<Task> func = async () => await LanguageModelConnector.CreateChatClientAsync(settings);

        // Assert
        func.ShouldThrow(expected)
            .Message.ShouldContain(message);
    }

    [Trait("Category", "IntegrationTest")]
    [Trait("Category", "LLMRequired")]
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
