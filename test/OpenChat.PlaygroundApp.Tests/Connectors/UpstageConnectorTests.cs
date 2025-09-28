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
    public void Given_Settings_Is_Null_When_EnsureLanguageModelSettingsValid_Invoked_Then_It_Should_Throw()
    {
        // Arrange
        var appSettings = new AppSettings { ConnectorType = ConnectorType.Upstage, Upstage = null };
        var connector = new UpstageConnector(appSettings);

        // Act & Assert
        Action action = () => connector.EnsureLanguageModelSettingsValid();
        action.ShouldThrow<InvalidOperationException>()
              .Message.ShouldContain("Upstage");
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData(null, typeof(InvalidOperationException), "Upstage:BaseUrl")]
    [InlineData("", typeof(InvalidOperationException), "Upstage:BaseUrl")]
    [InlineData("   ", typeof(InvalidOperationException), "Upstage:BaseUrl")]
    public void Given_Invalid_BaseUrl_When_EnsureLanguageModelSettingsValid_Invoked_Then_It_Should_Throw(string? baseUrl, Type expectedType, string expectedMessage)
    {
        // Arrange
        var appSettings = BuildAppSettings(baseUrl: baseUrl);
        var connector = new UpstageConnector(appSettings);

        // Act & Assert
        Action action = () => connector.EnsureLanguageModelSettingsValid();
        action.ShouldThrow(expectedType)
              .Message.ShouldContain(expectedMessage);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData(null, typeof(InvalidOperationException), "Upstage:ApiKey")]
    [InlineData("", typeof(InvalidOperationException), "Upstage:ApiKey")]
    [InlineData("   ", typeof(InvalidOperationException), "Upstage:ApiKey")]
    public void Given_Invalid_ApiKey_When_EnsureLanguageModelSettingsValid_Invoked_Then_It_Should_Throw(string? apiKey, Type expectedType, string expectedMessage)
    {
        // Arrange
        var appSettings = BuildAppSettings(apiKey: apiKey);
        var connector = new UpstageConnector(appSettings);

        // Act & Assert
        Action action = () => connector.EnsureLanguageModelSettingsValid();
        action.ShouldThrow(expectedType)
              .Message.ShouldContain(expectedMessage);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData(null, typeof(InvalidOperationException), "Upstage:Model")]
    [InlineData("", typeof(InvalidOperationException), "Upstage:Model")]
    [InlineData("   ", typeof(InvalidOperationException), "Upstage:Model")]
    public void Given_Invalid_Model_When_EnsureLanguageModelSettingsValid_Invoked_Then_It_Should_Throw(string? model, Type expectedType, string expectedMessage)
    {
        // Arrange
        var appSettings = BuildAppSettings(model: model);
        var connector = new UpstageConnector(appSettings);

        // Act & Assert
        Action action = () => connector.EnsureLanguageModelSettingsValid();
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
    public async Task Given_Valid_Settings_When_GetChatClient_Invoked_Then_It_Should_Return_ChatClient()
    {
        // Arrange
        var settings = BuildAppSettings();
        var connector = new UpstageConnector(settings);

        // Act
        var client = await connector.GetChatClientAsync();

        // Assert
        client.ShouldNotBeNull();
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

        // Act & Assert
        Func<Task> func = async () => await connector.GetChatClientAsync();
        func.ShouldThrow(expected)
            .Message.ShouldContain(message);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData(null, typeof(InvalidOperationException), "Upstage:BaseUrl")]
    [InlineData("", typeof(UriFormatException), "empty")]
    public void Given_Missing_BaseUrl_When_GetChatClient_Invoked_Then_It_Should_Throw(string? baseUrl, Type expected, string message)
    {
        // Arrange
        var settings = BuildAppSettings(baseUrl: baseUrl);
        var connector = new UpstageConnector(settings);

        // Act & Assert
        Func<Task> func = async () => await connector.GetChatClientAsync();
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

        // Act & Assert
        Func<Task> func = async () => await connector.GetChatClientAsync();
        func.ShouldThrow(expected)
            .Message.ShouldContain(message);
    }
}
