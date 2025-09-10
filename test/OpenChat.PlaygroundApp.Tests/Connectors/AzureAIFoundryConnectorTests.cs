using OpenChat.PlaygroundApp.Configurations;
using OpenChat.PlaygroundApp.Connectors;

namespace OpenChat.PlaygroundApp.Tests.Connectors;

public class AzureAIFoundryConnectorTests
{
    private static AppSettings BuildAppSettings(string? endpoint = "https://test.services.ai.azure.com/api/projects/test", string? apiKey = "test-api-key", string? deploymentName = "gpt-4o-mini")
    {
        return new AppSettings
        {
            ConnectorType = ConnectorType.AzureAIFoundry,
            AzureAIFoundry = new AzureAIFoundrySettings
            {
                Endpoint = endpoint,
                ApiKey = apiKey,
                DeploymentName = deploymentName
            }
        };
    }

    [Trait("Category", "UnitTest")]
    [Fact]
    public void Given_Settings_Is_Null_When_EnsureLanguageModelSettingsValid_Invoked_Then_It_Should_Throw()
    {
        // Arrange
        var appSettings = new AppSettings { ConnectorType = ConnectorType.AzureAIFoundry, AzureAIFoundry = null };
        var connector = new AzureAIFoundryConnector(appSettings);

        // Act
        var ex = Assert.Throws<InvalidOperationException>(() => connector.EnsureLanguageModelSettingsValid());

        // Assert
        ex.Message.ShouldContain("AzureAIFoundry");
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData(null, typeof(InvalidOperationException), "AzureAIFoundry:Endpoint")]
    [InlineData("", typeof(InvalidOperationException), "AzureAIFoundry:Endpoint")]
    [InlineData("   ", typeof(InvalidOperationException), "AzureAIFoundry:Endpoint")]
    public void Given_Invalid_Endpoint_When_EnsureLanguageModelSettingsValid_Invoked_Then_It_Should_Throw(string? endpoint, Type expectedType, string expectedMessage)
    {
        // Arrange
        var appSettings = BuildAppSettings(endpoint: endpoint);
        var connector = new AzureAIFoundryConnector(appSettings);

        // Act
        var ex = Assert.Throws(expectedType, () => connector.EnsureLanguageModelSettingsValid());

        // Assert
        ex.Message.ShouldContain(expectedMessage);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData(null, typeof(InvalidOperationException), "AzureAIFoundry:ApiKey")]
    [InlineData("", typeof(InvalidOperationException), "AzureAIFoundry:ApiKey")]
    [InlineData("   ", typeof(InvalidOperationException), "AzureAIFoundry:ApiKey")]
    public void Given_Invalid_ApiKey_When_EnsureLanguageModelSettingsValid_Invoked_Then_It_Should_Throw(string? apiKey, Type expectedType, string expectedMessage)
    {
        // Arrange
        var appSettings = BuildAppSettings(apiKey: apiKey);
        var connector = new AzureAIFoundryConnector(appSettings);

        // Act
        var ex = Assert.Throws(expectedType, () => connector.EnsureLanguageModelSettingsValid());

        // Assert
        ex.Message.ShouldContain(expectedMessage);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData(null, typeof(InvalidOperationException), "AzureAIFoundry:DeploymentName")]
    [InlineData("", typeof(InvalidOperationException), "AzureAIFoundry:DeploymentName")]
    [InlineData("   ", typeof(InvalidOperationException), "AzureAIFoundry:DeploymentName")]
    public void Given_Invalid_DeploymentName_When_EnsureLanguageModelSettingsValid_Invoked_Then_It_Should_Throw(string? deploymentName, Type expectedType, string expectedMessage)
    {
        // Arrange
        var appSettings = BuildAppSettings(deploymentName: deploymentName);
        var connector = new AzureAIFoundryConnector(appSettings);

        // Act
        var ex = Assert.Throws(expectedType, () => connector.EnsureLanguageModelSettingsValid());

        // Assert
        ex.Message.ShouldContain(expectedMessage);
    }

    [Trait("Category", "UnitTest")]
    [Fact]
    public void Given_Valid_Settings_When_EnsureLanguageModelSettingsValid_Invoked_Then_It_Should_Return_True()
    {
        // Arrange
        var appSettings = BuildAppSettings();
        var connector = new AzureAIFoundryConnector(appSettings);

        // Act
        var result = connector.EnsureLanguageModelSettingsValid();

        // Assert
        result.ShouldBeTrue();
    }

    [Trait("Category", "UnitTest")]
    [Fact]
    public async Task Given_Valid_Settings_When_GetChatClient_Invoked_Then_It_Should_Return_ChatClient()
    {
        // Arrange
        var settings = BuildAppSettings();
        var connector = new AzureAIFoundryConnector(settings);

        // Act
        var client = await connector.GetChatClientAsync();

        // Assert
        client.ShouldNotBeNull();
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData(null, typeof(InvalidOperationException), "AzureAIFoundry:ApiKey")]
    [InlineData("", typeof(ArgumentException), "key")]
    public async Task Given_Missing_ApiKey_When_GetChatClient_Invoked_Then_It_Should_Throw(string? apiKey, Type expected, string message)
    {
        // Arrange
        var settings = BuildAppSettings(apiKey: apiKey);
        var connector = new AzureAIFoundryConnector(settings);

        // Act
        var ex = await Assert.ThrowsAsync(expected, connector.GetChatClientAsync);

        // Assert
        ex.Message.ShouldContain(message);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData(null, typeof(InvalidOperationException), "AzureAIFoundry:Endpoint")]
    [InlineData("", typeof(UriFormatException), "empty")]
    public async Task Given_Missing_Endpoint_When_GetChatClient_Invoked_Then_It_Should_Throw(string? endpoint, Type expected, string message)
    {
        // Arrange
        var settings = BuildAppSettings(endpoint: endpoint);
        var connector = new AzureAIFoundryConnector(settings);

        // Act
        var ex = await Assert.ThrowsAsync(expected, connector.GetChatClientAsync);

        // Assert
        ex.Message.ShouldContain(message);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData(null, typeof(InvalidOperationException), "AzureAIFoundry:DeploymentName")]
    [InlineData("", typeof(ArgumentException), "model")]
    public async Task Given_Missing_DeploymentName_When_GetChatClient_Invoked_Then_It_Should_Throw(string? deploymentName, Type expected, string message)
    {
        // Arrange
        var settings = BuildAppSettings(deploymentName: deploymentName);
        var connector = new AzureAIFoundryConnector(settings);

        // Act
        var ex = await Assert.ThrowsAsync(expected, connector.GetChatClientAsync);

        // Assert
        ex.Message.ShouldContain(message);
    }
}
