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
    [Theory]
    [InlineData(null, typeof(InvalidOperationException), "GitHubModels:Token")]
    [InlineData("", typeof(ArgumentException), "key")]
	public void Given_Missing_Token_When_GetChatClient_Invoked_Then_It_Should_Throw(string? token, Type expected, string message)
    {
        var settings = BuildAppSettings(token: token);
        var connector = new GitHubModelsConnector(settings);

        var ex = Assert.Throws(expected, connector.GetChatClient);

        ex.Message.ShouldContain(message);
    }

	[Trait("Category", "UnitTest")]
    [Theory]
    [InlineData(null, typeof(InvalidOperationException), "GitHubModels:Endpoint")]
    [InlineData("", typeof(UriFormatException), "empty")]
	public void Given_Missing_Endpoint_When_GetChatClient_Invoked_Then_It_Should_Throw(string? endpoint, Type expected, string message)
	{
		var settings = BuildAppSettings(endpoint: endpoint);
		var connector = new GitHubModelsConnector(settings);

		var ex = Assert.Throws(expected, connector.GetChatClient);

		ex.Message.ShouldContain(message);
	}

	[Trait("Category", "UnitTest")]
    [Theory]
    [InlineData(null, typeof(ArgumentNullException), "model")]
    [InlineData("", typeof(ArgumentException), "model")]
	public void Given_Missing_Model_When_GetChatClient_Invoked_Then_It_Should_Throw(string? model, Type expected, string message)
	{
		var settings = BuildAppSettings(model: model);
		var connector = new GitHubModelsConnector(settings);

		var ex = Assert.Throws(expected, connector.GetChatClient);

		ex.Message.ShouldContain(message);
	}
}
