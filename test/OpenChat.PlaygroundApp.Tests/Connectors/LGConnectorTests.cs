using Microsoft.Extensions.AI;

using OpenChat.PlaygroundApp.Abstractions;
using OpenChat.PlaygroundApp.Configurations;
using OpenChat.PlaygroundApp.Connectors;

namespace OpenChat.PlaygroundApp.Tests.Connectors;

public class LGConnectorTests
{
    private const string BaseUrl = "https://test.lg-exaone/api";
    private const string Model = "hf.co/LGAI-EXAONE/EXAONE-4.0-1.2B-GGUF";

    private static AppSettings BuildAppSettings(string? baseUrl = BaseUrl, string? model = Model)
    {
        return new AppSettings
        {
            ConnectorType = ConnectorType.LG,
            LG = new LGSettings
            {
                BaseUrl = baseUrl,
                Model = model
            }
        };
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData(typeof(LanguageModelConnector), typeof(LGConnector), true)]
    [InlineData(typeof(LGConnector), typeof(LanguageModelConnector), false)]
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
        var appSettings = new AppSettings { ConnectorType = ConnectorType.LG, LG = null };
        var connector = new LGConnector(appSettings);

        // Act
        Action action = () => connector.EnsureLanguageModelSettingsValid();

        // Assert
        action.ShouldThrow<InvalidOperationException>()
              .Message.ShouldContain("LG");
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData(null, typeof(NullReferenceException), "Object reference not set to an instance of an object")]
    [InlineData("", typeof(InvalidOperationException), "LG:BaseUrl")]
    [InlineData("   ", typeof(InvalidOperationException), "LG:BaseUrl")]
    public void Given_Invalid_BaseUrl_When_EnsureLanguageModelSettingsValid_Invoked_Then_It_Should_Throw(string? baseUrl, Type expectedType, string expectedMessage)
    {
        // Arrange
        var appSettings = BuildAppSettings(baseUrl: baseUrl);
        var connector = new LGConnector(appSettings);

        // Act
        Action action = () => connector.EnsureLanguageModelSettingsValid();

        // Assert
        action.ShouldThrow(expectedType)
              .Message.ShouldContain(expectedMessage);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData(null, typeof(NullReferenceException), "Object reference not set to an instance of an object")]
    [InlineData("", typeof(InvalidOperationException), "LG:Model")]
    [InlineData("   ", typeof(InvalidOperationException), "LG:Model")]
    public void Given_Invalid_Model_When_EnsureLanguageModelSettingsValid_Invoked_Then_It_Should_Throw(string? model, Type expectedType, string expectedMessage)
    {
        // Arrange
        var appSettings = BuildAppSettings(model: model);
        var connector = new LGConnector(appSettings);

        // Act
        Action action = () => connector.EnsureLanguageModelSettingsValid();

        // Assert
        action.ShouldThrow(expectedType)
              .Message.ShouldContain(expectedMessage);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("invalid-model", typeof(InvalidOperationException), "Invalid LG model format")]
    [InlineData("random-name", typeof(InvalidOperationException), "Invalid LG model format")]
    [InlineData("test-model", typeof(InvalidOperationException), "Invalid LG model format")]
    public void Given_Invalid_Model_Format_When_EnsureLanguageModelSettingsValid_Invoked_Then_It_Should_Throw(string model, Type expectedType, string expectedMessage)
    {
        // Arrange
        var appSettings = BuildAppSettings(model: model);
        var connector = new LGConnector(appSettings);

        // Act
        Action action = () => connector.EnsureLanguageModelSettingsValid();

        // Assert
        action.ShouldThrow(expectedType)
              .Message.ShouldContain(expectedMessage);
    }

    [Trait("Category", "UnitTest")]
    [Fact]
    public void Given_Valid_Model_Format_When_EnsureLanguageModelSettingsValid_Invoked_Then_It_Should_Return_True()
    {
        // Arrange
        var appSettings = BuildAppSettings();
        var connector = new LGConnector(appSettings);

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
        var connector = new LGConnector(settings);

        // Act
        var client = await connector.GetChatClientAsync();

        // Assert
        client.ShouldNotBeNull();
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData(null, typeof(ArgumentNullException))]
    [InlineData("", typeof(UriFormatException))]
    public void Given_Missing_BaseUrl_When_GetChatClient_Invoked_Then_It_Should_Throw(string? baseUrl, Type expected)
    {
        // Arrange
        var settings = BuildAppSettings(baseUrl: baseUrl);
        var connector = new LGConnector(settings);

        // Act
        Func<Task> func = async () => await connector.GetChatClientAsync();

        // Assert
        func.ShouldThrow(expected);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData(null, typeof(ArgumentNullException))]
    [InlineData("", typeof(ArgumentException))]
    public void Given_Missing_Model_When_GetChatClient_Invoked_Then_It_Should_Throw(string? model, Type expected)
    {
        // Arrange
        var settings = BuildAppSettings(model: model);
        var connector = new LGConnector(settings);

        // Act
        Func<Task> func = async () => await connector.GetChatClientAsync();

        // Assert
        func.ShouldThrow(expected);
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

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData(null, "hf.co/LGAI-EXAONE/EXAONE-4.0-1.2B-GGUF", typeof(NullReferenceException))]
    [InlineData("", "hf.co/LGAI-EXAONE/EXAONE-1.2B", typeof(InvalidOperationException))]
    [InlineData("https://test.lg-exaone/api", null, typeof(NullReferenceException))]
    [InlineData("https://test.lg-exaone/api", "", typeof(InvalidOperationException))]
    [InlineData("https://test.lg-exaone/api", "invalid-model-format", typeof(InvalidOperationException))]
    public void Given_Invalid_Settings_When_CreateChatClientAsync_Invoked_Then_It_Should_Throw(string? baseUrl, string? model, Type expectedType)
    {
        // Arrange
        var settings = new AppSettings
        {
            ConnectorType = ConnectorType.LG,
            LG = new LGSettings
            {
                BaseUrl = baseUrl,
                Model = model
            }
        };

        // Act
        Func<Task> func = async () => await LanguageModelConnector.CreateChatClientAsync(settings);

        // Assert
        func.ShouldThrow(expectedType);
    }
}
