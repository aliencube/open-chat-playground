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
        string? model = "openai/gpt-4o-mini")
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
	public async Task Given_GitHubModels_Settings_When_CreateChatClient_Invoked_Then_It_Should_Return_ChatClient()
	{
		var settings = BuildAppSettings();
		var client = await LanguageModelConnector.CreateChatClientAsync(settings);

		client.ShouldNotBeNull();
	}

	[Trait("Category", "UnitTest")]
	[Theory]
	[InlineData(ConnectorType.AmazonBedrock)]
	[InlineData(ConnectorType.AzureAIFoundry)]
	[InlineData(ConnectorType.Unknown)]
	public async Task Given_Unsupported_ConnectorType_When_CreateChatClient_Invoked_Then_It_Should_Throw(ConnectorType connectorType)
	{
		var settings = BuildAppSettings(connectorType: connectorType);

		var ex = await Assert.ThrowsAsync<NotSupportedException>(() => LanguageModelConnector.CreateChatClientAsync(settings));

		ex.Message.ShouldContain($"Connector type '{connectorType}'");
	}

	[Trait("Category", "UnitTest")]
    [Theory]
    [InlineData(typeof(GitHubModelsConnector))]
    [InlineData(typeof(OpenAIConnector))]
    public void Given_Concrete_Connectors_When_Checking_Inheritance_Then_Should_Inherit_From_LanguageModelConnector(Type type)
    {
        type.IsSubclassOf(typeof(LanguageModelConnector)).ShouldBeTrue();
    }
}
