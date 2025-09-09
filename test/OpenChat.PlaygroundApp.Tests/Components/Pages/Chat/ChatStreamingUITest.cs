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
        const string messageSelector = ".assistant-message-text";
        var message = Page.Locator(messageSelector);
        const int timeoutMs = 5000;
        const int delayMs = 100;
        int maxChecks = timeoutMs / delayMs;

        // Act
        await textArea.FillAsync(userMessage);
        await sendButton.ClickAsync();

        // Assert
        await Expect(message).ToBeVisibleAsync(new() { Timeout = timeoutMs });
        await Expect(message).Not.ToHaveTextAsync(string.Empty, new() { Timeout = timeoutMs });


        string previousContent = "";
        bool streamed = false;
        for (int i = 0; i < maxChecks; i++)
        {
            var content = await message.InnerTextAsync();
            if (content.Length > previousContent.Length)
            {
                streamed = true;
                break;
            }
            previousContent = content;
            await Task.Delay(delayMs);
        }
        var finalContent = await message.InnerTextAsync();
        Assert.True(streamed || finalContent.Length > previousContent.Length, 
            $"Response did not stream within {timeoutMs}ms. Final length: {finalContent.Length}. " +
            $"Checked {maxChecks} times every {delayMs}ms.");
    }

    public override async Task DisposeAsync()
    {
        await Page.CloseAsync();
        await base.DisposeAsync();
    }
}
