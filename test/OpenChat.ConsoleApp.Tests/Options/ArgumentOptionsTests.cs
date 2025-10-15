using System.Text;

using Microsoft.Extensions.Configuration;

using OpenChat.ConsoleApp.Options;

namespace OpenChat.ConsoleApp.Tests.Options;

public class ArgumentOptionsTests
{
    private static IConfiguration BuildConfig(string? endpoint = null)
    {
        var values = new Dictionary<string, string?>
        {
            ["ApiApp:Endpoint"] = endpoint
        }!;

        return new ConfigurationBuilder()
            .AddInMemoryCollection(values!)
            .Build();
    }

    [Trait("Category", "UnitTest")]
    [Fact]
    public void Given_EndpointArgument_When_Parse_Then_Should_Set_Endpoint_And_Help_False()
    {
        // Arrange
        var config = BuildConfig(endpoint: "http://default");
        var expected = "http://localhost:1234";
        var args = new[] { "--endpoint", expected };

        // Act
        var result = ArgumentOptions.Parse(config, args);

        // Assert
        result.ShouldNotBeNull();
        result.ApiApp.Endpoint.ShouldBe(expected);
        result.Help.ShouldBeFalse();
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("--help")]
    [InlineData("-h")]
    public void Given_HelpArguments_When_Parse_Then_Should_Set_Help_True(string arg)
    {
        // Arrange
        var config = BuildConfig();
        var args = new[] { arg };

        // Act
        var result = ArgumentOptions.Parse(config, args);

        // Assert
        result.Help.ShouldBeTrue();
    }

    [Trait("Category", "UnitTest")]
    [Fact]
    public void Given_UnknownArgument_When_Parse_Then_Should_Set_Help_True()
    {
        // Arrange
        var config = BuildConfig();
        var args = new[] { "--unknown" };

        // Act
        var result = ArgumentOptions.Parse(config, args);

        // Assert
        result.Help.ShouldBeTrue();
    }

    [Trait("Category", "UnitTest")]
    [Fact]
    public void Given_EndpointArgumentWithoutValue_When_Parse_Then_Should_NotThrow_And_Help_False()
    {
        // Arrange
        var config = BuildConfig();
        var args = new[] { "--endpoint" }; // Missing value intentionally.

        // Act
        var result = ArgumentOptions.Parse(config, args);

        // Assert
        result.Help.ShouldBeFalse();
        result.ApiApp.Endpoint.ShouldBeNull();
    }

    [Trait("Category", "UnitTest")]
    [Fact]
    public void Given_DisplayHelp_When_Called_Then_Should_Write_Expected_Lines()
    {
        // Arrange
        var output = new StringBuilder();
        using var writer = new StringWriter(output);
        var originalOut = Console.Out;
        Console.SetOut(writer);

        try
        {
            // Act
            ArgumentOptions.DisplayHelp();
        }
        finally
        {
            Console.SetOut(originalOut);
        }

        // Assert
        var text = output.ToString();
        text.ShouldContain("OpenChat Playground");
        text.ShouldContain("--endpoint");
        text.ShouldContain("--help");
    }
}
