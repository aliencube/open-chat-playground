using Microsoft.Extensions.Configuration;

using OpenChat.PlaygroundApp.Abstractions;
using OpenChat.PlaygroundApp.Connectors;
using OpenChat.PlaygroundApp.Constants;
using OpenChat.PlaygroundApp.Options;

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
    [InlineData("ConnectorType", "GitHubModels", ConnectorType.GitHubModels)]
    [InlineData("ConnectorType", "GoogleVertexAI", ConnectorType.GoogleVertexAI)]
    [InlineData("ConnectorType", "DockerModelRunner", ConnectorType.DockerModelRunner)]
    [InlineData("ConnectorType", "FoundryLocal", ConnectorType.FoundryLocal)]
    [InlineData("ConnectorType", "HuggingFace", ConnectorType.HuggingFace)]
    [InlineData("ConnectorType", "Ollama", ConnectorType.Ollama)]
    [InlineData("ConnectorType", "Anthropic", ConnectorType.Anthropic)]
    [InlineData("ConnectorType", "LG", ConnectorType.LG)]
    [InlineData("ConnectorType", "Naver", ConnectorType.Naver)]
    [InlineData("ConnectorType", "OpenAI", ConnectorType.OpenAI)]
    [InlineData("ConnectorType", "Upstage", ConnectorType.Upstage)]
    public void Given_ConnectorType_When_VerifyConnectorType_Invoked_Then_It_Should_Return_Result(string key, string value, ConnectorType expected)
    {
        var config = BuildConfig((key, value));
        var args = Array.Empty<string>();

        var result = ArgumentOptions.VerifyConnectorType(config, args);

        result.ShouldBe(expected);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("ConnectorType", "AmazonBedrock", CommandLineArguments.ConnectorType, "Upstage", ConnectorType.Upstage)]
    [InlineData("ConnectorType", "AzureAIFoundry", CommandLineArguments.ConnectorType, "OpenAI", ConnectorType.OpenAI)]
    [InlineData("ConnectorType", "GitHubModels", CommandLineArguments.ConnectorType, "Naver", ConnectorType.Naver)]
    [InlineData("ConnectorType", "GoogleVertexAI", CommandLineArguments.ConnectorType, "LG", ConnectorType.LG)]
    [InlineData("ConnectorType", "DockerModelRunner", CommandLineArguments.ConnectorType, "Anthropic", ConnectorType.Anthropic)]
    [InlineData("ConnectorType", "FoundryLocal", CommandLineArguments.ConnectorType, "Ollama", ConnectorType.Ollama)]
    [InlineData("ConnectorType", "HuggingFace", CommandLineArguments.ConnectorType, "Ollama", ConnectorType.Ollama)]
    [InlineData("ConnectorType", "Ollama", CommandLineArguments.ConnectorType, "FoundryLocal", ConnectorType.FoundryLocal)]
    [InlineData("ConnectorType", "Anthropic", CommandLineArguments.ConnectorType, "DockerModelRunner", ConnectorType.DockerModelRunner)]
    [InlineData("ConnectorType", "LG", CommandLineArguments.ConnectorType, "GoogleVertexAI", ConnectorType.GoogleVertexAI)]
    [InlineData("ConnectorType", "Naver", CommandLineArguments.ConnectorType, "GitHubModels", ConnectorType.GitHubModels)]
    [InlineData("ConnectorType", "OpenAI", CommandLineArguments.ConnectorType, "AzureAIFoundry", ConnectorType.AzureAIFoundry)]
    [InlineData("ConnectorType", "Upstage", CommandLineArguments.ConnectorType, "AmazonBedrock", ConnectorType.AmazonBedrock)]
    [InlineData("ConnectorType", "AmazonBedrock", CommandLineArguments.ConnectorTypeShort, "Upstage", ConnectorType.Upstage)]
    [InlineData("ConnectorType", "AzureAIFoundry", CommandLineArguments.ConnectorTypeShort, "OpenAI", ConnectorType.OpenAI)]
    [InlineData("ConnectorType", "GitHubModels", CommandLineArguments.ConnectorTypeShort, "Naver", ConnectorType.Naver)]
    [InlineData("ConnectorType", "GoogleVertexAI", CommandLineArguments.ConnectorTypeShort, "LG", ConnectorType.LG)]
    [InlineData("ConnectorType", "DockerModelRunner", CommandLineArguments.ConnectorTypeShort, "Anthropic", ConnectorType.Anthropic)]
    [InlineData("ConnectorType", "FoundryLocal", CommandLineArguments.ConnectorTypeShort, "Ollama", ConnectorType.Ollama)]
    [InlineData("ConnectorType", "HuggingFace", CommandLineArguments.ConnectorTypeShort, "Ollama", ConnectorType.Ollama)]
    [InlineData("ConnectorType", "Ollama", CommandLineArguments.ConnectorTypeShort, "FoundryLocal", ConnectorType.FoundryLocal)]
    [InlineData("ConnectorType", "Anthropic", CommandLineArguments.ConnectorTypeShort, "DockerModelRunner", ConnectorType.DockerModelRunner)]
    [InlineData("ConnectorType", "LG", CommandLineArguments.ConnectorTypeShort, "GoogleVertexAI", ConnectorType.GoogleVertexAI)]
    [InlineData("ConnectorType", "Naver", CommandLineArguments.ConnectorTypeShort, "GitHubModels", ConnectorType.GitHubModels)]
    [InlineData("ConnectorType", "OpenAI", CommandLineArguments.ConnectorTypeShort, "AzureAIFoundry", ConnectorType.AzureAIFoundry)]
    [InlineData("ConnectorType", "Upstage", CommandLineArguments.ConnectorTypeShort, "AmazonBedrock", ConnectorType.AmazonBedrock)]
    public void Given_ConnectorType_And_Argument_When_VerifyConnectorType_Invoked_Then_It_Should_Return_Result(string key, string value, string argument1, string argument2, ConnectorType expected)
    {
        var config = BuildConfig((key, value));
        var args = new[] { argument1, argument2 };

        var result = ArgumentOptions.VerifyConnectorType(config, args);

        result.ShouldBe(expected);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("ConnectorType", "AmazonBedrock", CommandLineArguments.ConnectorType, "MaaS", ConnectorType.AmazonBedrock)]
    [InlineData("ConnectorType", "AzureAIFoundry", CommandLineArguments.ConnectorType, "MaaS", ConnectorType.AzureAIFoundry)]
    [InlineData("ConnectorType", "GitHubModels", CommandLineArguments.ConnectorType, "MaaS", ConnectorType.GitHubModels)]
    [InlineData("ConnectorType", "GoogleVertexAI", CommandLineArguments.ConnectorType, "MaaS", ConnectorType.GoogleVertexAI)]
    [InlineData("ConnectorType", "DockerModelRunner", CommandLineArguments.ConnectorType, "Local", ConnectorType.DockerModelRunner)]
    [InlineData("ConnectorType", "FoundryLocal", CommandLineArguments.ConnectorType, "Local", ConnectorType.FoundryLocal)]
    [InlineData("ConnectorType", "HuggingFace", CommandLineArguments.ConnectorType, "Local", ConnectorType.HuggingFace)]
    [InlineData("ConnectorType", "Ollama", CommandLineArguments.ConnectorType, "Local", ConnectorType.Ollama)]
    [InlineData("ConnectorType", "Anthropic", CommandLineArguments.ConnectorType, "Vendor", ConnectorType.Anthropic)]
    [InlineData("ConnectorType", "LG", CommandLineArguments.ConnectorType, "Vendor", ConnectorType.LG)]
    [InlineData("ConnectorType", "Naver", CommandLineArguments.ConnectorType, "Vendor", ConnectorType.Naver)]
    [InlineData("ConnectorType", "OpenAI", CommandLineArguments.ConnectorType, "Vendor", ConnectorType.OpenAI)]
    [InlineData("ConnectorType", "Upstage", CommandLineArguments.ConnectorType, "Vendor", ConnectorType.Upstage)]
    [InlineData("ConnectorType", "AmazonBedrock", CommandLineArguments.ConnectorTypeShort, "MaaS", ConnectorType.AmazonBedrock)]
    [InlineData("ConnectorType", "AzureAIFoundry", CommandLineArguments.ConnectorTypeShort, "MaaS", ConnectorType.AzureAIFoundry)]
    [InlineData("ConnectorType", "GitHubModels", CommandLineArguments.ConnectorTypeShort, "MaaS", ConnectorType.GitHubModels)]
    [InlineData("ConnectorType", "GoogleVertexAI", CommandLineArguments.ConnectorTypeShort, "MaaS", ConnectorType.GoogleVertexAI)]
    [InlineData("ConnectorType", "DockerModelRunner", CommandLineArguments.ConnectorTypeShort, "Local", ConnectorType.DockerModelRunner)]
    [InlineData("ConnectorType", "FoundryLocal", CommandLineArguments.ConnectorTypeShort, "Local", ConnectorType.FoundryLocal)]
    [InlineData("ConnectorType", "HuggingFace", CommandLineArguments.ConnectorTypeShort, "Local", ConnectorType.HuggingFace)]
    [InlineData("ConnectorType", "Ollama", CommandLineArguments.ConnectorTypeShort, "Local", ConnectorType.Ollama)]
    [InlineData("ConnectorType", "Anthropic", CommandLineArguments.ConnectorTypeShort, "Vendor", ConnectorType.Anthropic)]
    [InlineData("ConnectorType", "LG", CommandLineArguments.ConnectorTypeShort, "Vendor", ConnectorType.LG)]
    [InlineData("ConnectorType", "Naver", CommandLineArguments.ConnectorTypeShort, "Vendor", ConnectorType.Naver)]
    [InlineData("ConnectorType", "OpenAI", CommandLineArguments.ConnectorTypeShort, "Vendor", ConnectorType.OpenAI)]
    [InlineData("ConnectorType", "Upstage", CommandLineArguments.ConnectorTypeShort, "Vendor", ConnectorType.Upstage)]
    public void Given_ConnectorType_And_InvalidArgument_When_VerifyConnectorType_Invoked_Then_It_Should_Return_Result(string key, string value, string argument1, string argument2, ConnectorType expected)
    {
        var config = BuildConfig((key, value));
        var args = new[] { argument1, argument2 };

        var result = ArgumentOptions.VerifyConnectorType(config, args);

        result.ShouldBe(expected);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("ConnectorType", "AmazonBedrock", "MaaS", ConnectorType.AmazonBedrock)]
    [InlineData("ConnectorType", "AzureAIFoundry", "MaaS", ConnectorType.AzureAIFoundry)]
    [InlineData("ConnectorType", "GitHubModels", "MaaS", ConnectorType.GitHubModels)]
    [InlineData("ConnectorType", "GoogleVertexAI", "MaaS", ConnectorType.GoogleVertexAI)]
    [InlineData("ConnectorType", "DockerModelRunner", "Local", ConnectorType.DockerModelRunner)]
    [InlineData("ConnectorType", "FoundryLocal", "Local", ConnectorType.FoundryLocal)]
    [InlineData("ConnectorType", "HuggingFace", "Local", ConnectorType.HuggingFace)]
    [InlineData("ConnectorType", "Ollama", "Local", ConnectorType.Ollama)]
    [InlineData("ConnectorType", "Anthropic", "Vendor", ConnectorType.Anthropic)]
    [InlineData("ConnectorType", "LG", "Vendor", ConnectorType.LG)]
    [InlineData("ConnectorType", "Naver", "Vendor", ConnectorType.Naver)]
    [InlineData("ConnectorType", "OpenAI", "Vendor", ConnectorType.OpenAI)]
    [InlineData("ConnectorType", "Upstage", "Vendor", ConnectorType.Upstage)]
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
    [InlineData("ConnectorType", "GitHubModels", ConnectorType.GitHubModels)]
    [InlineData("ConnectorType", "GoogleVertexAI", ConnectorType.GoogleVertexAI)]
    // [InlineData("ConnectorType", "DockerModelRunner", ConnectorType.DockerModelRunner)]
    [InlineData("ConnectorType", "FoundryLocal", ConnectorType.FoundryLocal)]
    [InlineData("ConnectorType", "HuggingFace", ConnectorType.HuggingFace)]
    [InlineData("ConnectorType", "Ollama", ConnectorType.Ollama)]
    [InlineData("ConnectorType", "Anthropic", ConnectorType.Anthropic)]
    [InlineData("ConnectorType", "LG", ConnectorType.LG)]
    // [InlineData("ConnectorType", "Naver", ConnectorType.Naver)]
    [InlineData("ConnectorType", "OpenAI", ConnectorType.OpenAI)]
    [InlineData("ConnectorType", "Upstage", ConnectorType.Upstage)]
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
    [InlineData("ConnectorType", "AmazonBedrock", "Upstage", ConnectorType.Upstage)]
    [InlineData("ConnectorType", "AzureAIFoundry", "OpenAI", ConnectorType.OpenAI)]
    // [InlineData("ConnectorType", "GitHubModels", "Naver", ConnectorType.Naver)]
    [InlineData("ConnectorType", "GoogleVertexAI", "LG", ConnectorType.LG)]
    // [InlineData("ConnectorType", "DockerModelRunner", "Anthropic", ConnectorType.Anthropic)]
    [InlineData("ConnectorType", "FoundryLocal", "Ollama", ConnectorType.Ollama)]
    [InlineData("ConnectorType", "HuggingFace", "Ollama", ConnectorType.Ollama)]
    [InlineData("ConnectorType", "Ollama", "FoundryLocal", ConnectorType.FoundryLocal)]
    // [InlineData("ConnectorType", "Anthropic", "DockerModelRunner", ConnectorType.DockerModelRunner)]
    [InlineData("ConnectorType", "LG", "GoogleVertexAI", ConnectorType.GoogleVertexAI)]
    [InlineData("ConnectorType", "Naver", "GitHubModels", ConnectorType.GitHubModels)]
    [InlineData("ConnectorType", "OpenAI", "AzureAIFoundry", ConnectorType.AzureAIFoundry)]
    [InlineData("ConnectorType", "Upstage", "AmazonBedrock", ConnectorType.AmazonBedrock)]
    public void Given_ConnectorType_And_Argument_When_Parse_Invoked_Then_It_Should_Return_Result(string key, string value, string argument, ConnectorType expected)
    {
        var config = BuildConfig((key, value));
        var args = new[] { ArgumentOptionConstants.ConnectorType, argument };

        var settings = ArgumentOptions.Parse(config, args);

        settings.ShouldNotBeNull();
        settings.ConnectorType.ShouldBe(expected);
        settings.Help.ShouldBeFalse();
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("ConnectorType", "AmazonBedrock", "MaaS", ConnectorType.AmazonBedrock)]
    [InlineData("ConnectorType", "AzureAIFoundry", "MaaS", ConnectorType.AzureAIFoundry)]
    [InlineData("ConnectorType", "GitHubModels", "MaaS", ConnectorType.GitHubModels)]
    [InlineData("ConnectorType", "GoogleVertexAI", "MaaS", ConnectorType.GoogleVertexAI)]
    // [InlineData("ConnectorType", "DockerModelRunner", "Local", ConnectorType.DockerModelRunner)]
    [InlineData("ConnectorType", "FoundryLocal", "Local", ConnectorType.FoundryLocal)]
    [InlineData("ConnectorType", "HuggingFace", "Local", ConnectorType.HuggingFace)]
    [InlineData("ConnectorType", "Ollama", "Local", ConnectorType.Ollama)]
    [InlineData("ConnectorType", "Anthropic", "Vendor", ConnectorType.Anthropic)]
    [InlineData("ConnectorType", "LG", "Vendor", ConnectorType.LG)]
    // [InlineData("ConnectorType", "Naver", "Vendor", ConnectorType.Naver)]
    [InlineData("ConnectorType", "OpenAI", "Vendor", ConnectorType.OpenAI)]
    [InlineData("ConnectorType", "Upstage", "Vendor", ConnectorType.Upstage)]
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
    [InlineData(CommandLineArguments.Help, true)]
    [InlineData(CommandLineArguments.HelpShort, true)]
    [InlineData("--unknown", true)]
    public void Given_Help_When_Parse_Invoked_Then_It_Should_Return_Help(string argument, bool expected)
    {
        var config = BuildConfig(("ConnectorType", ConnectorType.GitHubModels.ToString()));
        var args = new[] { argument };

        var settings = ArgumentOptions.Parse(config, args);

        settings.Help.ShouldBe(expected);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData(typeof(AmazonBedrockArgumentOptions))]
    [InlineData(typeof(AzureAIFoundryArgumentOptions))]
    [InlineData(typeof(GitHubModelsArgumentOptions))]
    [InlineData(typeof(GoogleVertexAIArgumentOptions))]
    // [InlineData(typeof(DockerModelRunnerArgumentOptions))]
    [InlineData(typeof(FoundryLocalArgumentOptions))]
    [InlineData(typeof(HuggingFaceArgumentOptions))]
    [InlineData(typeof(OllamaArgumentOptions))]
    [InlineData(typeof(AnthropicArgumentOptions))]
    [InlineData(typeof(LGArgumentOptions))]
    // [InlineData(typeof(NaverArgumentOptions))]
    [InlineData(typeof(OpenAIArgumentOptions))]
    [InlineData(typeof(UpstageArgumentOptions))]
    public void Given_Concrete_ArgumentOptions_When_Checking_Inheritance_Then_Should_Inherit_From_ArgumentOptions(Type type)
    {
        // Act
        var isSubclass = type.IsSubclassOf(typeof(ArgumentOptions));

        // Assert
        isSubclass.ShouldBeTrue();
    }
}