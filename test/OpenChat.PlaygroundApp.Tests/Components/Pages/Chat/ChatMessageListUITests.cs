using Microsoft.Playwright;
using Microsoft.Playwright.Xunit;

namespace OpenChat.PlaygroundApp.Tests.Components.Pages.Chat;

public class ChatMessageListUITests : PageTest
{
    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        await Page.GotoAsync(TestConstants.LocalhostUrl);
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
    }

    [Trait("Category", "IntegrationTest")]
    [Theory]
    [InlineData("To get started, try asking about anything.")]
    public async Task Given_EmptyState_When_Page_Loads_Then_NoMessages_Content_Should_Be_Visible(string expectedText)
    {
        // Act
        var noMessagesElement = Page.Locator(".no-messages");
        
        // Assert
        var isVisible = await noMessagesElement.IsVisibleAsync();
        isVisible.ShouldBeTrue();
        
        var content = await noMessagesElement.InnerTextAsync();
        content.ShouldContain(expectedText);
    }

    [Trait("Category", "IntegrationTest")]
    [Theory]
    [InlineData("Hello, how are you?")]
    public async Task Given_MessagesExist_When_Rendered_Then_MessageList_Should_Display_Messages(string userMessage)
    {
        // Arrange
        var textArea = Page.GetByRole(AriaRole.Textbox, new() { Name = "User Message Textarea" });
        await textArea.FillAsync(userMessage);
        await textArea.PressAsync("Enter");
        await Page.Locator(".user-message").First.WaitForAsync(new() { State = WaitForSelectorState.Attached });
        
        // Act
        var messageListContainer = Page.Locator(".message-list-container");
        var messageListContent = await messageListContainer.InnerTextAsync();
        
        // Assert
        messageListContent.ShouldContain(userMessage);
    }

    [Trait("Category", "IntegrationTest")]
    [Fact]
    public async Task Given_InitialState_When_Page_Loads_Then_InProgress_Attribute_Should_Be_Null()
    {
        // Act - Check initial state before any user interaction
        var chatMessages = Page.Locator("chat-messages");
        var inProgressAttribute = await chatMessages.GetAttributeAsync("in-progress");
        
        // Assert - Initially no InProgressMessage, so attribute should be null
        inProgressAttribute.ShouldBeNull();
    }

    [Trait("Category", "IntegrationTest")]
    [Theory]
    [InlineData("Test message for container")]
    [InlineData("Another message for verification")]
    public async Task Given_MessageInput_When_TextEntered_Then_Container_Should_Remain_Functional(string userMessage)
    {
        // Arrange
        var textArea = Page.GetByRole(AriaRole.Textbox, new() { Name = "User Message Textarea" });
        var messageContainer = Page.Locator(".message-list-container");
        
        // Act - Test basic text input without expecting successful message sending
        await textArea.FillAsync(userMessage);
        
        // Verify container structure remains intact
        var containerExists = await messageContainer.IsVisibleAsync();
        var inputValue = await textArea.InputValueAsync();
        
        // Assert - Verify ChatMessageList container is functional and input works
        containerExists.ShouldBeTrue();
        inputValue.ShouldBe(userMessage);
    }

    [Trait("Category", "IntegrationTest")]
    [Theory]
    [InlineData(".message-list-container")]
    [InlineData("chat-messages")]
    public async Task Given_ChatMessageList_When_Rendered_Then_Should_Display_Container_Structure(string containerSelector)
    {
        // Act
        var container = Page.Locator(containerSelector);
        var containerExists = await container.IsVisibleAsync();
        
        // Assert
        containerExists.ShouldBeTrue();
    }

    [Trait("Category", "IntegrationTest")]
    [Theory]
    [InlineData("To get started, try asking about anything.")]
    [InlineData("User Message Textarea")]
    public async Task Given_EmptyState_When_NoMessages_Then_Expected_Elements_Should_Be_Visible(string expectedElement)
    {
        // Act - Test the empty state rendering of ChatMessageList
        if (expectedElement == "To get started, try asking about anything.")
        {
            var noMessagesElement = Page.Locator(".no-messages");
            var isVisible = await noMessagesElement.IsVisibleAsync();
            var content = await noMessagesElement.InnerTextAsync();
            
            // Assert - Verify NoMessagesContent is properly rendered
            isVisible.ShouldBeTrue();
            content.ShouldContain(expectedElement);
        }
        else if (expectedElement == "User Message Textarea")
        {
            // Test input field exists
            var textArea = Page.GetByRole(AriaRole.Textbox, new() { Name = expectedElement });
            var inputExists = await textArea.IsVisibleAsync();
            inputExists.ShouldBeTrue();
        }
    }

    public override async Task DisposeAsync()
    {
        await Page.CloseAsync();
        await base.DisposeAsync();
    }
}