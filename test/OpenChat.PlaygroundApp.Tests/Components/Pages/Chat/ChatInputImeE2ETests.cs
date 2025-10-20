using Microsoft.Playwright;
using Microsoft.Playwright.Xunit;

namespace OpenChat.PlaygroundApp.Tests.Components.Pages.Chat;

public class ChatInputImeE2ETests : PageTest
{
    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        await Page.GotoAsync(TestConstants.LocalhostUrl);
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
    }

    [Trait("Category", "IntegrationTest")]
    [Trait("Category", "UI")]
    [Theory]
    [InlineData("안녕하세요")]
    [InlineData("테스트")]
    public async Task Given_Korean_IME_Composition_When_Enter_During_Composition_Then_It_Should_Not_Submit(string testMessage)
    {
        // Arrange
        var textArea = Page.GetByRole(AriaRole.Textbox, new() { Name = "User Message Textarea" });
        await textArea.FocusAsync();
        await textArea.FillAsync(testMessage);
        var userCountBefore = await Page.Locator(".user-message").CountAsync();

        // Act: Enter during composition should NOT submit
        await Page.DispatchEventAsync("textarea", "compositionstart", new { });
        await Page.DispatchEventAsync("textarea", "keydown", new { bubbles = true, cancelable = true, key = "Enter", isComposing = true });

        // Assert: no user message added
        var userCountAfterComposeEnter = await Page.Locator(".user-message").CountAsync();
        userCountAfterComposeEnter.ShouldBe(userCountBefore);
    }

    [Trait("Category", "IntegrationTest")]
    [Trait("Category", "UI")]
    [Trait("Category", "LLMRequired")]
    [Theory]
    [InlineData("안녕하세요", "안")]
    [InlineData("테스트", "테")]
    public async Task Given_Korean_IME_Composition_Ended_When_Enter_Pressed_Then_It_Should_Submit_Once(string testMessage, string compositionData)
    {
        // Arrange
        var textArea = Page.GetByRole(AriaRole.Textbox, new() { Name = "User Message Textarea" });
        await textArea.FocusAsync();
        await textArea.FillAsync(testMessage);
        var userCountBefore = await Page.Locator(".user-message").CountAsync();

        // Act: Composition ends, then Enter should submit once
        await Page.DispatchEventAsync("textarea", "compositionstart", new { });
        await Page.DispatchEventAsync("textarea", "compositionend", new { data = compositionData });
        var assistantCountBefore = await Page.Locator(".assistant-message-header").CountAsync();
        await Page.DispatchEventAsync("textarea", "keydown", new { bubbles = true, cancelable = true, key = "Enter" });

        // Assert: assistant response begins and user message added once
        await Page.WaitForFunctionAsync(
            "args => document.querySelectorAll(args.selector).length >= args.expected",
            new { selector = ".assistant-message-header", expected = assistantCountBefore + 1 },
            options: new() { Timeout = 60000 }
        );
        var userCountAfterSubmit = await Page.Locator(".user-message").CountAsync();
        userCountAfterSubmit.ShouldBe(userCountBefore + 1);
    }

    [Trait("Category", "IntegrationTest")]
    [Trait("Category", "UI")]
    [Trait("Category", "LLMRequired")]
    [Theory]
    [InlineData("테스트 메시지")]
    [InlineData("안녕하세요")]
    public async Task Given_Message_Sent_When_Enter_Pressed_Immediately_Then_It_Should_Not_Send_Twice(string testMessage)
    {
        // Arrange
        var textArea = Page.GetByRole(AriaRole.Textbox, new() { Name = "User Message Textarea" });
        await textArea.FocusAsync();
        await textArea.FillAsync(testMessage);
        var userCountBefore = await Page.Locator(".user-message").CountAsync();

        // Act: Send via Enter
        var assistantCountBefore = await Page.Locator(".assistant-message-header").CountAsync();
        await textArea.PressAsync("Enter");

        // Assert: assistant response begins and one user message
        await Page.WaitForFunctionAsync(
            "args => document.querySelectorAll(args.selector).length >= args.expected",
            new { selector = ".assistant-message-header", expected = assistantCountBefore + 1 },
            options: new() { Timeout = 60000 }
        );
        var userCountAfterFirst = await Page.Locator(".user-message").CountAsync();
        userCountAfterFirst.ShouldBe(userCountBefore + 1);

        // Act: Press Enter again immediately without typing
        await textArea.PressAsync("Enter");

        // Assert: no additional user message
        var userCountAfterSecond = await Page.Locator(".user-message").CountAsync();
        userCountAfterSecond.ShouldBe(userCountBefore + 1);
    }

    [Trait("Category", "IntegrationTest")]
    [Trait("Category", "UI")]
    [Theory]
    [InlineData("첫 줄")]
    [InlineData("테스트")]
    public async Task Given_Text_Input_When_Shift_Enter_Pressed_Then_It_Should_Insert_Newline_Not_Submit(string initialText)
    {
        // Arrange
        var textArea = Page.GetByRole(AriaRole.Textbox, new() { Name = "User Message Textarea" });
        await textArea.FocusAsync();
        await textArea.FillAsync(initialText);
        var userCountBefore = await Page.Locator(".user-message").CountAsync();

        // Act: Shift+Enter should insert newline (not submit)
        await textArea.PressAsync("Shift+Enter");

        // Assert: value contains newline and no submission
        var value = await textArea.InputValueAsync();
        value.ShouldContain("\n");
        var userCountAfter = await Page.Locator(".user-message").CountAsync();
        userCountAfter.ShouldBe(userCountBefore);
    }

    public override async Task DisposeAsync()
    {
        await Page.CloseAsync();
        await base.DisposeAsync();
    }
}
