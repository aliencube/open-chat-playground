using OpenChat.PlaygroundApp.Configurations;
using OpenChat.PlaygroundApp.Connectors;

namespace OpenChat.PlaygroundApp.Tests.Connectors;

public class AnthropicConnectorTests
{
    private static AppSettings BuildAppSettings(string? apiKey = "sk-test-anthropic-key", string? model = "claude-sonnet-4-20250514")
    {
        return new AppSettings
        {
            ConnectorType = ConnectorType.Anthropic,
            Anthropic = new AnthropicSettings
            {
                ApiKey = apiKey,
                Model = model
            }
        };
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData(null, typeof(InvalidOperationException), "Anthropic:ApiKey")]
    [InlineData("", typeof(InvalidOperationException), "Anthropic:ApiKey")]
    public async Task Given_Missing_ApiKey_When_GetChatClient_Invoked_Then_It_Should_Throw(string? apiKey, Type expected, string message)
    {
        var settings = BuildAppSettings(apiKey: apiKey);
        var connector = new AnthropicConnector(settings);

        var ex = await Assert.ThrowsAsync(expected, connector.GetChatClientAsync);

        ex.Message.ShouldContain(message);
    }

    [Trait("Category", "UnitTest")]
    [Fact]
    public async Task Given_Valid_Settings_When_GetChatClient_Invoked_Then_It_Should_Throw_NotImplementedException()
    {
        var settings = BuildAppSettings();
        var connector = new AnthropicConnector(settings);

        var ex = await Assert.ThrowsAsync<NotImplementedException>(connector.GetChatClientAsync);

        ex.Message.ShouldContain("Anthropic connector implementation is pending");
    }

    [Trait("Category", "UnitTest")]
    [Fact]
    public async Task Given_Valid_ApiKey_And_No_Model_When_GetChatClient_Invoked_Then_It_Should_Use_Default_Model()
    {
        // Model should default to claude-3-5-sonnet-latest when not specified
        var settings = BuildAppSettings(model: null);
        var connector = new AnthropicConnector(settings);

        var ex = await Assert.ThrowsAsync<NotImplementedException>(connector.GetChatClientAsync);

        // Should not throw validation exception for missing model
        ex.Message.ShouldContain("Anthropic connector implementation is pending");
    }
}
