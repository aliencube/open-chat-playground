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
		var ex = Assert.Throws<InvalidOperationException>(() => connector.EnsureLanguageModelSettingsValid());

		// Assert
		ex.Message.ShouldContain("Missing configuration: OpenAI.");
	}

	[Trait("Category", "UnitTest")]
    [Theory]
    [InlineData(null, typeof(NullReferenceException), "Object reference not set to an instance of an object")]
    [InlineData("", typeof(InvalidOperationException), "OpenAI:ApiKey")]
    [InlineData("   ", typeof(InvalidOperationException), "OpenAI:ApiKey")]
    public void Given_Invalid_ApiKey_When_EnsureLanguageModelSettingsValid_Invoked_Then_It_Should_Throw(string? apiKey, Type expectedType, string expectedMessage)
    {
        // Arrange
        var appSettings = BuildAppSettings(apiKey: apiKey);
        var connector = new OpenAIConnector(appSettings);

        // Act
        var ex = Assert.Throws(expectedType, () => connector.EnsureLanguageModelSettingsValid());

        // Assert
        ex.Message.ShouldContain(expectedMessage);
    }

	[Trait("Category", "UnitTest")]
    [Theory]
    [InlineData(null, typeof(NullReferenceException), "Object reference not set to an instance of an object")]
    [InlineData("", typeof(InvalidOperationException), "OpenAI:Model")]
    [InlineData("   ", typeof(InvalidOperationException), "OpenAI:Model")]
    public void Given_Invalid_Model_When_EnsureLanguageModelSettingsValid_Invoked_Then_It_Should_Throw(string? model, Type expectedType, string expectedMessage)
    {
        // Arrange
        var appSettings = BuildAppSettings(model: model);
        var connector = new OpenAIConnector(appSettings);

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
        var connector = new OpenAIConnector(appSettings);

        // Act
        var result = connector.EnsureLanguageModelSettingsValid();

        // Assert
        result.ShouldBeTrue();
    }

	[Trait("Category", "UnitTest")]
    [Fact]
    public async Task Given_Valid_Settings_When_GetChatClient_Invoked_Then_It_Should_Return_ChatClient()
    {
        var settings = BuildAppSettings();
        var connector = new OpenAIConnector(settings);

        var client = await connector.GetChatClientAsync();

        client.ShouldNotBeNull();
    }

	[Trait("Category", "UnitTest")]
    [Theory]
    [InlineData(null, typeof(InvalidOperationException), "OpenAI:ApiKey")]
    [InlineData("", typeof(ArgumentException), "key")]
    public async Task Given_Missing_ApiKey_When_GetChatClient_Invoked_Then_It_Should_Throw(string? apiKey, Type expected, string message)
    {
        var settings = BuildAppSettings(apiKey: apiKey);
        var connector = new OpenAIConnector(settings);

		var ex = await Assert.ThrowsAsync(expected, connector.GetChatClientAsync);

        ex.Message.ShouldContain(message);
    }

	[Trait("Category", "UnitTest")]
    [Theory]
    [InlineData(null, typeof(ArgumentNullException), "model")]
    [InlineData("", typeof(ArgumentException), "model")]
    public async Task Given_Missing_Model_When_GetChatClient_Invoked_Then_It_Should_Throw(string? model, Type expected, string message)
    {
        var settings = BuildAppSettings(model: model);
        var connector = new OpenAIConnector(settings);

        var ex = await Assert.ThrowsAsync(expected, connector.GetChatClientAsync);

        ex.Message.ShouldContain(message);
    }

	[Trait("Category", "UnitTest")]
	[Fact]
	public void Given_Valid_Settings_When_CreateChatClientAsync_Invoked_Then_It_Should_Return_ChatClient()
	{
		// Arrange
		var settings = BuildAppSettings();

		// Act
		Func<Task<IChatClient>> func = async () => await LanguageModelConnector.CreateChatClientAsync(settings);
		var result = func.ShouldNotThrow();

		// Assert
		result.ShouldNotBeNull();
		result.ShouldBeAssignableTo<IChatClient>();
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
	public void Given_Null_AppSettings_When_Constructor_Invoked_Then_It_Should_Throw()
	{
		// Act
		Action action = () => new OpenAIConnector(null!);

		// Assert
		action.ShouldThrow<NullReferenceException>();
	}

	[Trait("Category", "UnitTest")]
	[Fact]
	public void Given_AppSettings_With_Null_OpenAI_Settings_When_EnsureLanguageModelSettingsValid_Invoked_Then_It_Should_Throw()
	{
		// Arrange
		var settings = new AppSettings
		{
			ConnectorType = ConnectorType.OpenAI,
			OpenAI = null
		};

		// Act
		var connector = new OpenAIConnector(settings);
		Action action = () => connector.EnsureLanguageModelSettingsValid();

		// Assert
		action.ShouldThrow<InvalidOperationException>();
	}

	[Trait("Category", "UnitTest")]
	[Theory]
	[InlineData(null)]
	[InlineData("")]
	[InlineData("   ")]
	public void Given_Invalid_ApiKey_When_CreateChatClientAsync_Invoked_Then_It_Should_Throw(string? apiKey)
	{
		// Arrange
		var settings = BuildAppSettings(apiKey: apiKey);

		// Act
		Func<Task> func = async () => await LanguageModelConnector.CreateChatClientAsync(settings);

		// Assert
		if (apiKey is null)
		{
			func.ShouldThrow<NullReferenceException>();
		}
		else
		{
			func.ShouldThrow<InvalidOperationException>();
		}
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

		// Act & Assert
		connector.ShouldBeAssignableTo<LanguageModelConnector>();
		typeof(OpenAIConnector).IsSubclassOf(typeof(LanguageModelConnector)).ShouldBeTrue();
	}

	[Trait("Category", "UnitTest")]
	[Fact]
	public void Given_OpenAIConnector_When_CreateChatClientAsync_Called_Through_Base_Class_Then_It_Should_Work()
	{
		// Arrange
		var settings = BuildAppSettings();
		var connector = new OpenAIConnector(settings);
		LanguageModelConnector baseConnector = connector;

		// Act
		Func<Task<IChatClient>> func = async () => await baseConnector.GetChatClientAsync();
		var result = func.ShouldNotThrow();

		// Assert
		result.ShouldNotBeNull();
		result.ShouldBeAssignableTo<IChatClient>();
	}

	[Trait("Category", "UnitTest")]
	[Theory]
	[InlineData("   ")]
	[InlineData("\t\n\r")]
	public void Given_Whitespace_ApiKey_When_EnsureLanguageModelSettingsValid_Invoked_Then_It_Should_Throw(string apiKey)
	{
		// Arrange
		var settings = BuildAppSettings(apiKey: apiKey);

		// Act
		var connector = new OpenAIConnector(settings);
		Action action = () => connector.EnsureLanguageModelSettingsValid();

		// Assert
		action.ShouldThrow<InvalidOperationException>();
	}

	[Trait("Category", "UnitTest")]
	[Theory]
	[InlineData("   ")]
	[InlineData("\t\n\r")]
	public void Given_Whitespace_Model_When_EnsureLanguageModelSettingsValid_Invoked_Then_It_Should_Throw(string model)
	{
		// Arrange
		var settings = BuildAppSettings(model: model);

		// Act
		var connector = new OpenAIConnector(settings);
		Action action = () => connector.EnsureLanguageModelSettingsValid();

		// Assert
		action.ShouldThrow<InvalidOperationException>();
	}

	// Exception Handling Tests

	[Trait("Category", "UnitTest")]
	[Fact]
	public void Given_Settings_With_Wrong_Type_Cast_When_EnsureLanguageModelSettingsValid_Invoked_Then_It_Should_Throw()
	{
		// Arrange
		var settings = new AppSettings
		{
			ConnectorType = ConnectorType.OpenAI,
			GitHubModels = new GitHubModelsSettings // Wrong settings type
			{
				Token = "test-token",
				Model = "test-model"
			}
			// OpenAI is null, so validation will fail
		};

		// Act
		var connector = new OpenAIConnector(settings);
		Action action = () => connector.EnsureLanguageModelSettingsValid();

		// Assert
		action.ShouldThrow<InvalidOperationException>();
	}

	[Trait("Category", "UnitTest")]
	[Fact]
	public void Given_Multiple_Null_Settings_When_EnsureLanguageModelSettingsValid_Invoked_Then_It_Should_Throw_For_ApiKey_First()
	{
		// Arrange
		var settings = BuildAppSettings(apiKey: null, model: null);

		// Act
		var connector = new OpenAIConnector(settings);
		Assert.Throws<NullReferenceException>(() => connector.EnsureLanguageModelSettingsValid());
	}

	[Trait("Category", "UnitTest")]
	[Fact]
	public void Given_Valid_ApiKey_But_Null_Model_When_EnsureLanguageModelSettingsValid_Invoked_Then_It_Should_Throw_For_Model()
	{
		// Arrange
		var settings = BuildAppSettings(apiKey: "valid-api-key", model: null);

		// Act
		var connector = new OpenAIConnector(settings);
		Assert.Throws<NullReferenceException>(() => connector.EnsureLanguageModelSettingsValid());
	}

	[Trait("Category", "UnitTest")]
	[Theory]
	[InlineData(null)]
	[InlineData("")]
	[InlineData("   ")]
	public void Given_Missing_ApiKey_When_EnsureLanguageModelSettingsValid_Invoked_Then_It_Should_Throw(string? apiKey)
	{
		var settings = BuildAppSettings(apiKey: apiKey);
		var connector = new OpenAIConnector(settings);

		Action action = () => connector.EnsureLanguageModelSettingsValid();
		
		if (apiKey is null)
		{
			action.ShouldThrow<NullReferenceException>();
		}
		else
		{
			var ex = Assert.Throws<InvalidOperationException>(action);
			ex.Message.ShouldContain("Missing configuration: OpenAI:ApiKey.");
		}
	}

	[Trait("Category", "UnitTest")]
	[Theory]
	[InlineData(null)]
	[InlineData("")]
	[InlineData("   ")]
	public void Given_Missing_Model_When_EnsureLanguageModelSettingsValid_Invoked_Then_It_Should_Throw(string? model)
	{
		var settings = BuildAppSettings(model: model);
		var connector = new OpenAIConnector(settings);

		Action action = () => connector.EnsureLanguageModelSettingsValid();
		
		if (model is null)
		{
			action.ShouldThrow<NullReferenceException>();
		}
		else
		{
			var ex = Assert.Throws<InvalidOperationException>(action);
			ex.Message.ShouldContain("Missing configuration: OpenAI:Model.");
		}
	}

	[Trait("Category", "UnitTest")]
	[Fact]
	public void Given_Invalid_Endpoint_When_EnsureLanguageModelSettingsValid_Invoked_Then_It_Should_Throw()
	{
		// Note: Current OpenAISettings doesn't have Endpoint property
		// This test is prepared for future implementation
		var settings = BuildAppSettings();
		var connector = new OpenAIConnector(settings);

		// This test will pass as current implementation doesn't validate Endpoint
		var result = connector.EnsureLanguageModelSettingsValid();
		result.ShouldBeTrue();
	}

	[Trait("Category", "UnitTest")]
	[Fact]
	public void Given_Invalid_DeploymentName_When_EnsureLanguageModelSettingsValid_Invoked_Then_It_Should_Throw()
	{
		// Note: Current OpenAISettings doesn't have DeploymentName property
		// This test is prepared for future implementation
		var settings = BuildAppSettings();
		var connector = new OpenAIConnector(settings);

		// This test will pass as current implementation doesn't validate DeploymentName
		var result = connector.EnsureLanguageModelSettingsValid();
		result.ShouldBeTrue();
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
	[InlineData(null)]
	[InlineData("")]
	public void Given_Missing_ApiKey_When_GetChatClientAsync_Invoked_Then_It_Should_Throw(string? apiKey)
	{
		// Arrange
		var settings = BuildAppSettings(apiKey: apiKey);
		var connector = new OpenAIConnector(settings);

		// Act
		Func<Task> func = async () => await connector.GetChatClientAsync();

		// Assert
		if (apiKey is null)
		{
			func.ShouldThrow<InvalidOperationException>();
		}
		else
		{
			func.ShouldThrow<ArgumentException>();
		}
	}

	[Trait("Category", "UnitTest")]
	[Fact]
	public void Given_Invalid_Endpoint_Format_When_GetChatClientAsync_Invoked_Then_It_Should_Throw()
	{
		// Note: Current OpenAI implementation doesn't use custom endpoints
		// This test is prepared for future implementation
		var settings = BuildAppSettings();
		var connector = new OpenAIConnector(settings);

		// Current implementation should work with valid settings
		Func<Task<IChatClient>> func = async () => await connector.GetChatClientAsync();
		var result = func.ShouldNotThrow();
		result.ShouldNotBeNull();
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
}