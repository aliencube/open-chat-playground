using Microsoft.Extensions.Configuration;
using OpenChat.PlaygroundApp.Abstractions;
using OpenChat.PlaygroundApp.Connectors;

namespace OpenChat.PlaygroundApp.Tests.Options;

public class OpenAIArgumentOptionsTests
{
    private static IConfiguration BuildConfig(string? apiKey = null, string? model = null, ConnectorType connectorType = ConnectorType.OpenAI)
    {
        var dict = new Dictionary<string, string?>
        {
            ["ConnectorType"] = connectorType.ToString(),
            ["OpenAI:ApiKey"] = apiKey,
            ["OpenAI:Model"] = model,
        }!;

        return new ConfigurationBuilder().AddInMemoryCollection(dict!).Build();
    }

    [Trait("Category", "UnitTest")]
    [Fact]
    public void Given_ConfigOnly_When_Parse_Invoked_Then_It_Should_Populate_Settings()
    {
        var config = BuildConfig(apiKey: "cfg-key", model: "gpt-4.1-mini");
        var settings = ArgumentOptions.Parse(config, []);

        settings.OpenAI.ShouldNotBeNull();
        settings.OpenAI.ApiKey.ShouldBe("cfg-key");
        settings.OpenAI.Model.ShouldBe("gpt-4.1-mini");
    }

    [Trait("Category", "UnitTest")]
    [Fact]
    public void Given_CLI_Overrides_When_Parse_Invoked_Then_CLI_Should_Win()
    {
        var config = BuildConfig(apiKey: "cfg-key", model: "cfg-model");
        var args = new [] { "--connector-type", "OpenAI", "--api-key", "cli-key", "--model", "cli-model" };

        var settings = ArgumentOptions.Parse(config, args);

        settings.OpenAI.ShouldNotBeNull();
        settings.OpenAI.ApiKey.ShouldBe("cli-key");
        settings.OpenAI.Model.ShouldBe("cli-model");
    }

    [Trait("Category", "UnitTest")]
    [Fact]
    public void Given_Missing_ConnectorType_When_Parse_Invoked_Then_Help_Should_Be_True()
    {
        var cfg = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string,string?>()).Build();
        var settings = ArgumentOptions.Parse(cfg, []);

        settings.Help.ShouldBeTrue();
        settings.ConnectorType.ShouldBe(ConnectorType.Unknown);
    }

    [Trait("Category", "UnitTest")]
    [Fact]
    public void Given_Invalid_OpenAI_Argument_When_Parse_Invoked_Then_Help_Should_Be_True()
    {
        var config = BuildConfig(apiKey: "cfg-key", model: "gpt-4.1-mini");
        var args = new [] { "--connector-type", "OpenAI", "--wrong-option" };

        var settings = ArgumentOptions.Parse(config, args);

        settings.Help.ShouldBeTrue();
    }

    [Trait("Category", "UnitTest")]
    [Fact]
    public void Given_Help_Flag_When_Parse_Invoked_Then_Help_Should_Be_True()
    {
        var config = BuildConfig(apiKey: "cfg-key", model: "gpt-4.1-mini");
        var args = new [] { "--connector-type", "OpenAI", "--help" };

        var settings = ArgumentOptions.Parse(config, args);

        settings.Help.ShouldBeTrue();
    }
}
