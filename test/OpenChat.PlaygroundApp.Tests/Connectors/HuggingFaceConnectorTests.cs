using OpenChat.PlaygroundApp.Configurations;
using OpenChat.PlaygroundApp.Connectors;

namespace OpenChat.PlaygroundApp.Tests.Connectors;

public class HuggingFaceConnectorTests
{
	private static AppSettings BuildAppSettings(string? baseUrl = "https://test.huggingface.co/api", string? model = "hf-model-name")
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
	public async Task Given_Valid_Settings_When_GetChatClient_Invoked_Then_It_Should_Return_ChatClient()
	{
		var settings = BuildAppSettings();
		var connector = new HuggingFaceConnector(settings);

		var client = await connector.GetChatClientAsync();

		client.ShouldNotBeNull();
	}

	[Trait("Category", "UnitTest")]
	[Theory]
	[InlineData(null, typeof(InvalidOperationException), "HuggingFace:BaseUrl")]
	[InlineData("", typeof(InvalidOperationException), "HuggingFace:BaseUrl")]
	[InlineData("strange-uri-format", typeof(UriFormatException), "Invalid URI")]
	public async Task Given_Missing_BaseUrl_When_GetChatClient_Invoked_Then_It_Should_Throw(string? baseUrl, Type expected, string message)
	{
		var settings = BuildAppSettings(baseUrl: baseUrl);
		var connector = new HuggingFaceConnector(settings);

		var ex = await Assert.ThrowsAsync(expected, connector.GetChatClientAsync);

		ex.Message.ShouldContain(message);
	}

	[Trait("Category", "UnitTest")]
	[Theory]
	[InlineData(null, typeof(InvalidOperationException), "HuggingFace:Model")]
	[InlineData("", typeof(InvalidOperationException), "HuggingFace:Model")]
	public async Task Given_Missing_Model_When_GetChatClient_Invoked_Then_It_Should_Throw(string? model, Type expected, string message)
	{
		var settings = BuildAppSettings(model: model);
		var connector = new HuggingFaceConnector(settings);

		var ex = await Assert.ThrowsAsync(expected, connector.GetChatClientAsync);

		ex.Message.ShouldContain(message);
	}
}
