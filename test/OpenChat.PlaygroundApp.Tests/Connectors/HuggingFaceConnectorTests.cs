using OpenChat.PlaygroundApp.Configurations;
using OpenChat.PlaygroundApp.Connectors;

namespace OpenChat.PlaygroundApp.Tests.Connectors;

public class HuggingFaceConnectorTests
{
	private const string BaseUrl = "https://test.huggingface.co/api";
	private const string Model = "hf.co/test-org/model-gguf";

	private static AppSettings BuildAppSettings(string? baseUrl = BaseUrl, string? model = Model)
	{
		return new AppSettings
		{
			ConnectorType = ConnectorType.HuggingFace,
			HuggingFace = new HuggingFaceSettings
			{
				BaseUrl = baseUrl,
				Model = model
			}
		};
	}

	[Trait("Category", "UnitTest")]
	[Fact]
	public void Given_Settings_Is_Null_When_EnsureLanguageModelSettingsValid_Invoked_Then_It_Should_Throw()
	{
        // Arrange
        var settings = new AppSettings { ConnectorType = ConnectorType.HuggingFace, HuggingFace = null };
        var connector = new HuggingFaceConnector(settings);

		// Act
		Action action = () => connector.EnsureLanguageModelSettingsValid();

		// Assert
		action.ShouldThrow<InvalidOperationException>()
			  .Message.ShouldContain("HuggingFace");
	}

	[Trait("Category", "UnitTest")]
	[Theory]
	[InlineData(null, typeof(NullReferenceException), "Object reference not set to an instance of an object")]
	[InlineData("", typeof(InvalidOperationException), "HuggingFace:BaseUrl")]
	[InlineData("   ", typeof(InvalidOperationException), "HuggingFace:BaseUrl")]
	[InlineData("\t\n\r", typeof(InvalidOperationException), "HuggingFace:BaseUrl")]
	public void Given_Invalid_BaseUrl_When_EnsureLanguageModelSettingsValid_Invoked_Then_It_Should_Throw(string? baseUrl, Type expectedType, string expectedMessage)
	{
		// Arrange
		var settings = BuildAppSettings(baseUrl: baseUrl);
		var connector = new HuggingFaceConnector(settings);

		// Act
		Action action = () => connector.EnsureLanguageModelSettingsValid();

		// Assert
		action.ShouldThrow(expectedType)
			  .Message.ShouldContain(expectedMessage);
	}

	[Trait("Category", "UnitTest")]
	[Theory]
	[InlineData(null, typeof(NullReferenceException), "Object reference not set to an instance of an object")]
	[InlineData("", typeof(InvalidOperationException), "HuggingFace:Model")]
	[InlineData("   ", typeof(InvalidOperationException), "HuggingFace:Model")]
	[InlineData("\t\n\r", typeof(InvalidOperationException), "HuggingFace:Model")]
	[InlineData("hf.co/org/model", typeof(InvalidOperationException), "HuggingFace:Model format")]
	[InlineData("org/model-gguf", typeof(InvalidOperationException), "HuggingFace:Model format")]
	[InlineData("hf.co//model-gguf", typeof(InvalidOperationException), "HuggingFace:Model format")]
	public void Given_Invalid_Model_When_EnsureLanguageModelSettingsValid_Invoked_Then_It_Should_Throw(string? model, Type expectedType, string expectedMessage)
	{
		// Arrange
		var settings = BuildAppSettings(model: model);
        var connector = new HuggingFaceConnector(settings);

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
        var connector = new HuggingFaceConnector(settings);

		// Act
		var result = connector.EnsureLanguageModelSettingsValid();

		// Assert
		result.ShouldBeTrue();
	}

    [Trait("Category", "IntegrationTest")]
    [Trait("Category", "LLMRequired")]
	[Fact]
	public async Task Given_Valid_Settings_When_GetChatClient_Invoked_Then_It_Should_Return_ChatClient()
	{
		// Arrange
		var settings = BuildAppSettings();
        var connector = new HuggingFaceConnector(settings);

		// Act
		var client = await connector.GetChatClientAsync();

		// Assert
		client.ShouldNotBeNull();
	}

	[Trait("Category", "UnitTest")]
	[Theory]
	[InlineData(null, typeof(ArgumentNullException), "null")]
	[InlineData("", typeof(UriFormatException), "empty")]
	public async Task Given_Missing_BaseUrl_When_GetChatClient_Invoked_Then_It_Should_Throw(string? baseUrl, Type expected, string message)
	{
		// Arrange
		var settings = BuildAppSettings(baseUrl: baseUrl);
        var connector = new HuggingFaceConnector(settings);

		// Act
		Func<Task> func = connector.GetChatClientAsync;

		// Assert
		var ex = await func.ShouldThrowAsync(expected);
		ex.Message.ShouldContain(message);
	}
}