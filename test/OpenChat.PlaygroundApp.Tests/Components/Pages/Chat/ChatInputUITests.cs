using Microsoft.Playwright;
using Microsoft.Playwright.Xunit;

namespace OpenChat.PlaygroundApp.Tests.Components.Pages.Chat;

public class ChatInputUITest : PageTest
{
    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        await Page.GotoAsync(TestConstants.LocalhostUrl);
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
    }

    [Trait("Category", "IntegrationTest")]
    [Fact]
    public async Task Given_Root_Page_When_Loaded_Then_Textarea_Should_Have_Correct_Placeholder()
    {
        // Arrange
        var textArea = Page.GetByRole(AriaRole.Textbox, new() { Name = "User Message Textarea" });

        // Act
        var placeholder = await textArea.GetAttributeAsync("placeholder");

        // Assert
        placeholder.ShouldBe("Type your message...");
    }

    [Trait("Category", "IntegrationTest")]
    [Fact]
    public async Task Given_Root_Page_When_Loaded_Then_SendButton_Should_Have_Correct_Title()
    {
        // Arrange
        var sendButton = Page.GetByRole(AriaRole.Button, new() { Name = "User Message Send Button" });

        // Act
        var title = await sendButton.GetAttributeAsync("title");

        // Assert
        title.ShouldBe("Send");
    }

    [Trait("Category", "IntegrationTest")]
    [Fact]
    public async Task Given_Root_Page_When_Loaded_Then_SendButton_Icon_Should_Be_Visible()
    {
        // Arrange
        var sendButtonIcon = Page.Locator("button[aria-label='User Message Send Button'] svg");

        // Act
        var isVisible = await sendButtonIcon.IsVisibleAsync();

        // Assert
        isVisible.ShouldBeTrue();
    }

    [Trait("Category", "IntegrationTest")]
    [Theory]
    [InlineData("Input usermessage")]
    public async Task Given_UserMessage_When_Tab_Pressed_Then_Focus_Should_Move_To_SendButton(string userMessage)
    {
        // Arrange
        var textArea = Page.GetByRole(AriaRole.Textbox, new() { Name = "User Message Textarea" });
        var sendButton = Page.GetByRole(AriaRole.Button, new() { Name = "User Message Send Button" });

        // Act
        await textArea.FocusAsync();
        await textArea.FillAsync(userMessage);
        await textArea.PressAsync("Tab");

        // Assert
        var isFocused = await sendButton.EvaluateAsync<bool>("el => document.activeElement === el");
        isFocused.ShouldBeTrue();
    }

    [Trait("Category", "IntegrationTest")]
    [Trait("Category", "LLMRequired")]
    [Theory]
    [InlineData("하늘은 왜 푸른 색인가요?", 1)]
    [InlineData("Why is the sky blue?", 1)]
    public async Task Given_UserMessage_When_SendButton_Clicked_Then_It_Should_SendMessage(string userMessage, int expectedMessageCount)
    {
        // Arrange
        var textArea = Page.GetByRole(AriaRole.Textbox, new() { Name = "User Message Textarea" });
        var sendButton = Page.GetByRole(AriaRole.Button, new() { Name = "User Message Send Button" });
        var messageCountBefore = await Page.Locator(".assistant-message-header").CountAsync();

        // Act
        await textArea.FillAsync(userMessage);
        await sendButton.ClickAsync();

        // Assert
        // Wait until an assistant message appears
        await Page.WaitForFunctionAsync(
            "args => document.querySelectorAll(args.selector).length >= args.expected",
            new { selector = ".assistant-message-header", expected = messageCountBefore + expectedMessageCount }
        );
        var textAreaAfter = await textArea.InnerTextAsync();
        textAreaAfter.ShouldBeEmpty();
        var messageCountAfter = await Page.Locator(".assistant-message-header").CountAsync();
        messageCountAfter.ShouldBe(messageCountBefore + expectedMessageCount);
    }

    [Trait("Category", "IntegrationTest")]
    [Theory]
    [InlineData("", 0)]
    [InlineData(" ", 0)]
    [InlineData("\n", 0)]
    [InlineData("\r\n", 0)]
    public async Task Given_Empty_UserMessage_When_SendButton_Clicked_Then_It_Should_Not_SendMessage(string userMessage, int expectedMessageCount)
    {
        // Arrange
        var textArea = Page.GetByRole(AriaRole.Textbox, new() { Name = "User Message Textarea" });
        var sendButton = Page.GetByRole(AriaRole.Button, new() { Name = "User Message Send Button" });
        var messageCountBefore = await Page.Locator(".assistant-message-header").CountAsync();

        // Act
        await textArea.FillAsync(userMessage);

        // Assert: button should be disabled for empty/whitespace input
        var isDisabled = (await sendButton.GetAttributeAsync("disabled")) is not null;
        isDisabled.ShouldBeTrue();
        var messageCountAfter = await Page.Locator(".assistant-message-header").CountAsync();
        messageCountAfter.ShouldBe(messageCountBefore + expectedMessageCount);
    }

    [Trait("Category", "IntegrationTest")]
    [Trait("Category", "LLMRequired")]
    [Theory]
    [InlineData("하늘은 왜 푸른 색인가요?", 1)]
    [InlineData("Why is the sky blue?", 1)]
    public async Task Given_UserMessage_When_EnterKey_Pressed_Then_It_Should_SendMessage(string userMessage, int expectedMessageCount)
    {
        // Arrange
        var textArea = Page.GetByRole(AriaRole.Textbox, new() { Name = "User Message Textarea" });
        var messageCountBefore = await Page.Locator(".assistant-message-header").CountAsync();

        // Act
        await textArea.FillAsync(userMessage);
        await textArea.PressAsync("Enter");

        // Assert
        // Wait until an assistant message appears
        await Page.WaitForFunctionAsync(
            "args => document.querySelectorAll(args.selector).length >= args.expected",
            new { selector = ".assistant-message-header", expected = messageCountBefore + expectedMessageCount }
        );
        var textAreaAfter = await textArea.InnerTextAsync();
        textAreaAfter.ShouldBeEmpty();
        var messageCountAfter = await Page.Locator(".assistant-message-header").CountAsync();
        messageCountAfter.ShouldBe(messageCountBefore + expectedMessageCount);
    }

    [Trait("Category", "IntegrationTest")]
    [Theory]
    [InlineData("", 0)]
    [InlineData(" ", 0)]
    [InlineData("\n", 0)]
    [InlineData("\r\n", 0)]
    public async Task Given_Empty_UserMessage_When_EnterKey_Pressed_Then_It_Should_Not_SendMessage(string userMessage, int expectedMessageCount)
    {
        // Arrange
        var textArea = Page.GetByRole(AriaRole.Textbox, new() { Name = "User Message Textarea" });
        var messageCountBefore = await Page.Locator(".assistant-message-header").CountAsync();

        // Act
        await textArea.FillAsync(userMessage);
        await textArea.PressAsync("Enter");

        // Assert
        var messageCountAfter = await Page.Locator(".assistant-message-header").CountAsync();
        messageCountAfter.ShouldBe(messageCountBefore + expectedMessageCount);
    }

    [Trait("Category", "IntegrationTest")]
    [Theory]
    [InlineData("하늘은 왜 푸를까?", "rgb(0, 0, 0)")]
    [InlineData("Why is the sky blue?", "rgb(0, 0, 0)")]
    public async Task Given_UserMessage_When_TextArea_FilledIn_Then_It_Should_Change_Color_Of_SendButton(string userMessage, string expectedButtonColor)
    {
        // Arrange
        var textArea = Page.GetByRole(AriaRole.Textbox, new() { Name = "User Message Textarea" });
        var sendButton = Page.GetByRole(AriaRole.Button, new() { Name = "User Message Send Button" });

        // Act
        await textArea.FillAsync(userMessage);

        // Assert
        var sendButtonColor = await sendButton.EvaluateAsync<string>("el => window.getComputedStyle(el).color");
        sendButtonColor.ShouldBe(expectedButtonColor);
    }

    [Trait("Category", "IntegrationTest")]
    [Theory]
    [InlineData("", "rgb(170, 170, 170)")]
    [InlineData(" ", "rgb(170, 170, 170)")]
    [InlineData("\n", "rgb(170, 170, 170)")]
    [InlineData("\r\n", "rgb(170, 170, 170)")]
    public async Task Given_Empty_UserMessage_Then_It_Should_Not_Change_Color_Of_SendButton(string userMessage, string expectedButtonColor)
    {
        // Arrange
        var textArea = Page.GetByRole(AriaRole.Textbox, new() { Name = "User Message Textarea" });
        var sendButton = Page.GetByRole(AriaRole.Button, new() { Name = "User Message Send Button" });

        // Act
        await textArea.FillAsync(userMessage);

        // Assert
        var sendButtonColor = await sendButton.EvaluateAsync<string>("el => window.getComputedStyle(el).color");
        sendButtonColor.ShouldBe(expectedButtonColor);
    }

    [Trait("Category", "IntegrationTest")]
    [Theory]
    [InlineData("Line 1\nLine 2\nLine 3\nLine 4\nLine 5")]
    public async Task Given_Textarea_When_MultipleLines_Added_Then_Height_Should_Adjust(string multilineText)
    {
        // Arrange
        var textArea = Page.GetByRole(AriaRole.Textbox, new() { Name = "User Message Textarea" });

        // Act
        var initialHeight = await textArea.EvaluateAsync<int>("el => el.scrollHeight");
        await textArea.FillAsync(multilineText);
        await textArea.DispatchEventAsync("input");
        
        // Assert
        var finalHeight = await textArea.EvaluateAsync<int>("el => el.scrollHeight");
        finalHeight.ShouldBeGreaterThan(initialHeight);
    }

    [Trait("Category", "IntegrationTest")]
    [Theory]
    [InlineData("Line 1\nLine 2\nLine 3\nLine 4\nLine 5", 5)]
    public async Task Given_Multiline_UserMessage_When_Filled_Then_TextArea_Should_Contain_Correct_LineCount(string userMessage, int expectedLineCount)
    {
        // Arrange
        var textArea = Page.GetByRole(AriaRole.Textbox, new() { Name = "User Message Textarea" });

        // Act
        await textArea.FillAsync(userMessage);

        // Assert
        var textAreaContent = await textArea.InputValueAsync();
        var actualLineCount = textAreaContent.Split('\n').Length;
        actualLineCount.ShouldBe(expectedLineCount);
    }

    public override async Task DisposeAsync()
    {
        await Page.CloseAsync();
        await base.DisposeAsync();
    }
}
