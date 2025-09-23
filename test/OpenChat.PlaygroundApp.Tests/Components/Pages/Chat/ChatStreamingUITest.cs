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
        const int timeoutMs = 5000;

        const string messageSelector = ".assistant-message-text";

        var textArea = Page.GetByRole(AriaRole.Textbox, new() { Name = "User Message Textarea" });
        var sendButton = Page.GetByRole(AriaRole.Button, new() { Name = "User Message Send Button" });
        var message = Page.Locator(messageSelector);

        // Act
        await textArea.FillAsync(userMessage);
        await sendButton.ClickAsync();

        // Assert
        await Expect(message).ToBeVisibleAsync(new() { Timeout = timeoutMs });
        await Expect(message).Not.ToHaveTextAsync(string.Empty, new() { Timeout = timeoutMs });

        var initialContent = await message.InnerTextAsync();

        await Expect(message).Not.ToHaveTextAsync(initialContent, new() { Timeout = timeoutMs });

        var finalContent = await message.InnerTextAsync();
        
        finalContent.ShouldNotBe(initialContent);
        finalContent.ShouldStartWith(initialContent); 
        finalContent.Length.ShouldBeGreaterThan(initialContent.Length); 
    }

    [Trait("Category", "IntegrationTest")]
    [Trait("Category", "LLMRequired")]
    [Theory]
    [InlineData("하늘은 왜 푸른 색인가요?")]
    [InlineData("Why is the sky blue?")]
    public async Task Given_UserMessage_When_SendButton_Clicked_Then_LoadingSpinner_Should_Be_Visible_Before_Text_Arrives(string userMessage)
    {
        // Arrange
        const int timeoutMs = 5000;

        const string spinnerSelector = ".lds-ellipsis";

        var textArea = Page.GetByRole(AriaRole.Textbox, new() { Name = "User Message Textarea" });
        var sendButton = Page.GetByRole(AriaRole.Button, new() { Name = "User Message Send Button" });
        var spinner = Page.Locator(spinnerSelector);

        // Act
        await textArea.FillAsync(userMessage);
        await sendButton.ClickAsync();

        // Assert
        await Expect(spinner).ToBeVisibleAsync(new() { Timeout = timeoutMs });
        
        var spinnerWasVisible = await spinner.IsVisibleAsync();
        
        spinnerWasVisible.ShouldBeTrue();
    }

    [Trait("Category", "IntegrationTest")]
    [Trait("Category", "LLMRequired")]
    [Theory]
    [InlineData("하늘은 왜 푸른 색인가요?")]
    [InlineData("Why is the sky blue?")]
    public async Task Given_UserMessage_When_Response_Text_Arrives_Then_LoadingSpinner_Should_Disappear(string userMessage)
    {
        // Arrange
        const int timeoutMs = 5000;

        const string spinnerSelector = ".lds-ellipsis";
        const string messageSelector = ".assistant-message-text";

        var textArea = Page.GetByRole(AriaRole.Textbox, new() { Name = "User Message Textarea" });
        var sendButton = Page.GetByRole(AriaRole.Button, new() { Name = "User Message Send Button" });
        var spinner = Page.Locator(spinnerSelector);
        var message = Page.Locator(messageSelector);

        // Act
        await textArea.FillAsync(userMessage);
        await sendButton.ClickAsync();

        // Assert
        await Expect(message).Not.ToHaveTextAsync(string.Empty, new() { Timeout = timeoutMs }); 

        var spinnerWasVisible = await spinner.IsVisibleAsync();

        spinnerWasVisible.ShouldBeFalse();
    }

    public override async Task DisposeAsync()
    {
        await Page.CloseAsync();
        await base.DisposeAsync();
    }
}
