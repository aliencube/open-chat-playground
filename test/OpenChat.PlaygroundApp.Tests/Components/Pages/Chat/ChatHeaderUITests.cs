using Microsoft.Playwright.Xunit;

namespace OpenChat.PlaygroundApp.Tests.Components.Pages.Chat;

public class ChatHeaderUITests : PageTest
{
    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        await Page.GotoAsync("http://localhost:8080");
    }

    [Fact]
    public async Task Given_Root_Page_When_Loaded_Then_Header_Is_Visible()
    {
        // Act
        var title = await Page.Locator("h1").InnerTextAsync();

        // Assert
        title.ShouldBe("OpenChat.PlaygroundApp");
    }

    public override async Task DisposeAsync()
    {
        await Page.CloseAsync();
    }
}
