using Microsoft.Playwright;
using Microsoft.Playwright.Xunit;

namespace OpenChat.PlaygroundApp.Tests.Components.Pages.Chat;

public class ChatStreamingUITest : PageTest
{
    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        await Page.GotoAsync(TestConstants.LocalhostUrl);
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
    }

    [Trait("Category", "IntegrationTest")]
    [Trait("Category", "LLMRequired")]
    [Theory]
    [InlineData("하늘은 왜 푸른 색인가요?")]
    [InlineData("Why is the sky blue?")]
    public async Task Given_UserMessage_When_SendButton_Clicked_Then_Response_Should_Stream_Progressively(string userMessage)
    {
        // Arrange
        var textArea = Page.GetByRole(AriaRole.Textbox, new() { Name = "User Message Textarea" });
        var sendButton = Page.GetByRole(AriaRole.Button, new() { Name = "User Message Send Button" });

        // Act
        await textArea.FillAsync(userMessage);
        await sendButton.ClickAsync();

        // Assert: Wait for the assistant message container to appear
        var messageSelector = ".assistant-message-text";
        await Page.WaitForSelectorAsync(messageSelector);

        // Check that the message content grows over time (streaming)
        string previousContent = "";
        bool streamed = false;
        for (int i = 0; i < 10; i++)
        {
            var content = await Page.Locator(messageSelector).InnerTextAsync();
            if (content.Length > previousContent.Length)
            {
                streamed = true;
                break;
            }
            previousContent = content;
            await Task.Delay(500); // Wait for more content to stream in
        }
        Assert.True(streamed, "Response should stream progressively, not appear all at once.");
    }

    public override async Task DisposeAsync()
    {
        await Page.CloseAsync();
        await base.DisposeAsync();
    }
}
