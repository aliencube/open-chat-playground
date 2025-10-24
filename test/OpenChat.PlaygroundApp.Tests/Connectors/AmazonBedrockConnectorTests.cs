using Microsoft.Extensions.AI;

using OpenChat.PlaygroundApp.Abstractions;
using OpenChat.PlaygroundApp.Configurations;
using OpenChat.PlaygroundApp.Connectors;

namespace OpenChat.PlaygroundApp.Tests.Connectors;

public class AmazonBedrockConnectorTests
{
    private const string AccessKeyId = "test-access-key-id";
    private const string SecretAccessKey = "test-secret-access-key";
    private const string Region = "test-region";
    private const string ModelId = "test-model-id";

    private static AppSettings BuildAppSettings(string? accessKeyId = AccessKeyId, string? secretAccessKey = SecretAccessKey, string? region = Region, string? modelId = ModelId)
    {
        return new AppSettings
        {
            ConnectorType = ConnectorType.AmazonBedrock,
            AmazonBedrock = new AmazonBedrockSettings
            {
                AccessKeyId = accessKeyId,
                SecretAccessKey = secretAccessKey,
                Region = region,
                ModelId = modelId
            }
        };
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData(typeof(LanguageModelConnector), typeof(AmazonBedrockConnector), true)]
    [InlineData(typeof(AmazonBedrockConnector), typeof(LanguageModelConnector), false)]
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
        Action action = () => new AmazonBedrockConnector(null!);

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
        var result = new AmazonBedrockConnector(settings);

