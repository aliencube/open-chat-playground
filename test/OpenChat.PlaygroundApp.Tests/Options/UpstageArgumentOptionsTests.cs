using Microsoft.Extensions.Configuration;

using OpenChat.PlaygroundApp.Abstractions;
using OpenChat.PlaygroundApp.Connectors;
using OpenChat.PlaygroundApp.Constants;

namespace OpenChat.PlaygroundApp.Tests.Options;

public class UpstageArgumentOptionsTests
{
    private const string BaseUrl = "https://test.upstage";
    private const string ApiKey = "test-api-key";
    private const string Model = "test-model";

    private static IConfiguration BuildConfigWithUpstage(
        string? configBaseUrl = BaseUrl,
        string? configApiKey = ApiKey,
        string? configModel = Model,
        string? envBaseUrl = null,
        string? envApiKey = null,
        string? envModel = null)
    {
        // Base configuration values
        var configDict = new Dictionary<string, string?>
        {
            ["ConnectorType"] = ConnectorType.Upstage.ToString()
        };

        if (string.IsNullOrWhiteSpace(configBaseUrl) == false)
        {
            configDict["Upstage:BaseUrl"] = configBaseUrl;
        }
        if (string.IsNullOrWhiteSpace(configApiKey) == false)
        {
            configDict["Upstage:ApiKey"] = configApiKey;
        }
        if (string.IsNullOrWhiteSpace(configModel) == false)
        {
            configDict["Upstage:Model"] = configModel;
        }
        if (string.IsNullOrWhiteSpace(envBaseUrl) == true &&
           string.IsNullOrWhiteSpace(envApiKey) == true &&
           string.IsNullOrWhiteSpace(envModel) == true)
        {
            return new ConfigurationBuilder()
                        .AddInMemoryCollection(configDict!)
                        .Build();
        }

        var envDict = new Dictionary<string, string?>();
        if (string.IsNullOrWhiteSpace(envBaseUrl) == false)
        {
            envDict["Upstage:BaseUrl"] = envBaseUrl;
        }
        if (string.IsNullOrWhiteSpace(envApiKey) == false)
        {
            envDict["Upstage:ApiKey"] = envApiKey;
        }
        if (string.IsNullOrWhiteSpace(envModel) == false)
        {
            envDict["Upstage:Model"] = envModel;
        }

        return new ConfigurationBuilder()
                    .AddInMemoryCollection(configDict!)
                    .AddInMemoryCollection(envDict)
                    .Build();
    }

