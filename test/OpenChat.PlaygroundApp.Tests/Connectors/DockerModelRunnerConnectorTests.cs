using Microsoft.Extensions.AI;

using OpenChat.PlaygroundApp.Abstractions;
using OpenChat.PlaygroundApp.Configurations;
using OpenChat.PlaygroundApp.Connectors;

namespace OpenChat.PlaygroundApp.Tests.Connectors;

public class DockerModelRunnerConnectorTests
{
    private const string BaseUrl = "http://localhost:12434";
    private const string Model = "ai/smollm2";

    private static AppSettings BuildAppSettings(string? baseUrl = BaseUrl, string? model = Model)
    {
        return new AppSettings
        {
            ConnectorType = ConnectorType.DockerModelRunner,
            DockerModelRunner = new DockerModelRunnerSettings
            {
                BaseUrl = baseUrl,
                Model = model
            }
        };
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData(typeof(LanguageModelConnector), typeof(DockerModelRunnerConnector), true)]
    [InlineData(typeof(DockerModelRunnerConnector), typeof(LanguageModelConnector), false)]
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
        Action action = () => new DockerModelRunnerConnector(null!);

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
        var result = new DockerModelRunnerConnector(settings);

        // Assert
        result.ShouldNotBeNull();
    }

    [Trait("Category", "UnitTest")]
    [Fact]
    public void Given_Null_DockerModelRunnerSettings_When_EnsureLanguageModelSettingsValid_Invoked_Then_It_Should_Throw()
    {
        // Arrange
        var settings = new AppSettings
        {
            ConnectorType = ConnectorType.DockerModelRunner,
            DockerModelRunner = null
        };
        var connector = new DockerModelRunnerConnector(settings);

        // Act
        Action action = () => connector.EnsureLanguageModelSettingsValid();

        // Assert
        action.ShouldThrow<InvalidOperationException>()
              .Message.ShouldContain("DockerModelRunner");
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData(null, typeof(InvalidOperationException), "DockerModelRunner:BaseUrl")]
    [InlineData("", typeof(InvalidOperationException), "DockerModelRunner:BaseUrl")]
    [InlineData("   ", typeof(InvalidOperationException), "DockerModelRunner:BaseUrl")]
    [InlineData("\t\r\n", typeof(InvalidOperationException), "DockerModelRunner:BaseUrl")]
    public void Given_Invalid_BaseUrl_When_EnsureLanguageModelSettingsValid_Invoked_Then_It_Should_Throw(string? baseUrl, Type expectedType, string expectedMessage)
    {
        // Arrange
        var settings = BuildAppSettings(baseUrl: baseUrl);
        var connector = new DockerModelRunnerConnector(settings);

        // Act
        Action action = () => connector.EnsureLanguageModelSettingsValid();

        // Assert
        action.ShouldThrow(expectedType)
              .Message.ShouldContain(expectedMessage);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData(null, typeof(InvalidOperationException), "DockerModelRunner:Model")]
    [InlineData("", typeof(InvalidOperationException), "DockerModelRunner:Model")]
    [InlineData("   ", typeof(InvalidOperationException), "DockerModelRunner:Model")]
    [InlineData("\t\r\n", typeof(InvalidOperationException), "DockerModelRunner:Model")]
    public void Given_Invalid_Model_When_EnsureLanguageModelSettingsValid_Invoked_Then_It_Should_Throw(string? model, Type expectedType, string expectedMessage)
    {
        // Arrange
        var settings = BuildAppSettings(model: model);
        var connector = new DockerModelRunnerConnector(settings);

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
        var connector = new DockerModelRunnerConnector(settings);

        // Act
        var result = connector.EnsureLanguageModelSettingsValid();

        // Assert
        result.ShouldBeTrue();
    }

    [Trait("Category", "UnitTest")]
    [Fact]
    public void Given_Null_DockerModelRunnerSettings_When_GetChatClientAsync_Invoked_Then_It_Should_Throw()
    {
        // Arrange
        var settings = new AppSettings
        {
            ConnectorType = ConnectorType.DockerModelRunner,
            DockerModelRunner = null
        };
        var connector = new DockerModelRunnerConnector(settings);

        // Act
        Func<Task> func = async () => await connector.GetChatClientAsync();

        // Assert
        func.ShouldThrow<NullReferenceException>()
            .Message.ShouldContain("Object reference not set to an instance of an object");
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData(null, typeof(NullReferenceException), "Object reference not set to an instance of an object")]
    [InlineData("", typeof(UriFormatException), "Invalid URI:")]
    [InlineData("   ", typeof(UriFormatException), "Invalid URI:")]
    [InlineData("\t\r\n", typeof(UriFormatException), "Invalid URI:")]
    [InlineData("invalid-uri-format", typeof(UriFormatException), "Invalid URI:")]
    public void Given_Invalid_BaseUrl_When_GetChatClient_Invoked_Then_It_Should_Throw(string? baseUrl, Type expectedType, string message)
    {
        // Arrange
        var settings = BuildAppSettings(baseUrl: baseUrl);
        var connector = new DockerModelRunnerConnector(settings);

        // Act
        Func<Task> func = async () => await connector.GetChatClientAsync();

        // Assert
        func.ShouldThrow(expectedType)
            .Message.ShouldContain(message);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData(null, typeof(NullReferenceException), "Object reference not set to an instance of an object")]
    [InlineData("", typeof(ArgumentException), "model")]
    [InlineData("   ", typeof(ArgumentException), "model")]
    [InlineData("\t\r\n", typeof(ArgumentException), "model")]
    public void Given_Invalid_Model_When_GetChatClient_Invoked_Then_It_Should_Throw(string? model, Type expectedType, string message)
    {
        // Arrange
        var settings = BuildAppSettings(model: model);
        var connector = new DockerModelRunnerConnector(settings);

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
        var connector = new DockerModelRunnerConnector(settings);

        // Act
        var client = await connector.GetChatClientAsync();

        // Assert
        client.ShouldNotBeNull();
        client.ShouldBeAssignableTo<IChatClient>();
    }

    [Trait("Category", "UnitTest")]
    [Theory]
	[InlineData(null, Model, typeof(InvalidOperationException),"DockerModelRunner:BaseUrl")]
    [InlineData("", Model, typeof(InvalidOperationException), "DockerModelRunner:BaseUrl")]
    [InlineData("   ", Model, typeof(InvalidOperationException), "DockerModelRunner:BaseUrl")]
    [InlineData("\t\r\n", Model, typeof(InvalidOperationException), "DockerModelRunner:BaseUrl")]
	[InlineData(BaseUrl, null, typeof(InvalidOperationException), "DockerModelRunner:Model")]
    [InlineData(BaseUrl, "", typeof(InvalidOperationException), "DockerModelRunner:Model")]
    [InlineData(BaseUrl, "   ", typeof(InvalidOperationException), "DockerModelRunner:Model")]
    [InlineData(BaseUrl, "\t\r\n", typeof(InvalidOperationException), "DockerModelRunner:Model")]
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

    [Trait("Category", "UnitTest")]
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
