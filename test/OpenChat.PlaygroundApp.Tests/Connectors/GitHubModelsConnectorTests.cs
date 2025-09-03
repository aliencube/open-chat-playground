using OpenChat.PlaygroundApp.Configurations;
using OpenChat.PlaygroundApp.Connectors;

namespace OpenChat.PlaygroundApp.Tests.Connectors;

public class GitHubModelsConnectorTests
{
	private static AppSettings BuildAppSettings(string? endpoint = "https://models.github.ai/inference", string? token = "test-token", string? model = "openai/gpt-4o-mini")
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
	public async Task Given_Valid_Settings_When_GetChatClient_Invoked_Then_It_Should_Return_ChatClient()
	{
        // Arrange
		var settings = BuildAppSettings();
		var connector = new GitHubModelsConnector(settings);

        // Act
        var client = await connector.GetChatClientAsync();

        // Assert
        client.ShouldNotBeNull();
	}

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData(null, typeof(InvalidOperationException), "GitHubModels:Token")]
    [InlineData("", typeof(ArgumentException), "key")]
	public async Task Given_Missing_Token_When_GetChatClient_Invoked_Then_It_Should_Throw(string? token, Type expected, string message)
    {
        // Arrange
        var settings = BuildAppSettings(token: token);
        var connector = new GitHubModelsConnector(settings);

        // Act
        var ex = await Assert.ThrowsAsync(expected, connector.GetChatClientAsync);

        // Assert
        ex.Message.ShouldContain(message);
    }

	[Trait("Category", "UnitTest")]
    [Theory]
    [InlineData(null, typeof(InvalidOperationException), "GitHubModels:Endpoint")]
    [InlineData("", typeof(UriFormatException), "empty")]
	public async Task Given_Missing_Endpoint_When_GetChatClient_Invoked_Then_It_Should_Throw(string? endpoint, Type expected, string message)
	{
        // Arrange
		var settings = BuildAppSettings(endpoint: endpoint);
		var connector = new GitHubModelsConnector(settings);

        // Act
        var ex = await Assert.ThrowsAsync(expected, connector.GetChatClientAsync);

        // Assert
        ex.Message.ShouldContain(message);
	}

	[Trait("Category", "UnitTest")]
    [Theory]
    [InlineData(null, typeof(ArgumentNullException), "model")]
    [InlineData("", typeof(ArgumentException), "model")]
	public async Task Given_Missing_Model_When_GetChatClient_Invoked_Then_It_Should_Throw(string? model, Type expected, string message)
	{
        // Arrange
		var settings = BuildAppSettings(model: model);
		var connector = new GitHubModelsConnector(settings);

        // Act
        var ex = await Assert.ThrowsAsync(expected, connector.GetChatClientAsync);

		// Assert
		ex.Message.ShouldContain(message);
	}
}
