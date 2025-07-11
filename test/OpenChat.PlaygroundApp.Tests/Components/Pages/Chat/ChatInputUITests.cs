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
    [InlineData("Why is the sky blue?")]
    [InlineData(" ")]
    [InlineData("\n")]
    [InlineData("\n\r")]
    public async Task GivenInputNotEmpty_WhenSendButtonClicked_ThenSend(string userMessage)
    {
        // Arrange
        var textArea = Page.GetByRole(AriaRole.Textbox);
        var sendButton = Page.GetByRole(AriaRole.Button, new() { Name = "Send" });
        var messageCountBefore = await Page.Locator(".assistant-message-header")
                                        .CountAsync();

        // Act
        await textArea.FillAsync(userMessage);
        await sendButton.ClickAsync();

        // Assert
        await Expect(textArea).ToHaveValueAsync("");
        await Expect(Page.Locator(".assistant-message-header"))
                .ToHaveCountAsync(messageCountBefore + 1);
    }

    [Theory]
    [InlineData("Why is the sky blue?")]
    [InlineData(" ")]
    [InlineData("\n")]
    [InlineData("\n\r")]
    public async Task GivenInputNotEmpty_ThenSendButtonColorShouldBeBlack(string userMessage)
    {
        // Arrange
        var textArea = Page.GetByRole(AriaRole.Textbox);
        var sendButton = Page.GetByRole(AriaRole.Button, new() { Name = "Send" });

        // Act
        await textArea.FillAsync(userMessage);

        // Assert
        await Expect(sendButton).ToHaveCSSAsync("color", "rgb(0, 0, 0)");
    }

    [Fact]
    public async Task GivenInputEmpty_WhenSendButtonClicked_ThenDoNotSend()
    {
        // Arrange
        var textArea = Page.GetByRole(AriaRole.Textbox);
        var sendButton = Page.GetByRole(AriaRole.Button, new() { Name = "Send" });
        var messageCountBefore = await Page.Locator(".assistant-message-header")
                                        .CountAsync();

        // Act
        await textArea.FillAsync("");
        await sendButton.ClickAsync();

        // Assert
        await Expect(Page.Locator(".assistant-message-header"))
                .ToHaveCountAsync(messageCountBefore);
    }

    [Fact]
    public async Task GivenInputEmpty_ThenButtonColorShouldBeGrey()
    {
        // Arrange
        var textArea = Page.GetByRole(AriaRole.Textbox);
        var sendButton = Page.GetByRole(AriaRole.Button, new() { Name = "Send" });

        // Act
        await textArea.FillAsync("");

        // Assert
        await Expect(sendButton).ToHaveCSSAsync("color", "rgb(170, 170, 170)");
    }

    public override async Task DisposeAsync()
    {
        await Page.CloseAsync();
        await base.DisposeAsync();
    }
}
