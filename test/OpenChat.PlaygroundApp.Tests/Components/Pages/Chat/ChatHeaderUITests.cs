using Microsoft.Extensions.Configuration;
using Microsoft.Playwright;
using Microsoft.Playwright.Xunit;

using OpenChat.PlaygroundApp.Configurations;
using OpenChat.PlaygroundApp.Connectors;

namespace OpenChat.PlaygroundApp.Tests.Components.Pages.Chat;


public class ChatHeaderUITests : PageTest
{
    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        await Page.GotoAsync(TestConstants.LocalhostUrl);
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
    }

    [Trait("Category", "IntegrationTest")]
    [Theory]
    [InlineData("OpenChat Playground")]
    public async Task Given_Root_Page_When_Loaded_Then_Header_Should_Be_Visible(string expected)
    {
        // Act
        var title = await Page.Locator("span.app-title-text").InnerTextAsync();

        // Assert
        title.ShouldBe(expected);
    }

    [Trait("Category", "IntegrationTest")]
    [Fact]
    public async Task Given_Root_Page_When_Loaded_Then_Header_Should_Display_ConnectorType_And_Model()
    {
        // Act
        var connector = await Page.Locator("span.app-connector").InnerTextAsync();
        var model = await Page.Locator("span.app-model").InnerTextAsync();

        // Assert
        connector.ShouldNotBeNullOrEmpty();
        Enum.IsDefined(typeof(ConnectorType), connector).ShouldBeTrue();
        model.ShouldNotBeNullOrEmpty();
    }

    [Trait("Category", "IntegrationTest")]
    [Fact]
    public async Task Given_Root_Page_When_Loaded_Then_NewChat_Button_Should_Be_Visible()
    {
        // Arrange
        var newChatButton = Page.GetByRole(AriaRole.Button, new() { Name = "New chat" });

        // Assert
        var isVisible = await newChatButton.IsVisibleAsync();
        isVisible.ShouldBeTrue();
    }

    [Trait("Category", "IntegrationTest")]
    [Fact]
    public async Task Given_Header_When_Loaded_Then_NewChat_Icon_Should_Be_Visible()
    {
        // Arrange
        var icon = Page.Locator("button svg.new-chat-icon");

        // Assert
        var isVisible = await icon.IsVisibleAsync();
        isVisible.ShouldBeTrue();
    }

    [Trait("Category", "IntegrationTest")]
    [Trait("Category", "LLMRequired")]
    [Theory]
    [InlineData("1+1의 결과는 무엇인가요?")]
    [InlineData("what is the result of 1 + 1?")]
    public async Task Given_UserAndAssistantMessages_When_NewChat_Clicked_Then_Conversation_Should_Reset(string userMessage)
    {
        // Arrange
        var textArea = Page.GetByRole(AriaRole.Textbox, new() { Name = "User Message Textarea" });
        var sendButton = Page.GetByRole(AriaRole.Button, new() { Name = "User Message Send Button" });
        var newChatButton = Page.GetByRole(AriaRole.Button, new() { Name = "New chat" });

        var loadingSpinner = Page.Locator(".lds-ellipsis");
        var userMessages = Page.Locator(".user-message");
        var assistantMessages = Page.Locator(".assistant-message-header");
        var noMessagesPlaceholder = Page.Locator(".no-messages");

        // Act
        await textArea.FillAsync(userMessage);
        await sendButton.ClickAsync();
        await newChatButton.ClickAsync();
        await noMessagesPlaceholder.WaitForAsync();

        // Assert
        var userMessageCount = await userMessages.CountAsync();
        userMessageCount.ShouldBe(0);

        var assistantMessageCount = await assistantMessages.CountAsync();
        assistantMessageCount.ShouldBe(0);

        var placeholderVisible = await noMessagesPlaceholder.IsVisibleAsync();
        placeholderVisible.ShouldBeTrue();

        var spinnerVisible = await loadingSpinner.IsVisibleAsync();
        spinnerVisible.ShouldBeFalse();
    }

    public override async Task DisposeAsync()
    {
        await Page.CloseAsync();
        await base.DisposeAsync();
    }
}
