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
    [InlineData("하늘은 왜 푸른 색인가요? 다섯 개의 단락으로 자세히 설명해주세요.")]
    [InlineData("Why is the sky blue? Please explain in five paragraphs.")]
    public async Task Given_UserMessage_When_SendButton_Clicked_Then_Response_Should_Stream_Progressively(string userMessage)
    {
        // Arrange
        var textArea = Page.GetByRole(AriaRole.Textbox, new() { Name = "User Message Textarea" });
        var sendButton = Page.GetByRole(AriaRole.Button, new() { Name = "User Message Send Button" });
        const string messageSelector = ".assistant-message-text";
        var message = Page.Locator(messageSelector);
        const int timeoutMs = 5000;
        const int streamingCheckDelayMs = 3000;

        // Act
        await textArea.FillAsync(userMessage);
        await sendButton.ClickAsync();

        // Assert
        await Expect(message).ToBeVisibleAsync(new() { Timeout = timeoutMs });
        await Expect(message).Not.ToHaveTextAsync(string.Empty, new() { Timeout = timeoutMs });

        var initialContent = await message.InnerTextAsync();

        try
        {
            await Page.WaitForFunctionAsync(
                $"selector => document.querySelector(selector).innerText.length > {initialContent.Length}",
                messageSelector,
                new() { Timeout = streamingCheckDelayMs } 
            );
        }
        catch (TimeoutException)
        {
            var finalContent = await message.InnerTextAsync();
            Assert.Fail($"Streaming was not detected. The content did not grow after the initial part. Initial length: {initialContent.Length}, Final length: {finalContent.Length}.");
        }
    }

    public override async Task DisposeAsync()
    {
        await Page.CloseAsync();
        await base.DisposeAsync();
    }
}