    [Trait("Category", "UnitTest")]
    [Fact]
    public void Given_Nothing_When_Parse_Invoked_Then_It_Should_Set_Config()
    {
        // Arrange
        var config = BuildConfigWithUpstage();
        var args = Array.Empty<string>();

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.Upstage.ShouldNotBeNull();
        settings.Upstage.BaseUrl.ShouldBe(BaseUrl);
        settings.Upstage.ApiKey.ShouldBe(ApiKey);
        settings.Upstage.Model.ShouldBe(Model);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("https://cli.upstage.ai/api/v1")]
    public void Given_CLI_BaseUrl_When_Parse_Invoked_Then_It_Should_Use_CLI_BaseUrl(string cliBaseUrl)
    {
        // Arrange
        var config = BuildConfigWithUpstage();
        var args = new[] { ArgumentOptionConstants.Upstage.BaseUrl, cliBaseUrl };

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.Upstage.ShouldNotBeNull();
        settings.Upstage.BaseUrl.ShouldBe(cliBaseUrl);
        settings.Upstage.ApiKey.ShouldBe(ApiKey);
        settings.Upstage.Model.ShouldBe(Model);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("cli-api-key")]
    public void Given_CLI_ApiKey_When_Parse_Invoked_Then_It_Should_Use_CLI_ApiKey(string cliApiKey)
    {
        // Arrange
        var config = BuildConfigWithUpstage();
        var args = new[] { ArgumentOptionConstants.Upstage.ApiKey, cliApiKey };

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.Upstage.ShouldNotBeNull();
        settings.Upstage.BaseUrl.ShouldBe(BaseUrl);
        settings.Upstage.ApiKey.ShouldBe(cliApiKey);
        settings.Upstage.Model.ShouldBe(Model);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("cli-model")]
    public void Given_CLI_Model_When_Parse_Invoked_Then_It_Should_Use_CLI_Model(string cliModel)
    {
        // Arrange
        var config = BuildConfigWithUpstage();
        var args = new[] { ArgumentOptionConstants.Upstage.Model, cliModel };

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.Upstage.ShouldNotBeNull();
        settings.Upstage.BaseUrl.ShouldBe(BaseUrl);
        settings.Upstage.ApiKey.ShouldBe(ApiKey);
        settings.Upstage.Model.ShouldBe(cliModel);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("https://cli.upstage.ai/api/v1", "cli-api-key", "cli-model")]
    public void Given_All_CLI_Arguments_When_Parse_Invoked_Then_It_Should_Use_CLI(string cliBaseUrl, string cliApiKey, string cliModel)
    {
        // Arrange
        var config = BuildConfigWithUpstage();
        var args = new[] { ArgumentOptionConstants.Upstage.BaseUrl, cliBaseUrl, ArgumentOptionConstants.Upstage.ApiKey, cliApiKey, ArgumentOptionConstants.Upstage.Model, cliModel };

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.Upstage.ShouldNotBeNull();
        settings.Upstage.BaseUrl.ShouldBe(cliBaseUrl);
        settings.Upstage.ApiKey.ShouldBe(cliApiKey);
        settings.Upstage.Model.ShouldBe(cliModel);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData(ArgumentOptionConstants.Upstage.BaseUrl)]
    [InlineData(ArgumentOptionConstants.Upstage.ApiKey)]
    [InlineData(ArgumentOptionConstants.Upstage.Model)]
    public void Given_CLI_ArgumentWithoutValue_When_Parse_Invoked_Then_It_Should_Use_Config(string argument)
    {
        // Arrange
        var config = BuildConfigWithUpstage();
        var args = new[] { argument };

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.Upstage.ShouldNotBeNull();
        settings.Upstage.BaseUrl.ShouldBe(BaseUrl);
        settings.Upstage.ApiKey.ShouldBe(ApiKey);
        settings.Upstage.Model.ShouldBe(Model);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("--something", "else", "--another", "value")]
    public void Given_Unrelated_CLI_Arguments_When_Parse_Invoked_Then_It_Should_Use_Config(params string[] args)
    {
        // Arrange
        var config = BuildConfigWithUpstage();

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.Upstage.ShouldNotBeNull();
        settings.Upstage.BaseUrl.ShouldBe(BaseUrl);
        settings.Upstage.ApiKey.ShouldBe(ApiKey);
        settings.Upstage.Model.ShouldBe(Model);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("--strange-model-name")]
    public void Given_Upstage_With_ModelName_StartingWith_Dashes_When_Parse_Invoked_Then_It_Should_Treat_As_Value(string model)
    {
        // Arrange
        var config = BuildConfigWithUpstage();
        var args = new[] { ArgumentOptionConstants.Upstage.Model, model };

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.Upstage.ShouldNotBeNull();
        settings.Upstage.Model.ShouldBe(model);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("https://config.upstage.ai/api/v1", "config-api-key", "config-model")]
    public void Given_ConfigValues_And_No_CLI_When_Parse_Invoked_Then_It_Should_Use_Config(string configBaseUrl, string configApiKey, string configModel)
    {
        // Arrange
        var config = BuildConfigWithUpstage(configBaseUrl, configApiKey, configModel);
        var args = Array.Empty<string>();

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.Upstage.ShouldNotBeNull();
        settings.Upstage.BaseUrl.ShouldBe(configBaseUrl);
        settings.Upstage.ApiKey.ShouldBe(configApiKey);
        settings.Upstage.Model.ShouldBe(configModel);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("https://config.upstage.ai/api/v1", "config-api-key", "config-model",
                "https://cli.upstage.ai/api/v1", "cli-api-key", "cli-model")]
    public void Given_ConfigValues_And_CLI_When_Parse_Invoked_Then_It_Should_Use_CLI(
        string configBaseUrl, string configApiKey, string configModel,
        string cliBaseUrl, string cliApiKey, string cliModel)
    {
        // Arrange
        var config = BuildConfigWithUpstage(configBaseUrl, configApiKey, configModel);
        var args = new[] { ArgumentOptionConstants.Upstage.BaseUrl, cliBaseUrl, ArgumentOptionConstants.Upstage.ApiKey, cliApiKey, ArgumentOptionConstants.Upstage.Model, cliModel };

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.Upstage.ShouldNotBeNull();
        settings.Upstage.BaseUrl.ShouldBe(cliBaseUrl);
        settings.Upstage.ApiKey.ShouldBe(cliApiKey);
        settings.Upstage.Model.ShouldBe(cliModel);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("https://cli.upstage.ai/api/v1", "cli-api-key", "cli-model")]
    public void Given_Upstage_With_KnownArguments_When_Parse_Invoked_Then_Help_ShouldBe_False(string cliBaseUrl, string cliApiKey, string cliModel)
    {
        // Arrange
        var config = BuildConfigWithUpstage(BaseUrl, ApiKey, Model);
        var args = new[] { ArgumentOptionConstants.Upstage.BaseUrl, cliBaseUrl, ArgumentOptionConstants.Upstage.ApiKey, cliApiKey, ArgumentOptionConstants.Upstage.Model, cliModel };

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.Help.ShouldBeFalse();
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData(ArgumentOptionConstants.Upstage.BaseUrl)]
    [InlineData(ArgumentOptionConstants.Upstage.ApiKey)]
    [InlineData(ArgumentOptionConstants.Upstage.Model)]
    public void Given_Upstage_With_KnownArgument_WithoutValue_When_Parse_Invoked_Then_Help_ShouldBe_False(string argument)
    {
        // Arrange
        var config = BuildConfigWithUpstage();
        var args = new[] { argument };

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.Help.ShouldBeFalse();
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("https://cli.upstage.ai/api/v1", "--unknown-flag")]
    public void Given_Upstage_With_Known_And_Unknown_Argument_When_Parse_Invoked_Then_Help_ShouldBe_True(string cliBaseUrl, string argument)
    {
        // Arrange
        var config = BuildConfigWithUpstage();
        var args = new[] { ArgumentOptionConstants.Upstage.BaseUrl, cliBaseUrl, argument };

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.Help.ShouldBeTrue();
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("https://cli.upstage.ai/api/v1", "cli-api-key", "cli-model")]
    public void Given_CLI_Only_When_Parse_Invoked_Then_Help_Should_Be_False(string cliBaseUrl, string cliApiKey, string cliModel)
    {
        // Arrange
        var config = BuildConfigWithUpstage();
        var args = new[] { ArgumentOptionConstants.Upstage.BaseUrl, cliBaseUrl, ArgumentOptionConstants.Upstage.ApiKey, cliApiKey, ArgumentOptionConstants.Upstage.Model, cliModel };

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.Help.ShouldBeFalse();
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("https://env.upstage.ai/api/v1", "env-api-key", "env-model")]
    public void Given_Environment_Variables_When_Parse_Invoked_Then_It_Should_Use_Environment_Variables(string envBaseUrl, string envApiKey, string envModel)
    {
        // Arrange
        var config = BuildConfigWithUpstage(envBaseUrl: envBaseUrl, envApiKey: envApiKey, envModel: envModel);
        var args = Array.Empty<string>();

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.Upstage.ShouldNotBeNull();
        settings.Upstage.BaseUrl.ShouldBe(envBaseUrl);
        settings.Upstage.ApiKey.ShouldBe(envApiKey);
        settings.Upstage.Model.ShouldBe(envModel);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("https://env.upstage.ai/api/v1", "env-api-key", "env-model",
                "https://cli.upstage.ai/api/v1", "cli-api-key", "cli-model")]
    public void Given_Environment_Variables_And_CLI_When_Parse_Invoked_Then_It_Should_Prioritize_CLI(
        string envBaseUrl, string envApiKey, string envModel,
        string cliBaseUrl, string cliApiKey, string cliModel)
    {
        // Arrange
        var config = BuildConfigWithUpstage(envBaseUrl: envBaseUrl, envApiKey: envApiKey, envModel: envModel);
        var args = new[] { ArgumentOptionConstants.Upstage.BaseUrl, cliBaseUrl, ArgumentOptionConstants.Upstage.ApiKey, cliApiKey, ArgumentOptionConstants.Upstage.Model, cliModel };

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.Upstage.ShouldNotBeNull();
        settings.Upstage.BaseUrl.ShouldBe(cliBaseUrl); // CLI should win
        settings.Upstage.ApiKey.ShouldBe(cliApiKey);   // CLI should win
        settings.Upstage.Model.ShouldBe(cliModel);     // CLI should win
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("https://config.upstage.ai/api/v1", "config-api-key", "config-model",
                "https://env.upstage.ai/api/v1", "env-api-key", "env-model")]
    public void Given_Config_And_Environment_Variables_When_Parse_Invoked_Then_It_Should_Prioritize_Environment(
        string configBaseUrl, string configApiKey, string configModel,
        string envBaseUrl, string envApiKey, string envModel)
    {
        // Arrange
        var config = BuildConfigWithUpstage(configBaseUrl, configApiKey, configModel, envBaseUrl, envApiKey, envModel);
        var args = Array.Empty<string>();

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.Upstage.ShouldNotBeNull();
        settings.Upstage.BaseUrl.ShouldBe(envBaseUrl); // Environment should win over config
        settings.Upstage.ApiKey.ShouldBe(envApiKey);   // Environment should win over config
        settings.Upstage.Model.ShouldBe(envModel);     // Environment should win over config
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("https://config.upstage.ai/api/v1", "config-api-key", "config-model",
                "https://env.upstage.ai/api/v1", "env-api-key", "env-model",
                "https://cli.upstage.ai/api/v1", "cli-api-key", "cli-model")]
    public void Given_All_Three_Sources_When_Parse_Invoked_Then_It_Should_Follow_Priority_Order(
        string configBaseUrl, string configApiKey, string configModel,
        string envBaseUrl, string envApiKey, string envModel,
        string cliBaseUrl, string cliApiKey, string cliModel)
    {
        // Arrange
        var config = BuildConfigWithUpstage(configBaseUrl, configApiKey, configModel, envBaseUrl, envApiKey, envModel);
        var args = new[] { ArgumentOptionConstants.Upstage.BaseUrl, cliBaseUrl, ArgumentOptionConstants.Upstage.ApiKey, cliApiKey, ArgumentOptionConstants.Upstage.Model, cliModel };

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert - CLI should have highest priority
        settings.Upstage.ShouldNotBeNull();
        settings.Upstage.BaseUrl.ShouldBe(cliBaseUrl);
        settings.Upstage.ApiKey.ShouldBe(cliApiKey);
        settings.Upstage.Model.ShouldBe(cliModel);
    }
}
