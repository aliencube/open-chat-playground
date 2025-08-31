using Microsoft.Extensions.Configuration;

using OpenChat.PlaygroundApp.Abstractions;
using OpenChat.PlaygroundApp.Connectors;

namespace OpenChat.PlaygroundApp.Tests.Options;

public class OpenAIArgumentOptionsTests
{
    private const string ApiKey = "openai-key";
    private const string Model = "gpt-4.1-mini";

    private static IConfiguration BuildConfigWithOpenAI(
        string? configApiKey = ApiKey,
        string? configModel = Model,
        string? envApiKey = null,
        string? envModel = null)
    {
        var configDict = new Dictionary<string, string?>
        {
            ["ConnectorType"] = ConnectorType.OpenAI.ToString()
        };

        if (string.IsNullOrWhiteSpace(configApiKey) == false)
        {
            configDict["OpenAI:ApiKey"] = configApiKey;
        }
        if (string.IsNullOrWhiteSpace(configModel) == false)
        {
            configDict["OpenAI:Model"] = configModel;
        }

        if (string.IsNullOrWhiteSpace(envApiKey) && string.IsNullOrWhiteSpace(envModel))
        {
            return new ConfigurationBuilder().AddInMemoryCollection(configDict!).Build();
        }

        var envDict = new Dictionary<string, string?>();
        if (string.IsNullOrWhiteSpace(envApiKey) == false)
        {
            envDict["OpenAI:ApiKey"] = envApiKey;
        }
        if (string.IsNullOrWhiteSpace(envModel) == false)
        {
            envDict["OpenAI:Model"] = envModel;
        }

        return new ConfigurationBuilder()
            .AddInMemoryCollection(configDict!) // base config
            .AddInMemoryCollection(envDict!)    // env overrides
            .Build();
    }

