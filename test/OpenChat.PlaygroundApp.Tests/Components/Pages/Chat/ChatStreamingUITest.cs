using Microsoft.Playwright;
using Microsoft.Playwright.Xunit;

namespace OpenChat.PlaygroundApp.Tests.Components.Pages.Chat;

public class ChatStreamingUITest : PageTest
{
    private const int TimeoutMs = 60000;

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        await Page.GotoAsync(TestConstants.LocalhostUrl);
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
    }

    [Trait("Category", "IntegrationTest")]
    [Trait("Category", "UI")]
    [Trait("Category", "LLMRequired")]
    [Theory]
    [InlineData("Why is the sky blue? Please explain in five paragraphs.")]
    public async Task Given_UserMessage_When_SendButton_Clicked_Then_Response_Should_Stream_Progressively(string userMessage)
    {
        // Arrange
        var messageSelector = ".assistant-message-text";

        var textArea = Page.GetByRole(AriaRole.Textbox, new() { Name = "User Message Textarea" });
        var sendButton = Page.GetByRole(AriaRole.Button, new() { Name = "User Message Send Button" });

        // Act
        await textArea.FillAsync(userMessage);
        await sendButton.ClickAsync();

        var message = Page.Locator(messageSelector);

        // Assert
        await Expect(message).ToBeVisibleAsync(new() { Timeout = TimeoutMs });
        await Expect(message).Not.ToHaveTextAsync(string.Empty, new() { Timeout = TimeoutMs });

        var initialContent = await message.InnerTextAsync(new() { Timeout = TimeoutMs });

        await Expect(message).Not.ToHaveTextAsync(initialContent, new() { Timeout = TimeoutMs });

        var finalContent = await message.InnerTextAsync(new() { Timeout = TimeoutMs });
        
        finalContent.ShouldNotBe(initialContent);
        finalContent.ShouldStartWith(initialContent); 
        finalContent.Length.ShouldBeGreaterThan(initialContent.Length); 
    }

    [Trait("Category", "IntegrationTest")]
    [Trait("Category", "UI")]
    [Trait("Category", "LLMRequired")]
    [Theory]
    [InlineData("Why is the sky blue?")]
    public async Task Given_UserMessage_When_SendButton_Clicked_Then_LoadingSpinner_Should_Be_Visible_Before_Text_Arrives(string userMessage)
    {
        // Arrange
        var spinnerSelector = ".lds-ellipsis";

        var textArea = Page.GetByRole(AriaRole.Textbox, new() { Name = "User Message Textarea" });
        var sendButton = Page.GetByRole(AriaRole.Button, new() { Name = "User Message Send Button" });
        var loadingSpinner = Page.Locator(spinnerSelector);

        // Act
        await textArea.FillAsync(userMessage);
        await sendButton.ClickAsync();

        // Assert
        await Page.WaitForSelectorAsync(spinnerSelector, new() { State = WaitForSelectorState.Visible, Timeout = TimeoutMs });

        var spinnerVisible = await loadingSpinner.IsVisibleAsync();
        spinnerVisible.ShouldBeTrue();
    }

    [Trait("Category", "IntegrationTest")]
    [Trait("Category", "UI")]
    [Trait("Category", "LLMRequired")]
    [Theory]
    [InlineData("Why is the sky blue?")]
    public async Task Given_UserMessage_When_Response_Text_Arrives_Then_LoadingSpinner_Should_Disappear(string userMessage)
    {
        // Arrange
        const string spinnerSelector = ".lds-ellipsis";
        const string messageSelector = ".assistant-message-text";

        var textArea = Page.GetByRole(AriaRole.Textbox, new() { Name = "User Message Textarea" });
        var sendButton = Page.GetByRole(AriaRole.Button, new() { Name = "User Message Send Button" });
        var loadingSpinner = Page.Locator(spinnerSelector);
        var message = Page.Locator(messageSelector);

        // Act
        await textArea.FillAsync(userMessage);
        await sendButton.ClickAsync();

        // Assert
        var messageContent = await message.InnerTextAsync(new() { Timeout = TimeoutMs });
        messageContent.ShouldNotBeEmpty();

        var spinnerVisible = await loadingSpinner.IsVisibleAsync();
        spinnerVisible.ShouldBeFalse();
    }

    public override async Task DisposeAsync()
    {
        await Page.CloseAsync();
        await base.DisposeAsync();
    }
}
