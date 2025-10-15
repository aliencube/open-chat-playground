using Microsoft.Extensions.AI;

using OpenChat.PlaygroundApp.Abstractions;
using OpenChat.PlaygroundApp.Configurations;
using OpenChat.PlaygroundApp.Connectors;

namespace OpenChat.PlaygroundApp.Tests.Connectors;

public class UpstageConnectorTests
{
    private const string BaseUrl = "https://api.upstage.ai/v1/solar";
    private const string ApiKey = "test-api-key";
    private const string Model = "solar-1-mini-chat";

    private static AppSettings BuildAppSettings(string? baseUrl = BaseUrl, string? apiKey = ApiKey, string? model = Model)
    {
        return new AppSettings
        {
            ConnectorType = ConnectorType.Upstage,
            Upstage = new UpstageSettings
            {
                BaseUrl = baseUrl,
                ApiKey = apiKey,
                Model = model
            }
        };
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData(typeof(LanguageModelConnector), typeof(UpstageConnector), true)]
    [InlineData(typeof(UpstageConnector), typeof(LanguageModelConnector), false)]
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
        Action action = () => new UpstageConnector(null!);

        // Assert
        action.ShouldThrow<NullReferenceException>()
              .Message.ShouldContain("Object reference not set to an instance of an object");
    }

    [Trait("Category", "UnitTest")]
    [Fact]
    public void Given_Settings_When_Instantiated_Then_It_Should_Return()
    {
        // Arrange
        var settings = BuildAppSettings();

        // Act
        var result = new UpstageConnector(settings);

        // Assert
        result.ShouldNotBeNull();
    }

    [Trait("Category", "UnitTest")]
    [Fact]
    public void Given_Settings_Is_Null_When_EnsureLanguageModelSettingsValid_Invoked_Then_It_Should_Throw()
    {
        // Arrange
        var appSettings = new AppSettings {
            ConnectorType = ConnectorType.Upstage,
            Upstage = null
        };
        var connector = new UpstageConnector(appSettings);

        // Act
        Action action = () => connector.EnsureLanguageModelSettingsValid();

        // Assert
        action.ShouldThrow<InvalidOperationException>()
              .Message.ShouldContain("Missing configuration: Upstage.");
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData(null, typeof(InvalidOperationException), "Upstage:BaseUrl")]
    [InlineData("", typeof(InvalidOperationException), "Upstage:BaseUrl")]
    [InlineData("   ", typeof(InvalidOperationException), "Upstage:BaseUrl")]
    [InlineData("\t\r\n", typeof(InvalidOperationException), "Upstage:BaseUrl")]
    public void Given_Invalid_BaseUrl_When_EnsureLanguageModelSettingsValid_Invoked_Then_It_Should_Throw(string? baseUrl, Type expectedType, string expectedMessage)
    {
        // Arrange
        var appSettings = BuildAppSettings(baseUrl: baseUrl);
        var connector = new UpstageConnector(appSettings);

        // Act
        Action action = () => connector.EnsureLanguageModelSettingsValid();

        // Assert
        action.ShouldThrow(expectedType)
              .Message.ShouldContain(expectedMessage);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData(null, typeof(InvalidOperationException), "Upstage:ApiKey")]
    [InlineData("", typeof(InvalidOperationException), "Upstage:ApiKey")]
    [InlineData("   ", typeof(InvalidOperationException), "Upstage:ApiKey")]
    [InlineData("\t\r\n", typeof(InvalidOperationException), "Upstage:ApiKey")]
    public void Given_Invalid_ApiKey_When_EnsureLanguageModelSettingsValid_Invoked_Then_It_Should_Throw(string? apiKey, Type expectedType, string expectedMessage)
    {
        // Arrange
        var appSettings = BuildAppSettings(apiKey: apiKey);
        var connector = new UpstageConnector(appSettings);

        // Act
        Action action = () => connector.EnsureLanguageModelSettingsValid();

        // Assert
        action.ShouldThrow(expectedType)
              .Message.ShouldContain(expectedMessage);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData(null, typeof(InvalidOperationException), "Upstage:Model")]
    [InlineData("", typeof(InvalidOperationException), "Upstage:Model")]
    [InlineData("   ", typeof(InvalidOperationException), "Upstage:Model")]
    [InlineData("\t\r\n", typeof(InvalidOperationException), "Upstage:Model")]
    public void Given_Invalid_Model_When_EnsureLanguageModelSettingsValid_Invoked_Then_It_Should_Throw(string? model, Type expectedType, string expectedMessage)
    {
        // Arrange
        var appSettings = BuildAppSettings(model: model);
        var connector = new UpstageConnector(appSettings);

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
        var appSettings = BuildAppSettings();
        var connector = new UpstageConnector(appSettings);

        // Act
        var result = connector.EnsureLanguageModelSettingsValid();

        // Assert
        result.ShouldBeTrue();
    }

    [Trait("Category", "UnitTest")]
    [Fact]
    public void Given_Null_UpstageSettings_When_GetChatClientAsync_Invoked_Then_It_Should_Throw()
    {
        // Arrange
        var settings = new AppSettings
        {
            ConnectorType = ConnectorType.Upstage,
            Upstage = null
        };
        var connector = new UpstageConnector(settings);

        // Act
        Func<Task> func = connector.GetChatClientAsync;

        // Assert
        func.ShouldThrow<InvalidOperationException>()
            .Message.ShouldContain("Missing configuration");
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData(null, typeof(InvalidOperationException), "Upstage:BaseUrl")]
    [InlineData("", typeof(UriFormatException), "empty")]
    [InlineData("invalid-uri-format", typeof(UriFormatException), "Invalid URI: The format of the URI could not be determined.")]
    [InlineData("not-a-url", typeof(UriFormatException), "Invalid URI: The format of the URI could not be determined.")]
    [InlineData("   ", typeof(UriFormatException), "Invalid URI: The format of the URI could not be determined.")]
    public void Given_Missing_BaseUrl_When_GetChatClient_Invoked_Then_It_Should_Throw(string? baseUrl, Type expected, string message)
    {
        // Arrange
        var settings = BuildAppSettings(baseUrl: baseUrl);
        var connector = new UpstageConnector(settings);

        // Act
        Func<Task> func = connector.GetChatClientAsync;

        // Assert
        func.ShouldThrow(expected)
            .Message.ShouldContain(message);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData(null, typeof(InvalidOperationException), "Upstage:ApiKey")]
    [InlineData("", typeof(ArgumentException), "key")]
    public void Given_Missing_ApiKey_When_GetChatClient_Invoked_Then_It_Should_Throw(string? apiKey, Type expected, string message)
    {
        // Arrange
        var settings = BuildAppSettings(apiKey: apiKey);
        var connector = new UpstageConnector(settings);

        // Act
        Func<Task> func = connector.GetChatClientAsync;

        // Assert
        func.ShouldThrow(expected)
            .Message.ShouldContain(message);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData(null, typeof(ArgumentNullException), "model")]
    [InlineData("", typeof(ArgumentException), "model")]
    public void Given_Missing_Model_When_GetChatClient_Invoked_Then_It_Should_Throw(string? model, Type expected, string message)
    {
        // Arrange
        var settings = BuildAppSettings(model: model);
        var connector = new UpstageConnector(settings);

        // Act
        Func<Task> func = connector.GetChatClientAsync;

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
        var connector = new UpstageConnector(settings);

        // Act
        var client = await connector.GetChatClientAsync();

        // Assert
        client.ShouldNotBeNull();
        client.ShouldBeAssignableTo<IChatClient>();
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("apiKey", null, typeof(InvalidOperationException), "Upstage:ApiKey")]
    [InlineData("apiKey", "", typeof(InvalidOperationException), "Upstage:ApiKey")]
    [InlineData("apiKey", "   ", typeof(InvalidOperationException), "Upstage:ApiKey")]
    [InlineData("apiKey", "\t\n\r", typeof(InvalidOperationException), "Upstage:ApiKey")]
    [InlineData("baseUrl", null, typeof(InvalidOperationException), "Upstage:BaseUrl")]
    [InlineData("baseUrl", "", typeof(InvalidOperationException), "Upstage:BaseUrl")]
    [InlineData("baseUrl", "   ", typeof(InvalidOperationException), "Upstage:BaseUrl")]
    [InlineData("baseUrl", "\t\n\r", typeof(InvalidOperationException), "Upstage:BaseUrl")]
    [InlineData("model", null, typeof(InvalidOperationException), "Upstage:Model")]
    [InlineData("model", "", typeof(InvalidOperationException), "Upstage:Model")]
    [InlineData("model", "   ", typeof(InvalidOperationException), "Upstage:Model")]
    [InlineData("model", "\t\n\r", typeof(InvalidOperationException), "Upstage:Model")]
    public void Given_Invalid_Settings_When_CreateChatClientAsync_Invoked_Then_It_Should_Throw(string parameterName, string? invalidValue, Type expectedType, string expectedMessage)
    {
        // Arrange
        var settings = parameterName switch
        {
            "apiKey" => BuildAppSettings(apiKey: invalidValue),
            "baseUrl" => BuildAppSettings(baseUrl: invalidValue),
            "model" => BuildAppSettings(model: invalidValue),
            _ => throw new ArgumentException($"Unknown parameter: {parameterName}")
        };

        // Act
        Func<Task> func = async () => await LanguageModelConnector.CreateChatClientAsync(settings);

        // Assert
        func.ShouldThrow(expectedType)
            .Message.ShouldContain(expectedMessage);
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
        client.ShouldBeAssignableTo<IChatClient>();
    }
}