using Microsoft.Extensions.Configuration;

using OpenChat.PlaygroundApp.Abstractions;
using OpenChat.PlaygroundApp.Connectors;
using OpenChat.PlaygroundApp.Constants;

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
        // Base configuration values (lowest priority)
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
            envDict["OpenAI:ApiKey"] = envApiKey;
        }
        if (string.IsNullOrWhiteSpace(envModel) == false)
        {
            envDict["OpenAI:Model"] = envModel;
        }

        return new ConfigurationBuilder()
                   .AddInMemoryCollection(configDict!)   // Base configuration (lowest priority)
                   .AddInMemoryCollection(envDict!)      // Environment variables (medium priority)
                   .Build();
    }

    [Trait("Category", "UnitTest")]
    [Fact]
    public void Given_Nothing_When_Parse_Invoked_Then_It_Should_Set_Config()
    {
        // Arrange
        var config = BuildConfigWithOpenAI();
        var args = Array.Empty<string>();

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.OpenAI.ShouldNotBeNull();
        settings.OpenAI.ApiKey.ShouldBe(ApiKey);
        settings.OpenAI.Model.ShouldBe(Model);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("cli-key")]
    public void Given_CLI_ApiKey_When_Parse_Invoked_Then_It_Should_Use_CLI_ApiKey(string cliApiKey)
    {
        // Arrange
        var config = BuildConfigWithOpenAI();
        var args = new[] { ArgumentOptionConstants.OpenAI.ApiKey, cliApiKey };

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.OpenAI.ShouldNotBeNull();
        settings.OpenAI.ApiKey.ShouldBe(cliApiKey);
        settings.OpenAI.Model.ShouldBe(Model);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("cli-model")]
    public void Given_CLI_Model_When_Parse_Invoked_Then_It_Should_Use_CLI_Model(string cliModel)
    {
        // Arrange
        var config = BuildConfigWithOpenAI();
        var args = new[] { ArgumentOptionConstants.OpenAI.Model, cliModel };

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.OpenAI.ShouldNotBeNull();
        settings.OpenAI.ApiKey.ShouldBe(ApiKey);
        settings.OpenAI.Model.ShouldBe(cliModel);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("cli-key", "cli-model")]
    public void Given_All_CLI_Arguments_When_Parse_Invoked_Then_It_Should_Use_CLI(string cliApiKey, string cliModel)
    {
        // Arrange
        var config = BuildConfigWithOpenAI();
        var args = new[] { ArgumentOptionConstants.OpenAI.ApiKey, cliApiKey, ArgumentOptionConstants.OpenAI.Model, cliModel };

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.OpenAI.ShouldNotBeNull();
        settings.OpenAI.ApiKey.ShouldBe(cliApiKey);
        settings.OpenAI.Model.ShouldBe(cliModel);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData(ArgumentOptionConstants.OpenAI.ApiKey)]
    [InlineData(ArgumentOptionConstants.OpenAI.Model)]
    public void Given_CLI_ArgumentWithoutValue_When_Parse_Invoked_Then_It_Should_Use_Config(string argument)
    {
        // Arrange
        var config = BuildConfigWithOpenAI();
        var args = new[] { argument };

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.OpenAI.ShouldNotBeNull();
        settings.OpenAI.ApiKey.ShouldBe(ApiKey);
        settings.OpenAI.Model.ShouldBe(Model);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("--something", "else")]
    public void Given_Unrelated_CLI_Arguments_When_Parse_Invoked_Then_It_Should_Use_Config(params string[] args)
    {
        // Arrange
        var config = BuildConfigWithOpenAI();

        // Act
        var settings = ArgumentOptions.Parse(config, args!);

        // Assert
        settings.OpenAI.ShouldNotBeNull();
        settings.OpenAI.ApiKey.ShouldBe(ApiKey);
        settings.OpenAI.Model.ShouldBe(Model);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("--weird-model-name")]
    public void Given_OpenAI_With_ModelName_StartingWith_Dashes_When_Parse_Invoked_Then_It_Should_Treat_As_Value(string cliModel)
    {
        // Arrange
        var config = BuildConfigWithOpenAI();
        var args = new[] { ArgumentOptionConstants.OpenAI.Model, cliModel };

        // Act
        var settings = ArgumentOptions.Parse(config, args!);

        // Assert
        settings.OpenAI.ShouldNotBeNull();
        settings.OpenAI.ApiKey.ShouldBe(ApiKey);
        settings.OpenAI.Model.ShouldBe(cliModel);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("--weird-api-key")]
    public void Given_OpenAI_With_ApiKey_StartingWith_Dashes_When_Parse_Invoked_Then_It_Should_Treat_As_Value(string cliApiKey)
    {
        // Arrange
        var config = BuildConfigWithOpenAI();
        var args = new[] { ArgumentOptionConstants.OpenAI.ApiKey, cliApiKey };

        // Act
        var settings = ArgumentOptions.Parse(config, args!);

        // Assert
        settings.OpenAI.ShouldNotBeNull();
        settings.OpenAI.ApiKey.ShouldBe(cliApiKey);
        settings.OpenAI.Model.ShouldBe(Model);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("config-key", "config-model")]
    public void Given_ConfigValues_And_No_CLI_When_Parse_Invoked_Then_It_Should_Use_Config(string configApiKey, string configModel)
    {
        // Arrange
        var config = BuildConfigWithOpenAI(configApiKey, configModel);
        var args = Array.Empty<string>();

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.OpenAI.ShouldNotBeNull();
        settings.OpenAI.ApiKey.ShouldBe(configApiKey);
        settings.OpenAI.Model.ShouldBe(configModel);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("config-key", "config-model", "cli-key", "cli-model")]
    public void Given_Config_And_CLI_When_Parse_Invoked_Then_It_Should_Use_CLI(
        string configApiKey, string configModel,
        string cliApiKey, string cliModel)
    {
        // Arrange
        var config = BuildConfigWithOpenAI(configApiKey, configModel);
        var args = new[] { ArgumentOptionConstants.OpenAI.ApiKey, cliApiKey, ArgumentOptionConstants.OpenAI.Model, cliModel };

        // Act
        var settings = ArgumentOptions.Parse(config, args!);

        // Assert
        settings.OpenAI.ShouldNotBeNull();
        settings.OpenAI.ApiKey.ShouldBe(cliApiKey);
        settings.OpenAI.Model.ShouldBe(cliModel);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("env-key", "env-model")]
    public void Given_EnvironmentVariables_And_No_Config_When_Parse_Invoked_Then_It_Should_Use_EnvironmentVariables(string envApiKey, string envModel)
    {
        // Arrange
        var config = BuildConfigWithOpenAI(
            configApiKey: null, configModel: null,
            envApiKey: envApiKey, envModel: envModel);
        var args = Array.Empty<string>();

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.OpenAI.ShouldNotBeNull();
        settings.OpenAI.ApiKey.ShouldBe(envApiKey);
        settings.OpenAI.Model.ShouldBe(envModel);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("config-key", "config-model", "env-key", "env-model")]
    public void Given_ConfigValues_And_EnvironmentVariables_When_Parse_Invoked_Then_It_Should_Use_EnvironmentVariables(
        string configApiKey, string configModel,
        string envApiKey, string envModel)
    {
        // Arrange
        var config = BuildConfigWithOpenAI(configApiKey, configModel, envApiKey, envModel);
        var args = Array.Empty<string>();

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.OpenAI.ShouldNotBeNull();
        settings.OpenAI.ApiKey.ShouldBe(envApiKey);
        settings.OpenAI.Model.ShouldBe(envModel);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("config-key", "config-model", "env-key", "env-model", "cli-key", "cli-model")]
    public void Given_ConfigValues_And_EnvironmentVariables_And_CLI_When_Parse_Invoked_Then_It_Should_Use_CLI(
        string configApiKey, string configModel,
        string envApiKey, string envModel,
        string cliApiKey, string cliModel)
    {
        // Arrange
        var config = BuildConfigWithOpenAI(configApiKey, configModel, envApiKey, envModel);
        var args = new[] { ArgumentOptionConstants.OpenAI.ApiKey, cliApiKey, ArgumentOptionConstants.OpenAI.Model, cliModel };

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.OpenAI.ShouldNotBeNull();
        settings.OpenAI.ApiKey.ShouldBe(cliApiKey);
        settings.OpenAI.Model.ShouldBe(cliModel);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("config-key", "config-model", null, "env-model")]
    public void Given_Partial_EnvironmentVariables_When_Parse_Invoked_Then_It_Should_Mix_Config_And_Environment(
        string configApiKey, string configModel,
        string? envApiKey, string envModel)
    {
        // Arrange
        var config = BuildConfigWithOpenAI(configApiKey, configModel, envApiKey, envModel);
        var args = Array.Empty<string>();

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.OpenAI.ShouldNotBeNull();
        settings.OpenAI.ApiKey.ShouldBe(configApiKey);
        settings.OpenAI.Model.ShouldBe(envModel);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("config-key", "config-model", "env-key", null, null, "cli-model")]
    public void Given_Mixed_Priority_Sources_When_Parse_Invoked_Then_It_Should_Respect_Priority_Order(
        string configApiKey, string configModel,
        string envApiKey, string? envModel,
        string? cliApiKey, string cliModel)
    {
        // Arrange
        var config = BuildConfigWithOpenAI(configApiKey, configModel, envApiKey, envModel);
        var args = new[] { ArgumentOptionConstants.OpenAI.ApiKey, cliApiKey, ArgumentOptionConstants.OpenAI.Model, cliModel };

        // Act
        var settings = ArgumentOptions.Parse(config, args!);

        // Assert
        settings.OpenAI.ShouldNotBeNull();
        settings.OpenAI.ApiKey.ShouldBe(envApiKey);
        settings.OpenAI.Model.ShouldBe(cliModel);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("cli-key", "cli-model")]
    public void Given_OpenAI_With_KnownArguments_When_Parse_Invoked_Then_Help_Should_Be_False(string cliApiKey, string cliModel)
    {
        // Arrange
        var config = BuildConfigWithOpenAI(ApiKey, Model);
        var args = new[] { ArgumentOptionConstants.OpenAI.ApiKey, cliApiKey, ArgumentOptionConstants.OpenAI.Model, cliModel };

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.Help.ShouldBeFalse();
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData(ArgumentOptionConstants.OpenAI.ApiKey)]
    [InlineData(ArgumentOptionConstants.OpenAI.Model)]
    public void Given_OpenAI_With_KnownArgument_WithoutValue_When_Parse_Invoked_Then_Help_Should_Be_False(string argument)
    {
        // Arrange
        var config = BuildConfigWithOpenAI();
        var args = new[] { argument };

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.Help.ShouldBeFalse();
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("cli-key", "--unknown-flag")]
    public void Given_OpenAI_With_Known_And_Unknown_Argument_When_Parse_Invoked_Then_Help_Should_Be_True(string cliApiKey, string unknown)
    {
        // Arrange
        var config = BuildConfigWithOpenAI();
        var args = new[] { ArgumentOptionConstants.OpenAI.ApiKey, cliApiKey, unknown };

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.Help.ShouldBeTrue();
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("cli-key", "cli-model")]
    public void Given_CLI_Only_When_Parse_Invoked_Then_Help_Should_Be_False(string cliApiKey, string cliModel)
    {
        // Arrange
        var config = BuildConfigWithOpenAI();
        var args = new[] { ArgumentOptionConstants.OpenAI.ApiKey, cliApiKey, ArgumentOptionConstants.OpenAI.Model, cliModel };

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.Help.ShouldBeFalse();
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("env-key", "env-model")]
    public void Given_EnvironmentVariables_Only_When_Parse_Invoked_Then_Help_Should_Be_False(string envApiKey, string envModel)
    {
        // Arrange
        var config = BuildConfigWithOpenAI(
            configApiKey: null, configModel: null,
            envApiKey: envApiKey, envModel: envModel);
        var args = Array.Empty<string>();

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.Help.ShouldBeFalse();
    }
}
