using Microsoft.Extensions.Configuration;

using OpenChat.PlaygroundApp.Abstractions;
using OpenChat.PlaygroundApp.Connectors;
using OpenChat.PlaygroundApp.Constants;
using OpenChat.PlaygroundApp.Options;

namespace OpenChat.PlaygroundApp.Tests.Options;

public class AnthropicArgumentOptionsTests
{
    private const string ApiKey = "test-api-key";
    private const string Model = "test-model";
    private const int MaxTokens = 512;
    private const string ApiKeyConfigKey = "Anthropic:ApiKey";
    private const string ModelConfigKey = "Anthropic:Model";
    private const string MaxTokensConfigKey = "Anthropic:MaxTokens";

    private static IConfiguration BuildConfigWithAnthropic(
        string? configApiKey = ApiKey,
        string? configModel = Model,
        int? configMaxTokens = MaxTokens,
        string? envApiKey = null,
        string? envModel = null,
        int? envMaxTokens = null)
    {
        // Base configuration values (lowest priority)
        var configDict = new Dictionary<string, string?>
        {
            [AppSettingConstants.ConnectorType] = ConnectorType.Anthropic.ToString()
        };

        if (string.IsNullOrWhiteSpace(configApiKey) == false)
        {
            configDict[ApiKeyConfigKey] = configApiKey;
        }
        if (string.IsNullOrWhiteSpace(configModel) == false)
        {
            configDict[ModelConfigKey] = configModel;
        }
        if (configMaxTokens.HasValue)
        {
            configDict[MaxTokensConfigKey] = configMaxTokens.Value.ToString();
        }
        if (string.IsNullOrWhiteSpace(envApiKey) == true &&
            string.IsNullOrWhiteSpace(envModel) == true &&
            envMaxTokens == null)
        {
            return new ConfigurationBuilder()
                       .AddInMemoryCollection(configDict!)
                       .Build();
        }

        // Environment variables (medium priority)
        var envDict = new Dictionary<string, string?>();
        if (string.IsNullOrWhiteSpace(envApiKey) == false)
        {
            envDict[ApiKeyConfigKey] = envApiKey;
        }
        if (string.IsNullOrWhiteSpace(envModel) == false)
        {
            envDict[ModelConfigKey] = envModel;
        }
        if (envMaxTokens.HasValue)
        {
            envDict[MaxTokensConfigKey] = envMaxTokens.Value.ToString();
        }

        return new ConfigurationBuilder()
                   .AddInMemoryCollection(configDict!)   // Base configuration (lowest priority)
                   .AddInMemoryCollection(envDict!)      // Environment variables (medium priority)
                   .Build();
    }
    
