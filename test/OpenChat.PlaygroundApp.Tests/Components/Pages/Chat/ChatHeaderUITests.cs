using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;

using NUnit.Framework;

namespace OpenChat.PlaygroundApp.Tests.Components.Pages.Chat;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class ChatHeaderUITests : PageTest
{
    public override BrowserNewContextOptions ContextOptions() => new()
    {
        IgnoreHTTPSErrors = true,
    };

    [SetUp]
    public async Task Init()
    {
        await Page.GotoAsync("https://localhost:5001");
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
    }

    [Test]
    public async Task Given_Root_Page_When_Loaded_Then_Header_Is_Visible()
    {
        // Act
        var title = await Page.Locator("h1").InnerTextAsync();

        // Assert
        title.ShouldBe("OpenChat.PlaygroundApp");
    }

    [TearDown]
    public async Task CleanUp()
    {
        await Page.CloseAsync();
    }
}

