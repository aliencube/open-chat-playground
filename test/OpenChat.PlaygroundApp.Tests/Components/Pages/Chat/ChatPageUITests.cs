using Microsoft.Playwright;
using Microsoft.Playwright.Xunit;

namespace OpenChat.PlaygroundApp.Tests.Components.Pages.Chat;

public class ChatPageUITests : PageTest
{
    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        await Page.GotoAsync(TestConstants.LocalhostUrl);
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
    }

    [Trait("Category", "IntegrationTest")]
    [Fact]
    public async Task Given_Root_Page_When_Loaded_Then_NoMessagesContent_Should_Be_Visible()
    {
        // Arrange
        var noMessages = Page.Locator(".no-messages");

        // Act
        var isVisible = await noMessages.IsVisibleAsync();
        var text = isVisible ? await noMessages.InnerTextAsync() : string.Empty;

        // Assert
        isVisible.ShouldBeTrue();
        text.ShouldBe("To get started, try asking about anything.");
    }

    [Trait("Category", "IntegrationTest")]
    [Fact]
    public async Task Given_Root_Page_When_Loaded_Then_PageTitle_Should_Be_Set()
    {
        // Arrange
        // (no specific arrangement required)

        // Act
        var title = await Page.TitleAsync();

        // Assert
        title.ShouldBe("OpenChat Playground");
    }

    [Trait("Category", "IntegrationTest")]
    [Fact]
    public async Task Given_NewChat_Clicked_Then_Input_Textarea_Should_Be_Focused()
    {
        // Arrange
        var newChatButton = Page.GetByRole(AriaRole.Button, new() { Name = "New chat" });
        var textArea = Page.GetByRole(AriaRole.Textbox, new() { Name = "User Message Textarea" });

        // Act
        await newChatButton.ClickAsync();
        var isFocused = await textArea.EvaluateAsync<bool>("el => el === document.activeElement");

        // Assert
        isFocused.ShouldBeTrue();
    }

    public override async Task DisposeAsync()
    {
        await Page.CloseAsync();
        await base.DisposeAsync();
    }
}
