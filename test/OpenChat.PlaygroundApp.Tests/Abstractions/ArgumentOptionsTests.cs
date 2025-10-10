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
    [InlineData(AppSettingConstants.ConnectorType, "AmazonBedrock", ConnectorType.AmazonBedrock)]
    [InlineData(AppSettingConstants.ConnectorType, "AzureAIFoundry", ConnectorType.AzureAIFoundry)]
    [InlineData(AppSettingConstants.ConnectorType, "GitHubModels", ConnectorType.GitHubModels)]
    [InlineData(AppSettingConstants.ConnectorType, "GoogleVertexAI", ConnectorType.GoogleVertexAI)]
    [InlineData(AppSettingConstants.ConnectorType, "DockerModelRunner", ConnectorType.DockerModelRunner)]
    [InlineData(AppSettingConstants.ConnectorType, "FoundryLocal", ConnectorType.FoundryLocal)]
    [InlineData(AppSettingConstants.ConnectorType, "HuggingFace", ConnectorType.HuggingFace)]
    [InlineData(AppSettingConstants.ConnectorType, "Ollama", ConnectorType.Ollama)]
    [InlineData(AppSettingConstants.ConnectorType, "Anthropic", ConnectorType.Anthropic)]
    [InlineData(AppSettingConstants.ConnectorType, "LG", ConnectorType.LG)]
    [InlineData(AppSettingConstants.ConnectorType, "Naver", ConnectorType.Naver)]
    [InlineData(AppSettingConstants.ConnectorType, "OpenAI", ConnectorType.OpenAI)]
    [InlineData(AppSettingConstants.ConnectorType, "Upstage", ConnectorType.Upstage)]
    public void Given_ConnectorType_When_VerifyConnectorType_Invoked_Then_It_Should_Return_Result(string key, string value, ConnectorType expected)
    {
        var config = BuildConfig((key, value));
        var args = Array.Empty<string>();

        var result = ArgumentOptions.VerifyConnectorType(config, args);

        result.ShouldBe(expected);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData(AppSettingConstants.ConnectorType, "AmazonBedrock", ArgumentOptionConstants.ConnectorType, "Upstage", ConnectorType.Upstage)]
    [InlineData(AppSettingConstants.ConnectorType, "AzureAIFoundry", ArgumentOptionConstants.ConnectorType, "OpenAI", ConnectorType.OpenAI)]
    [InlineData(AppSettingConstants.ConnectorType, "GitHubModels", ArgumentOptionConstants.ConnectorType, "Naver", ConnectorType.Naver)]
    [InlineData(AppSettingConstants.ConnectorType, "GoogleVertexAI", ArgumentOptionConstants.ConnectorType, "LG", ConnectorType.LG)]
    [InlineData(AppSettingConstants.ConnectorType, "DockerModelRunner", ArgumentOptionConstants.ConnectorType, "Anthropic", ConnectorType.Anthropic)]
    [InlineData(AppSettingConstants.ConnectorType, "FoundryLocal", ArgumentOptionConstants.ConnectorType, "Ollama", ConnectorType.Ollama)]
    [InlineData(AppSettingConstants.ConnectorType, "HuggingFace", ArgumentOptionConstants.ConnectorType, "Ollama", ConnectorType.Ollama)]
    [InlineData(AppSettingConstants.ConnectorType, "Ollama", ArgumentOptionConstants.ConnectorType, "FoundryLocal", ConnectorType.FoundryLocal)]
    [InlineData(AppSettingConstants.ConnectorType, "Anthropic", ArgumentOptionConstants.ConnectorType, "DockerModelRunner", ConnectorType.DockerModelRunner)]
    [InlineData(AppSettingConstants.ConnectorType, "LG", ArgumentOptionConstants.ConnectorType, "GoogleVertexAI", ConnectorType.GoogleVertexAI)]
    [InlineData(AppSettingConstants.ConnectorType, "Naver", ArgumentOptionConstants.ConnectorType, "GitHubModels", ConnectorType.GitHubModels)]
    [InlineData(AppSettingConstants.ConnectorType, "OpenAI", ArgumentOptionConstants.ConnectorType, "AzureAIFoundry", ConnectorType.AzureAIFoundry)]
    [InlineData(AppSettingConstants.ConnectorType, "Upstage", ArgumentOptionConstants.ConnectorType, "AmazonBedrock", ConnectorType.AmazonBedrock)]
    [InlineData(AppSettingConstants.ConnectorType, "AmazonBedrock", ArgumentOptionConstants.ConnectorTypeInShort, "Upstage", ConnectorType.Upstage)]
    [InlineData(AppSettingConstants.ConnectorType, "AzureAIFoundry", ArgumentOptionConstants.ConnectorTypeInShort, "OpenAI", ConnectorType.OpenAI)]
    [InlineData(AppSettingConstants.ConnectorType, "GitHubModels", ArgumentOptionConstants.ConnectorTypeInShort, "Naver", ConnectorType.Naver)]
    [InlineData(AppSettingConstants.ConnectorType, "GoogleVertexAI", ArgumentOptionConstants.ConnectorTypeInShort, "LG", ConnectorType.LG)]
    [InlineData(AppSettingConstants.ConnectorType, "DockerModelRunner", ArgumentOptionConstants.ConnectorTypeInShort, "Anthropic", ConnectorType.Anthropic)]
    [InlineData(AppSettingConstants.ConnectorType, "FoundryLocal", ArgumentOptionConstants.ConnectorTypeInShort, "Ollama", ConnectorType.Ollama)]
    [InlineData(AppSettingConstants.ConnectorType, "HuggingFace", ArgumentOptionConstants.ConnectorTypeInShort, "Ollama", ConnectorType.Ollama)]
    [InlineData(AppSettingConstants.ConnectorType, "Ollama", ArgumentOptionConstants.ConnectorTypeInShort, "FoundryLocal", ConnectorType.FoundryLocal)]
    [InlineData(AppSettingConstants.ConnectorType, "Anthropic", ArgumentOptionConstants.ConnectorTypeInShort, "DockerModelRunner", ConnectorType.DockerModelRunner)]
    [InlineData(AppSettingConstants.ConnectorType, "LG", ArgumentOptionConstants.ConnectorTypeInShort, "GoogleVertexAI", ConnectorType.GoogleVertexAI)]
    [InlineData(AppSettingConstants.ConnectorType, "Naver", ArgumentOptionConstants.ConnectorTypeInShort, "GitHubModels", ConnectorType.GitHubModels)]
    [InlineData(AppSettingConstants.ConnectorType, "OpenAI", ArgumentOptionConstants.ConnectorTypeInShort, "AzureAIFoundry", ConnectorType.AzureAIFoundry)]
    [InlineData(AppSettingConstants.ConnectorType, "Upstage", ArgumentOptionConstants.ConnectorTypeInShort, "AmazonBedrock", ConnectorType.AmazonBedrock)]
    public void Given_ConnectorType_And_Argument_When_VerifyConnectorType_Invoked_Then_It_Should_Return_Result(string key, string value, string argument1, string argument2, ConnectorType expected)
    {
        var config = BuildConfig((key, value));
        var args = new[] { argument1, argument2 };

        var result = ArgumentOptions.VerifyConnectorType(config, args);

        result.ShouldBe(expected);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData(AppSettingConstants.ConnectorType, "AmazonBedrock", ArgumentOptionConstants.ConnectorType, "MaaS", ConnectorType.AmazonBedrock)]
    [InlineData(AppSettingConstants.ConnectorType, "AzureAIFoundry", ArgumentOptionConstants.ConnectorType, "MaaS", ConnectorType.AzureAIFoundry)]
    [InlineData(AppSettingConstants.ConnectorType, "GitHubModels", ArgumentOptionConstants.ConnectorType, "MaaS", ConnectorType.GitHubModels)]
    [InlineData(AppSettingConstants.ConnectorType, "GoogleVertexAI", ArgumentOptionConstants.ConnectorType, "MaaS", ConnectorType.GoogleVertexAI)]
    [InlineData(AppSettingConstants.ConnectorType, "DockerModelRunner", ArgumentOptionConstants.ConnectorType, "Local", ConnectorType.DockerModelRunner)]
    [InlineData(AppSettingConstants.ConnectorType, "FoundryLocal", ArgumentOptionConstants.ConnectorType, "Local", ConnectorType.FoundryLocal)]
    [InlineData(AppSettingConstants.ConnectorType, "HuggingFace", ArgumentOptionConstants.ConnectorType, "Local", ConnectorType.HuggingFace)]
    [InlineData(AppSettingConstants.ConnectorType, "Ollama", ArgumentOptionConstants.ConnectorType, "Local", ConnectorType.Ollama)]
    [InlineData(AppSettingConstants.ConnectorType, "Anthropic", ArgumentOptionConstants.ConnectorType, "Vendor", ConnectorType.Anthropic)]
    [InlineData(AppSettingConstants.ConnectorType, "LG", ArgumentOptionConstants.ConnectorType, "Vendor", ConnectorType.LG)]
    [InlineData(AppSettingConstants.ConnectorType, "Naver", ArgumentOptionConstants.ConnectorType, "Vendor", ConnectorType.Naver)]
    [InlineData(AppSettingConstants.ConnectorType, "OpenAI", ArgumentOptionConstants.ConnectorType, "Vendor", ConnectorType.OpenAI)]
    [InlineData(AppSettingConstants.ConnectorType, "Upstage", ArgumentOptionConstants.ConnectorType, "Vendor", ConnectorType.Upstage)]
    [InlineData(AppSettingConstants.ConnectorType, "AmazonBedrock", ArgumentOptionConstants.ConnectorTypeInShort, "MaaS", ConnectorType.AmazonBedrock)]
    [InlineData(AppSettingConstants.ConnectorType, "AzureAIFoundry", ArgumentOptionConstants.ConnectorTypeInShort, "MaaS", ConnectorType.AzureAIFoundry)]
    [InlineData(AppSettingConstants.ConnectorType, "GitHubModels", ArgumentOptionConstants.ConnectorTypeInShort, "MaaS", ConnectorType.GitHubModels)]
    [InlineData(AppSettingConstants.ConnectorType, "GoogleVertexAI", ArgumentOptionConstants.ConnectorTypeInShort, "MaaS", ConnectorType.GoogleVertexAI)]
    [InlineData(AppSettingConstants.ConnectorType, "DockerModelRunner", ArgumentOptionConstants.ConnectorTypeInShort, "Local", ConnectorType.DockerModelRunner)]
    [InlineData(AppSettingConstants.ConnectorType, "FoundryLocal", ArgumentOptionConstants.ConnectorTypeInShort, "Local", ConnectorType.FoundryLocal)]
    [InlineData(AppSettingConstants.ConnectorType, "HuggingFace", ArgumentOptionConstants.ConnectorTypeInShort, "Local", ConnectorType.HuggingFace)]
    [InlineData(AppSettingConstants.ConnectorType, "Ollama", ArgumentOptionConstants.ConnectorTypeInShort, "Local", ConnectorType.Ollama)]
    [InlineData(AppSettingConstants.ConnectorType, "Anthropic", ArgumentOptionConstants.ConnectorTypeInShort, "Vendor", ConnectorType.Anthropic)]
    [InlineData(AppSettingConstants.ConnectorType, "LG", ArgumentOptionConstants.ConnectorTypeInShort, "Vendor", ConnectorType.LG)]
    [InlineData(AppSettingConstants.ConnectorType, "Naver", ArgumentOptionConstants.ConnectorTypeInShort, "Vendor", ConnectorType.Naver)]
    [InlineData(AppSettingConstants.ConnectorType, "OpenAI", ArgumentOptionConstants.ConnectorTypeInShort, "Vendor", ConnectorType.OpenAI)]
    [InlineData(AppSettingConstants.ConnectorType, "Upstage", ArgumentOptionConstants.ConnectorTypeInShort, "Vendor", ConnectorType.Upstage)]
    public void Given_ConnectorType_And_InvalidArgument_When_VerifyConnectorType_Invoked_Then_It_Should_Return_Result(string key, string value, string argument1, string argument2, ConnectorType expected)
    {
        var config = BuildConfig((key, value));
        var args = new[] { argument1, argument2 };

        var result = ArgumentOptions.VerifyConnectorType(config, args);

        result.ShouldBe(expected);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData(AppSettingConstants.ConnectorType, "AmazonBedrock", "MaaS", ConnectorType.AmazonBedrock)]
    [InlineData(AppSettingConstants.ConnectorType, "AzureAIFoundry", "MaaS", ConnectorType.AzureAIFoundry)]
    [InlineData(AppSettingConstants.ConnectorType, "GitHubModels", "MaaS", ConnectorType.GitHubModels)]
    [InlineData(AppSettingConstants.ConnectorType, "GoogleVertexAI", "MaaS", ConnectorType.GoogleVertexAI)]
    [InlineData(AppSettingConstants.ConnectorType, "DockerModelRunner", "Local", ConnectorType.DockerModelRunner)]
    [InlineData(AppSettingConstants.ConnectorType, "FoundryLocal", "Local", ConnectorType.FoundryLocal)]
    [InlineData(AppSettingConstants.ConnectorType, "HuggingFace", "Local", ConnectorType.HuggingFace)]
    [InlineData(AppSettingConstants.ConnectorType, "Ollama", "Local", ConnectorType.Ollama)]
    [InlineData(AppSettingConstants.ConnectorType, "Anthropic", "Vendor", ConnectorType.Anthropic)]
    [InlineData(AppSettingConstants.ConnectorType, "LG", "Vendor", ConnectorType.LG)]
    [InlineData(AppSettingConstants.ConnectorType, "Naver", "Vendor", ConnectorType.Naver)]
    [InlineData(AppSettingConstants.ConnectorType, "OpenAI", "Vendor", ConnectorType.OpenAI)]
    [InlineData(AppSettingConstants.ConnectorType, "Upstage", "Vendor", ConnectorType.Upstage)]
    public void Given_ConnectorType_And_UnrelatedArgument_When_VerifyConnectorType_Invoked_Then_It_Should_Return_Result(string key, string value, string argument, ConnectorType expected)
    {
        var config = BuildConfig((key, value));
        var args = new[] { "--something-else", argument };

        var result = ArgumentOptions.VerifyConnectorType(config, args);

        result.ShouldBe(expected);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData(AppSettingConstants.ConnectorType, "Naver")]
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
    [InlineData(AppSettingConstants.ConnectorType, "AmazonBedrock", ConnectorType.AmazonBedrock)]
    [InlineData(AppSettingConstants.ConnectorType, "AzureAIFoundry", ConnectorType.AzureAIFoundry)]
    [InlineData(AppSettingConstants.ConnectorType, "GitHubModels", ConnectorType.GitHubModels)]
    [InlineData(AppSettingConstants.ConnectorType, "GoogleVertexAI", ConnectorType.GoogleVertexAI)]
    // [InlineData(AppSettingConstants.ConnectorType, "DockerModelRunner", ConnectorType.DockerModelRunner)]
    [InlineData(AppSettingConstants.ConnectorType, "FoundryLocal", ConnectorType.FoundryLocal)]
    [InlineData(AppSettingConstants.ConnectorType, "HuggingFace", ConnectorType.HuggingFace)]
    [InlineData(AppSettingConstants.ConnectorType, "Ollama", ConnectorType.Ollama)]
    [InlineData(AppSettingConstants.ConnectorType, "Anthropic", ConnectorType.Anthropic)]
    [InlineData(AppSettingConstants.ConnectorType, "LG", ConnectorType.LG)]
    // [InlineData(AppSettingConstants.ConnectorType, "Naver", ConnectorType.Naver)]
    [InlineData(AppSettingConstants.ConnectorType, "OpenAI", ConnectorType.OpenAI)]
    [InlineData(AppSettingConstants.ConnectorType, "Upstage", ConnectorType.Upstage)]
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
    [InlineData(AppSettingConstants.ConnectorType, "AmazonBedrock", "Upstage", ConnectorType.Upstage)]
    [InlineData(AppSettingConstants.ConnectorType, "AzureAIFoundry", "OpenAI", ConnectorType.OpenAI)]
    // [InlineData(AppSettingConstants.ConnectorType, "GitHubModels", "Naver", ConnectorType.Naver)]
    [InlineData(AppSettingConstants.ConnectorType, "GoogleVertexAI", "LG", ConnectorType.LG)]
    // [InlineData(AppSettingConstants.ConnectorType, "DockerModelRunner", "Anthropic", ConnectorType.Anthropic)]
    [InlineData(AppSettingConstants.ConnectorType, "FoundryLocal", "Ollama", ConnectorType.Ollama)]
    [InlineData(AppSettingConstants.ConnectorType, "HuggingFace", "Ollama", ConnectorType.Ollama)]
    [InlineData(AppSettingConstants.ConnectorType, "Ollama", "FoundryLocal", ConnectorType.FoundryLocal)]
    // [InlineData(AppSettingConstants.ConnectorType, "Anthropic", "DockerModelRunner", ConnectorType.DockerModelRunner)]
    [InlineData(AppSettingConstants.ConnectorType, "LG", "GoogleVertexAI", ConnectorType.GoogleVertexAI)]
    [InlineData(AppSettingConstants.ConnectorType, "Naver", "GitHubModels", ConnectorType.GitHubModels)]
    [InlineData(AppSettingConstants.ConnectorType, "OpenAI", "AzureAIFoundry", ConnectorType.AzureAIFoundry)]
    [InlineData(AppSettingConstants.ConnectorType, "Upstage", "AmazonBedrock", ConnectorType.AmazonBedrock)]
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
    [InlineData(AppSettingConstants.ConnectorType, "AmazonBedrock", "MaaS", ConnectorType.AmazonBedrock)]
    [InlineData(AppSettingConstants.ConnectorType, "AzureAIFoundry", "MaaS", ConnectorType.AzureAIFoundry)]
    [InlineData(AppSettingConstants.ConnectorType, "GitHubModels", "MaaS", ConnectorType.GitHubModels)]
    [InlineData(AppSettingConstants.ConnectorType, "GoogleVertexAI", "MaaS", ConnectorType.GoogleVertexAI)]
    // [InlineData(AppSettingConstants.ConnectorType, "DockerModelRunner", "Local", ConnectorType.DockerModelRunner)]
    [InlineData(AppSettingConstants.ConnectorType, "FoundryLocal", "Local", ConnectorType.FoundryLocal)]
    [InlineData(AppSettingConstants.ConnectorType, "HuggingFace", "Local", ConnectorType.HuggingFace)]
    [InlineData(AppSettingConstants.ConnectorType, "Ollama", "Local", ConnectorType.Ollama)]
    [InlineData(AppSettingConstants.ConnectorType, "Anthropic", "Vendor", ConnectorType.Anthropic)]
    [InlineData(AppSettingConstants.ConnectorType, "LG", "Vendor", ConnectorType.LG)]
    // [InlineData(AppSettingConstants.ConnectorType, "Naver", "Vendor", ConnectorType.Naver)]
    [InlineData(AppSettingConstants.ConnectorType, "OpenAI", "Vendor", ConnectorType.OpenAI)]
    [InlineData(AppSettingConstants.ConnectorType, "Upstage", "Vendor", ConnectorType.Upstage)]
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
    [InlineData(ArgumentOptionConstants.Help, true)]
    [InlineData(ArgumentOptionConstants.HelpInShort, true)]
    [InlineData("--unknown", true)]
    public void Given_Help_When_Parse_Invoked_Then_It_Should_Return_Help(string argument, bool expected)
    {
        var config = BuildConfig((AppSettingConstants.ConnectorType, ConnectorType.GitHubModels.ToString()));
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