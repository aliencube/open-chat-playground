using Microsoft.Playwright;
using Microsoft.Playwright.Xunit;

namespace OpenChat.PlaygroundApp.Tests.Components.Pages.Chat;

public class ChatInputUITest : PageTest
{
    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        await Page.GotoAsync("http://localhost:8080");
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
    }

    [Theory]
    [InlineData("하늘은 왜 푸른 색인가요?", 1)]
    [InlineData("Why is the sky blue?", 1)]
    [InlineData(" ", 1)]
    [InlineData("\n", 1)]
    [InlineData("\r\n", 1)]
    [InlineData("", 0)]
    [Trait("Category", "Skip")]
    public async Task GivenUserMessage_WhenSendButtonClicked_ThenSendOnlyNotEmptyString(string userMessage, int expectedMessageReturn)
    {
        // Arrange
        var textArea = Page.GetByRole(AriaRole.Textbox, new() { Name = "User Message Textarea" });
        var sendButton = Page.GetByRole(AriaRole.Button, new() { Name = "Send Button" });
        var messageCountBefore = await Page.Locator(".assistant-message-header")
                                        .CountAsync();

        // Act
        await textArea.FillAsync(userMessage);
        await sendButton.ClickAsync();

        // Assert
        await Expect(textArea).ToHaveValueAsync("");
        await Expect(Page.Locator(".assistant-message-header"))
                .ToHaveCountAsync(messageCountBefore + expectedMessageReturn);
    }

    [Theory]
    [InlineData("하늘은 왜 푸른 색인가요?", 1)]
    [InlineData("Why is the sky blue?", 1)]
    [InlineData(" ", 1)]
    [InlineData("\n", 1)]
    [InlineData("\r\n", 1)]
    [InlineData("", 0)]
    [Trait("Category", "Skip")]
    public async Task GivenUserMessage_WhenEnterPressed_ThenSendOnlyNotEmptyString(string userMessage, int expectedMessageReturn)
    {
        // Arrange
        var textArea = Page.GetByRole(AriaRole.Textbox, new() { Name = "User Message Textarea" });
        var messageCountBefore = await Page.Locator(".assistant-message-header")
                                        .CountAsync();

        // Act
        await textArea.FillAsync(userMessage);
        await textArea.PressAsync("Enter");

        // Assert
        await Expect(textArea).ToHaveValueAsync("");
        await Expect(Page.Locator(".assistant-message-header"))
                .ToHaveCountAsync(messageCountBefore + expectedMessageReturn);
    }

    [Theory]
    [InlineData("하늘은 왜 푸를까?", "rgb(0, 0, 0)")]
    [InlineData("Why is the sky blue?", "rgb(0, 0, 0)")]
    [InlineData(" ", "rgb(0, 0, 0)")]
    [InlineData("\n", "rgb(0, 0, 0)")]
    [InlineData("\r\n", "rgb(0, 0, 0)")]
    [InlineData("", "rgb(170, 170, 170)")]
    public async Task GivenUserMessage_WhenFillInTextarea_ThenColorReflects(string userMessage, string buttonColor)
    {
        // Arrange
        var textArea = Page.GetByRole(AriaRole.Textbox, new() { Name = "User Message Textarea" });
        var sendButton = Page.GetByRole(AriaRole.Button, new() { Name = "Send Button" });

        // Act
        await textArea.FillAsync(userMessage);

        // Assert
        await Expect(sendButton).ToHaveCSSAsync("color", buttonColor);
    }

    public override async Task DisposeAsync()
    {
        await Page.CloseAsync();
        await base.DisposeAsync();
    }
}
