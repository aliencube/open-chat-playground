using Microsoft.Extensions.AI;

using OpenChat.PlaygroundApp.Abstractions;
using OpenChat.PlaygroundApp.Configurations;
using OpenChat.PlaygroundApp.Connectors;

namespace OpenChat.PlaygroundApp.Tests.Connectors;

public class OpenAIConnectorTests
{
    private const string ApiKey = "test-api-key";
    private const string Model = "test-model";

    private static AppSettings BuildAppSettings(string? apiKey = ApiKey, string? model = Model)
    {
        return new AppSettings
        {
            ConnectorType = ConnectorType.OpenAI,
            OpenAI = new OpenAISettings
            {
                ApiKey = apiKey,
                Model = model
            }
        };
    }

    [Trait("Category", "UnitTest")]
    [Fact]
    public void Given_Settings_Is_Null_When_EnsureLanguageModelSettingsValid_Invoked_Then_It_Should_Throw()
    {
        // Arrange
        var appSettings = new AppSettings { ConnectorType = ConnectorType.OpenAI, OpenAI = null };
        var connector = new OpenAIConnector(appSettings);

        // Act
        Action action = () => connector.EnsureLanguageModelSettingsValid();

        // Assert
        action.ShouldThrow<InvalidOperationException>()
              .Message.ShouldContain("Missing configuration: OpenAI.");
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData(null, typeof(NullReferenceException), "Object reference not set to an instance of an object")]
    [InlineData("", typeof(InvalidOperationException), "OpenAI:ApiKey")]
    [InlineData("   ", typeof(InvalidOperationException), "OpenAI:ApiKey")]
    [InlineData("\t\n\r", typeof(InvalidOperationException), "OpenAI:ApiKey")]
    public void Given_Invalid_ApiKey_When_EnsureLanguageModelSettingsValid_Invoked_Then_It_Should_Throw(string? apiKey, Type expectedType, string expectedMessage)
    {
        // Arrange
        var appSettings = BuildAppSettings(apiKey: apiKey);
        var connector = new OpenAIConnector(appSettings);

        // Act
        Action action = () => connector.EnsureLanguageModelSettingsValid();

        // Assert
        if (expectedType == typeof(NullReferenceException))
        {
            action.ShouldThrow<NullReferenceException>()
                  .Message.ShouldContain(expectedMessage);
        }
        else
        {
            action.ShouldThrow<InvalidOperationException>()
                  .Message.ShouldContain(expectedMessage);
        }
    }

