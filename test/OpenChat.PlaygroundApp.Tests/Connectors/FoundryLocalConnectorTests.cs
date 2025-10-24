using Microsoft.Extensions.AI;

using OpenChat.PlaygroundApp.Abstractions;
using OpenChat.PlaygroundApp.Configurations;
using OpenChat.PlaygroundApp.Connectors;

namespace OpenChat.PlaygroundApp.Tests.Connectors;

public class FoundryLocalConnectorTests
{
    private const string Alias = "phi-4-mini";

    private static AppSettings BuildAppSettings(string? alias = Alias)
    {
        return new AppSettings
        {
            ConnectorType = ConnectorType.FoundryLocal,
            FoundryLocal = new FoundryLocalSettings
            {
                Alias = alias
            }
        };
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData(typeof(LanguageModelConnector), typeof(FoundryLocalConnector), true)]
    [InlineData(typeof(FoundryLocalConnector), typeof(LanguageModelConnector), false)]
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
        Action action = () => new FoundryLocalConnector(null!);

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
        var result = new FoundryLocalConnector(settings);

        // Assert
        result.ShouldNotBeNull();
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("FoundryLocal")]
    public void Given_Null_Settings_When_EnsureLanguageModelSettingsValid_Invoked_Then_It_Should_Throw(string expectedMessage)
    {
        // Arrange
        var settings = new AppSettings
        {
            ConnectorType = ConnectorType.FoundryLocal,
            FoundryLocal = null
        };
        var connector = new FoundryLocalConnector(settings);

        // Act
        Action action = () => connector.EnsureLanguageModelSettingsValid();

        // Assert
        action.ShouldThrow<InvalidOperationException>()
              .Message.ShouldContain(expectedMessage);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData(null, typeof(NullReferenceException), "Object reference not set to an instance of an object")]
    [InlineData("", typeof(InvalidOperationException), "FoundryLocal:Alias")]
    [InlineData("   ", typeof(InvalidOperationException), "FoundryLocal:Alias")]
    [InlineData("\t\n\r", typeof(InvalidOperationException), "FoundryLocal:Alias")]
    public void Given_Invalid_Alias_When_EnsureLanguageModelSettingsValid_Invoked_Then_It_Should_Throw(string? alias, Type expectedType, string expectedMessage)
    {
        // Arrange
        var settings = BuildAppSettings(alias: alias);
        var connector = new FoundryLocalConnector(settings);

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
        var connector = new FoundryLocalConnector(settings);

        // Act
        var result = connector.EnsureLanguageModelSettingsValid();

        // Assert
        result.ShouldBeTrue();
    }

    [Trait("Category", "IgnoreGitHubActions")]
    [Trait("Category", "IntegrationTest")]
    [Trait("Category", "LLMRequired")]
    [Theory]
    [InlineData(null, typeof(InvalidOperationException), "Model  not found in catalog.")]
    [InlineData("", typeof(InvalidOperationException), "Model  not found in catalog.")]
    [InlineData("   ", typeof(InvalidOperationException), "Model     not found in catalog.")]
    [InlineData("not-a-model", typeof(InvalidOperationException), "Model not-a-model not found in catalog.")]
    public void Given_Invalid_Alias_When_GetChatClient_Invoked_Then_It_Should_Throw(string? alias, Type expected, string message)
    {
        // Arrange
        var settings = BuildAppSettings(alias: alias);
        var connector = new FoundryLocalConnector(settings);

        // Act
        Func<Task> func = async () => await connector.GetChatClientAsync();

        // Assert
        func.ShouldThrow(expected)
            .Message.ShouldContain(message);
    }

    [Trait("Category", "IgnoreGitHubActions")]
    [Trait("Category", "IntegrationTest")]
    [Trait("Category", "LLMRequired")]
    [Fact]
    public async Task Given_Valid_Settings_When_GetChatClient_Invoked_Then_It_Should_Return_ChatClient()
    {
        // Arrange
        var settings = BuildAppSettings();
        var connector = new FoundryLocalConnector(settings);

        // Act
        var client = await connector.GetChatClientAsync();

        // Assert
        client.ShouldNotBeNull();
        client.ShouldBeAssignableTo<IChatClient>();
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData(null, typeof(NullReferenceException), "Object reference not set to an instance of an object")]
    [InlineData("", typeof(InvalidOperationException), "Missing configuration: FoundryLocal")]
    [InlineData("   ", typeof(InvalidOperationException), "Missing configuration: FoundryLocal")]
    public void Given_Invalid_Settings_When_CreateChatClientAsync_Invoked_Then_It_Should_Throw(string? alias, Type expected, string expectedMessage)
    {
        // Arrange
        var settings = new AppSettings
        {
            ConnectorType = ConnectorType.FoundryLocal,
            FoundryLocal = new FoundryLocalSettings
            {
                Alias = alias
            }
        };

        // Act
        Func<Task> func = async () => await LanguageModelConnector.CreateChatClientAsync(settings);

        // Assert  
        func.ShouldThrow(expected)
            .Message.ShouldContain(expectedMessage);
    }

    [Trait("Category", "IgnoreGitHubActions")]
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
