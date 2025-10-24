using Microsoft.Extensions.AI;

using OllamaSharp.Models.Exceptions;

using OpenChat.PlaygroundApp.Abstractions;
using OpenChat.PlaygroundApp.Configurations;
using OpenChat.PlaygroundApp.Connectors;

namespace OpenChat.PlaygroundApp.Tests.Connectors;

public class LGConnectorTests
{
    private const string BaseUrl = "http://localhost:11434";
    private const string Model = "hf.co/LGAI-EXAONE/EXAONE-4.0-1.2B-GGUF";

    private static AppSettings BuildAppSettings(string? baseUrl = BaseUrl, string? model = Model)
    {
        return new AppSettings
        {
            ConnectorType = ConnectorType.LG,
            LG = new LGSettings
            {
                BaseUrl = baseUrl,
                Model = model
            }
        };
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData(typeof(LanguageModelConnector), typeof(LGConnector), true)]
    [InlineData(typeof(LGConnector), typeof(LanguageModelConnector), false)]
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
        Action action = () => new LGConnector(null!);

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
        var result = new LGConnector(settings);

        // Assert
        result.ShouldNotBeNull();
    }

    [Trait("Category", "UnitTest")]
    [Fact]
    public void Given_Null_LGSettings_When_EnsureLanguageModelSettingsValid_Invoked_Then_It_Should_Throw()
    {
        // Arrange
        var settings = new AppSettings
        {
            ConnectorType = ConnectorType.LG,
            LG = null
        };
        var connector = new LGConnector(settings);

        // Act
        Action action = () => connector.EnsureLanguageModelSettingsValid();

        // Assert
        action.ShouldThrow<InvalidOperationException>()
              .Message.ShouldContain("LG");
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData(null, typeof(NullReferenceException), "Object reference not set to an instance of an object")]
    [InlineData("", typeof(InvalidOperationException), "LG:BaseUrl")]
    [InlineData("   ", typeof(InvalidOperationException), "LG:BaseUrl")]
    [InlineData("\t\r\n", typeof(InvalidOperationException), "LG:BaseUrl")]
    public void Given_Invalid_BaseUrl_When_EnsureLanguageModelSettingsValid_Invoked_Then_It_Should_Throw(string? baseUrl, Type expectedType, string expectedMessage)
    {
        // Arrange
        var settings = BuildAppSettings(baseUrl: baseUrl);
        var connector = new LGConnector(settings);

        // Act
        Action action = () => connector.EnsureLanguageModelSettingsValid();

        // Assert
        action.ShouldThrow(expectedType)
              .Message.ShouldContain(expectedMessage);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData(null, typeof(NullReferenceException), "Object reference not set to an instance of an object")]
    [InlineData("", typeof(InvalidOperationException), "LG:Model")]
    [InlineData("   ", typeof(InvalidOperationException), "LG:Model")]
    [InlineData("\t\r\n", typeof(InvalidOperationException), "LG:Model")]
    [InlineData("invalid-model-format", typeof(InvalidOperationException), "Expected 'hf.co/LGAI-EXAONE/EXAONE-*-GGUF' format")]
    [InlineData("random-name", typeof(InvalidOperationException), "Expected 'hf.co/LGAI-EXAONE/EXAONE-*-GGUF' format")]
    [InlineData("hf.co/other-org/model-GGUF", typeof(InvalidOperationException), "Expected 'hf.co/LGAI-EXAONE/EXAONE-*-GGUF' format")]
    [InlineData("hf.co/LGAI-EXAONE/other-model-GGUF", typeof(InvalidOperationException), "Expected 'hf.co/LGAI-EXAONE/EXAONE-*-GGUF' format")]
    [InlineData("hf.co/LGAI-EXAONE/EXAONE-4.0-1.2B", typeof(InvalidOperationException), "Expected 'hf.co/LGAI-EXAONE/EXAONE-*-GGUF' format")]
    [InlineData("hf.co/LGAI-EXAONE/EXAONE-4.0-1.2B-FP8", typeof(InvalidOperationException), "Expected 'hf.co/LGAI-EXAONE/EXAONE-*-GGUF' format")]
    public void Given_Invalid_Model_When_EnsureLanguageModelSettingsValid_Invoked_Then_It_Should_Throw(string? model, Type expectedType, string expectedMessage)
    {
        // Arrange
        var settings = BuildAppSettings(model: model);
        var connector = new LGConnector(settings);

        // Act
        Action action = () => connector.EnsureLanguageModelSettingsValid();

        // Assert
        action.ShouldThrow(expectedType)
              .Message.ShouldContain(expectedMessage);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("hf.co/LGAI-EXAONE/EXAONE-4.0-1.2B-GGUF")]
    [InlineData("hf.co/LGAI-EXAONE/EXAONE-4.0-32B-GGUF")]
    public void Given_Valid_Model_Format_When_EnsureLanguageModelSettingsValid_Invoked_Then_It_Should_Return_True(string model)
    {
        // Arrange
        var settings = BuildAppSettings(model: model);
        var connector = new LGConnector(settings);

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
    [InlineData("\t\r\n", typeof(UriFormatException), "Invalid URI:")]
    [InlineData("invalid-uri-format", typeof(UriFormatException), "Invalid URI:")]
    public void Given_Invalid_BaseUrl_When_GetChatClient_Invoked_Then_It_Should_Throw(string? baseUrl, Type expectedType, string message)
    {
        // Arrange
        var settings = BuildAppSettings(baseUrl: baseUrl);
        var connector = new LGConnector(settings);

        // Act
        Func<Task> func = async () => await connector.GetChatClientAsync();

        // Assert
        func.ShouldThrow(expectedType)
            .Message.ShouldContain(message);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData(null, typeof(NullReferenceException), "Object reference not set to an instance of an object")]
    public void Given_Null_Model_When_GetChatClient_Invoked_Then_It_Should_Throw(string? model, Type expectedType, string message)
    {
        // Arrange
        var settings = BuildAppSettings(model: model);
        var connector = new LGConnector(settings);

        // Act
        Func<Task> func = async () => await connector.GetChatClientAsync();

        // Assert
        func.ShouldThrow(expectedType)
            .Message.ShouldContain(message);
    }
    [Trait("Category", "IntegrationTest")]
    [Trait("Category", "LLMRequired")]
    [Theory]
    [InlineData("", typeof(OllamaException), "invalid model name")]
    [InlineData("   ", typeof(OllamaException), "invalid model name")]
    [InlineData("invalid-model-format", typeof(ResponseError), "pull model manifest")]
    [InlineData("random-name", typeof(ResponseError), "pull model manifest")]
    [InlineData("hf.co/other-org/model-GGUF", typeof(ResponseError), "pull model manifest")]
    [InlineData("hf.co/LGAI-EXAONE/other-model-GGUF", typeof(ResponseError), "pull model manifest")]
    [InlineData("hf.co/LGAI-EXAONE/EXAONE-4.0-1.2B", typeof(ResponseError), "pull model manifest")]
    [InlineData("hf.co/LGAI-EXAONE/EXAONE-4.0-1.2B-FP8", typeof(ResponseError), "pull model manifest")]
    public void Given_Invalid_Model_When_GetChatClient_Invoked_Then_It_Should_Throw(string? model, Type expectedType, string message)
    {
        // Arrange
        var settings = BuildAppSettings(model: model);
        var connector = new LGConnector(settings);

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
        var connector = new LGConnector(settings);

        // Act
        var client = await connector.GetChatClientAsync();

        // Assert
        client.ShouldNotBeNull();
        client.ShouldBeAssignableTo<IChatClient>();
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData(null, Model, typeof(NullReferenceException), "Object reference not set to an instance of an object")]
    [InlineData("", Model, typeof(InvalidOperationException), "Missing configuration: LG:BaseUrl")]
    [InlineData("   ", Model, typeof(InvalidOperationException), "Missing configuration: LG:BaseUrl")]
    [InlineData("\t\r\n", Model, typeof(InvalidOperationException), "Missing configuration: LG:BaseUrl")]
    [InlineData(BaseUrl, null, typeof(NullReferenceException), "Object reference not set to an instance of an object")]
    [InlineData(BaseUrl, "", typeof(InvalidOperationException), "Missing configuration: LG:Model")]
    [InlineData(BaseUrl, "   ", typeof(InvalidOperationException), "Missing configuration: LG:Model")]
    [InlineData(BaseUrl, "\t\r\n", typeof(InvalidOperationException), "Missing configuration: LG:Model")]
    [InlineData(BaseUrl, "invalid-model-format", typeof(InvalidOperationException), "Invalid configuration: Expected 'hf.co/LGAI-EXAONE/EXAONE-*-GGUF' format")]
    [InlineData(BaseUrl, "random-name", typeof(InvalidOperationException), "Invalid configuration: Expected 'hf.co/LGAI-EXAONE/EXAONE-*-GGUF' format")]
    [InlineData(BaseUrl, "hf.co/other-org/model-GGUF", typeof(InvalidOperationException), "Invalid configuration: Expected 'hf.co/LGAI-EXAONE/EXAONE-*-GGUF' format")]
    [InlineData(BaseUrl, "hf.co/LGAI-EXAONE/other-model-GGUF", typeof(InvalidOperationException), "Invalid configuration: Expected 'hf.co/LGAI-EXAONE/EXAONE-*-GGUF' format")]
    [InlineData(BaseUrl, "hf.co/LGAI-EXAONE/EXAONE-4.0-1.2B", typeof(InvalidOperationException), "Invalid configuration: Expected 'hf.co/LGAI-EXAONE/EXAONE-*-GGUF' format")]
    [InlineData(BaseUrl, "hf.co/LGAI-EXAONE/EXAONE-4.0-1.2B-FP8", typeof(InvalidOperationException), "Invalid configuration: Expected 'hf.co/LGAI-EXAONE/EXAONE-*-GGUF' format")]
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