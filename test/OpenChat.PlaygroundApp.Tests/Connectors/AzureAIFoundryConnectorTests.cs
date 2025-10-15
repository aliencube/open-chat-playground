using Microsoft.Extensions.AI;

using OpenChat.PlaygroundApp.Abstractions;
using OpenChat.PlaygroundApp.Configurations;
using OpenChat.PlaygroundApp.Connectors;

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
    public void Given_Null_Settings_When_Instantiated_Then_It_Should_Throw()
    {
        // Act
        Action action = () => new AzureAIFoundryConnector(null!);

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
        var result = new AzureAIFoundryConnector(settings);

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
            ConnectorType = ConnectorType.AzureAIFoundry,
            AzureAIFoundry = null
        };
        var connector = new AzureAIFoundryConnector(settings);

        // Act
        Action action = () => connector.EnsureLanguageModelSettingsValid();

        // Assert
        action.ShouldThrow<InvalidOperationException>()
              .Message.ShouldContain("AzureAIFoundry");
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData(null, typeof(InvalidOperationException), "AzureAIFoundry:Endpoint")]
    [InlineData("", typeof(InvalidOperationException), "AzureAIFoundry:Endpoint")]
    [InlineData("   ", typeof(InvalidOperationException), "AzureAIFoundry:Endpoint")]
    [InlineData("\t\r\n", typeof(InvalidOperationException), "AzureAIFoundry:Endpoint")]
    public void Given_Invalid_Endpoint_When_EnsureLanguageModelSettingsValid_Invoked_Then_It_Should_Throw(string? endpoint, Type expectedType, string expectedMessage)
    {
        // Arrange
        var settings = BuildAppSettings(endpoint: endpoint);
        var connector = new AzureAIFoundryConnector(settings);

        // Act
        Action action = () => connector.EnsureLanguageModelSettingsValid();

        // Assert
        action.ShouldThrow(expectedType)
              .Message.ShouldContain(expectedMessage);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData(null, typeof(InvalidOperationException), "AzureAIFoundry:ApiKey")]
    [InlineData("", typeof(InvalidOperationException), "AzureAIFoundry:ApiKey")]
    [InlineData("   ", typeof(InvalidOperationException), "AzureAIFoundry:ApiKey")]
    [InlineData("\t\r\n", typeof(InvalidOperationException), "AzureAIFoundry:ApiKey")]
    public void Given_Invalid_ApiKey_When_EnsureLanguageModelSettingsValid_Invoked_Then_It_Should_Throw(string? apiKey, Type expectedType, string expectedMessage)
    {
        // Arrange
        var settings = BuildAppSettings(apiKey: apiKey);
        var connector = new AzureAIFoundryConnector(settings);

        // Act
        Action action = () => connector.EnsureLanguageModelSettingsValid();

        // Assert
        action.ShouldThrow(expectedType)
              .Message.ShouldContain(expectedMessage);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData(null, typeof(InvalidOperationException), "AzureAIFoundry:DeploymentName")]
    [InlineData("", typeof(InvalidOperationException), "AzureAIFoundry:DeploymentName")]
    [InlineData("   ", typeof(InvalidOperationException), "AzureAIFoundry:DeploymentName")]
    [InlineData("\t\r\n", typeof(InvalidOperationException), "AzureAIFoundry:DeploymentName")]
    public void Given_Invalid_DeploymentName_When_EnsureLanguageModelSettingsValid_Invoked_Then_It_Should_Throw(string? deploymentName, Type expectedType, string expectedMessage)
    {
        // Arrange
        var settings = BuildAppSettings(deploymentName: deploymentName);
        var connector = new AzureAIFoundryConnector(settings);

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
        var connector = new AzureAIFoundryConnector(settings);

        // Act
        var result = connector.EnsureLanguageModelSettingsValid();

        // Assert
        result.ShouldBeTrue();
    }

    [Trait("Category", "UnitTest")]
    [Fact]
    public void Given_Null_AzureAIFoundrySettings_When_GetChatClientAsync_Invoked_Then_It_Should_Throw()
    {
        // Arrange
        var settings = new AppSettings
        {
            ConnectorType = ConnectorType.AzureAIFoundry,
            AzureAIFoundry = null
        };
        var connector = new AzureAIFoundryConnector(settings);

        // Act
        Func<Task> func = connector.GetChatClientAsync;

        // Assert
        func.ShouldThrow<NullReferenceException>()
            .Message.ShouldContain("Object reference not set to an instance of an object");
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData(null, typeof(ArgumentNullException), "Value cannot be null.")]
    [InlineData("", typeof(UriFormatException), "Invalid URI: The URI is empty.")]
    [InlineData("invalid-uri-format", typeof(UriFormatException), "Invalid URI: The format of the URI could not be determined.")]
    [InlineData("not-a-url", typeof(UriFormatException), "Invalid URI: The format of the URI could not be determined.")]
    [InlineData("   ", typeof(UriFormatException), "Invalid URI: The format of the URI could not be determined.")]
    public void Given_Invalid_Endpoint_When_GetChatClientAsync_Invoked_Then_It_Should_Throw(string? endpoint, Type expectedType, string message)
    {
        // Arrange
        var settings = BuildAppSettings(endpoint: endpoint);
        var connector = new AzureAIFoundryConnector(settings);

        // Act
        Func<Task> func = async () => await connector.GetChatClientAsync();

        // Assert
        func.ShouldThrow(expectedType)
            .Message.ShouldContain(message);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData(null, "key")]
    [InlineData("", "key")]
    public void Given_Invalid_ApiKey_When_GetChatClientAsync_Invoked_Then_It_Should_Throw(string? apiKey, string message)
    {
        // Arrange
        var settings = BuildAppSettings(apiKey: apiKey);
        var connector = new AzureAIFoundryConnector(settings);

        // Act
        Func<Task> func = async () => await connector.GetChatClientAsync();

        // Assert  
        func.ShouldThrow<ArgumentException>()
            .Message.ShouldContain(message);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData(null, "model")]
    [InlineData("", "model")]
    public void Given_Invalid_DeploymentName_When_GetChatClientAsync_Invoked_Then_It_Should_Throw(string? deploymentName, string message)
    {
        // Arrange
        var settings = BuildAppSettings(deploymentName: deploymentName);
        var connector = new AzureAIFoundryConnector(settings);

        // Act
        Func<Task> func = async () => await connector.GetChatClientAsync();

        // Assert  
        func.ShouldThrow<ArgumentException>()
            .Message.ShouldContain(message);
    }

    [Trait("Category", "UnitTest")]
    [Fact]
    public async Task Given_Valid_Settings_When_GetChatClient_Invoked_Then_It_Should_Return_ChatClient()
    {
        // Arrange
        var settings = BuildAppSettings();
        var connector = new AzureAIFoundryConnector(settings);

        // Act
        var result = await connector.GetChatClientAsync();

        // Assert
        result.ShouldNotBeNull();
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData(null, null, null, "Missing configuration: AzureAIFoundry")]
    [InlineData("", ApiKey, DeploymentName, "Missing configuration: AzureAIFoundry:Endpoint")]
    [InlineData("   ", ApiKey, DeploymentName, "Missing configuration: AzureAIFoundry:Endpoint")]
    [InlineData(Endpoint, null, DeploymentName, "Missing configuration: AzureAIFoundry:ApiKey")]
    [InlineData(Endpoint, "", DeploymentName, "Missing configuration: AzureAIFoundry:ApiKey")]
    [InlineData(Endpoint, "   ", DeploymentName, "Missing configuration: AzureAIFoundry:ApiKey")]
    [InlineData(Endpoint, ApiKey, null, "Missing configuration: AzureAIFoundry:DeploymentName")]
    [InlineData(Endpoint, ApiKey, "", "Missing configuration: AzureAIFoundry:DeploymentName")]
    [InlineData(Endpoint, ApiKey, "   ", "Missing configuration: AzureAIFoundry:DeploymentName")]
    public void Given_Invalid_Settings_When_CreateChatClientAsync_Invoked_Then_It_Should_Throw(string? endpoint, string? apiKey, string? deploymentName, string expectedMessage)
    {
        // Arrange
        var settings = new AppSettings
        {
            ConnectorType = ConnectorType.AzureAIFoundry,
            AzureAIFoundry = new AzureAIFoundrySettings
            {
                Endpoint = endpoint,
                ApiKey = apiKey,
                DeploymentName = deploymentName
            }
        };

        // Act
        Func<Task> func = async () => await LanguageModelConnector.CreateChatClientAsync(settings);

        // Assert  
        func.ShouldThrow<InvalidOperationException>()
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
