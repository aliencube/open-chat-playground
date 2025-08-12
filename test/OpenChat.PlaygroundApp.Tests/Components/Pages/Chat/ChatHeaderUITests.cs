using Microsoft.Playwright;
using Microsoft.Playwright.Xunit;

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
    [InlineData("OpenChat.PlaygroundApp")]
    public async Task Given_Root_Page_When_Loaded_Then_Header_Should_Be_Visible(string expected)
    {
        // Act
        var title = await Page.Locator("h1").InnerTextAsync();

        // Assert
        title.ShouldBe(expected);
    }

    public override async Task DisposeAsync()
    {
        await Page.CloseAsync();
        await base.DisposeAsync();
    }
}
