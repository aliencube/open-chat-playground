using Microsoft.Extensions.AI;

using OpenChat.PlaygroundApp.Abstractions;
using OpenChat.PlaygroundApp.Configurations;
using OpenChat.PlaygroundApp.Connectors;

namespace OpenChat.PlaygroundApp.Tests.Connectors;

public class OpenAIConnectorTests
{
	private static AppSettings BuildAppSettings(string? apiKey = "test-api-key", string? model = "gpt-4o")
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

	#region Constructor Tests

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
		// Act & Assert
		var ex = Assert.Throws<NullReferenceException>(() => new OpenAIConnector(null!));
		ex.ShouldNotBeNull();
	}

	[Trait("Category", "UnitTest")]
	[Fact]
	public void Given_AppSettings_With_Null_OpenAI_Settings_When_Constructor_Invoked_Then_It_Should_Create_Instance()
	{
		// Arrange
		var settings = new AppSettings
		{
			ConnectorType = ConnectorType.OpenAI,
			OpenAI = null
		};

		// Act
		var connector = new OpenAIConnector(settings);

		// Assert
		connector.ShouldNotBeNull();
	}

	#endregion

	#region Type Casting and Inheritance Tests

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
	public async Task Given_OpenAIConnector_When_CreateChatClientAsync_Called_Through_Base_Class_Then_It_Should_Work()
	{
		// Arrange
		var settings = BuildAppSettings();
		var connector = new OpenAIConnector(settings);
		LanguageModelConnector baseConnector = connector;

		// Act
		var client = await baseConnector.GetChatClientAsync();

		// Assert
		client.ShouldNotBeNull();
		client.ShouldBeAssignableTo<IChatClient>();
	}

	#endregion

	#region Boundary Value Tests

	[Trait("Category", "UnitTest")]
	[Theory]
	[InlineData("   ")]
	[InlineData("\t\n\r")]
	public async Task Given_Whitespace_ApiKey_When_GetChatClient_Invoked_Then_It_Should_Return_Client(string apiKey)
	{
		// Arrange
		var settings = BuildAppSettings(apiKey: apiKey);
		var connector = new OpenAIConnector(settings);

		// Act
		var client = await connector.GetChatClientAsync();

		// Assert
		// OpenAI SDK accepts whitespace API keys, so client should be created
		client.ShouldNotBeNull();
	}

	[Trait("Category", "UnitTest")]
	[Theory]
	[InlineData("   ")]
	[InlineData("\t\n\r")]
	public async Task Given_Whitespace_Model_When_GetChatClient_Invoked_Then_It_Should_Return_Client(string model)
	{
		// Arrange
		var settings = BuildAppSettings(model: model);
		var connector = new OpenAIConnector(settings);

		// Act
		var client = await connector.GetChatClientAsync();

		// Assert
		// OpenAI SDK accepts whitespace model names, so client should be created
		client.ShouldNotBeNull();
	}

	[Trait("Category", "UnitTest")]
	[Theory]
	[InlineData("sk-1234567890abcdef1234567890abcdef1234567890abcdef", "gpt-4o")]
	[InlineData("sk-proj-abcdef1234567890abcdef1234567890abcdef1234567890abcdef1234567890abcdef", "gpt-4o-mini")]
	[InlineData("very-long-api-key-xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx", "gpt-4.1-mini")]
	public async Task Given_Valid_Long_ApiKey_And_Model_When_GetChatClient_Invoked_Then_It_Should_Return_Client(string apiKey, string model)
	{
		// Arrange
		var settings = BuildAppSettings(apiKey: apiKey, model: model);
		var connector = new OpenAIConnector(settings);

		// Act
		var client = await connector.GetChatClientAsync();

		// Assert
		client.ShouldNotBeNull();
	}

	[Trait("Category", "UnitTest")]
	[Theory]
	[InlineData("test-key-with-special-chars-!@#$%^&*()", "gpt-4o")]
	[InlineData("test-key", "model-with-special-chars-!@#$%^&*()")]
	public async Task Given_ApiKey_Or_Model_With_Special_Characters_When_GetChatClient_Invoked_Then_It_Should_Return_Client(string apiKey, string model)
	{
		// Arrange
		var settings = BuildAppSettings(apiKey: apiKey, model: model);
		var connector = new OpenAIConnector(settings);

		// Act
		var client = await connector.GetChatClientAsync();

		// Assert
		client.ShouldNotBeNull();
	}

	#endregion

	#region Exception Handling Tests

	[Trait("Category", "UnitTest")]
	[Fact]
	public async Task Given_Settings_With_Wrong_Type_Cast_When_GetChatClient_Invoked_Then_It_Should_Handle_Gracefully()
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
		};
		var connector = new OpenAIConnector(settings);

		// Act & Assert
		var ex = await Assert.ThrowsAsync<InvalidOperationException>(connector.GetChatClientAsync);
		ex.Message.ShouldContain("OpenAI:ApiKey");
	}

	[Trait("Category", "UnitTest")]
	[Fact]
	public async Task Given_Multiple_Null_Settings_When_GetChatClient_Invoked_Then_It_Should_Throw_For_ApiKey_First()
	{
		// Arrange
		var settings = BuildAppSettings(apiKey: null, model: null);
		var connector = new OpenAIConnector(settings);

		// Act & Assert
		var ex = await Assert.ThrowsAsync<InvalidOperationException>(connector.GetChatClientAsync);
		ex.Message.ShouldContain("OpenAI:ApiKey");
	}

	#endregion

	#region Static Factory Method Tests

	[Trait("Category", "UnitTest")]
	[Fact]
	public async Task Given_OpenAI_ConnectorType_When_CreateChatClientAsync_Invoked_Then_It_Should_Return_ChatClient()
	{
		// Arrange
		var settings = BuildAppSettings();

		// Act
		var client = await LanguageModelConnector.CreateChatClientAsync(settings);

		// Assert
		client.ShouldNotBeNull();
		client.ShouldBeAssignableTo<IChatClient>();
	}

	[Trait("Category", "UnitTest")]
	[Fact]
	public async Task Given_OpenAI_ConnectorType_With_Invalid_ApiKey_When_CreateChatClientAsync_Invoked_Then_It_Should_Throw()
	{
		// Arrange
		var settings = BuildAppSettings(apiKey: null);

		// Act & Assert
		var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => LanguageModelConnector.CreateChatClientAsync(settings));
		ex.Message.ShouldContain("OpenAI:ApiKey");
	}

	#endregion

}