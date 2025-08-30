using Microsoft.Extensions.Configuration;

using OpenChat.PlaygroundApp.Abstractions;
using OpenChat.PlaygroundApp.Connectors;

namespace OpenChat.PlaygroundApp.Tests.Abstractions;

public class ArgumentOptionsTests
{
    private static IConfiguration BuildConfig(params (string Key, string Value)[] pairs)
    {
        var dict = pairs.ToDictionary(p => p.Key, p => (string?)p.Value);
        var config = new ConfigurationBuilder()
                         .AddInMemoryCollection(dict!)
                         .Build();

        return config;
    }

    [Trait("Category", "UnitTest")]
    [Fact]
    public void Given_Empty_ConnectorType_When_VerifyConnectorType_Invoked_Then_It_Should_Return_Unknown()
    {
        var config = BuildConfig();
        var args = Array.Empty<string>();

        var result = ArgumentOptions.VerifyConnectorType(config, args);

        result.ShouldBe(ConnectorType.Unknown);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("ConnectorType", "AmazonBedrock", ConnectorType.AmazonBedrock)]
    [InlineData("ConnectorType", "AzureAIFoundry", ConnectorType.AzureAIFoundry)]
    [InlineData("ConnectorType", "FoundryLocal", ConnectorType.FoundryLocal)]
    [InlineData("ConnectorType", "GitHubModels", ConnectorType.GitHubModels)]
    [InlineData("ConnectorType", "OpenAI", ConnectorType.OpenAI)]
    public void Given_ConnectorType_When_VerifyConnectorType_Invoked_Then_It_Should_Return_Result(string key, string value, ConnectorType expected)
    {
        var config = BuildConfig((key, value));
        var args = Array.Empty<string>();

        var result = ArgumentOptions.VerifyConnectorType(config, args);

        result.ShouldBe(expected);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("ConnectorType", "AmazonBedrock", "--connector-type", "OpenAI", ConnectorType.OpenAI)]
    [InlineData("ConnectorType", "AzureAIFoundry", "--connector-type", "AmazonBedrock", ConnectorType.AmazonBedrock)]
    [InlineData("ConnectorType", "FoundryLocal", "--connector-type", "AzureAIFoundry", ConnectorType.AzureAIFoundry)]
    [InlineData("ConnectorType", "GitHubModels", "--connector-type", "OpenAI", ConnectorType.OpenAI)]
    [InlineData("ConnectorType", "OpenAI", "--connector-type", "FoundryLocal", ConnectorType.FoundryLocal)]
    [InlineData("ConnectorType", "AmazonBedrock", "-c", "OpenAI", ConnectorType.OpenAI)]
    [InlineData("ConnectorType", "AzureAIFoundry", "-c", "AmazonBedrock", ConnectorType.AmazonBedrock)]
    [InlineData("ConnectorType", "FoundryLocal", "-c", "AzureAIFoundry", ConnectorType.AzureAIFoundry)]
    [InlineData("ConnectorType", "GitHubModels", "-c", "OpenAI", ConnectorType.OpenAI)]
    [InlineData("ConnectorType", "OpenAI", "-c", "FoundryLocal", ConnectorType.FoundryLocal)]
    public void Given_ConnectorType_And_Argument_When_VerifyConnectorType_Invoked_Then_It_Should_Return_Result(string key, string value, string argument1, string argument2, ConnectorType expected)
    {
        var config = BuildConfig((key, value));
        var args = new[] { argument1, argument2 };

        var result = ArgumentOptions.VerifyConnectorType(config, args);

        result.ShouldBe(expected);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("ConnectorType", "AmazonBedrock", "--connector-type", "Kimchi", ConnectorType.AmazonBedrock)]
    [InlineData("ConnectorType", "AzureAIFoundry", "--connector-type", "Bulgogi", ConnectorType.AzureAIFoundry)]
    [InlineData("ConnectorType", "AmazonBedrock", "-c", "Kimchi", ConnectorType.AmazonBedrock)]
    [InlineData("ConnectorType", "AzureAIFoundry", "-c", "Bulgogi", ConnectorType.AzureAIFoundry)]
    public void Given_ConnectorType_And_InvalidArgument_When_VerifyConnectorType_Invoked_Then_It_Should_Return_Result(string key, string value, string argument1, string argument2, ConnectorType expected)
    {
        var config = BuildConfig((key, value));
        var args = new[] { argument1, argument2 };

        var result = ArgumentOptions.VerifyConnectorType(config, args);

        result.ShouldBe(expected);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("ConnectorType", "AmazonBedrock", "Kimchi", ConnectorType.AmazonBedrock)]
    [InlineData("ConnectorType", "AzureAIFoundry", "Bulgogi", ConnectorType.AzureAIFoundry)]
    public void Given_ConnectorType_And_UnrelatedArgument_When_VerifyConnectorType_Invoked_Then_It_Should_Return_Result(string key, string value, string argument, ConnectorType expected)
    {
        var config = BuildConfig((key, value));
        var args = new[] { "--something-else", argument };

        var result = ArgumentOptions.VerifyConnectorType(config, args);

        result.ShouldBe(expected);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("ConnectorType", "Naver")]
    public void Given_Unimplemented_ConnectorType_When_Parse_Invoked_Then_It_Should_Throw(string key, string value)
    {
        var config = BuildConfig((key, value));
        var args = Array.Empty<string>();

        var ex = Assert.Throws<InvalidOperationException>(() => ArgumentOptions.Parse(config, args));

        ex.Message.ShouldContain($"{value}ArgumentOptions");
    }

