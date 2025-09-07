using Microsoft.Extensions.Configuration;

using OpenChat.PlaygroundApp.Abstractions;
using OpenChat.PlaygroundApp.Connectors;

namespace OpenChat.PlaygroundApp.Tests.Options;

public class AnthropicArgumentOptionsTests
{
    private const string ApiKey = "anthropic-api-key";
    private const string Model = "claude-sonnet-4-0";

    private static IConfiguration BuildConfigWithAnthropic(
        string? configApiKey = ApiKey,
        string? configModel = Model,
        string? envApiKey = null,
        string? envModel = null)
    {
        // Base configuration values (lowest priority)
        var configDict = new Dictionary<string, string?>
        {
            ["ConnectorType"] = ConnectorType.Anthropic.ToString()
        };

        if (string.IsNullOrWhiteSpace(configApiKey) == false)
        {
            configDict["Anthropic:ApiKey"] = configApiKey;
        }
        if (string.IsNullOrWhiteSpace(configModel) == false)
        {
            configDict["Anthropic:Model"] = configModel;
        }

        if (string.IsNullOrWhiteSpace(envApiKey) == true &&
            string.IsNullOrWhiteSpace(envModel) == true)
        {
            return new ConfigurationBuilder()
                       .AddInMemoryCollection(configDict!)
                       .Build();
        }

        // Environment variables (medium priority)
        var envDict = new Dictionary<string, string?>();
        if (string.IsNullOrWhiteSpace(envApiKey) == false)
        {
            envDict["Anthropic:ApiKey"] = envApiKey;
        }
        if (string.IsNullOrWhiteSpace(envModel) == false)
        {
            envDict["Anthropic:Model"] = envModel;
        }

        return new ConfigurationBuilder()
                   .AddInMemoryCollection(configDict!)  // Base configuration (lowest priority)
                   .AddInMemoryCollection(envDict!)     // Environment variables (medium priority)
                   .Build();
    }