    [Trait("Category", "UnitTest")]
    [Fact]
    public void Given_Valid_Settings_When_EnsureLanguageModelSettingsValid_Invoked_Then_It_Should_Return_True()
    {
        // Arrange
        var appSettings = BuildAppSettings();
        var connector = new OpenAIConnector(appSettings);

        // Act
        var result = connector.EnsureLanguageModelSettingsValid();

        // Assert
        result.ShouldBeTrue();
    }
    
    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData(null, typeof(NullReferenceException), "Object reference not set to an instance of an object")]
    [InlineData("", typeof(InvalidOperationException), "OpenAI:Model")]
    [InlineData("   ", typeof(InvalidOperationException), "OpenAI:Model")]
    [InlineData("\t\n\r", typeof(InvalidOperationException), "OpenAI:Model")]
    public void Given_Invalid_Model_When_EnsureLanguageModelSettingsValid_Invoked_Then_It_Should_Throw(string? model, Type expectedType, string expectedMessage)
    {
        // Arrange
        var appSettings = BuildAppSettings(apiKey: "valid-key", model: model);
        var connector = new OpenAIConnector(appSettings);

        // Act
        Action action = () => connector.EnsureLanguageModelSettingsValid();

        // Assert
        if (expectedType == typeof(NullReferenceException))
        {
            action.ShouldThrow<NullReferenceException>()
                  .Message.ShouldContain(expectedMessage);
        }
        else
        {
            action.ShouldThrow<InvalidOperationException>()
                  .Message.ShouldContain(expectedMessage);
        }
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData(null, null)]
    [InlineData("", "")]
    [InlineData("   ", "   ")]
    [InlineData(null, "")]
    [InlineData("", null)]
    public void Given_Both_ApiKey_And_Model_Invalid_When_EnsureLanguageModelSettingsValid_Invoked_Then_It_Should_Throw_ApiKey_First(string? apiKey, string? model)
    {
        // Arrange
        var appSettings = BuildAppSettings(apiKey: apiKey, model: model);
        var connector = new OpenAIConnector(appSettings);

        // Act
        Action action = () => connector.EnsureLanguageModelSettingsValid();

        // Assert
        if (apiKey is null)
        {
            action.ShouldThrow<NullReferenceException>();
        }
        else
        {
            // ApiKey가 먼저 검증되므로 ApiKey 에러가 먼저 발생해야 함
            action.ShouldThrow<InvalidOperationException>()
                  .Message.ShouldContain("OpenAI:ApiKey");
        }
    }
    
    [Trait("Category", "UnitTest")]
    [Fact]
    public async Task Given_Valid_Settings_When_GetChatClientAsync_Invoked_Then_It_Should_Return_ChatClient()
    {
        // Arrange
        var settings = BuildAppSettings();
        var connector = new OpenAIConnector(settings);

        // Act
        var client = await connector.GetChatClientAsync();

        // Assert
        client.ShouldNotBeNull();
    }
    
    [Trait("Category", "UnitTest")]
    [Fact]
    public void Given_Settings_Is_Null_When_GetChatClientAsync_Invoked_Then_It_Should_Throw()
    {
        // Arrange
        var appSettings = new AppSettings { ConnectorType = ConnectorType.OpenAI, OpenAI = null };
        var connector = new OpenAIConnector(appSettings);

        // Act
        Func<Task> func = async () => await connector.GetChatClientAsync();

        // Assert
        func.ShouldThrow<InvalidOperationException>();
    }
    
    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData(null, typeof(InvalidOperationException), "OpenAI:ApiKey")]
    [InlineData("", typeof(ArgumentException), "key")]
    public void Given_Missing_ApiKey_When_GetChatClientAsync_Invoked_Then_It_Should_Throw(string? apiKey, Type expected, string message)
    {
        // Arrange
        var settings = BuildAppSettings(apiKey: apiKey);
        var connector = new OpenAIConnector(settings);

        // Act
        Func<Task> func = async () => await connector.GetChatClientAsync();

        // Assert
        if (expected == typeof(InvalidOperationException))
        {
            func.ShouldThrow<InvalidOperationException>()
                .Message.ShouldContain(message);
        }
        else if (expected == typeof(ArgumentException))
        {
            func.ShouldThrow<ArgumentException>()
                .Message.ShouldContain(message);
        }
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData(null, typeof(ArgumentNullException), "model")]
    [InlineData("", typeof(ArgumentException), "model")]
    public void Given_Missing_Model_When_GetChatClientAsync_Invoked_Then_It_Should_Throw(string? model, Type expected, string message)
    {
        // Arrange
        var settings = BuildAppSettings(model: model);
        var connector = new OpenAIConnector(settings);

        // Act
        Func<Task> func = async () => await connector.GetChatClientAsync();

        // Assert
        if (expected == typeof(ArgumentNullException))
        {
            func.ShouldThrow<ArgumentNullException>()
                .Message.ShouldContain(message);
        }
        else if (expected == typeof(ArgumentException))
        {
            func.ShouldThrow<ArgumentException>()
                .Message.ShouldContain(message);
        }
    }

    [Trait("Category", "UnitTest")]
    [Fact]
    public void Given_Valid_Format_Settings_When_EnsureLanguageModelSettingsValid_Invoked_Then_It_Should_Pass()
    {
        // Note: This test verifies format validation passes with properly formatted values
        
        // Arrange
        var settings = BuildAppSettings(apiKey: "sk-test-key-with-proper-format", model: "gpt-3.5-turbo");
        var connector = new OpenAIConnector(settings);

        // Act
        var isValid = connector.EnsureLanguageModelSettingsValid();
        
        // Assert
        isValid.ShouldBeTrue();
    }

    [Trait("Category", "UnitTest")]
    [Fact]
    public void Given_Valid_Format_Settings_When_GetChatClientAsync_Invoked_Then_It_Should_Pass()
    {
        // Note: This test verifies client creation passes with properly formatted values
        
        // Arrange
        var settings = BuildAppSettings(apiKey: "sk-test-key-with-proper-format", model: "gpt-3.5-turbo");
        var connector = new OpenAIConnector(settings);

        // Act
        Func<Task> func = async () => await connector.GetChatClientAsync();
        
        // Assert
        func.ShouldNotThrow();
    }

    
    [Trait("Category", "UnitTest")]
    [Fact]
    public async Task Given_OpenAIConnector_When_GetChatClientAsync_Called_Through_Base_Class_Then_It_Should_Work()
    {
        // Arrange
        var settings = BuildAppSettings();
        var connector = new OpenAIConnector(settings);
        LanguageModelConnector baseConnector = connector;

        // Act
        var result = await baseConnector.GetChatClientAsync();
        
        // Assert
        result.ShouldNotBeNull();
        result.ShouldBeAssignableTo<IChatClient>();
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
    [Fact]
    public void Given_Invalid_Settings_When_CreateChatClientAsync_Invoked_Then_It_Should_Throw()
    {
        // Arrange
        var settings = new AppSettings
        {
            ConnectorType = ConnectorType.OpenAI,
            OpenAI = new OpenAISettings
            {
                ApiKey = null,
                Model = "test-model"
            }
        };

        // Act
        Func<Task> func = async () => await LanguageModelConnector.CreateChatClientAsync(settings);

        // Assert
        func.ShouldThrow<NullReferenceException>();
    }
    
    [Trait("Category", "UnitTest")]
    [Fact]
    public void Given_Null_AppSettings_When_Constructor_Invoked_Then_It_Should_Throw()
    {
        // Arrange
        AppSettings? nullSettings = null;

        // Act
        Action action = () => new OpenAIConnector(nullSettings!);

        // Assert
        action.ShouldThrow<NullReferenceException>();
    }

    [Trait("Category", "UnitTest")]
    [Fact]
    public void Given_Valid_AppSettings_When_Constructor_Invoked_Then_It_Should_Create_Instance()
    {
        // Arrange
        var settings = BuildAppSettings();

        // Act
        var connector = new OpenAIConnector(settings);

        // Assert
        connector.ShouldNotBeNull();
    }

    [Trait("Category", "UnitTest")]
    [Fact]
    public void Given_OpenAIConnector_When_Cast_To_LanguageModelConnector_Then_It_Should_Succeed()
    {
        // Arrange
        var settings = BuildAppSettings();
        var connector = new OpenAIConnector(settings);

        // Act
        LanguageModelConnector baseConnector = connector;

        // Assert
        baseConnector.ShouldNotBeNull();
        baseConnector.ShouldBeOfType<OpenAIConnector>();
    }

    [Trait("Category", "UnitTest")]
    [Fact]
    public void Given_OpenAIConnector_When_Check_Type_Then_It_Should_Be_LanguageModelConnector_Subclass()
    {
        // Arrange
        var settings = BuildAppSettings();
        var connector = new OpenAIConnector(settings);

        // Act
        var isAssignable = connector is LanguageModelConnector;
        var isSubclass = typeof(OpenAIConnector).IsSubclassOf(typeof(LanguageModelConnector));

        // Assert
        isAssignable.ShouldBeTrue();
        isSubclass.ShouldBeTrue();
    }
}