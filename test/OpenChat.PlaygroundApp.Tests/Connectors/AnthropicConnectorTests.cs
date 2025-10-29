using Microsoft.Extensions.AI;

using OpenChat.PlaygroundApp.Abstractions;
using OpenChat.PlaygroundApp.Configurations;
using OpenChat.PlaygroundApp.Connectors;

namespace OpenChat.PlaygroundApp.Tests.Connectors;

public class AnthropicConnectorTests
{
    private const string ApiKey = "test-api-key";
    private const string Model = "test-model";
    private const int MaxTokens = 512;

    private static AppSettings BuildAppSettings(string? apiKey = ApiKey, string? model = Model, int? maxTokens = MaxTokens)

    {
        return new AppSettings
        {
            ConnectorType = ConnectorType.Anthropic,
            Anthropic = new AnthropicSettings
            {
                ApiKey = apiKey,
                Model = model,
                MaxTokens = maxTokens
            }
        };
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData(typeof(LanguageModelConnector), typeof(AnthropicConnector), true)]
    [InlineData(typeof(AnthropicConnector), typeof(LanguageModelConnector), false)]
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
        Action action = () => new AnthropicConnector(null!);

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
        var result = new AnthropicConnector(settings);

        // Assert
        result.ShouldNotBeNull();
    }

    [Trait("Category", "UnitTest")]
    [Fact]
    public void Given_Null_AnthropicSettings_When_EnsureLanguageModelSettingsValid_Invoked_Then_It_Should_Throw()
    {
        // Arrange
        var settings = new AppSettings
        {
            ConnectorType = ConnectorType.Anthropic,
            Anthropic = null
        };
        var connector = new AnthropicConnector(settings);

        // Act
        Action action = () => connector.EnsureLanguageModelSettingsValid();

        // Assert
        action.ShouldThrow<InvalidOperationException>()
              .Message.ShouldContain("Anthropic");
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData(null, typeof(InvalidOperationException), "Anthropic:ApiKey")]
    [InlineData("", typeof(InvalidOperationException), "Anthropic:ApiKey")]
    [InlineData("   ", typeof(InvalidOperationException), "Anthropic:ApiKey")]
    [InlineData("\t\r\n", typeof(InvalidOperationException), "Anthropic:ApiKey")]
    public void Given_Invalid_ApiKey_When_EnsureLanguageModelSettingsValid_Invoked_Then_It_Should_Throw(string? apiKey, Type expectedType, string expectedMessage)
    {
        // Arrange
        var settings = BuildAppSettings(apiKey: apiKey);
        var connector = new AnthropicConnector(settings);

        // Act
        Action action = () => connector.EnsureLanguageModelSettingsValid();

        // Assert
        action.ShouldThrow(expectedType)
              .Message.ShouldContain(expectedMessage);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData(null, typeof(InvalidOperationException), "Anthropic:Model")]
    [InlineData("", typeof(InvalidOperationException), "Anthropic:Model")]
    [InlineData("   ", typeof(InvalidOperationException), "Anthropic:Model")]
    [InlineData("\t\r\n", typeof(InvalidOperationException), "Anthropic:Model")]
    public void Given_Invalid_Model_When_EnsureLanguageModelSettingsValid_Invoked_Then_It_Should_Throw(string? model, Type expectedType, string expectedMessage)
    {
        // Arrange
        var settings = BuildAppSettings(model: model);
        var connector = new AnthropicConnector(settings);

        // Act
        Action action = () => connector.EnsureLanguageModelSettingsValid();

        // Assert
        action.ShouldThrow(expectedType)
              .Message.ShouldContain(expectedMessage);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData(null, typeof(InvalidOperationException), "Anthropic:MaxTokens")]
    [InlineData(0, typeof(InvalidOperationException), "Anthropic:MaxTokens")]
    [InlineData(-1, typeof(InvalidOperationException), "Anthropic:MaxTokens")]
    public void Given_Invalid_MaxToken_When_EnsureLanguageModelSettingsValid_Invoked_Then_It_Should_Throw(int? maxToken, Type expectedType, string expectedMessage)
    {
        // Arrange
        var settings = BuildAppSettings(maxTokens: maxToken);
        var connector = new AnthropicConnector(settings);

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
        var connector = new AnthropicConnector(settings);

        // Act
        var result = connector.EnsureLanguageModelSettingsValid();

        // Assert
        result.ShouldBeTrue();
    }

    [Trait("Category", "UnitTest")]
    [Fact]
    public void Given_Null_AnthropicSettings_When_GetChatClientAsync_Invoked_Then_It_Should_Throw()
    {
        // Arrange
        var settings = new AppSettings
        {
            ConnectorType = ConnectorType.Anthropic,
            Anthropic = null
        };
        var connector = new AnthropicConnector(settings);

        // Act
        Func<Task> func = async () => await connector.GetChatClientAsync();

        // Assert
        func.ShouldThrow<NullReferenceException>()
            .Message.ShouldContain("Object reference not set to an instance of an object");
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData(null, typeof(NullReferenceException), "Object reference not set to an instance of an object.")]
    public void Given_Invalid_ApiKey_When_GetChatClientAsync_Invoked_Then_It_Should_Throw(string? apiKey, Type expectedType, string message)
    {
        // Arrange
        var settings = BuildAppSettings(apiKey: apiKey);
        var connector = new AnthropicConnector(settings);

        // Act
        Func<Task> func = async () => await connector.GetChatClientAsync();

        // Assert  
        func.ShouldThrow(expectedType)
            .Message.ShouldContain(message);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData(null, typeof(NullReferenceException), "Object reference not set to an instance of an object.")]
    public void Given_Invalid_Model_When_GetChatClientAsync_Invoked_Then_It_Should_Throw(string? model, Type expectedType, string message)
    {
        // Arrange
        var settings = BuildAppSettings(model: model);
        var connector = new AnthropicConnector(settings);

        // Act
        Func<Task> func = async () => await connector.GetChatClientAsync();

        // Assert  
        func.ShouldThrow(expectedType)
            .Message.ShouldContain(message);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData(null, typeof(InvalidOperationException), "Anthropic:MaxTokens")]
    [InlineData(0, typeof(InvalidOperationException), "Anthropic:MaxTokens")]
    [InlineData(-1, typeof(InvalidOperationException), "Anthropic:MaxTokens")]
    public void Given_Invalid_MaxTokens_When_GetChatClientAsync_Invoked_Then_It_Should_Throw(int? maxTokens, Type expectedType, string message)
    {
        // Arrange
        var settings = BuildAppSettings(maxTokens: maxTokens);
        var connector = new AnthropicConnector(settings);

        // Act
        Func<Task> func = async () => await connector.GetChatClientAsync();

        // Assert  
        func.ShouldThrow(expectedType)
            .Message.ShouldContain(message);
    }

    [Trait("Category", "UnitTest")]
    [Fact]
    public async Task Given_Valid_Settings_When_GetChatClient_Invoked_Then_It_Should_Return_ChatClient()
    {
        // Arrange
        var settings = BuildAppSettings();
        var connector = new AnthropicConnector(settings);

        // Act
        var client = await connector.GetChatClientAsync();

        // Assert
        client.ShouldNotBeNull();
        client.ShouldBeAssignableTo<IChatClient>();
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData(null, null, null, typeof(InvalidOperationException), "Missing configuration: Anthropic:ApiKey")]
    [InlineData("", Model, MaxTokens, typeof(InvalidOperationException), "Missing configuration: Anthropic:ApiKey")]
    [InlineData("   ", Model, MaxTokens, typeof(InvalidOperationException), "Missing configuration: Anthropic:ApiKey")]
    [InlineData("\t\r\n", Model, MaxTokens, typeof(InvalidOperationException), "Missing configuration: Anthropic:ApiKey")]
    [InlineData(ApiKey, null, MaxTokens, typeof(InvalidOperationException), "Missing configuration: Anthropic:Model")]
    [InlineData(ApiKey, "", MaxTokens, typeof(InvalidOperationException), "Missing configuration: Anthropic:Model")]
    [InlineData(ApiKey, "   ", MaxTokens, typeof(InvalidOperationException), "Missing configuration: Anthropic:Model")]
    [InlineData(ApiKey, "\t\r\n", MaxTokens, typeof(InvalidOperationException), "Missing configuration: Anthropic:Model")]
    [InlineData(ApiKey, Model, null, typeof(InvalidOperationException), "Invalid configuration: Anthropic:MaxTokens")]
    [InlineData(ApiKey, Model, 0, typeof(InvalidOperationException), "Invalid configuration: Anthropic:MaxTokens")]
    [InlineData(ApiKey, Model, -1, typeof(InvalidOperationException), "Invalid configuration: Anthropic:MaxTokens")]
    public void Given_Invalid_Settings_When_CreateChatClientAsync_Invoked_Then_It_Should_Throw(string? apiKey, string? model, int? maxTokens, Type expected, string expectedMessage)
    {
        // Arrange
        var settings = new AppSettings
        {
            ConnectorType = ConnectorType.Anthropic,
            Anthropic = new AnthropicSettings
            {
                ApiKey = apiKey,
                Model = model,
                MaxTokens = maxTokens
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