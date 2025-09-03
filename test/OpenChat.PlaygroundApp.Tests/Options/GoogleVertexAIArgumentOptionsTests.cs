using Microsoft.Extensions.Configuration;

using OpenChat.PlaygroundApp.Abstractions;
using OpenChat.PlaygroundApp.Connectors;

namespace OpenChat.PlaygroundApp.Tests.Options;

public class GoogleVertexAIArgumentOptionsTests
{
    private const string ApiKey = "vertex-ai-api-key";
    private const string Model = "vertex-ai-model-name";

    private static IConfiguration BuildConfigWithGoogleVertexAI(
        string? configApiKey = ApiKey,
        string? configModel = Model,
        string? envApiKey = null,
        string? envModel = null)
    {
        var configDict = new Dictionary<string, string?>
        {
            ["ConnectorType"] = ConnectorType.GoogleVertexAI.ToString()
        };

        if (string.IsNullOrWhiteSpace(configApiKey) == false)
        {
            configDict["GoogleVertexAI:ApiKey"] = configApiKey;
        }
        if (string.IsNullOrWhiteSpace(configModel) == false)
        {
            configDict["GoogleVertexAI:Model"] = configModel;
        }

        if (string.IsNullOrWhiteSpace(envApiKey) == true &&
            string.IsNullOrWhiteSpace(envModel) == true)
        {
            return new ConfigurationBuilder()
                       .AddInMemoryCollection(configDict!)
                       .Build();
        }

        var envDict = new Dictionary<string, string?>();
        if (string.IsNullOrWhiteSpace(envApiKey) == false)
        {
            envDict["GoogleVertexAI:ApiKey"] = envApiKey;
        }
        if (string.IsNullOrWhiteSpace(envModel) == false)
        {
            envDict["GoogleVertexAI:Model"] = envModel;
        }

        return new ConfigurationBuilder()
                   .AddInMemoryCollection(configDict!)
                   .AddInMemoryCollection(envDict!)
                   .Build();
    }

