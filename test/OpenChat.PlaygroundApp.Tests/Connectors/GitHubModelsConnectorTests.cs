using OpenChat.PlaygroundApp.Configurations;
using OpenChat.PlaygroundApp.Connectors;

namespace OpenChat.PlaygroundApp.Tests.Connectors;

public class GitHubModelsConnectorTests
{
	private static AppSettings BuildAppSettings(string? endpoint = "https://models.github.ai/inference", string? token = "test-token", string? model = "openai/gpt-5-mini")
	{
		return new AppSettings
		{
			ConnectorType = ConnectorType.GitHubModels,
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
	public void Given_Valid_Settings_When_GetChatClient_Invoked_Then_It_Should_Return_ChatClient()
	{
		var settings = BuildAppSettings();
		var connector = new GitHubModelsConnector(settings);

		var client = connector.GetChatClient();

		client.ShouldNotBeNull();
	}

	[Trait("Category", "UnitTest")]
	[Fact]
	public void Given_Missing_Token_When_GetChatClient_Invoked_Then_It_Should_Throw()
	{
		var settings = BuildAppSettings(token: null);
		var connector = new GitHubModelsConnector(settings);

		var ex = Assert.Throws<InvalidOperationException>(() => connector.GetChatClient());

		ex.Message.ShouldContain("GitHubModels:Token");
	}

	[Trait("Category", "UnitTest")]
	[Fact]
	public void Given_Missing_Endpoint_When_GetChatClient_Invoked_Then_It_Should_Throw()
	{
		var settings = BuildAppSettings(endpoint: null);
		var connector = new GitHubModelsConnector(settings);

		var ex = Assert.Throws<InvalidOperationException>(() => connector.GetChatClient());

		ex.Message.ShouldContain("GitHubModels:Endpoint");
	}

	[Trait("Category", "UnitTest")]
	[Fact]
	public void Given_Missing_Model_When_GetChatClient_Invoked_Then_It_Should_Throw()
	{
		var settings = BuildAppSettings(model: null);
		var connector = new GitHubModelsConnector(settings);

		var ex = Assert.Throws<ArgumentNullException>(() => connector.GetChatClient());

		ex.ParamName.ShouldBe("model");
	}
}
