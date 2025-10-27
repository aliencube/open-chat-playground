using Microsoft.Extensions.AI;

using OpenChat.PlaygroundApp.Abstractions;
using OpenChat.PlaygroundApp.Configurations;
using OpenChat.PlaygroundApp.Connectors;

namespace OpenChat.PlaygroundApp.Tests.Connectors;

public class GitHubModelsConnectorTests
{
    private const string Endpoint = "https://models.github.ai/inference";
    private const string Token = "test-token";
    private const string Model = "openai/gpt-4o-mini";

    private static AppSettings BuildAppSettings(string? endpoint = Endpoint, string? token = Token, string? model = Model)
    {
        return new AppSettings
        {
            ConnectorType = ConnectorType.GitHubModels,
            GitHubModels = new GitHubModelsSettings
            {
                Endpoint = endpoint,
                Token = token,
                Model = model
            }
        };
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData(typeof(LanguageModelConnector), typeof(GitHubModelsConnector), true)]
    [InlineData(typeof(GitHubModelsConnector), typeof(LanguageModelConnector), false)]
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
        Action action = () => new GitHubModelsConnector(null!);

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
        var result = new GitHubModelsConnector(settings);

        // Assert
        result.ShouldNotBeNull();
    }

    [Trait("Category", "UnitTest")]
    [Fact]
    public void Given_Null_GitHubModelsSettings_When_EnsureLanguageModelSettingsValid_Invoked_Then_It_Should_Throw()
    {
        // Arrange
        var settings = new AppSettings
        {
            ConnectorType = ConnectorType.GitHubModels,
            GitHubModels = null
        };
        var connector = new GitHubModelsConnector(settings);

        // Act
        Action action = () => connector.EnsureLanguageModelSettingsValid();

        // Assert
        action.ShouldThrow<InvalidOperationException>()
              .Message.ShouldContain("GitHubModels");
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData(null, typeof(NullReferenceException), "Object reference not set to an instance of an object")]
    [InlineData("", typeof(InvalidOperationException), "GitHubModels:Endpoint")]
    [InlineData("   ", typeof(InvalidOperationException), "GitHubModels:Endpoint")]
    [InlineData("\t\r\n", typeof(InvalidOperationException), "GitHubModels:Endpoint")]
    public void Given_Invalid_Endpoint_When_EnsureLanguageModelSettingsValid_Invoked_Then_It_Should_Throw(string? endpoint, Type expectedType, string expectedMessage)
    {
        // Arrange
        var settings = BuildAppSettings(endpoint: endpoint);
        var connector = new GitHubModelsConnector(settings);

        // Act
        Action action = () => connector.EnsureLanguageModelSettingsValid();

        // Assert
        action.ShouldThrow(expectedType)
              .Message.ShouldContain(expectedMessage);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData(null, typeof(NullReferenceException), "Object reference not set to an instance of an object")]
    [InlineData("", typeof(InvalidOperationException), "GitHubModels:Token")]
    [InlineData("   ", typeof(InvalidOperationException), "GitHubModels:Token")]
    [InlineData("\t\r\n", typeof(InvalidOperationException), "GitHubModels:Token")]
    public void Given_Invalid_Token_When_EnsureLanguageModelSettingsValid_Invoked_Then_It_Should_Throw(string? token, Type expectedType, string expectedMessage)
    {
        // Arrange
        var settings = BuildAppSettings(token: token);
        var connector = new GitHubModelsConnector(settings);

        // Act
        Action action = () => connector.EnsureLanguageModelSettingsValid();

        // Assert
        action.ShouldThrow(expectedType)
              .Message.ShouldContain(expectedMessage);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData(null, typeof(NullReferenceException), "Object reference not set to an instance of an object")]
    [InlineData("", typeof(InvalidOperationException), "GitHubModels:Model")]
    [InlineData("   ", typeof(InvalidOperationException), "GitHubModels:Model")]
    [InlineData("\t\r\n", typeof(InvalidOperationException), "GitHubModels:Model")]
    public void Given_Invalid_Model_When_EnsureLanguageModelSettingsValid_Invoked_Then_It_Should_Throw(string? model, Type expectedType, string expectedMessage)
    {
        // Arrange
        var settings = BuildAppSettings(model: model);
        var connector = new GitHubModelsConnector(settings);

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
        var connector = new GitHubModelsConnector(settings);

        // Act
        var result = connector.EnsureLanguageModelSettingsValid();

        // Assert
        result.ShouldBeTrue();
    }

    [Trait("Category", "UnitTest")]
    [Fact]
    public void Given_Null_GitHubModelsSettings_When_GetChatClientAsync_Invoked_Then_It_Should_Throw()
    {
        // Arrange
        var settings = new AppSettings
        {
            ConnectorType = ConnectorType.GitHubModels,
            GitHubModels = null
        };
        var connector = new GitHubModelsConnector(settings);

        // Act
        Func<Task> func = async () => await connector.GetChatClientAsync();

        // Assert
        func.ShouldThrow<NullReferenceException>()
            .Message.ShouldContain("Object reference not set to an instance of an object.");
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData(null, typeof(NullReferenceException), "Object reference not set to an instance of an object.")]
    [InlineData("", typeof(UriFormatException), "Invalid URI:")]
    [InlineData("   ", typeof(UriFormatException), "Invalid URI:")]
    [InlineData("\t\r\n", typeof(UriFormatException), "Invalid URI:")]
    [InlineData("invalid-uri-format", typeof(UriFormatException), "Invalid URI:")]
    public void Given_Invalid_Endpoint_When_GetChatClient_Invoked_Then_It_Should_Throw(string? endpoint, Type expected, string message)
    {
        // Arrange
        var settings = BuildAppSettings(endpoint: endpoint);
        var connector = new GitHubModelsConnector(settings);

        // Act
        Func<Task> func = async () => await connector.GetChatClientAsync();

        // Assert
        func.ShouldThrow(expected)
            .Message.ShouldContain(message);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData(null, typeof(NullReferenceException), "Object reference not set to an instance of an object.")]
    [InlineData("", typeof(ArgumentException), "key")]
    [InlineData("   ", typeof(ArgumentException), "key")]
    [InlineData("\t\r\n", typeof(ArgumentException), "key")]
    public void Given_Invalid_Token_When_GetChatClient_Invoked_Then_It_Should_Throw(string? token, Type expected, string message)
    {
        // Arrange
        var settings = BuildAppSettings(token: token);
        var connector = new GitHubModelsConnector(settings);

        // Act
        Func<Task> func = async () => await connector.GetChatClientAsync();

        // Assert
        func.ShouldThrow(expected)
            .Message.ShouldContain(message);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData(null, typeof(NullReferenceException), "Object reference not set to an instance of an object.")]
    [InlineData("", typeof(ArgumentException), "model")]
    [InlineData("   ", typeof(ArgumentException), "model")]
    [InlineData("\t\r\n", typeof(ArgumentException), "model")]
    public void Given_Invalid_Model_When_GetChatClient_Invoked_Then_It_Should_Throw(string? model, Type expected, string message)
    {
        // Arrange
        var settings = BuildAppSettings(model: model);
        var connector = new GitHubModelsConnector(settings);

        // Act
        Func<Task> func = async () => await connector.GetChatClientAsync();

        // Assert
        func.ShouldThrow(expected)
            .Message.ShouldContain(message);
    }

    [Trait("Category", "UnitTest")]
    [Fact]
    public async Task Given_Valid_Settings_When_GetChatClient_Invoked_Then_It_Should_Return_ChatClient()
    {
        // Arrange
        var settings = BuildAppSettings();
        var connector = new GitHubModelsConnector(settings);

        // Act
        var client = await connector.GetChatClientAsync();

        // Assert
        client.ShouldNotBeNull();
        client.ShouldBeAssignableTo<IChatClient>();
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData(null, null, null, typeof(NullReferenceException), "Object reference not set to an instance of an object.")]
    [InlineData("", Token, Model, typeof(InvalidOperationException), "Missing configuration: GitHubModels:Endpoint")]
    [InlineData("   ", Token, Model, typeof(InvalidOperationException), "Missing configuration: GitHubModels:Endpoint")]
    [InlineData("\t\r\n", Token, Model, typeof(InvalidOperationException), "Missing configuration: GitHubModels:Endpoint")]
    [InlineData(Endpoint, null, Model, typeof(NullReferenceException), "Object reference not set to an instance of an object.")]
    [InlineData(Endpoint, "", Model, typeof(InvalidOperationException), "Missing configuration: GitHubModels:Token")]
    [InlineData(Endpoint, "   ", Model, typeof(InvalidOperationException), "Missing configuration: GitHubModels:Token")]
    [InlineData(Endpoint, "\t\r\n", Model, typeof(InvalidOperationException), "Missing configuration: GitHubModels:Token")]
    [InlineData(Endpoint, Token, null, typeof(NullReferenceException), "Object reference not set to an instance of an object.")]
    [InlineData(Endpoint, Token, "", typeof(InvalidOperationException), "Missing configuration: GitHubModels:Model")]
    [InlineData(Endpoint, Token, "   ", typeof(InvalidOperationException), "Missing configuration: GitHubModels:Model")]
    [InlineData(Endpoint, Token, "\t\r\n", typeof(InvalidOperationException), "Missing configuration: GitHubModels:Model")]
    public void Given_Invalid_Settings_When_CreateChatClientAsync_Invoked_Then_It_Should_Throw(string? endpoint, string? token, string? model, Type expected, string expectedMessage)
    {
        // Arrange
        var settings = new AppSettings
        {
            ConnectorType = ConnectorType.GitHubModels,
            GitHubModels = new GitHubModelsSettings
            {
                Endpoint = endpoint,
                Token = token,
                Model = model
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