    [Trait("Category", "UnitTest")]
    [Fact]
    public void Given_Nothing_When_Parse_Invoked_Then_It_Should_Set_Config()
    {
        var config = BuildConfigWithOpenAI();
        var settings = ArgumentOptions.Parse(config, Array.Empty<string>());

        settings.OpenAI.ShouldNotBeNull();
        settings.OpenAI.ApiKey.ShouldBe(ApiKey);
        settings.OpenAI.Model.ShouldBe(Model);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("cli-key")]
    public void Given_CLI_ApiKey_When_Parse_Invoked_Then_It_Should_Use_CLI_ApiKey(string cliApiKey)
    {
        var config = BuildConfigWithOpenAI();
        var args = new[] { "--api-key", cliApiKey };
        var settings = ArgumentOptions.Parse(config, args);

        settings.OpenAI.ShouldNotBeNull();
        settings.OpenAI.ApiKey.ShouldBe(cliApiKey);
        settings.OpenAI.Model.ShouldBe(Model);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("cli-model")]    
    public void Given_CLI_Model_When_Parse_Invoked_Then_It_Should_Use_CLI_Model(string cliModel)
    {
        var config = BuildConfigWithOpenAI();
        var args = new[] { "--model", cliModel };
        var settings = ArgumentOptions.Parse(config, args);

        settings.OpenAI.ShouldNotBeNull();
        settings.OpenAI.ApiKey.ShouldBe(ApiKey);
        settings.OpenAI.Model.ShouldBe(cliModel);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("cli-key", "cli-model")]
    public void Given_All_CLI_Arguments_When_Parse_Invoked_Then_It_Should_Use_CLI(string cliApiKey, string cliModel)
    {
        var config = BuildConfigWithOpenAI();
        var args = new[] { "--api-key", cliApiKey, "--model", cliModel };
        var settings = ArgumentOptions.Parse(config, args);

        settings.OpenAI.ShouldNotBeNull();
        settings.OpenAI.ApiKey.ShouldBe(cliApiKey);
        settings.OpenAI.Model.ShouldBe(cliModel);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("--api-key")]
    [InlineData("--model")]
    public void Given_CLI_ArgumentWithoutValue_When_Parse_Invoked_Then_It_Should_Use_Config(string argument)
    {
        var config = BuildConfigWithOpenAI();
        var args = new[] { argument };
        var settings = ArgumentOptions.Parse(config, args);

        settings.OpenAI.ShouldNotBeNull();
        settings.OpenAI.ApiKey.ShouldBe(ApiKey);
        settings.OpenAI.Model.ShouldBe(Model);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("--something", "else")]
    public void Given_Unrelated_CLI_Arguments_When_Parse_Invoked_Then_It_Should_Use_Config(params string[] args)
    {
        var config = BuildConfigWithOpenAI();
    var settings = ArgumentOptions.Parse(config, args!);

        settings.OpenAI.ShouldNotBeNull();
        settings.OpenAI.ApiKey.ShouldBe(ApiKey);
        settings.OpenAI.Model.ShouldBe(Model);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("--weird-model-name")] // leading dashes inside value
    public void Given_OpenAI_With_ModelName_StartingWith_Dashes_When_Parse_Invoked_Then_It_Should_Treat_As_Value(string cliModel)
    {
        var config = BuildConfigWithOpenAI();
        var args = new[] { "--model", cliModel };
    var settings = ArgumentOptions.Parse(config, args!);

        settings.OpenAI.ShouldNotBeNull();
        settings.OpenAI.Model.ShouldBe(cliModel);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("config-key", "config-model")]
    public void Given_ConfigValues_And_No_CLI_When_Parse_Invoked_Then_It_Should_Use_Config(string configApiKey, string configModel)
    {
        var config = BuildConfigWithOpenAI(configApiKey, configModel);
        var settings = ArgumentOptions.Parse(config, Array.Empty<string>());

        settings.OpenAI.ShouldNotBeNull();
        settings.OpenAI.ApiKey.ShouldBe(configApiKey);
        settings.OpenAI.Model.ShouldBe(configModel);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("config-key", "config-model", "cli-key", "cli-model")]
    public void Given_ConfigValues_And_CLI_When_Parse_Invoked_Then_It_Should_Use_CLI(
        string configApiKey, string configModel, string cliApiKey, string cliModel)
    {
        var config = BuildConfigWithOpenAI(configApiKey, configModel);
        var args = new[] { "--api-key", cliApiKey, "--model", cliModel };
    var settings = ArgumentOptions.Parse(config, args!);

        settings.OpenAI.ShouldNotBeNull();
        settings.OpenAI.ApiKey.ShouldBe(cliApiKey);
        settings.OpenAI.Model.ShouldBe(cliModel);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData(null, null, "env-key", "env-model")]
    public void Given_EnvironmentVariables_And_No_Config_When_Parse_Invoked_Then_It_Should_Use_Environment(string? configApiKey, string? configModel, string envApiKey, string envModel)
    {
        var config = BuildConfigWithOpenAI(configApiKey, configModel, envApiKey, envModel);
        var settings = ArgumentOptions.Parse(config, Array.Empty<string>());

        settings.OpenAI.ShouldNotBeNull();
        settings.OpenAI.ApiKey.ShouldBe(envApiKey);
        settings.OpenAI.Model.ShouldBe(envModel);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("config-key", "config-model", "env-key", "env-model")]
    public void Given_ConfigValues_And_EnvironmentVariables_When_Parse_Invoked_Then_Env_Should_Win(string configApiKey, string configModel, string envApiKey, string envModel)
    {
        var config = BuildConfigWithOpenAI(configApiKey, configModel, envApiKey, envModel);
        var settings = ArgumentOptions.Parse(config, Array.Empty<string>());

        settings.OpenAI.ShouldNotBeNull();
        settings.OpenAI.ApiKey.ShouldBe(envApiKey);
        settings.OpenAI.Model.ShouldBe(envModel);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("config-key", "config-model", "env-key", "env-model", "cli-key", "cli-model")]
    public void Given_Config_Env_And_CLI_When_Parse_Invoked_Then_CLI_Should_Win(string configApiKey, string configModel, string envApiKey, string envModel, string cliApiKey, string cliModel)
    {
        var config = BuildConfigWithOpenAI(configApiKey, configModel, envApiKey, envModel);
        var args = new[] { "--api-key", cliApiKey, "--model", cliModel };
    var settings = ArgumentOptions.Parse(config, args!);

        settings.OpenAI.ShouldNotBeNull();
        settings.OpenAI.ApiKey.ShouldBe(cliApiKey);
        settings.OpenAI.Model.ShouldBe(cliModel);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("config-key", "config-model", "env-key", null, "cli-key", null)]
    public void Given_Mixed_Partial_Sources_When_Parse_Invoked_Then_Priority_Should_Apply(
        string configApiKey, string configModel, string envApiKey, string? envModel, string cliApiKey, string? cliModel)
    {
        var config = BuildConfigWithOpenAI(configApiKey, configModel, envApiKey, envModel);
        var args = new[] { "--api-key", cliApiKey, "--model", cliModel };
    var settings = ArgumentOptions.Parse(config, args!);

        settings.OpenAI.ShouldNotBeNull();
        settings.OpenAI.ApiKey.ShouldBe(cliApiKey);    // CLI overrides all
        settings.OpenAI.Model.ShouldBe(envModel ?? cliModel ?? configModel); // env > cli(null) > config
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("cli-key", "cli-model")]
    public void Given_OpenAI_With_KnownArguments_When_Parse_Invoked_Then_Help_ShouldBe_False(string cliApiKey, string cliModel)
    {
        var config = BuildConfigWithOpenAI(ApiKey, Model);
        var args = new[] { "--api-key", cliApiKey, "--model", cliModel };
        var settings = ArgumentOptions.Parse(config, args);
        settings.Help.ShouldBeFalse();
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("--api-key")]
    [InlineData("--model")]
    public void Given_OpenAI_With_KnownArgument_WithoutValue_When_Parse_Invoked_Then_Help_ShouldBe_False(string argument)
    {
        var config = BuildConfigWithOpenAI();
        var args = new[] { argument };
        var settings = ArgumentOptions.Parse(config, args);
        settings.Help.ShouldBeFalse();
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("cli-key", "--unknown-flag")]
    public void Given_OpenAI_With_Known_And_Unknown_Argument_When_Parse_Invoked_Then_Help_ShouldBe_True(string cliApiKey, string unknown)
    {
        var config = BuildConfigWithOpenAI();
        var args = new[] { "--api-key", cliApiKey, unknown };
        var settings = ArgumentOptions.Parse(config, args);
        settings.Help.ShouldBeTrue();
    }

    [Trait("Category", "UnitTest")]
    [Fact]
    public void Given_Help_Flag_When_Parse_Invoked_Then_Help_Should_Be_True()
    {
        var config = BuildConfigWithOpenAI();
        var args = new[] { "--help" };
        var settings = ArgumentOptions.Parse(config, args);
        settings.Help.ShouldBeTrue();
    }

    [Trait("Category", "UnitTest")]
    [Fact]
    public void Given_Missing_ConnectorType_When_Parse_Invoked_Then_It_Should_Set_Help()
    {
        var empty = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string,string?>()).Build();
        var settings = ArgumentOptions.Parse(empty, Array.Empty<string>());
        settings.ConnectorType.ShouldBe(ConnectorType.Unknown);
        settings.Help.ShouldBeTrue();
    }
}
