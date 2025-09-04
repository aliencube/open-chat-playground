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
}