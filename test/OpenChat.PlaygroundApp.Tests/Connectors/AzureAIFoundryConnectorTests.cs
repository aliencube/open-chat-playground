using OpenChat.PlaygroundApp.Configurations;
using OpenChat.PlaygroundApp.Connectors;
using OpenChat.PlaygroundApp.Abstractions;

namespace OpenChat.PlaygroundApp.Tests.Connectors;

public class AzureAIFoundryConnectorTests
{
    private const string Endpoint = "http://test.azure-ai-foundry/api";
    private const string ApiKey = "test-api-key";
    private const string DeploymentName = "test-deployment-name";

    private static AppSettings BuildAppSettings(string? endpoint = Endpoint, string? apiKey = ApiKey, string? deploymentName = DeploymentName)
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
    [Theory]
    [InlineData(typeof(LanguageModelConnector), typeof(AzureAIFoundryConnector), true)]
    [InlineData(typeof(AzureAIFoundryConnector), typeof(LanguageModelConnector), false)]
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
        var appSettings = new AppSettings
        {
            ConnectorType = ConnectorType.AzureAIFoundry,
            AzureAIFoundry = null
        };
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
    [Fact]
    public async Task Given_Settings_Is_Null_When_GetChatClientAsync_Invoked_Then_It_Should_Throw()
    {
        // Arrange
        var appSettings = new AppSettings
        {
            ConnectorType = ConnectorType.AzureAIFoundry,
            AzureAIFoundry = null
        };
        var connector = new AzureAIFoundryConnector(appSettings);

        // Act
        var ex = await Assert.ThrowsAsync<NullReferenceException>(async () => 
            await connector.GetChatClientAsync());

        // Assert
        ex.ShouldNotBeNull();
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData(null, typeof(ArgumentNullException), "key")]
    [InlineData("", typeof(ArgumentException), "key")]
    public async Task Given_Missing_ApiKey_When_GetChatClientAsync_Invoked_Then_It_Should_Throw(string? apiKey, Type expected, string message)
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
    [InlineData("invalid-uri-format")]
    [InlineData("not-a-url")]
    public async Task Given_Invalid_Endpoint_Format_When_GetChatClientAsync_Invoked_Then_It_Should_Throw(string invalidEndpoint)
    {
        // Arrange
        var settings = BuildAppSettings(endpoint: invalidEndpoint);
        var connector = new AzureAIFoundryConnector(settings);

        // Act
        var ex = await Assert.ThrowsAsync<UriFormatException>(async () => await connector.GetChatClientAsync());

        // Assert
        ex.ShouldNotBeNull();
    }

    [Trait("Category", "UnitTest")]
    [Fact]
    public async Task Given_Valid_Settings_When_CreateChatClientAsync_Invoked_Then_It_Should_Return_ChatClient()
    {
        // Arrange
        var settings = BuildAppSettings();

        // Act
        var client = await LanguageModelConnector.CreateChatClientAsync(settings);

        // Assert
        client.ShouldNotBeNull();
    }

    [Trait("Category", "UnitTest")]
    [Fact]
    public async Task Given_Invalid_Settings_When_CreateChatClientAsync_Invoked_Then_It_Should_Throw()
    {
        // Arrange
        var settings = new AppSettings
        {
            ConnectorType = ConnectorType.AzureAIFoundry,
            AzureAIFoundry = new AzureAIFoundrySettings
            {
                Endpoint = null, // Invalid
                ApiKey = ApiKey,
                DeploymentName = DeploymentName
            }
        };

        // Act
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () => 
            await LanguageModelConnector.CreateChatClientAsync(settings));
        
        // Assert
        ex.Message.ShouldContain("AzureAIFoundry:Endpoint");
    }
}
