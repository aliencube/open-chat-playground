using OpenChat.PlaygroundApp.Abstractions;
using OpenChat.PlaygroundApp.Configurations;
using OpenChat.PlaygroundApp.Connectors;

namespace OpenChat.PlaygroundApp.Tests.Abstractions;

public class LanguageModelConnectorTests
{
	private static AppSettings BuildAppSettings(
        ConnectorType connectorType = ConnectorType.GitHubModels,
        string? endpoint = "https://models.github.ai/inference",
        string? token = "test-token",
        string? model = "openai/gpt-5-mini")
	{
		return new AppSettings
		{
			ConnectorType = connectorType,
			GitHubModels = new GitHubModelsSettings
			{
				Endpoint = endpoint,
				Token = token,
				Model = model
			}
		};
	}

	[Trait("Category", "UnitTest")]
	[Fact]
	public void Given_GitHubModels_Settings_When_CreateChatClient_Invoked_Then_It_Should_Return_ChatClient()
	{
		var settings = BuildAppSettings();
		var client = LanguageModelConnector.CreateChatClient(settings);

		client.ShouldNotBeNull();
	}

	[Trait("Category", "UnitTest")]
	[Theory]
	[InlineData(ConnectorType.OpenAI)]
	[InlineData(ConnectorType.AmazonBedrock)]
	[InlineData(ConnectorType.AzureAIFoundry)]
	[InlineData(ConnectorType.Unknown)]
	public void Given_Unsupported_ConnectorType_When_CreateChatClient_Invoked_Then_It_Should_Throw(ConnectorType connectorType)
	{
		var settings = BuildAppSettings(connectorType: connectorType);

		var ex = Assert.Throws<NotSupportedException>(() => LanguageModelConnector.CreateChatClient(settings));

		ex.Message.ShouldContain($"Connector type '{connectorType}'");
	}
}