    [Trait("Category", "UnitTest")]
    [Fact]
    public void Given_Nothing_When_Parse_Invoked_Then_It_Should_Set_Config()
    {
        // Arrange
        var config = BuildConfigWithAnthropic();
        var args = Array.Empty<string>();

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.Anthropic.ShouldNotBeNull();
        settings.Anthropic.ApiKey.ShouldBe(ApiKey);
        settings.Anthropic.Model.ShouldBe(Model);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("cli-api-key")]
    public void Given_CLI_ApiKey_When_Parse_Invoked_Then_It_Should_Use_CLI_ApiKey(string cliApiKey)
    {
        // Arrange
        var config = BuildConfigWithAnthropic();
        var args = new[] { "--api-key", cliApiKey };

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.Anthropic.ShouldNotBeNull();
        settings.Anthropic.ApiKey.ShouldBe(cliApiKey);
        settings.Anthropic.Model.ShouldBe(Model);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("claude-3-5-sonnet-latest")]
    public void Given_CLI_Model_When_Parse_Invoked_Then_It_Should_Use_CLI_Model(string cliModel)
    {
        // Arrange
        var config = BuildConfigWithAnthropic();
        var args = new[] { "--model", cliModel };

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.Anthropic.ShouldNotBeNull();
        settings.Anthropic.ApiKey.ShouldBe(ApiKey);
        settings.Anthropic.Model.ShouldBe(cliModel);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("cli-api-key", "claude-3-5-sonnet-latest")]
    public void Given_All_CLI_Arguments_When_Parse_Invoked_Then_It_Should_Use_CLI(string cliApiKey, string cliModel)
    {
        // Arrange
        var config = BuildConfigWithAnthropic();
        var args = new[] { "--api-key", cliApiKey, "--model", cliModel };

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.Anthropic.ShouldNotBeNull();
        settings.Anthropic.ApiKey.ShouldBe(cliApiKey);
        settings.Anthropic.Model.ShouldBe(cliModel);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("--api-key")]
    [InlineData("--model")]
    public void Given_CLI_ArgumentWithoutValue_When_Parse_Invoked_Then_It_Should_Use_Config(string argument)
    {
        // Arrange
        var config = BuildConfigWithAnthropic();
        var args = new[] { argument };

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.Anthropic.ShouldNotBeNull();
        settings.Anthropic.ApiKey.ShouldBe(ApiKey);
        settings.Anthropic.Model.ShouldBe(Model);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("--something", "else", "--another", "value")]
    public void Given_Unrelated_CLI_Arguments_When_Parse_Invoked_Then_It_Should_Use_Config(params string[] args)
    {
        // Arrange
        var config = BuildConfigWithAnthropic();

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.Anthropic.ShouldNotBeNull();
        settings.Anthropic.ApiKey.ShouldBe(ApiKey);
        settings.Anthropic.Model.ShouldBe(Model);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("--strange-model-name")]
    public void Given_Anthropic_With_ModelName_StartingWith_Dashes_When_Parse_Invoked_Then_It_Should_Treat_As_Value(string model)
    {
        // Arrange
        var config = BuildConfigWithAnthropic();
        var args = new[] { "--model", model };

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.Anthropic.ShouldNotBeNull();
        settings.Anthropic.Model.ShouldBe(model);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("config-api-key", "config-model")]
    public void Given_ConfigValues_And_No_CLI_When_Parse_Invoked_Then_It_Should_Use_Config(string configApiKey, string configModel)
    {
        // Arrange
        var config = BuildConfigWithAnthropic(configApiKey, configModel);
        var args = Array.Empty<string>();

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.Anthropic.ShouldNotBeNull();
        settings.Anthropic.ApiKey.ShouldBe(configApiKey);
        settings.Anthropic.Model.ShouldBe(configModel);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("config-api-key", "config-model", "cli-api-key", "cli-model")]
    public void Given_ConfigValues_And_CLI_When_Parse_Invoked_Then_It_Should_Use_CLI(
        string configApiKey, string configModel,
        string cliApiKey, string cliModel)
    {
        // Arrange
        var config = BuildConfigWithAnthropic(configApiKey, configModel);
        var args = new[] { "--api-key", cliApiKey, "--model", cliModel };

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.Anthropic.ShouldNotBeNull();
        settings.Anthropic.ApiKey.ShouldBe(cliApiKey);
        settings.Anthropic.Model.ShouldBe(cliModel);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("env-api-key", "env-model")]
    public void Given_EnvironmentVariables_And_No_Config_When_Parse_Invoked_Then_It_Should_Use_EnvironmentVariables(
        string envApiKey, string envModel)
    {
        // Arrange
        var config = BuildConfigWithAnthropic(
            configApiKey: null, configModel: null,
            envApiKey: envApiKey, envModel: envModel);
        var args = Array.Empty<string>();

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.Anthropic.ShouldNotBeNull();
        settings.Anthropic.ApiKey.ShouldBe(envApiKey);
        settings.Anthropic.Model.ShouldBe(envModel);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("config-api-key", "config-model", "env-api-key", "env-model")]
    public void Given_ConfigValues_And_EnvironmentVariables_When_Parse_Invoked_Then_It_Should_Use_EnvironmentVariables(
        string configApiKey, string configModel,
        string envApiKey, string envModel)
    {
        // Arrange
        var config = BuildConfigWithAnthropic(
            configApiKey, configModel,
            envApiKey, envModel);
        var args = Array.Empty<string>();

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.Anthropic.ShouldNotBeNull();
        settings.Anthropic.ApiKey.ShouldBe(envApiKey);
        settings.Anthropic.Model.ShouldBe(envModel);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("config-api-key", "config-model", "env-api-key", "env-model", "cli-api-key", "cli-model")]
    public void Given_ConfigValues_And_EnvironmentVariables_And_CLI_When_Parse_Invoked_Then_It_Should_Use_CLI(
        string configApiKey, string configModel,
        string envApiKey, string envModel,
        string cliApiKey, string cliModel)
    {
        // Arrange
        var config = BuildConfigWithAnthropic(
            configApiKey, configModel,
            envApiKey, envModel);
        var args = new[] { "--api-key", cliApiKey, "--model", cliModel };

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.Anthropic.ShouldNotBeNull();
        settings.Anthropic.ApiKey.ShouldBe(cliApiKey);
        settings.Anthropic.Model.ShouldBe(cliModel);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("config-api-key", "config-model", "env-api-key", null)]
    public void Given_Partial_EnvironmentVariables_When_Parse_Invoked_Then_It_Should_Mix_Config_And_Environment(
        string configApiKey, string configModel,
        string envApiKey, string? envModel)
    {
        // Arrange
        var config = BuildConfigWithAnthropic(
            configApiKey, configModel,
            envApiKey, envModel);
        var args = Array.Empty<string>();

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.Anthropic.ShouldNotBeNull();
        settings.Anthropic.ApiKey.ShouldBe(envApiKey);    // From environment
        settings.Anthropic.Model.ShouldBe(configModel);  // From config (no env override)
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("config-api-key", "config-model", null, "env-model", "cli-api-key", null)]
    public void Given_Mixed_Priority_Sources_When_Parse_Invoked_Then_It_Should_Respect_Priority_Order(
        string configApiKey, string configModel,
        string? envApiKey, string envModel,
        string cliApiKey, string? cliModel)
    {
        // Arrange
        var config = BuildConfigWithAnthropic(
            configApiKey, configModel,
            envApiKey, envModel);
        var args = new[] { "--api-key", cliApiKey, "--model", cliModel };

        // Act
        var settings = ArgumentOptions.Parse(config, args!);

        // Assert
        settings.Anthropic.ShouldNotBeNull();
        settings.Anthropic.ApiKey.ShouldBe(cliApiKey);    // CLI wins (highest priority)
        settings.Anthropic.Model.ShouldBe(envModel);     // Env wins over config (medium priority)
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("cli-api-key", "cli-model")]
    public void Given_Anthropic_With_KnownArguments_When_Parse_Invoked_Then_Help_ShouldBe_False(string cliApiKey, string cliModel)
    {
        // Arrange
        var config = BuildConfigWithAnthropic(ApiKey, Model);
        var args = new[] { "--api-key", cliApiKey, "--model", cliModel };

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.Help.ShouldBeFalse();
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("--api-key")]
    [InlineData("--model")]
    public void Given_Anthropic_With_KnownArgument_WithoutValue_When_Parse_Invoked_Then_Help_ShouldBe_False(string argument)
    {
        // Arrange
        var config = BuildConfigWithAnthropic();
        var args = new[] { argument };

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.Help.ShouldBeFalse();
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("cli-api-key", "--unknown-flag")]
    public void Given_Anthropic_With_Known_And_Unknown_Argument_When_Parse_Invoked_Then_Help_ShouldBe_True(string cliApiKey, string argument)
    {
        // Arrange
        var config = BuildConfigWithAnthropic();
        var args = new[] { "--api-key", cliApiKey, argument };

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.Help.ShouldBeTrue();
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("env-api-key", "env-model")]
    public void Given_EnvironmentVariables_Only_When_Parse_Invoked_Then_Help_Should_Be_False(
        string envApiKey, string envModel)
    {
        // Arrange
        var config = BuildConfigWithAnthropic(
            configApiKey: null, configModel: null,
            envApiKey: envApiKey, envModel: envModel);
        var args = Array.Empty<string>();

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.Help.ShouldBeFalse();
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("cli-api-key", "cli-model")]
    public void Given_CLI_Only_When_Parse_Invoked_Then_Help_Should_Be_False(string cliApiKey, string cliModel)
    {
        // Arrange
        var config = BuildConfigWithAnthropic();
        var args = new[] { "--api-key", cliApiKey, "--model", cliModel };

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.Help.ShouldBeFalse();
    }
}