        // Assert
        result.ShouldNotBeNull();
    }

    [Trait("Category", "UnitTest")]
    [Fact]
    public void Given_Null_AmazonBedrockSettings_When_EnsureLanguageModelSettingsValid_Invoked_Then_It_Should_Throw()
    {
        // Arrange
        var settings = new AppSettings
        {
            ConnectorType = ConnectorType.AmazonBedrock,
            AmazonBedrock = null
        };
        var connector = new AmazonBedrockConnector(settings);

        // Act
        Action action = () => connector.EnsureLanguageModelSettingsValid();

        // Assert
        action.ShouldThrow<InvalidOperationException>()
              .Message.ShouldContain("AmazonBedrock");
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData(null, typeof(InvalidOperationException), "AmazonBedrock:AccessKeyId")]
    [InlineData("", typeof(InvalidOperationException), "AmazonBedrock:AccessKeyId")]
    [InlineData("   ", typeof(InvalidOperationException), "AmazonBedrock:AccessKeyId")]
    [InlineData("\t\r\n", typeof(InvalidOperationException), "AmazonBedrock:AccessKeyId")]
    public void Given_Invalid_AccessKeyId_When_EnsureLanguageModelSettingsValid_Invoked_Then_It_Should_Throw(string? accessKeyId, Type expectedType, string expectedMessage)
    {
        // Arrange
        var settings = BuildAppSettings(accessKeyId: accessKeyId);
        var connector = new AmazonBedrockConnector(settings);

        // Act
        Action action = () => connector.EnsureLanguageModelSettingsValid();

        // Assert
        action.ShouldThrow(expectedType)
              .Message.ShouldContain(expectedMessage);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData(null, typeof(InvalidOperationException), "AmazonBedrock:SecretAccessKey")]
    [InlineData("", typeof(InvalidOperationException), "AmazonBedrock:SecretAccessKey")]
    [InlineData("   ", typeof(InvalidOperationException), "AmazonBedrock:SecretAccessKey")]
    [InlineData("\t\r\n", typeof(InvalidOperationException), "AmazonBedrock:SecretAccessKey")]
    public void Given_Invalid_SecretAccessKey_When_EnsureLanguageModelSettingsValid_Invoked_Then_It_Should_Throw(string? secretAccessKey, Type expectedType, string expectedMessage)
    {
        // Arrange
        var settings = BuildAppSettings(secretAccessKey: secretAccessKey);
        var connector = new AmazonBedrockConnector(settings);

        // Act
        Action action = () => connector.EnsureLanguageModelSettingsValid();

        // Assert
        action.ShouldThrow(expectedType)
              .Message.ShouldContain(expectedMessage);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData(null, typeof(InvalidOperationException), "AmazonBedrock:Region")]
    [InlineData("", typeof(InvalidOperationException), "AmazonBedrock:Region")]
    [InlineData("   ", typeof(InvalidOperationException), "AmazonBedrock:Region")]
    [InlineData("\t\r\n", typeof(InvalidOperationException), "AmazonBedrock:Region")]
    public void Given_Invalid_Region_When_EnsureLanguageModelSettingsValid_Invoked_Then_It_Should_Throw(string? region, Type expectedType, string expectedMessage)
    {
        // Arrange
        var settings = BuildAppSettings(region: region);
        var connector = new AmazonBedrockConnector(settings);

        // Act
        Action action = () => connector.EnsureLanguageModelSettingsValid();

        // Assert
        action.ShouldThrow(expectedType)
              .Message.ShouldContain(expectedMessage);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData(null, typeof(InvalidOperationException), "AmazonBedrock:ModelId")]
    [InlineData("", typeof(InvalidOperationException), "AmazonBedrock:ModelId")]
    [InlineData("   ", typeof(InvalidOperationException), "AmazonBedrock:ModelId")]
    [InlineData("\t\r\n", typeof(InvalidOperationException), "AmazonBedrock:ModelId")]
    public void Given_Invalid_ModelId_When_EnsureLanguageModelSettingsValid_Invoked_Then_It_Should_Throw(string? modelId, Type expectedType, string expectedMessage)
    {
        // Arrange
        var settings = BuildAppSettings(modelId: modelId);
        var connector = new AmazonBedrockConnector(settings);

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
        var connector = new AmazonBedrockConnector(settings);

        // Act
        var result = connector.EnsureLanguageModelSettingsValid();

        // Assert
        result.ShouldBeTrue();
    }

    [Trait("Category", "UnitTest")]
    [Fact]
    public void Given_Null_AmazonBedrockSettings_When_GetChatClientAsync_Invoked_Then_It_Should_Throw()
    {
        // Arrange
        var settings = new AppSettings
        {
            ConnectorType = ConnectorType.AmazonBedrock,
            AmazonBedrock = null
        };
        var connector = new AmazonBedrockConnector(settings);

        // Act
        Func<Task> func = async () => await connector.GetChatClientAsync();

        // Assert
        func.ShouldThrow<InvalidOperationException>()
            .Message.ShouldContain("AmazonBedrock");
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData(null, typeof(InvalidOperationException), "AmazonBedrock:AccessKeyId")]
    [InlineData("", typeof(InvalidOperationException), "AmazonBedrock:AccessKeyId")]
    [InlineData("   ", typeof(InvalidOperationException), "AmazonBedrock:AccessKeyId")]
    [InlineData("\t\r\n", typeof(InvalidOperationException), "AmazonBedrock:AccessKeyId")]
    public void Given_Invalid_AccessKeyId_When_GetChatClientAsync_Invoked_Then_It_Should_Throw(string? accessKeyId, Type expectedType, string message)
    {
        // Arrange
        var settings = BuildAppSettings(accessKeyId: accessKeyId);
        var connector = new AmazonBedrockConnector(settings);

        // Act
        Func<Task> func = async () => await connector.GetChatClientAsync();

        // Assert
        func.ShouldThrow(expectedType)
            .Message.ShouldContain(message);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData(null, typeof(InvalidOperationException), "AmazonBedrock:SecretAccessKey")]
    [InlineData("", typeof(InvalidOperationException), "AmazonBedrock:SecretAccessKey")]
    [InlineData("   ", typeof(InvalidOperationException), "AmazonBedrock:SecretAccessKey")]
    [InlineData("\t\r\n", typeof(InvalidOperationException), "AmazonBedrock:SecretAccessKey")]
    public void Given_Invalid_SecretAccessKey_When_GetChatClientAsync_Invoked_Then_It_Should_Throw(string? secretAccessKey, Type expectedType, string message)
    {
        // Arrange
        var settings = BuildAppSettings(secretAccessKey: secretAccessKey);
        var connector = new AmazonBedrockConnector(settings);

        // Act
        Func<Task> func = async () => await connector.GetChatClientAsync();

        // Assert
        func.ShouldThrow(expectedType)
            .Message.ShouldContain(message);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData(null, typeof(InvalidOperationException), "AmazonBedrock:Region")]
    [InlineData("", typeof(InvalidOperationException), "AmazonBedrock:Region")]
    [InlineData("   ", typeof(InvalidOperationException), "AmazonBedrock:Region")]
    [InlineData("\t\r\n", typeof(InvalidOperationException), "AmazonBedrock:Region")]
    public void Given_Invalid_Region_When_GetChatClientAsync_Invoked_Then_It_Should_Throw(string? region, Type expectedType, string message)
    {
        // Arrange
        var settings = BuildAppSettings(region: region);
        var connector = new AmazonBedrockConnector(settings);

        // Act
        Func<Task> func = async () => await connector.GetChatClientAsync();

        // Assert
        func.ShouldThrow(expectedType)
            .Message.ShouldContain(message);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData(null, typeof(InvalidOperationException), "AmazonBedrock:ModelId")]
    [InlineData("", typeof(InvalidOperationException), "AmazonBedrock:ModelId")]
    [InlineData("   ", typeof(InvalidOperationException), "AmazonBedrock:ModelId")]
    [InlineData("\t\r\n", typeof(InvalidOperationException), "AmazonBedrock:ModelId")]
    public void Given_Invalid_ModelId_When_GetChatClientAsync_Invoked_Then_It_Should_Throw(string? modelId, Type expectedType, string message)
    {
        // Arrange
        var settings = BuildAppSettings(modelId: modelId);
        var connector = new AmazonBedrockConnector(settings);

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
        var connector = new AmazonBedrockConnector(settings);

        // Act
        var result = await connector.GetChatClientAsync();

        // Assert
        result.ShouldNotBeNull();
        result.ShouldBeAssignableTo<IChatClient>();
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData(null, null, null, null, "Missing configuration: AmazonBedrock")]
    [InlineData(null, SecretAccessKey, Region, ModelId, "Missing configuration: AmazonBedrock:AccessKeyId")]
    [InlineData("", SecretAccessKey, Region, ModelId, "Missing configuration: AmazonBedrock:AccessKeyId")]
    [InlineData("   ", SecretAccessKey, Region, ModelId, "Missing configuration: AmazonBedrock:AccessKeyId")]
    [InlineData("\t\r\n", SecretAccessKey, Region, ModelId, "Missing configuration: AmazonBedrock:AccessKeyId")]
    [InlineData(AccessKeyId, null, Region, ModelId, "Missing configuration: AmazonBedrock:SecretAccessKey")]
    [InlineData(AccessKeyId, "", Region, ModelId, "Missing configuration: AmazonBedrock:SecretAccessKey")]
    [InlineData(AccessKeyId, "   ", Region, ModelId, "Missing configuration: AmazonBedrock:SecretAccessKey")]
    [InlineData(AccessKeyId, "\t\r\n", Region, ModelId, "Missing configuration: AmazonBedrock:SecretAccessKey")]
    [InlineData(AccessKeyId, SecretAccessKey, null, ModelId, "Missing configuration: AmazonBedrock:Region")]
    [InlineData(AccessKeyId, SecretAccessKey, "", ModelId, "Missing configuration: AmazonBedrock:Region")]
    [InlineData(AccessKeyId, SecretAccessKey, "   ", ModelId, "Missing configuration: AmazonBedrock:Region")]
    [InlineData(AccessKeyId, SecretAccessKey, "\t\r\n", ModelId, "Missing configuration: AmazonBedrock:Region")]
    [InlineData(AccessKeyId, SecretAccessKey, Region, null, "Missing configuration: AmazonBedrock:ModelId")]
    [InlineData(AccessKeyId, SecretAccessKey, Region, "", "Missing configuration: AmazonBedrock:ModelId")]
    [InlineData(AccessKeyId, SecretAccessKey, Region, "   ", "Missing configuration: AmazonBedrock:ModelId")]
    [InlineData(AccessKeyId, SecretAccessKey, Region, "\t\r\n", "Missing configuration: AmazonBedrock:ModelId")]
    public void Given_Invalid_Settings_When_CreateChatClientAsync_Invoked_Then_It_Should_Throw(string? accessKeyId, string? secretAccessKey, string? region, string? modelId, string expectedMessage)
    {
        // Arrange
        var settings = new AppSettings
        {
            ConnectorType = ConnectorType.AmazonBedrock,
            AmazonBedrock = new AmazonBedrockSettings
            {
                AccessKeyId = accessKeyId,
                SecretAccessKey = secretAccessKey,
                Region = region,
                ModelId = modelId
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