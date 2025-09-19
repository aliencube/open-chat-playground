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
        var textAreaAfter = await textArea.InnerTextAsync();
        textAreaAfter.ShouldBeEmpty();
        var messageCountAfter = await Page.Locator(".assistant-message-header").CountAsync();
        messageCountAfter.ShouldBe(messageCountBefore + expectedMessageCount);
    }

    [Trait("Category", "IntegrationTest")]
    [Theory]
    [InlineData("", 0)]
    //[InlineData(" ", 0)]
    //[InlineData("\n", 0)]
    //[InlineData("\r\n", 0)]
    public async Task Given_Empty_UserMessage_When_SendButton_Clicked_Then_It_Should_Not_SendMessage(string userMessage, int expectedMessageCount)
    {
        // Arrange
        var textArea = Page.GetByRole(AriaRole.Textbox, new() { Name = "User Message Textarea" });
        var sendButton = Page.GetByRole(AriaRole.Button, new() { Name = "User Message Send Button" });
        var messageCountBefore = await Page.Locator(".assistant-message-header").CountAsync();

        // Act
        await textArea.FillAsync(userMessage);
        await sendButton.ClickAsync();

        // Assert
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
        var textAreaAfter = await textArea.InnerTextAsync();
        textAreaAfter.ShouldBeEmpty();
        var messageCountAfter = await Page.Locator(".assistant-message-header").CountAsync();
        messageCountAfter.ShouldBe(messageCountBefore + expectedMessageCount);
    }

    [Trait("Category", "IntegrationTest")]
    [Theory]
    [InlineData("", 0)]
    //[InlineData(" ", 0)]
    //[InlineData("\n", 0)]
    //[InlineData("\r\n", 0)]
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
    //[InlineData(" ", "rgb(170, 170, 170)")]
    //[InlineData("\n", "rgb(170, 170, 170)")]
    //[InlineData("\r\n", "rgb(170, 170, 170)")]
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
    [Trait("Category", "LLMRequired")]
    [Theory]
    [InlineData("하늘은 왜 푸른 색인가요?")]
    [InlineData("Why is the sky blue?")]
    public async Task Given_UserMessage_When_SendButton_Clicked_Then_TextArea_Should_Remain_Focused(string userMessage)
    {
        // Arrange
        var textArea = Page.GetByRole(AriaRole.Textbox, new() { Name = "User Message Textarea" });
        var sendButton = Page.GetByRole(AriaRole.Button, new() { Name = "User Message Send Button" });

        // Act
        await textArea.FillAsync(userMessage);
        await sendButton.ClickAsync();

        // Assert
        var isFocused = await textArea.EvaluateAsync<bool>("el => document.activeElement === el");
        isFocused.ShouldBeTrue();
    }

    public override async Task DisposeAsync()
    {
        await Page.CloseAsync();
        await base.DisposeAsync();
    }
}
