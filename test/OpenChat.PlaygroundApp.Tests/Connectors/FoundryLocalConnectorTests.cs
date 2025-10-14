using OpenChat.PlaygroundApp.Abstractions;
using OpenChat.PlaygroundApp.Configurations;
using OpenChat.PlaygroundApp.Connectors;

namespace OpenChat.PlaygroundApp.Tests.Connectors;

public class FoundryLocalConnectorTests
{
    private const string Alais = "test-alias";

    private static AppSettings BuildAppSettings(string? alias = Alais)
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
    public void Given_Settings_Is_Null_When_EnsureLanguageModelSettingsValid_Invoked_Then_It_Should_Throw()
    {
        // Arrange
        var settings = new AppSettings { ConnectorType = ConnectorType.FoundryLocal, FoundryLocal = null };
        var connector = new FoundryLocalConnector(settings);

        // Act
        Action action = () => connector.EnsureLanguageModelSettingsValid();

        // Assert
        action.ShouldThrow<InvalidOperationException>()
               .Message.ShouldContain("FoundryLocal");
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData(null, typeof(InvalidOperationException), "FoundryLocal:Alias")]
    [InlineData("", typeof(InvalidOperationException), "FoundryLocal:Alias")]
    [InlineData("   ", typeof(InvalidOperationException), "FoundryLocal:Alias")]
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
    }
}
