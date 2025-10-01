using System;
using System.Diagnostics;
using Microsoft.Playwright;
using Microsoft.Playwright.Xunit;

namespace OpenChat.PlaygroundApp.Tests.Components.Pages.Chat;

public class ChatMessageItemUITests : PageTest
{
    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        await Page.GotoAsync(TestConstants.LocalhostUrl);
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
    }

    [Trait("Category", "IntegrationTest")]
    [Theory]
    [InlineData("Input usermessage")]
    public async Task Given_UserMessage_When_Sent_Then_UserMessage_Count_Should_Increment(string userMessage)
    {
        // Arrange
        var textArea = Page.GetByRole(AriaRole.Textbox, new() { Name = "User Message Textarea" });
        var userMessages = Page.Locator(".user-message");
        var initialCount = await userMessages.CountAsync();

        // Act
        await textArea.FillAsync(userMessage);
        await textArea.PressAsync("Enter");
        var newUserMessage = userMessages.Nth(initialCount);
        await newUserMessage.WaitForAsync(new() { State = WaitForSelectorState.Attached });

        // Assert
        var finalCount = await userMessages.CountAsync();
        finalCount.ShouldBe(initialCount + 1);
    }

    [Trait("Category", "IntegrationTest")]
    [Theory]
    [InlineData("Input usermessage")]
    public async Task Given_UserMessage_When_Sent_Then_Rendered_Text_Should_Match_UserMessage(string userMessage)
    {
        // Arrange
        var textArea = Page.GetByRole(AriaRole.Textbox, new() { Name = "User Message Textarea" });
        var userMessages = Page.Locator(".user-message");
        var initialCount = await userMessages.CountAsync();

        // Act
        await textArea.FillAsync(userMessage);
        await textArea.PressAsync("Enter");
        var newUserMessage = userMessages.Nth(initialCount);
        await newUserMessage.WaitForAsync(new() { State = WaitForSelectorState.Attached });

        // Assert
        var latestUserMessage = userMessages.Nth(initialCount);
        var renderedText = await latestUserMessage.InnerTextAsync();
        renderedText.ShouldBe(userMessage);
    }

    [Trait("Category", "IntegrationTest")]
    [Trait("Category", "LLMRequired")]
    [Theory]
    [InlineData("Input usermessage")]
    public async Task Given_AssistantResponse_When_Streamed_Then_Latest_Text_Should_NotBeEmpty(string userMessage)
    {
        // Arrange
        var textArea = Page.GetByRole(AriaRole.Textbox, new() { Name = "User Message Textarea" });
        var assistantTexts = Page.Locator(".assistant-message-text");
        var initialTextCount = await assistantTexts.CountAsync();

        // Act
        await textArea.FillAsync(userMessage);
        await textArea.PressAsync("Enter");
        var newAssistantText = assistantTexts.Nth(initialTextCount);
        await newAssistantText.WaitForAsync(new() { State = WaitForSelectorState.Attached });
        // Assert
        var finalContent = (await newAssistantText.InnerTextAsync())?.Trim();
        finalContent.ShouldNotBeNullOrWhiteSpace();
    }

    [Trait("Category", "IntegrationTest")]
    [Trait("Category", "LLMRequired")]
    [Theory]
    [InlineData("Input usermessage")]
    public async Task Given_AssistantResponse_When_Message_Arrives_Then_Assistant_Icon_Should_Be_Visible(string userMessage)
    {
        // Arrange
        var textArea = Page.GetByRole(AriaRole.Textbox, new() { Name = "User Message Textarea" });
        var assistantHeaders = Page.Locator(".assistant-message-header");
        var assistantIcons = Page.Locator(".assistant-message-icon svg");
        var initialHeaderCount = await assistantHeaders.CountAsync();

        // Act
        await textArea.FillAsync(userMessage);
        await textArea.PressAsync("Enter");
        var newAssistantHeader = assistantHeaders.Nth(initialHeaderCount);
        await newAssistantHeader.WaitForAsync(new() { State = WaitForSelectorState.Attached });
    
        // Assert
        var finalIconCount = await assistantIcons.CountAsync();
        var iconIndex = finalIconCount - 1;
        var iconVisible = await assistantIcons.Nth(iconIndex).IsVisibleAsync();
        iconVisible.ShouldBeTrue();
    }

    [Trait("Category", "IntegrationTest")]
    [Trait("Category", "LLMRequired")]
    [Theory]
    [InlineData("Input usermessage")]
    public async Task Given_Response_InProgress_When_Stream_Completes_Then_Spinner_Should_Toggle(string userMessage)
    {
        // Arrange
        var textArea = Page.GetByRole(AriaRole.Textbox, new() { Name = "User Message Textarea" });
        var spinner = Page.Locator(".lds-ellipsis");

        // Act
        await textArea.FillAsync(userMessage);
        await textArea.PressAsync("Enter");
        await spinner.WaitForAsync(new() { State = WaitForSelectorState.Visible });
        var visibleWhileStreaming = await spinner.IsVisibleAsync();
        await spinner.WaitForAsync(new() { State = WaitForSelectorState.Hidden });
        var visibleAfterComplete = await spinner.IsVisibleAsync();

        // Assert
        visibleWhileStreaming.ShouldBeTrue();
        visibleAfterComplete.ShouldBeFalse();
    }

    public override async Task DisposeAsync()
    {
        await Page.CloseAsync();
        await base.DisposeAsync();
    }
}