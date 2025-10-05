using Microsoft.Extensions.Configuration;

using OpenChat.PlaygroundApp.Abstractions;
using OpenChat.PlaygroundApp.Connectors;
using OpenChat.PlaygroundApp.Constants;

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
        // For future extensibility, but envDict is not used in this PR
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
        // Arrange
        var config = BuildConfigWithGoogleVertexAI();
        var args = Array.Empty<string>();

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.GoogleVertexAI.ShouldNotBeNull();
        settings.GoogleVertexAI.ApiKey.ShouldBe(ApiKey);
        settings.GoogleVertexAI.Model.ShouldBe(Model);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("cli-api-key")]
    public void Given_CLI_ApiKey_When_Parse_Invoked_Then_It_Should_Use_CLI_ApiKey(string cliApiKey)
    {
        // Arrange
        var config = BuildConfigWithGoogleVertexAI();
        var args = new[] { CommandLineArguments.GoogleVertexAI.ApiKey, cliApiKey };

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.GoogleVertexAI.ShouldNotBeNull();
        settings.GoogleVertexAI.ApiKey.ShouldBe(cliApiKey);
        settings.GoogleVertexAI.Model.ShouldBe(Model);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("cli-model")]
    public void Given_CLI_Model_When_Parse_Invoked_Then_It_Should_Use_CLI_Model(string cliModel)
    {
        // Arrange
        var config = BuildConfigWithGoogleVertexAI();
        var args = new[] { CommandLineArguments.GoogleVertexAI.Model, cliModel };

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.GoogleVertexAI.ShouldNotBeNull();
        settings.GoogleVertexAI.ApiKey.ShouldBe(ApiKey);
        settings.GoogleVertexAI.Model.ShouldBe(cliModel);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("cli-api-key", "cli-model")]
    public void Given_All_CLI_Arguments_When_Parse_Invoked_Then_It_Should_Use_CLI(string cliApiKey, string cliModel)
    {
        // Arrange
        var config = BuildConfigWithGoogleVertexAI();
        var args = new[] { CommandLineArguments.GoogleVertexAI.ApiKey, cliApiKey, CommandLineArguments.GoogleVertexAI.Model, cliModel };

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.GoogleVertexAI.ShouldNotBeNull();
        settings.GoogleVertexAI.ApiKey.ShouldBe(cliApiKey);
        settings.GoogleVertexAI.Model.ShouldBe(cliModel);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData(CommandLineArguments.GoogleVertexAI.ApiKey)]
    [InlineData(CommandLineArguments.GoogleVertexAI.Model)]
    public void Given_CLI_ArgumentWithoutValue_When_Parse_Invoked_Then_It_Should_Use_Config(string argument)
    {
        // Arrange
        var config = BuildConfigWithGoogleVertexAI();
        var args = new[] { argument };

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.GoogleVertexAI.ShouldNotBeNull();
        settings.GoogleVertexAI.ApiKey.ShouldBe(ApiKey);
        settings.GoogleVertexAI.Model.ShouldBe(Model);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("--something", "else", "--another", "value")]
    public void Given_Unrelated_CLI_Arguments_When_Parse_Invoked_Then_It_Should_Use_Config(params string[] args)
    {
        // Arrange
        var config = BuildConfigWithGoogleVertexAI();

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.GoogleVertexAI.ShouldNotBeNull();
        settings.GoogleVertexAI.ApiKey.ShouldBe(ApiKey);
        settings.GoogleVertexAI.Model.ShouldBe(Model);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("--strange-model-name")]
    public void Given_GoogleVertexAI_With_ModelName_StartingWith_Dashes_When_Parse_Invoked_Then_It_Should_Treat_As_Value(string model)
    {
        // Arrange
        var config = BuildConfigWithGoogleVertexAI();
        var args = new[] { CommandLineArguments.GoogleVertexAI.Model, model };

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.GoogleVertexAI.ShouldNotBeNull();
        settings.GoogleVertexAI.ApiKey.ShouldBe(ApiKey);
        settings.GoogleVertexAI.Model.ShouldBe(model);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("--strange-api-name")]
    public void Given_GoogleVertexAI_With_ApiKey_StartingWith_Dashes_When_Parse_Invoked_Then_It_Should_Treat_As_Value(string cliApiKey)
    {
        // Arrange
        var config = BuildConfigWithGoogleVertexAI();
        var args = new[] { CommandLineArguments.GoogleVertexAI.ApiKey, cliApiKey };

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.GoogleVertexAI.ShouldNotBeNull();
        settings.GoogleVertexAI.ApiKey.ShouldBe(cliApiKey);
        settings.GoogleVertexAI.Model.ShouldBe(Model);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("config-api-key", "config-model")]
    public void Given_ConfigValues_And_No_CLI_When_Parse_Invoked_Then_It_Should_Use_Config(string configApiKey, string configModel)
    {
        // Arrange
        var config = BuildConfigWithGoogleVertexAI(configApiKey, configModel);
        var args = Array.Empty<string>();

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.GoogleVertexAI.ShouldNotBeNull();
        settings.GoogleVertexAI.ApiKey.ShouldBe(configApiKey);
        settings.GoogleVertexAI.Model.ShouldBe(configModel);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("config-key", "config-model", "cli-key", "cli-model")]
    public void Given_Config_And_CLI_When_Parse_Invoked_Then_It_Should_Use_CLI(
        string configApiKey, string configModel,
        string cliApiKey, string cliModel)
    {
        // Arrange
        var config = BuildConfigWithGoogleVertexAI(configApiKey, configModel);
        var args = new[] { CommandLineArguments.GoogleVertexAI.ApiKey, cliApiKey, CommandLineArguments.GoogleVertexAI.Model, cliModel };

        // Act
        var settings = ArgumentOptions.Parse(config, args!);

        // Assert
        settings.GoogleVertexAI.ShouldNotBeNull();
        settings.GoogleVertexAI.ApiKey.ShouldBe(cliApiKey);
        settings.GoogleVertexAI.Model.ShouldBe(cliModel);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("env-key", "env-model")]
    public void Given_EnvironmentVariables_And_No_Config_When_Parse_Invoked_Then_It_Should_Use_EnvironmentVariables(string envApiKey, string envModel)
    {
        // Arrange
        var config = BuildConfigWithGoogleVertexAI(
            configApiKey: null, configModel: null,
            envApiKey: envApiKey, envModel: envModel);
        var args = Array.Empty<string>();

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.GoogleVertexAI.ShouldNotBeNull();
        settings.GoogleVertexAI.ApiKey.ShouldBe(envApiKey);
        settings.GoogleVertexAI.Model.ShouldBe(envModel);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("config-api-key", "config-model", "env-api-key", "env-model")]
    public void Given_ConfigValues_And_EnvironmentVariables_When_Parse_Invoked_Then_It_Should_Use_EnvironmentVariables(
        string configApiKey, string configModel,
        string envApiKey, string envModel)
    {
        // Arrange
        var config = BuildConfigWithGoogleVertexAI(configApiKey, configModel, envApiKey, envModel);
        var args = Array.Empty<string>();

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.GoogleVertexAI.ShouldNotBeNull();
        settings.GoogleVertexAI.ApiKey.ShouldBe(envApiKey);
        settings.GoogleVertexAI.Model.ShouldBe(envModel);
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
        var config = BuildConfigWithGoogleVertexAI(configApiKey, configModel, envApiKey, envModel);
        var args = new[] { CommandLineArguments.GoogleVertexAI.ApiKey, cliApiKey, CommandLineArguments.GoogleVertexAI.Model, cliModel };

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.GoogleVertexAI.ShouldNotBeNull();
        settings.GoogleVertexAI.ApiKey.ShouldBe(cliApiKey);
        settings.GoogleVertexAI.Model.ShouldBe(cliModel);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("config-api-key", "config-model", null, "env-model")]
    public void Given_Partial_EnvironmentVariables_When_Parse_Invoked_Then_It_Should_Mix_Config_And_Environment(
        string configApiKey, string configModel,
        string? envApiKey, string envModel)
    {
        // Arrange
        var config = BuildConfigWithGoogleVertexAI(configApiKey, configModel, envApiKey, envModel);
        var args = Array.Empty<string>();

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.GoogleVertexAI.ShouldNotBeNull();
        settings.GoogleVertexAI.ApiKey.ShouldBe(configApiKey); // From config (no env override)
        settings.GoogleVertexAI.Model.ShouldBe(envModel);      // From environment
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("config-api-key", "config-model", "env-key", null, null, "cli-model")]
    public void Given_Mixed_Priority_Sources_When_Parse_Invoked_Then_It_Should_Respect_Priority_Order(
        string configApiKey, string configModel,
        string envApiKey, string? envModel,
        string? cliApiKey, string cliModel)
    {
        // Arrange
        var config = BuildConfigWithGoogleVertexAI(configApiKey, configModel, envApiKey, envModel);
        var args = new[] { CommandLineArguments.GoogleVertexAI.ApiKey, cliApiKey, CommandLineArguments.GoogleVertexAI.Model, cliModel };

        // Act
        var settings = ArgumentOptions.Parse(config, args!);

        // Assert
        settings.GoogleVertexAI.ShouldNotBeNull();
        settings.GoogleVertexAI.ApiKey.ShouldBe(envApiKey);
        settings.GoogleVertexAI.Model.ShouldBe(cliModel);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("cli-api-key", "cli-model")]
    public void Given_GoogleVertexAI_With_KnownArguments_When_Parse_Invoked_Then_Help_ShouldBe_False(string cliApiKey, string cliModel)
    {
        // Arrange
        var config = BuildConfigWithGoogleVertexAI(ApiKey, Model);
        var args = new[] { CommandLineArguments.GoogleVertexAI.ApiKey, cliApiKey, CommandLineArguments.GoogleVertexAI.Model, cliModel };

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.Help.ShouldBeFalse();
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData(CommandLineArguments.GoogleVertexAI.ApiKey)]
    [InlineData(CommandLineArguments.GoogleVertexAI.Model)]
    public void Given_GoogleVertexAI_With_KnownArgument_WithoutValue_When_Parse_Invoked_Then_Help_ShouldBe_False(string argument)
    {
        // Arrange
        var config = BuildConfigWithGoogleVertexAI();
        var args = new[] { argument };

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.Help.ShouldBeFalse();
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("cli-api-key", "--unknown-flag")]
    public void Given_GoogleVertexAI_With_Known_And_Unknown_Argument_When_Parse_Invoked_Then_Help_ShouldBe_True(string cliApiKey, string argument)
    {
        // Arrange
        var config = BuildConfigWithGoogleVertexAI();
        var args = new[] { CommandLineArguments.GoogleVertexAI.ApiKey, cliApiKey, argument };

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
        var config = BuildConfigWithGoogleVertexAI(
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
        var config = BuildConfigWithGoogleVertexAI();
        var args = new[] { CommandLineArguments.GoogleVertexAI.ApiKey, cliApiKey, CommandLineArguments.GoogleVertexAI.Model, cliModel };

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.Help.ShouldBeFalse();
    }
}
