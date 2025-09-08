using Microsoft.Extensions.Configuration;

using OpenChat.PlaygroundApp.Abstractions;
using OpenChat.PlaygroundApp.Connectors;
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
    [InlineData("ConnectorType", "AmazonBedrock", "--connector-type", "Upstage", ConnectorType.Upstage)]
    [InlineData("ConnectorType", "AzureAIFoundry", "--connector-type", "OpenAI", ConnectorType.OpenAI)]
    [InlineData("ConnectorType", "GitHubModels", "--connector-type", "Naver", ConnectorType.Naver)]
    [InlineData("ConnectorType", "GoogleVertexAI", "--connector-type", "LG", ConnectorType.LG)]
    [InlineData("ConnectorType", "DockerModelRunner", "--connector-type", "Anthropic", ConnectorType.Anthropic)]
    [InlineData("ConnectorType", "FoundryLocal", "--connector-type", "Ollama", ConnectorType.Ollama)]
    [InlineData("ConnectorType", "HuggingFace", "--connector-type", "Ollama", ConnectorType.Ollama)]
    [InlineData("ConnectorType", "Ollama", "--connector-type", "FoundryLocal", ConnectorType.FoundryLocal)]
    [InlineData("ConnectorType", "Anthropic", "--connector-type", "DockerModelRunner", ConnectorType.DockerModelRunner)]
    [InlineData("ConnectorType", "LG", "--connector-type", "GoogleVertexAI", ConnectorType.GoogleVertexAI)]
    [InlineData("ConnectorType", "Naver", "--connector-type", "GitHubModels", ConnectorType.GitHubModels)]
    [InlineData("ConnectorType", "OpenAI", "--connector-type", "AzureAIFoundry", ConnectorType.AzureAIFoundry)]
    [InlineData("ConnectorType", "Upstage", "--connector-type", "AmazonBedrock", ConnectorType.AmazonBedrock)]
    [InlineData("ConnectorType", "AmazonBedrock", "-c", "Upstage", ConnectorType.Upstage)]
    [InlineData("ConnectorType", "AzureAIFoundry", "-c", "OpenAI", ConnectorType.OpenAI)]
    [InlineData("ConnectorType", "GitHubModels", "-c", "Naver", ConnectorType.Naver)]
    [InlineData("ConnectorType", "GoogleVertexAI", "-c", "LG", ConnectorType.LG)]
    [InlineData("ConnectorType", "DockerModelRunner", "-c", "Anthropic", ConnectorType.Anthropic)]
    [InlineData("ConnectorType", "FoundryLocal", "-c", "Ollama", ConnectorType.Ollama)]
    [InlineData("ConnectorType", "HuggingFace", "-c", "Ollama", ConnectorType.Ollama)]
    [InlineData("ConnectorType", "Ollama", "-c", "FoundryLocal", ConnectorType.FoundryLocal)]
    [InlineData("ConnectorType", "Anthropic", "-c", "DockerModelRunner", ConnectorType.DockerModelRunner)]
    [InlineData("ConnectorType", "LG", "-c", "GoogleVertexAI", ConnectorType.GoogleVertexAI)]
    [InlineData("ConnectorType", "Naver", "-c", "GitHubModels", ConnectorType.GitHubModels)]
    [InlineData("ConnectorType", "OpenAI", "-c", "AzureAIFoundry", ConnectorType.AzureAIFoundry)]
    [InlineData("ConnectorType", "Upstage", "-c", "AmazonBedrock", ConnectorType.AmazonBedrock)]
    public void Given_ConnectorType_And_Argument_When_VerifyConnectorType_Invoked_Then_It_Should_Return_Result(string key, string value, string argument1, string argument2, ConnectorType expected)
    {
        var config = BuildConfig((key, value));
        var args = new[] { argument1, argument2 };

        var result = ArgumentOptions.VerifyConnectorType(config, args);

        result.ShouldBe(expected);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("ConnectorType", "AmazonBedrock", "--connector-type", "MaaS", ConnectorType.AmazonBedrock)]
    [InlineData("ConnectorType", "AzureAIFoundry", "--connector-type", "MaaS", ConnectorType.AzureAIFoundry)]
    [InlineData("ConnectorType", "GitHubModels", "--connector-type", "MaaS", ConnectorType.GitHubModels)]
    [InlineData("ConnectorType", "GoogleVertexAI", "--connector-type", "MaaS", ConnectorType.GoogleVertexAI)]
    [InlineData("ConnectorType", "DockerModelRunner", "--connector-type", "Local", ConnectorType.DockerModelRunner)]
    [InlineData("ConnectorType", "FoundryLocal", "--connector-type", "Local", ConnectorType.FoundryLocal)]
    [InlineData("ConnectorType", "HuggingFace", "--connector-type", "Local", ConnectorType.HuggingFace)]
    [InlineData("ConnectorType", "Ollama", "--connector-type", "Local", ConnectorType.Ollama)]
    [InlineData("ConnectorType", "Anthropic", "--connector-type", "Vendor", ConnectorType.Anthropic)]
    [InlineData("ConnectorType", "LG", "--connector-type", "Vendor", ConnectorType.LG)]
    [InlineData("ConnectorType", "Naver", "--connector-type", "Vendor", ConnectorType.Naver)]
    [InlineData("ConnectorType", "OpenAI", "--connector-type", "Vendor", ConnectorType.OpenAI)]
    [InlineData("ConnectorType", "Upstage", "--connector-type", "Vendor", ConnectorType.Upstage)]
    [InlineData("ConnectorType", "AmazonBedrock", "-c", "MaaS", ConnectorType.AmazonBedrock)]
    [InlineData("ConnectorType", "AzureAIFoundry", "-c", "MaaS", ConnectorType.AzureAIFoundry)]
    [InlineData("ConnectorType", "GitHubModels", "-c", "MaaS", ConnectorType.GitHubModels)]
    [InlineData("ConnectorType", "GoogleVertexAI", "-c", "MaaS", ConnectorType.GoogleVertexAI)]
    [InlineData("ConnectorType", "DockerModelRunner", "-c", "Local", ConnectorType.DockerModelRunner)]
    [InlineData("ConnectorType", "FoundryLocal", "-c", "Local", ConnectorType.FoundryLocal)]
    [InlineData("ConnectorType", "HuggingFace", "-c", "Local", ConnectorType.HuggingFace)]
    [InlineData("ConnectorType", "Ollama", "-c", "Local", ConnectorType.Ollama)]
    [InlineData("ConnectorType", "Anthropic", "-c", "Vendor", ConnectorType.Anthropic)]
    [InlineData("ConnectorType", "LG", "-c", "Vendor", ConnectorType.LG)]
    [InlineData("ConnectorType", "Naver", "-c", "Vendor", ConnectorType.Naver)]
    [InlineData("ConnectorType", "OpenAI", "-c", "Vendor", ConnectorType.OpenAI)]
    [InlineData("ConnectorType", "Upstage", "-c", "Vendor", ConnectorType.Upstage)]
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
    // [InlineData("ConnectorType", "Anthropic", ConnectorType.Anthropic)]
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
        var args = new[] { "--connector-type", argument };

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
    // [InlineData("ConnectorType", "Anthropic", "Vendor", ConnectorType.Anthropic)]
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
    [InlineData("--help", true)]
    [InlineData("-h", true)]
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
    // [InlineData(typeof(AnthropicArgumentOptions))]
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