    [Trait("Category", "UnitTest")]
    [Fact]
    public void Given_Nothing_When_Parse_Invoked_Then_It_Should_Set_Config()
    {
        var config = BuildConfigWithGoogleVertexAI();
        var args = Array.Empty<string>();

        var settings = ArgumentOptions.Parse(config, args);

        settings.GoogleVertexAI.ShouldNotBeNull();
        settings.GoogleVertexAI.ApiKey.ShouldBe(ApiKey);
        settings.GoogleVertexAI.Model.ShouldBe(Model);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("cli-api-key")]
    public void Given_CLI_ApiKey_When_Parse_Invoked_Then_It_Should_Use_CLI_ApiKey(string cliApiKey)
    {
        var config = BuildConfigWithGoogleVertexAI();
        var args = new[] { "--api-key", cliApiKey };

        var settings = ArgumentOptions.Parse(config, args);

        settings.GoogleVertexAI.ShouldNotBeNull();
        settings.GoogleVertexAI.ApiKey.ShouldBe(cliApiKey);
        settings.GoogleVertexAI.Model.ShouldBe(Model);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("cli-model")]
    public void Given_CLI_Model_When_Parse_Invoked_Then_It_Should_Use_CLI_Model(string cliModel)
    {
        var config = BuildConfigWithGoogleVertexAI();
        var args = new[] { "--model", cliModel };

        var settings = ArgumentOptions.Parse(config, args);

        settings.GoogleVertexAI.ShouldNotBeNull();
        settings.GoogleVertexAI.ApiKey.ShouldBe(ApiKey);
        settings.GoogleVertexAI.Model.ShouldBe(cliModel);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("cli-api-key", "cli-model")]
    public void Given_All_CLI_Arguments_When_Parse_Invoked_Then_It_Should_Use_CLI(string cliApiKey, string cliModel)
    {
        var config = BuildConfigWithGoogleVertexAI();
        var args = new[] { "--api-key", cliApiKey, "--model", cliModel };

        var settings = ArgumentOptions.Parse(config, args);

        settings.GoogleVertexAI.ShouldNotBeNull();
        settings.GoogleVertexAI.ApiKey.ShouldBe(cliApiKey);
        settings.GoogleVertexAI.Model.ShouldBe(cliModel);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("--api-key")]
    [InlineData("--model")]
    public void Given_CLI_ArgumentWithoutValue_When_Parse_Invoked_Then_It_Should_Use_Config(string argument)
    {
        var config = BuildConfigWithGoogleVertexAI();
        var args = new[] { argument };

        var settings = ArgumentOptions.Parse(config, args);

        settings.GoogleVertexAI.ShouldNotBeNull();
        settings.GoogleVertexAI.ApiKey.ShouldBe(ApiKey);
        settings.GoogleVertexAI.Model.ShouldBe(Model);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("--something", "else", "--another", "value")]
    public void Given_Unrelated_CLI_Arguments_When_Parse_Invoked_Then_It_Should_Use_Config(params string[] args)
    {
        var config = BuildConfigWithGoogleVertexAI();

        var settings = ArgumentOptions.Parse(config, args);

        settings.GoogleVertexAI.ShouldNotBeNull();
        settings.GoogleVertexAI.ApiKey.ShouldBe(ApiKey);
        settings.GoogleVertexAI.Model.ShouldBe(Model);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("--strange-model-name")]
    public void Given_GoogleVertexAI_With_ModelName_StartingWith_Dashes_When_Parse_Invoked_Then_It_Should_Treat_As_Value(string model)
    {
        var config = BuildConfigWithGoogleVertexAI();
        var args = new[] { "--model", model };

        var settings = ArgumentOptions.Parse(config, args);

        settings.GoogleVertexAI.ShouldNotBeNull();
        settings.GoogleVertexAI.Model.ShouldBe(model);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("config-api-key", "config-model")]
    public void Given_ConfigValues_And_No_CLI_When_Parse_Invoked_Then_It_Should_Use_Config(string configApiKey, string configModel)
    {
        var config = BuildConfigWithGoogleVertexAI(configApiKey, configModel);
        var args = Array.Empty<string>();

        var settings = ArgumentOptions.Parse(config, args);

        settings.GoogleVertexAI.ShouldNotBeNull();
        settings.GoogleVertexAI.ApiKey.ShouldBe(configApiKey);
        settings.GoogleVertexAI.Model.ShouldBe(configModel);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("cli-api-key", "cli-model")]
    public void Given_GoogleVertexAI_With_KnownArguments_When_Parse_Invoked_Then_Help_ShouldBe_False(string cliApiKey, string cliModel)
    {
        var config = BuildConfigWithGoogleVertexAI(ApiKey, Model);
        var args = new[] { "--api-key", cliApiKey, "--model", cliModel };

        var settings = ArgumentOptions.Parse(config, args);

        settings.Help.ShouldBeFalse();
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("--api-key")]
    [InlineData("--model")]
    public void Given_GoogleVertexAI_With_KnownArgument_WithoutValue_When_Parse_Invoked_Then_Help_ShouldBe_False(string argument)
    {
        var config = BuildConfigWithGoogleVertexAI();
        var args = new[] { argument };

        var settings = ArgumentOptions.Parse(config, args);

        settings.Help.ShouldBeFalse();
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("cli-api-key", "--unknown-flag")]
    public void Given_GoogleVertexAI_With_Known_And_Unknown_Argument_When_Parse_Invoked_Then_Help_ShouldBe_True(string cliApiKey, string argument)
    {
        var config = BuildConfigWithGoogleVertexAI();
        var args = new[] { "--api-key", cliApiKey, argument };

        var settings = ArgumentOptions.Parse(config, args);

        settings.Help.ShouldBeTrue();
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("cli-api-key", "cli-model")]
    public void Given_CLI_Only_When_Parse_Invoked_Then_Help_Should_Be_False(string cliApiKey, string cliModel)
    {
        var config = BuildConfigWithGoogleVertexAI();
        var args = new[] { "--api-key", cliApiKey, "--model", cliModel };

        var settings = ArgumentOptions.Parse(config, args);

        settings.Help.ShouldBeFalse();
    }
}
