using Microsoft.Extensions.AI;

using OllamaSharp.Models.Exceptions;

using OpenChat.PlaygroundApp.Abstractions;
using OpenChat.PlaygroundApp.Configurations;
using OpenChat.PlaygroundApp.Connectors;

namespace OpenChat.PlaygroundApp.Tests.Connectors;

public class HuggingFaceConnectorTests
{
    private const string BaseUrl = "http://localhost:11434";
    private const string Model = "hf.co/Qwen/Qwen3-0.6B-GGUF";

    private static AppSettings BuildAppSettings(string? baseUrl = BaseUrl, string? model = Model)
    {
        return new AppSettings
        {
            ConnectorType = ConnectorType.HuggingFace,
            HuggingFace = new HuggingFaceSettings
            {
                BaseUrl = baseUrl,
                Model = model
            }
        };
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData(typeof(LanguageModelConnector), typeof(HuggingFaceConnector), true)]
    [InlineData(typeof(HuggingFaceConnector), typeof(LanguageModelConnector), false)]
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
        Action action = () => new HuggingFaceConnector(null!);

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
            ConnectorType = ConnectorType.HuggingFace,
            HuggingFace = null
        };
        var connector = new HuggingFaceConnector(settings);

        // Act
        Action action = () => connector.EnsureLanguageModelSettingsValid();

        // Assert
        action.ShouldThrow<InvalidOperationException>()
              .Message.ShouldContain("HuggingFace");
    }

    [Trait("Category", "UnitTest")]
    [Fact]
    public void Given_Settings_When_Instantiated_Then_It_Should_Return()
    {
        // Arrange
        var settings = BuildAppSettings();

        // Act
        var result = new HuggingFaceConnector(settings);

        // Assert
        result.ShouldNotBeNull();
    }
    
    [Trait("Category", "UnitTest")]
    [Fact]
    public void Given_Null_Settings_When_EnsureLanguageModelSettingsValid_Invoked_Then_It_Should_Throw()
    {
        // Arrange
        var settings = new AppSettings
        {
            ConnectorType = ConnectorType.HuggingFace,
            HuggingFace = null
        };
        var connector = new HuggingFaceConnector(settings);

        // Act
        Action action = () => connector.EnsureLanguageModelSettingsValid();

        // Assert
        action.ShouldThrow<InvalidOperationException>()
              .Message.ShouldContain("HuggingFace");
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData(null, typeof(NullReferenceException), "Object reference not set to an instance of an object")]
    [InlineData("", typeof(InvalidOperationException), "HuggingFace:BaseUrl")]
    [InlineData("   ", typeof(InvalidOperationException), "HuggingFace:BaseUrl")]
    [InlineData("\t\n\r", typeof(InvalidOperationException), "HuggingFace:BaseUrl")]
    public void Given_Invalid_BaseUrl_When_EnsureLanguageModelSettingsValid_Invoked_Then_It_Should_Throw(string? baseUrl, Type expectedType, string expectedMessage)
    {
        // Arrange
        var settings = BuildAppSettings(baseUrl: baseUrl);
        var connector = new HuggingFaceConnector(settings);

        // Act
        Action action = () => connector.EnsureLanguageModelSettingsValid();

        // Assert
        action.ShouldThrow(expectedType)
              .Message.ShouldContain(expectedMessage);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData(null, typeof(NullReferenceException), "Object reference not set to an instance of an object")]
    [InlineData("", typeof(InvalidOperationException), "HuggingFace:Model")]
    [InlineData("   ", typeof(InvalidOperationException), "HuggingFace:Model")]
    [InlineData("\t\n\r", typeof(InvalidOperationException), "HuggingFace:Model")]
    [InlineData("hf.co/org/model", typeof(InvalidOperationException), "HuggingFace:Model format")]
    [InlineData("org/model-gguf", typeof(InvalidOperationException), "HuggingFace:Model format")]
    [InlineData("hf.co//model-gguf", typeof(InvalidOperationException), "HuggingFace:Model format")]
    public void Given_Invalid_Model_When_EnsureLanguageModelSettingsValid_Invoked_Then_It_Should_Throw(string? model, Type expectedType, string expectedMessage)
    {
        // Arrange
        var settings = BuildAppSettings(model: model);
        var connector = new HuggingFaceConnector(settings);

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
        var connector = new HuggingFaceConnector(settings);

        // Act
        var result = connector.EnsureLanguageModelSettingsValid();

        // Assert
        result.ShouldBeTrue();
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData(null, typeof(NullReferenceException), "Object reference not set to an instance of an object")]
    [InlineData("", typeof(UriFormatException), "empty")]
    [InlineData("   ", typeof(UriFormatException), "Invalid URI:")]
    [InlineData("\t\n\r", typeof(UriFormatException), "Invalid URI:")]
    [InlineData("invalid-uri-format", typeof(UriFormatException), "Invalid URI:")]
    [InlineData("not-a-url", typeof(UriFormatException), "Invalid URI:")]
    public void Given_Invalid_BaseUrl_When_GetChatClient_Invoked_Then_It_Should_Throw(string? baseUrl, Type expected, string message)
    {
        // Arrange
        var settings = BuildAppSettings(baseUrl: baseUrl);
        var connector = new HuggingFaceConnector(settings);

        // Act
        Func<Task> func = async () => await connector.GetChatClientAsync();

        // Assert
        func.ShouldThrow(expected)
            .Message.ShouldContain(message);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData(null, typeof(NullReferenceException), "Object reference not set to an instance of an object")]
    public void Given_Null_Model_When_GetChatClient_Invoked_Then_It_Should_Throw(string? model, Type expected, string message)
    {
        // Arrange        
        var settings = BuildAppSettings(model: model);
        var connector = new HuggingFaceConnector(settings);

        // Act
        Func<Task> func = async () => await connector.GetChatClientAsync();

        // Assert
        func.ShouldThrow(expected)
            .Message.ShouldContain(message);
    }

    [Trait("Category", "IntegrationTest")]
    [Trait("Category", "LLMRequired")]
    [Theory]
    [InlineData("", typeof(OllamaException), "invalid model name")]
    [InlineData("   ", typeof(OllamaException), "invalid model name")]
    [InlineData("\t\n\r", typeof(OllamaException), "invalid model name")]
    [InlineData("hf.co//model-gguf", typeof(OllamaException), "invalid model name")]
    [InlineData("hf.co/org/model", typeof(ResponseError), "pull model manifest")]
    [InlineData("org/model-gguf", typeof(ResponseError), "pull model manifest")]
    public void Given_Invalid_Model_When_GetChatClient_Invoked_Then_It_Should_Throw(string? model, Type expected, string message)
    {
        // Arrange        
        var settings = BuildAppSettings(model: model);
        var connector = new HuggingFaceConnector(settings);

        // Act
        Func<Task> func = async () => await connector.GetChatClientAsync();

        // Assert
        func.ShouldThrow(expected)
            .Message.ShouldContain(message);
    }

    [Trait("Category", "IntegrationTest")]
    [Trait("Category", "LLMRequired")]
    [Fact]
    public async Task Given_Valid_Settings_When_GetChatClient_Invoked_Then_It_Should_Return_ChatClient()
    {
        // Arrange
        var settings = BuildAppSettings();
        var connector = new HuggingFaceConnector(settings);

        // Act
        var client = await connector.GetChatClientAsync();

        // Assert
        client.ShouldNotBeNull();
        client.ShouldBeAssignableTo<IChatClient>();
    }
    
    [Trait("Category", "UnitTest")]
    [Theory]
	[InlineData(null, null, typeof(NullReferenceException), "Object reference not set to an instance of an object")]
	[InlineData(null, Model, typeof(NullReferenceException),"Object reference not set to an instance of an object")]
	[InlineData("", Model, typeof(InvalidOperationException), "Missing configuration: HuggingFace")]
	[InlineData("   ", Model, typeof(InvalidOperationException), "Missing configuration: HuggingFace")]
	[InlineData(BaseUrl, null, typeof(NullReferenceException), "Object reference not set to an instance of an object")]
	[InlineData(BaseUrl, "", typeof(InvalidOperationException), "Missing configuration: HuggingFace")]
	[InlineData(BaseUrl, "  ", typeof(InvalidOperationException), "Missing configuration: HuggingFace")]
	[InlineData(BaseUrl, "hf.co/org/model", typeof(InvalidOperationException), "Invalid configuration: HuggingFace:Model format")]
	[InlineData(BaseUrl, "org/model-gguf", typeof(InvalidOperationException), "Invalid configuration: HuggingFace:Model format")]
	[InlineData(BaseUrl, "hf.co//model-gguf", typeof(InvalidOperationException), "Invalid configuration: HuggingFace:Model format")]
    public void Given_Invalid_Settings_When_CreateChatClientAsync_Invoked_Then_It_Should_Throw(string? baseUrl, string? model, Type expected, string expectedMessage)
    {
        // Arrange
        var settings = new AppSettings
        {
            ConnectorType = ConnectorType.HuggingFace,
            HuggingFace = new HuggingFaceSettings
            {
                BaseUrl = baseUrl,
                Model = model
            }
        };

        // Act
        Func<Task> func = async () => await LanguageModelConnector.CreateChatClientAsync(settings);

        // Assert  
        func.ShouldThrow(expected)
            .Message.ShouldContain(expectedMessage);
    }

    [Trait("Category", "IntegrationTest")]
    [Trait("Category", "LLMRequired")]
    [Fact]
    public async Task Given_Valid_Settings_When_CreateChatClientAsync_Invoked_Then_It_Should_Return_IChatClient()
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