    private static int? IntValueOf(string? value) => string.IsNullOrWhiteSpace(value) ? null : int.Parse(value);

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData(typeof(ArgumentOptions), typeof(AnthropicArgumentOptions), true)]
    [InlineData(typeof(AnthropicArgumentOptions), typeof(ArgumentOptions), false)]
    public void Given_AnthropicArgumentOptions_When_Checking_Inheritance_Then_Should_Inherit_From_ArgumentOptions(
        Type baseType, Type derivedType, bool expected)
    {
        // Act
        var result = baseType.IsAssignableFrom(derivedType);

        // Assert
        result.ShouldBe(expected);
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
        settings.Anthropic.MaxTokens.ShouldBe(MaxTokens);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("cli-api-key")]
    public void Given_CLI_ApiKey_When_Parse_Invoked_Then_It_Should_Use_CLI_ApiKey(string cliApiKey)
    {
        // Arrange
        var config = BuildConfigWithAnthropic();
        var args = new[]
        {
            ArgumentOptionConstants.Anthropic.ApiKey, cliApiKey
        };

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.Anthropic.ShouldNotBeNull();
        settings.Anthropic.ApiKey.ShouldBe(cliApiKey);
        settings.Anthropic.Model.ShouldBe(Model);
        settings.Anthropic.MaxTokens.ShouldBe(MaxTokens);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("cli-model")]
    public void Given_CLI_Model_When_Parse_Invoked_Then_It_Should_Use_CLI_Model(string cliModel)
    {
        // Arrange
        var config = BuildConfigWithAnthropic();
        var args = new[]
        {
            ArgumentOptionConstants.Anthropic.Model, cliModel
        };

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.Anthropic.ShouldNotBeNull();
        settings.Anthropic.ApiKey.ShouldBe(ApiKey);
        settings.Anthropic.Model.ShouldBe(cliModel);
        settings.Anthropic.MaxTokens.ShouldBe(MaxTokens);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData(1024)]
    public void Given_CLI_MaxTokens_When_Parse_Invoked_Then_It_Should_Use_CLI_MaxTokens(int cliMaxTokens)
    {
        // Arrange
        var config = BuildConfigWithAnthropic();
        var args = new[]
        {
            ArgumentOptionConstants.Anthropic.MaxTokens, cliMaxTokens.ToString()
        };

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.Anthropic.ShouldNotBeNull();
        settings.Anthropic.ApiKey.ShouldBe(ApiKey);
        settings.Anthropic.Model.ShouldBe(Model);
        settings.Anthropic.MaxTokens.ShouldBe(cliMaxTokens);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("cli-api-key", "cli-model", 1024)]
    public void Given_All_CLI_Arguments_When_Parse_Invoked_Then_It_Should_Use_CLI(string cliApiKey, string cliModel, int cliMaxTokens)
    {
        // Arrange
        var config = BuildConfigWithAnthropic();
        var args = new[]
        {
            ArgumentOptionConstants.Anthropic.ApiKey, cliApiKey,
            ArgumentOptionConstants.Anthropic.Model, cliModel,
            ArgumentOptionConstants.Anthropic.MaxTokens, cliMaxTokens.ToString()
        };

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.Anthropic.ShouldNotBeNull();
        settings.Anthropic.ApiKey.ShouldBe(cliApiKey);
        settings.Anthropic.Model.ShouldBe(cliModel);
        settings.Anthropic.MaxTokens.ShouldBe(cliMaxTokens);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData(ArgumentOptionConstants.Anthropic.ApiKey)]
    [InlineData(ArgumentOptionConstants.Anthropic.Model)]
    [InlineData(ArgumentOptionConstants.Anthropic.MaxTokens)]
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
        settings.Anthropic.MaxTokens.ShouldBe(MaxTokens);
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
        settings.Anthropic.MaxTokens.ShouldBe(MaxTokens);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("--strange-api-key")]
    public void Given_Anthropic_With_ApiKey_StartingWith_Dashes_When_Parse_Invoked_Then_It_Should_Treat_As_Value(string cliApiKey)
    {
        // Arrange
        var config = BuildConfigWithAnthropic();
        var args = new[]
        {
            ArgumentOptionConstants.Anthropic.ApiKey, cliApiKey
        };

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.Anthropic.ShouldNotBeNull();
        settings.Anthropic.ApiKey.ShouldBe(cliApiKey);
        settings.Anthropic.Model.ShouldBe(Model);
        settings.Anthropic.MaxTokens.ShouldBe(MaxTokens);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("--strange-model-name")]
    public void Given_Anthropic_With_ModelName_StartingWith_Dashes_When_Parse_Invoked_Then_It_Should_Treat_As_Value(string cliModel)
    {
        // Arrange
        var config = BuildConfigWithAnthropic();
        var args = new[]
        {
            ArgumentOptionConstants.Anthropic.Model, cliModel
        };

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.Anthropic.ShouldNotBeNull();
        settings.Anthropic.ApiKey.ShouldBe(ApiKey);
        settings.Anthropic.Model.ShouldBe(cliModel);
        settings.Anthropic.MaxTokens.ShouldBe(MaxTokens);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData(-1)]
    public void Given_Anthropic_With_MaxTokens_StartingWith_Dashes_When_Parse_Invoked_Then_It_Should_Treat_As_Value(int cliMaxTokens)
    {
        // Arrange
        var config = BuildConfigWithAnthropic();
        var args = new[]
        {
            ArgumentOptionConstants.Anthropic.MaxTokens, cliMaxTokens.ToString()
        };

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.Anthropic.ShouldNotBeNull();
        settings.Anthropic.ApiKey.ShouldBe(ApiKey);
        settings.Anthropic.Model.ShouldBe(Model);
        settings.Anthropic.MaxTokens.ShouldBe(cliMaxTokens);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("not-a-number")]
    [InlineData("512abc")]
    public void Given_Anthropic_With_Invalid_MaxTokens_When_Parse_Invoked_Then_It_Should_Keep_Config(string cliMaxTokens)
    {
        // Arrange
        var config = BuildConfigWithAnthropic();
        var args = new[]
        {
            ArgumentOptionConstants.Anthropic.MaxTokens, cliMaxTokens
        };

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.Anthropic.ShouldNotBeNull();
        settings.Anthropic.ApiKey.ShouldBe(ApiKey);
        settings.Anthropic.Model.ShouldBe(Model);
        settings.Anthropic.MaxTokens.ShouldBe(MaxTokens);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("config-api-key", "config-model", 2048)]
    public void Given_ConfigValues_And_No_CLI_When_Parse_Invoked_Then_It_Should_Use_Config(string configApiKey, string configModel, int configMaxTokens)
    {
        // Arrange
        var config = BuildConfigWithAnthropic(configApiKey, configModel, configMaxTokens);
        var args = Array.Empty<string>();

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.Anthropic.ShouldNotBeNull();
        settings.Anthropic.ApiKey.ShouldBe(configApiKey);
        settings.Anthropic.Model.ShouldBe(configModel);
        settings.Anthropic.MaxTokens.ShouldBe(configMaxTokens);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("config-api-key", "config-model", 2048, "cli-api-key", "cli-model", 1024)]
    public void Given_ConfigValues_And_CLI_When_Parse_Invoked_Then_It_Should_Use_CLI(
        string configApiKey, string configModel, int configMaxTokens,
        string cliApiKey, string cliModel, int cliMaxTokens)
    {
        // Arrange
        var config = BuildConfigWithAnthropic(configApiKey, configModel, configMaxTokens);
        var args = new[]
        {
            ArgumentOptionConstants.Anthropic.ApiKey, cliApiKey,
            ArgumentOptionConstants.Anthropic.Model, cliModel,
            ArgumentOptionConstants.Anthropic.MaxTokens, cliMaxTokens.ToString()
        };

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.Anthropic.ShouldNotBeNull();
        settings.Anthropic.ApiKey.ShouldBe(cliApiKey);
        settings.Anthropic.Model.ShouldBe(cliModel);
        settings.Anthropic.MaxTokens.ShouldBe(cliMaxTokens);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("env-api-key", "env-model", 4096)]
    public void Given_EnvironmentVariables_And_No_Config_When_Parse_Invoked_Then_It_Should_Use_EnvironmentVariables(string envApiKey, string envModel, int envMaxTokens)
    {
        // Arrange
        var config = BuildConfigWithAnthropic(
            configApiKey: null, configModel: null, configMaxTokens: null,
            envApiKey: envApiKey, envModel: envModel, envMaxTokens: envMaxTokens);
        var args = Array.Empty<string>();

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.Anthropic.ShouldNotBeNull();
        settings.Anthropic.ApiKey.ShouldBe(envApiKey);
        settings.Anthropic.Model.ShouldBe(envModel);
        settings.Anthropic.MaxTokens.ShouldBe(envMaxTokens);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("config-api-key", "config-model", 2048, "env-api-key", "env-model", 4096)]
    public void Given_ConfigValues_And_EnvironmentVariables_When_Parse_Invoked_Then_It_Should_Use_EnvironmentVariables(
        string configApiKey, string configModel, int configMaxTokens,
        string envApiKey, string envModel, int envMaxTokens)
    {
        // Arrange
        var config = BuildConfigWithAnthropic(configApiKey, configModel, configMaxTokens, envApiKey, envModel, envMaxTokens);
        var args = Array.Empty<string>();

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.Anthropic.ShouldNotBeNull();
        settings.Anthropic.ApiKey.ShouldBe(envApiKey);
        settings.Anthropic.Model.ShouldBe(envModel);
        settings.Anthropic.MaxTokens.ShouldBe(envMaxTokens);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("config-api-key", "config-model", 2048, "env-api-key", "env-model", 4096, "cli-api-key", "cli-model", 1024)]
    public void Given_ConfigValues_And_EnvironmentVariables_And_CLI_When_Parse_Invoked_Then_It_Should_Use_CLI(
        string configApiKey, string configModel, int configMaxTokens,
        string envApiKey, string envModel, int envMaxTokens,
        string cliApiKey, string cliModel, int cliMaxTokens)
    {
        // Arrange
        var config = BuildConfigWithAnthropic(configApiKey, configModel, configMaxTokens, envApiKey, envModel, envMaxTokens);
        var args = new[]
        {
            ArgumentOptionConstants.Anthropic.ApiKey, cliApiKey,
            ArgumentOptionConstants.Anthropic.Model, cliModel,
            ArgumentOptionConstants.Anthropic.MaxTokens, cliMaxTokens.ToString()
        };

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.Anthropic.ShouldNotBeNull();
        settings.Anthropic.ApiKey.ShouldBe(cliApiKey);
        settings.Anthropic.Model.ShouldBe(cliModel);
        settings.Anthropic.MaxTokens.ShouldBe(cliMaxTokens);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("config-api-key", "config-model", 2048, null, "env-model", 4096)]
    public void Given_Partial_EnvironmentVariables_When_Parse_Invoked_Then_It_Should_Mix_Config_And_Environment(
        string configApiKey, string configModel, int configMaxTokens,
        string? envApiKey, string envModel, int envMaxTokens)
    {
        // Arrange
        var config = BuildConfigWithAnthropic(configApiKey, configModel, configMaxTokens, envApiKey, envModel, envMaxTokens);
        var args = Array.Empty<string>();

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.Anthropic.ShouldNotBeNull();
        settings.Anthropic.ApiKey.ShouldBe(configApiKey);
        settings.Anthropic.Model.ShouldBe(envModel);
        settings.Anthropic.MaxTokens.ShouldBe(envMaxTokens);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("config-api-key", "config-model", 2048, "env-api-key", null, 4096, null, "cli-model", 1024)]
    public void Given_Mixed_Priority_Sources_When_Parse_Invoked_Then_It_Should_Respect_Priority_Order(
        string configApiKey, string configModel, int configMaxTokens,
        string envApiKey, string? envModel, int envMaxTokens,
        string? cliApiKey, string cliModel, int cliMaxTokens)
    {
        // Arrange
        var config = BuildConfigWithAnthropic(configApiKey, configModel, configMaxTokens, envApiKey, envModel, envMaxTokens);
        var args = new[]
        {
            ArgumentOptionConstants.Anthropic.ApiKey, cliApiKey,
            ArgumentOptionConstants.Anthropic.Model, cliModel,
            ArgumentOptionConstants.Anthropic.MaxTokens, cliMaxTokens.ToString()
        };

        // Act
        var settings = ArgumentOptions.Parse(config, args!);

        // Assert
        settings.Anthropic.ShouldNotBeNull();
        settings.Anthropic.ApiKey.ShouldBe(envApiKey);
        settings.Anthropic.Model.ShouldBe(cliModel);
        settings.Anthropic.MaxTokens.ShouldBe(cliMaxTokens);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("cli-api-key", "cli-model", 1024)]
    public void Given_Anthropic_With_KnownArguments_When_Parse_Invoked_Then_Help_Should_Be_False(string cliApiKey, string cliModel, int cliMaxTokens)
    {
        // Arrange
        var config = BuildConfigWithAnthropic(ApiKey, Model, MaxTokens);
        var args = new[]
        {
            ArgumentOptionConstants.Anthropic.ApiKey, cliApiKey,
            ArgumentOptionConstants.Anthropic.Model, cliModel,
            ArgumentOptionConstants.Anthropic.MaxTokens, cliMaxTokens.ToString()
        };

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.Help.ShouldBeFalse();
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData(ArgumentOptionConstants.Anthropic.ApiKey)]
    [InlineData(ArgumentOptionConstants.Anthropic.Model)]
    [InlineData(ArgumentOptionConstants.Anthropic.MaxTokens)]
    public void Given_Anthropic_With_KnownArgument_WithoutValue_When_Parse_Invoked_Then_Help_Should_Be_False(string argument)
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
    public void Given_Anthropic_With_Known_ApiKey_And_Unknown_Argument_When_Parse_Invoked_Then_Help_Should_Be_True(
        string cliApiKey, string argument)
    {
        // Arrange
        var config = BuildConfigWithAnthropic();
        var args = new[]
        {
            ArgumentOptionConstants.Anthropic.ApiKey, cliApiKey,
            argument
        };

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.Help.ShouldBeTrue();
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("cli-api-key", "cli-model", 1024)]
    public void Given_CLI_Only_When_Parse_Invoked_Then_Help_Should_Be_False(string cliApiKey, string cliModel, int cliMaxTokens)
    {
        // Arrange
        var config = BuildConfigWithAnthropic();
        var args = new[]
        {
            ArgumentOptionConstants.Anthropic.ApiKey, cliApiKey,
            ArgumentOptionConstants.Anthropic.Model, cliModel,
            ArgumentOptionConstants.Anthropic.MaxTokens, cliMaxTokens.ToString()
        };

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.Help.ShouldBeFalse();
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("env-api-key", "env-model", 4096)]
    public void Given_EnvironmentVariables_Only_When_Parse_Invoked_Then_Help_Should_Be_False(string envApiKey, string envModel, int envMaxTokens)
    {
        // Arrange
        var config = BuildConfigWithAnthropic(
            configApiKey: null, configModel: null, configMaxTokens: null,
            envApiKey: envApiKey, envModel: envModel, envMaxTokens: envMaxTokens);
        var args = Array.Empty<string>();

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.Help.ShouldBeFalse();
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData(null, null, null, ConnectorType.Unknown, false)]
    public void Given_AnthropicArgumentOptions_When_Creating_Instance_Then_Should_Have_Correct_Properties(string? expectedApiKey, string? expectedModel, string? expectedMaxTokens, ConnectorType expectedConnectorType, bool expectedHelp)
    {
        // Act
        var options = new AnthropicArgumentOptions();

        // Assert
        options.ShouldNotBeNull();
        options.ApiKey.ShouldBe(expectedApiKey);
        options.Model.ShouldBe(expectedModel);
        options.MaxTokens.ShouldBe(IntValueOf(expectedMaxTokens));
        options.ConnectorType.ShouldBe(expectedConnectorType);
        options.Help.ShouldBe(expectedHelp);
    }
}