    [Trait("Category", "UnitTest")]
    [Fact]
    public void Given_Empty_ConnectorType_When_Parse_Invoked_Then_It_Should_Return_Unknown()
    {
        var config = BuildConfig();
        var args = Array.Empty<string>();

        var settings = ArgumentOptions.Parse(config, args);

        settings.ShouldNotBeNull();
        settings.ConnectorType.ShouldBe(ConnectorType.Unknown);
        settings.Help.ShouldBeTrue();
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("ConnectorType", "AmazonBedrock", ConnectorType.AmazonBedrock)]
    [InlineData("ConnectorType", "AzureAIFoundry", ConnectorType.AzureAIFoundry)]
    [InlineData("ConnectorType", "FoundryLocal", ConnectorType.FoundryLocal)]
    [InlineData("ConnectorType", "GitHubModels", ConnectorType.GitHubModels)]
    [InlineData("ConnectorType", "OpenAI", ConnectorType.OpenAI)]
    public void Given_ConnectorType_When_Parse_Invoked_Then_It_Should_Return_Result(string key, string value, ConnectorType expected)
    {
        var config = BuildConfig((key, value));
        var args = Array.Empty<string>();

        var settings = ArgumentOptions.Parse(config, args);

        settings.ShouldNotBeNull();
        settings.ConnectorType.ShouldBe(expected);
        settings.Help.ShouldBeFalse();
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("ConnectorType", "AmazonBedrock", "OpenAI", ConnectorType.OpenAI)]
    [InlineData("ConnectorType", "AzureAIFoundry", "AmazonBedrock", ConnectorType.AmazonBedrock)]
    [InlineData("ConnectorType", "FoundryLocal", "GitHubModels", ConnectorType.GitHubModels)]
    [InlineData("ConnectorType", "GitHubModels", "OpenAI", ConnectorType.OpenAI)]
    [InlineData("ConnectorType", "OpenAI", "FoundryLocal", ConnectorType.FoundryLocal)]
    public void Given_ConnectorType_And_Argument_When_Parse_Invoked_Then_It_Should_Return_Result(string key, string value, string argument, ConnectorType expected)
    {
        var config = BuildConfig((key, value));
        var args = new[] { "--connector-type", argument };

        var settings = ArgumentOptions.Parse(config, args);

        settings.ShouldNotBeNull();
        settings.ConnectorType.ShouldBe(expected);
        settings.Help.ShouldBeFalse();
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("ConnectorType", "AmazonBedrock", "Kimchi", ConnectorType.AmazonBedrock)]
    [InlineData("ConnectorType", "AzureAIFoundry", "Bulgogi", ConnectorType.AzureAIFoundry)]
    public void Given_ConnectorType_And_UnrelatedArgument_When_Parse_Invoked_Then_It_Should_Return_Result(string key, string value, string argument, ConnectorType expected)
    {
        var config = BuildConfig((key, value));
        var args = new[] { "--something-else", argument };

        var settings = ArgumentOptions.Parse(config, args);

        settings.ShouldNotBeNull();
        settings.ConnectorType.ShouldBe(expected);
        settings.Help.ShouldBeTrue();
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("--help", true)]
    [InlineData("-h", true)]
    [InlineData("--unknown", true)]
    public void Given_Help_When_Parse_Invoked_Then_It_Should_Return_Help(string argument, bool expected)
    {
        var config = BuildConfig(("ConnectorType", "OpenAI"));
        var args = new[] { argument };

        var settings = ArgumentOptions.Parse(config, args);

        settings.Help.ShouldBe(expected);
    }
}
