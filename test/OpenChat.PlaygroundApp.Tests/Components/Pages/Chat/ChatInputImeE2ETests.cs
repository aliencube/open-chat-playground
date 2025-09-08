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
    [Trait("Category", "LLMRequired")]
    [Fact]
    public async Task KoreanIme_Composition_Enter_DoesNotSubmit_UntilCompositionEnd()
    {
        // Arrange
        var textArea = Page.GetByRole(AriaRole.Textbox, new() { Name = "User Message Textarea" });
        await textArea.FocusAsync();
        await textArea.FillAsync("가나다 테스트");
        var userCountBefore = await Page.Locator(".user-message").CountAsync();

        // Act: Enter during composition should NOT submit
        await Page.DispatchEventAsync("textarea", "compositionstart", new { });
        await Page.DispatchEventAsync("textarea", "keydown", new { bubbles = true, cancelable = true, key = "Enter", isComposing = true });

        // Assert: no user message added
        var userCountAfterComposeEnter = await Page.Locator(".user-message").CountAsync();
        userCountAfterComposeEnter.ShouldBe(userCountBefore);

        // Act: Composition ends, then Enter should submit once
        await Page.DispatchEventAsync("textarea", "compositionend", new { data = "가" });
        await Page.DispatchEventAsync("textarea", "keydown", new { bubbles = true, cancelable = true, key = "Enter" });

        // Assert: exactly one user message added
        var userCountAfterSubmit = await Page.Locator(".user-message").CountAsync();
        userCountAfterSubmit.ShouldBe(userCountBefore + 1);
    }

    [Trait("Category", "IntegrationTest")]
    [Trait("Category", "LLMRequired")]
    [Fact]
    public async Task AfterSend_ImmediateEnter_DoesNotDuplicateSend()
    {
        // Arrange
        var textArea = Page.GetByRole(AriaRole.Textbox, new() { Name = "User Message Textarea" });
        await textArea.FocusAsync();
        await textArea.FillAsync("중복 전송 방지 테스트");
        var userCountBefore = await Page.Locator(".user-message").CountAsync();

        // Act: Send via Enter
        await textArea.PressAsync("Enter");

        // Wait for the user message to be added (submit completes)
        await Page.WaitForFunctionAsync(
            "([expected]) => document.querySelectorAll('.user-message').length >= expected",
            new object[] { userCountBefore + 1 }
        );

        // Assert: one user message
        var userCountAfterFirst = await Page.Locator(".user-message").CountAsync();
        userCountAfterFirst.ShouldBe(userCountBefore + 1);

        // Act: Press Enter again immediately without typing
        await textArea.PressAsync("Enter");

        // Assert: no additional user message
        var userCountAfterSecond = await Page.Locator(".user-message").CountAsync();
        userCountAfterSecond.ShouldBe(userCountBefore + 1);
    }

    [Trait("Category", "IntegrationTest")]
    [Fact]
    public async Task ShiftEnter_InsertsNewline_DoesNotSubmit()
    {
        // Arrange
        var textArea = Page.GetByRole(AriaRole.Textbox, new() { Name = "User Message Textarea" });
        await textArea.FocusAsync();
        await textArea.FillAsync("첫 줄");
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
