using Microsoft.Extensions.AI;

using OpenChat.PlaygroundApp.Abstractions;
using OpenChat.PlaygroundApp.Configurations;
using OpenChat.PlaygroundApp.Connectors;

namespace OpenChat.PlaygroundApp.Tests.Connectors;

public class LGConnectorTests
{
    private const string BaseUrl = "https://test.lg-exaone/api";
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
    public void Given_Null_Settings_When_EnsureLanguageModelSettingsValid_Invoked_Then_It_Should_Throw()
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
    [InlineData(null, typeof(ArgumentNullException), "null")]
    [InlineData("", typeof(UriFormatException), "empty")]
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

    [Trait("Category", "IntegrationTest")]
    [Trait("Category", "LLMRequired")]
    [Theory]
    [InlineData(null, typeof(ArgumentNullException), "null")]
    [InlineData("", typeof(UriFormatException), "empty")]
    [InlineData("   ", typeof(InvalidOperationException), "LG:Model")]
    [InlineData("invalid-model-format", typeof(InvalidOperationException), "Expected 'hf.co/LGAI-EXAONE/EXAONE-*-GGUF' format")]
    [InlineData("random-name", typeof(InvalidOperationException), "Expected 'hf.co/LGAI-EXAONE/EXAONE-*-GGUF' format")]
    [InlineData("hf.co/other-org/model-GGUF", typeof(InvalidOperationException), "Expected 'hf.co/LGAI-EXAONE/EXAONE-*-GGUF' format")]
    [InlineData("hf.co/LGAI-EXAONE/other-model-GGUF", typeof(InvalidOperationException), "Expected 'hf.co/LGAI-EXAONE/EXAONE-*-GGUF' format")]
    [InlineData("hf.co/LGAI-EXAONE/EXAONE-4.0-1.2B", typeof(InvalidOperationException), "Expected 'hf.co/LGAI-EXAONE/EXAONE-*-GGUF' format")]
    [InlineData("hf.co/LGAI-EXAONE/EXAONE-4.0-1.2B-FP8", typeof(InvalidOperationException), "Expected 'hf.co/LGAI-EXAONE/EXAONE-*-GGUF' format")]
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
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData(null, "hf.co/LGAI-EXAONE/EXAONE-4.0-1.2B-GGUF", typeof(NullReferenceException))]
    [InlineData("", "hf.co/LGAI-EXAONE/EXAONE-4.0-32B-GGUF", typeof(InvalidOperationException))]
    [InlineData("   ", "hf.co/LGAI-EXAONE/EXAONE-4.0-32B-GGUF", typeof(InvalidOperationException))]
    [InlineData("https://test.lg-exaone/api", null, typeof(NullReferenceException))]
    [InlineData("https://test.lg-exaone/api", "", typeof(InvalidOperationException))]
    [InlineData("https://test.lg-exaone/api", "   ", typeof(InvalidOperationException))]
    [InlineData("https://test.lg-exaone/api", "invalid-model-format", typeof(InvalidOperationException))]
    [InlineData("https://test.lg-exaone/api", "random-name", typeof(InvalidOperationException))]
    [InlineData("https://test.lg-exaone/api", "hf.co/other-org/model-GGUF", typeof(InvalidOperationException))]
    [InlineData("https://test.lg-exaone/api", "hf.co/LGAI-EXAONE/other-model-GGUF", typeof(InvalidOperationException))]
    [InlineData("https://test.lg-exaone/api", "hf.co/LGAI-EXAONE/EXAONE-4.0-1.2B", typeof(InvalidOperationException))]
    [InlineData("https://test.lg-exaone/api", "hf.co/LGAI-EXAONE/EXAONE-4.0-1.2B-FP8", typeof(InvalidOperationException))]
    public void Given_Invalid_Settings_When_CreateChatClientAsync_Invoked_Then_It_Should_Throw(string? baseUrl, string? model, Type expectedType)
    {
        // Arrange
        var settings = BuildAppSettings(baseUrl: baseUrl, model: model);

        // Act
        Func<Task> func = async () => await LanguageModelConnector.CreateChatClientAsync(settings);

        // Assert
        func.ShouldThrow(expectedType);
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
