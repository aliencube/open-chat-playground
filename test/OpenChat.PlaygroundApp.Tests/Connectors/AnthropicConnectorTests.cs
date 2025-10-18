using Microsoft.Extensions.AI;

using OpenChat.PlaygroundApp.Abstractions;
using OpenChat.PlaygroundApp.Configurations;
using OpenChat.PlaygroundApp.Connectors;

namespace OpenChat.PlaygroundApp.Tests.Connectors;

public class AnthropicConnectorTests
{
    private const string ApiKey = "test-api-key";
    private const string Model = "test-model";

    private static AppSettings BuildAppSettings(string? apiKey = ApiKey, string? model = Model)
    {
        return new AppSettings
        {
            ConnectorType = ConnectorType.Anthropic,
            Anthropic = new AnthropicSettings
            {
                ApiKey = apiKey,
                Model = model
            }
        };
    }

    [Trait("Category", "UnitTest")]
    [Fact(Skip = "Anthropic connector is not enabled yet.")]
    public void Given_Null_Settings_When_Instantiated_Then_It_Should_Throw()
    {
        // Act
        Action action = () => new AnthropicConnector(null!);

        // Assert
        action.ShouldThrow<ArgumentNullException>()
              .Message.ShouldContain("settings");
    }

    [Trait("Category", "UnitTest")]
    [Fact(Skip = "Anthropic connector is not enabled yet.")]
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
    [Theory(Skip = "Anthropic connector is not enabled yet.")]
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
    [Fact(Skip = "Anthropic connector is not enabled yet.")]
    public void Given_Settings_Is_Null_When_EnsureLanguageModelSettingsValid_Invoked_Then_It_Should_Throw()
    {
        // Arrange
        var settings = new AppSettings { ConnectorType = ConnectorType.Anthropic, Anthropic = null };
        var connector = new AnthropicConnector(settings);

        // Act
        Action action = () => connector.EnsureLanguageModelSettingsValid();

        // Assert
        action.ShouldThrow<InvalidOperationException>()
              .Message.ShouldContain("Missing configuration: Anthropic.");
    }

    [Trait("Category", "UnitTest")]
    [Theory(Skip = "Anthropic connector is not enabled yet.")]
    [InlineData(null, typeof(NullReferenceException), "Object reference not set to an instance of an object")]
    [InlineData("", typeof(InvalidOperationException), "Anthropic:ApiKey")]
    [InlineData("   ", typeof(InvalidOperationException), "Anthropic:ApiKey")]
    [InlineData("\t\n\r", typeof(InvalidOperationException), "Anthropic:ApiKey")]
    public void Given_Invalid_ApiKey_When_EnsureLanguageModelSettingsValid_Invoked_Then_It_Should_Throw(string? apiKey, Type expectedException, string expectedMessage)
    {
        // Arrange
        var settings = BuildAppSettings(apiKey: apiKey);
        var connector = new AnthropicConnector(settings);

        // Act
        Action action = () => connector.EnsureLanguageModelSettingsValid();

        // Assert
        action.ShouldThrow(expectedException)
              .Message.ShouldContain(expectedMessage);
    }

    [Trait("Category", "UnitTest")]
    [Theory(Skip = "Anthropic connector is not enabled yet.")]
    [InlineData(null, typeof(NullReferenceException), "Object reference not set to an instance of an object")]
    [InlineData("", typeof(InvalidOperationException), "Anthropic:Model")]
    [InlineData("   ", typeof(InvalidOperationException), "Anthropic:Model")]
    [InlineData("\t\n\r", typeof(InvalidOperationException), "Anthropic:Model")]
    public void Given_Invalid_Model_When_EnsureLanguageModelSettingsValid_Invoked_Then_It_Should_Throw(string? model, Type expectedException, string expectedMessage)
    {
        // Arrange
        var settings = BuildAppSettings(apiKey: "valid-key", model: model);
        var connector = new AnthropicConnector(settings);

        // Act
        Action action = () => connector.EnsureLanguageModelSettingsValid();

        // Assert
        action.ShouldThrow(expectedException)
              .Message.ShouldContain(expectedMessage);
    }

    [Trait("Category", "UnitTest")]
    [Fact(Skip = "Anthropic connector is not enabled yet.")]
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
    [Fact(Skip = "Anthropic connector is not enabled yet.")]
    public async Task Given_Valid_Settings_When_GetChatClientAsync_Invoked_Then_It_Should_Return_ChatClient()
    {
        // Arrange
        var settings = BuildAppSettings();
        var connector = new AnthropicConnector(settings);

        // Act
        var client = await connector.GetChatClientAsync();

        // Assert
        client.ShouldNotBeNull();
    }

    [Trait("Category", "UnitTest")]
    [Fact(Skip = "Anthropic connector is not enabled yet.")]
    public void Given_Settings_Is_Null_When_GetChatClientAsync_Invoked_Then_It_Should_Throw()
    {
        // Arrange
        var settings = new AppSettings { ConnectorType = ConnectorType.Anthropic, Anthropic = null };
        var connector = new AnthropicConnector(settings);

        // Act
        Func<Task> func = async () => await connector.GetChatClientAsync();

        // Assert
        func.ShouldThrow<InvalidOperationException>()
            .Message.ShouldContain("Missing configuration: Anthropic:ApiKey.");
    }

    [Trait("Category", "UnitTest")]
    [Theory(Skip = "Anthropic connector is not enabled yet.")]
    [InlineData(null, typeof(InvalidOperationException), "Anthropic:ApiKey")]
    [InlineData("", typeof(InvalidOperationException), "Anthropic:ApiKey")]
    public void Given_Missing_ApiKey_When_GetChatClientAsync_Invoked_Then_It_Should_Throw(string? apiKey, Type expectedException, string expectedMessage)
    {
        // Arrange
        var settings = BuildAppSettings(apiKey: apiKey);
        var connector = new AnthropicConnector(settings);

        // Act
        Func<Task> func = async () => await connector.GetChatClientAsync();

        // Assert
        func.ShouldThrow(expectedException)
            .Message.ShouldContain(expectedMessage);
    }

    [Trait("Category", "UnitTest")]
    [Fact(Skip = "Anthropic connector is not enabled in the factory yet.")]
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
    [Theory(Skip = "Anthropic connector is not enabled in the factory yet.")]
    [InlineData(null, "claude-sonnet-4-0", typeof(NullReferenceException))]
    [InlineData("", "claude-sonnet-4-0", typeof(InvalidOperationException))]
    [InlineData("   ", "claude-sonnet-4-0", typeof(InvalidOperationException))]
    [InlineData("test-api-key", null, typeof(NullReferenceException))]
    [InlineData("test-api-key", "", typeof(InvalidOperationException))]
    [InlineData("test-api-key", "   ", typeof(InvalidOperationException))]
    public void Given_Invalid_Settings_When_CreateChatClientAsync_Invoked_Then_It_Should_Throw(string? apiKey, string? model, Type expectedType)
    {
        // Arrange
        var settings = BuildAppSettings(apiKey: apiKey, model: model);

        // Act
        Func<Task> func = async () => await LanguageModelConnector.CreateChatClientAsync(settings);

        // Assert
        func.ShouldThrow(expectedType);
    }
}