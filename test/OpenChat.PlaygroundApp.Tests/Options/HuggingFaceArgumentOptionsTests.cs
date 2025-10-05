using Microsoft.Extensions.Configuration;

using OpenChat.PlaygroundApp.Abstractions;
using OpenChat.PlaygroundApp.Connectors;
using OpenChat.PlaygroundApp.Constants;

namespace OpenChat.PlaygroundApp.Tests.Options;

public class HuggingFaceArgumentOptionsTests
{
    private const string BaseUrl = "https://test.huggingface.co/api";
    private const string Model = "hf-model-name";
    private const string BaseUrlConfigKey = "HuggingFace:BaseUrl";
    private const string ModelConfigKey = "HuggingFace:Model";

    private static IConfiguration BuildConfigWithHuggingFace(
        string? configBaseUrl = BaseUrl,
        string? configModel = Model,
        string? envBaseUrl = null,
        string? envModel = null
    )
    {
        // Base configuration values (lowest priority)
        var configDict = new Dictionary<string, string?>
        {
            [AppSettingConstants.ConnectorType] = ConnectorType.HuggingFace.ToString(),
        };

        if (string.IsNullOrWhiteSpace(configBaseUrl) == false)
        {
            configDict[BaseUrlConfigKey] = configBaseUrl;
        }
        if (string.IsNullOrWhiteSpace(configModel) == false)
        {
            configDict[ModelConfigKey] = configModel;
        }

        if (string.IsNullOrWhiteSpace(envBaseUrl) == true && string.IsNullOrWhiteSpace(envModel) == true)
        {
            return new ConfigurationBuilder()
                       .AddInMemoryCollection(configDict!)
                       .Build();
        }

        // Environment variables (medium priority)
        var envDict = new Dictionary<string, string?>();
        if (string.IsNullOrWhiteSpace(envBaseUrl) == false)
        {
            envDict[BaseUrlConfigKey] = envBaseUrl;
        }
        if (string.IsNullOrWhiteSpace(envModel) == false)
        {
            envDict[ModelConfigKey] = envModel;
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
        var config = BuildConfigWithHuggingFace();
        var args = Array.Empty<string>();

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.HuggingFace.ShouldNotBeNull();
        settings.HuggingFace.BaseUrl.ShouldBe(BaseUrl);
        settings.HuggingFace.Model.ShouldBe(Model);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("https://cli.huggingface.co/api")]
    public void Given_CLI_BaseUrl_When_Parse_Invoked_Then_It_Should_Use_CLI_BaseUrl(string cliBaseUrl)
    {
        // Arrange
        var config = BuildConfigWithHuggingFace();
        var args = new[] { ArgumentOptionConstants.HuggingFace.BaseUrl, cliBaseUrl };

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.HuggingFace.ShouldNotBeNull();
        settings.HuggingFace.BaseUrl.ShouldBe(cliBaseUrl);
        settings.HuggingFace.Model.ShouldBe(Model);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("cli-model")]
    public void Given_CLI_Model_When_Parse_Invoked_Then_It_Should_Use_CLI_Model(string cliModel)
    {
        // Arrange
        var config = BuildConfigWithHuggingFace();
        var args = new[] { ArgumentOptionConstants.HuggingFace.Model, cliModel };

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.HuggingFace.ShouldNotBeNull();
        settings.HuggingFace.BaseUrl.ShouldBe(BaseUrl);
        settings.HuggingFace.Model.ShouldBe(cliModel);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("https://cli.huggingface.co/api", "cli-model")]
    public void Given_All_CLI_Arguments_When_Parse_Invoked_Then_It_Should_Use_CLI(string cliBaseUrl, string cliModel)
    {
        // Arrange
        var config = BuildConfigWithHuggingFace();
        var args = new[] { ArgumentOptionConstants.HuggingFace.BaseUrl, cliBaseUrl, ArgumentOptionConstants.HuggingFace.Model, cliModel };

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.HuggingFace.ShouldNotBeNull();
        settings.HuggingFace.BaseUrl.ShouldBe(cliBaseUrl);
        settings.HuggingFace.Model.ShouldBe(cliModel);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData(ArgumentOptionConstants.HuggingFace.BaseUrl)]
    [InlineData(ArgumentOptionConstants.HuggingFace.Model)]
    public void Given_CLI_ArgumentWithoutValue_When_Parse_Invoked_Then_It_Should_Use_Config(string argument)
    {
        // Arrange
        var config = BuildConfigWithHuggingFace();
        var args = new[] { argument };

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.HuggingFace.ShouldNotBeNull();
        settings.HuggingFace.BaseUrl.ShouldBe(BaseUrl);
        settings.HuggingFace.Model.ShouldBe(Model);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("--something", "else", "--another", "value")]
    public void Given_Unrelated_CLI_Arguments_When_Parse_Invoked_Then_It_Should_Use_Config(params string[] args)
    {
        // Arrange
        var config = BuildConfigWithHuggingFace();

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.HuggingFace.ShouldNotBeNull();
        settings.HuggingFace.BaseUrl.ShouldBe(BaseUrl);
        settings.HuggingFace.Model.ShouldBe(Model);
    }


    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("--strange-model-name")]
    public void Given_HuggingFace_With_ModelName_StartingWith_Dashes_When_Parse_Invoked_Then_It_Should_Treat_As_Value(string model)
    {
        // Arrange
        var config = BuildConfigWithHuggingFace();
        var args = new[] { ArgumentOptionConstants.HuggingFace.Model, model };

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.HuggingFace.ShouldNotBeNull();
        settings.HuggingFace.Model.ShouldBe(model);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("https://config.huggingface.co/api", "config-model")]
    public void Given_ConfigValues_And_No_CLI_When_Parse_Invoked_Then_It_Should_Use_Config(string configBaseUrl, string configModel)
    {
        // Arrange
        var config = BuildConfigWithHuggingFace(configBaseUrl, configModel);
        var args = Array.Empty<string>();

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.HuggingFace.ShouldNotBeNull();
        settings.HuggingFace.BaseUrl.ShouldBe(configBaseUrl);
        settings.HuggingFace.Model.ShouldBe(configModel);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("https://config.huggingface.co/api", "config-model",
                "https://cli.huggingface.co/api", "cli-model")]
    public void Given_ConfigValues_And_CLI_When_Parse_Invoked_Then_It_Should_Use_CLI(
        string configBaseUrl, string configModel,
        string cliBaseUrl, string cliModel)
    {
        // Arrange
        var config = BuildConfigWithHuggingFace(configBaseUrl, configModel);
        var args = new[] { ArgumentOptionConstants.HuggingFace.BaseUrl, cliBaseUrl, ArgumentOptionConstants.HuggingFace.Model, cliModel };

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.HuggingFace.ShouldNotBeNull();
        settings.HuggingFace.BaseUrl.ShouldBe(cliBaseUrl);
        settings.HuggingFace.Model.ShouldBe(cliModel);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("https://env.huggingface.co/api", "env-model")]
    public void Given_EnvironmentVariables_And_No_Config_When_Parse_Invoked_Then_It_Should_Use_EnvironmentVariables(
        string envBaseUrl, string envModel)
    {
        // Arrange
        var config = BuildConfigWithHuggingFace(
            configBaseUrl: null, configModel: null,
            envBaseUrl: envBaseUrl, envModel: envModel
        );
        var args = Array.Empty<string>();

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.HuggingFace.ShouldNotBeNull();
        settings.HuggingFace.BaseUrl.ShouldBe(envBaseUrl);
        settings.HuggingFace.Model.ShouldBe(envModel);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("https://config.huggingface.co/api", "config-model",
                "https://env.huggingface.co/api", "env-model")]
    public void Given_ConfigValues_And_EnvironmentVariables_When_Parse_Invoked_Then_It_Should_Use_EnvironmentVariables(
        string configBaseUrl, string configModel,
        string envBaseUrl, string envModel)
    {
        // Arrange
        var config = BuildConfigWithHuggingFace(
            configBaseUrl, configModel,
            envBaseUrl, envModel);
        var args = Array.Empty<string>();

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.HuggingFace.ShouldNotBeNull();
        settings.HuggingFace.BaseUrl.ShouldBe(envBaseUrl);
        settings.HuggingFace.Model.ShouldBe(envModel);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("https://config.huggingface.co/api", "config-model",
                "https://env.huggingface.co/api", "env-model",
                "https://cli.huggingface.co/api", "cli-model")]
    public void Given_ConfigValues_And_EnvironmentVariables_And_CLI_When_Parse_Invoked_Then_It_Should_Use_CLI(
        string configBaseUrl, string configModel,
        string envBaseUrl, string envModel,
        string cliBaseUrl, string cliModel)
    {
        // Arrange
        var config = BuildConfigWithHuggingFace(
            configBaseUrl, configModel,
            envBaseUrl, envModel);
        var args = new[] { ArgumentOptionConstants.HuggingFace.BaseUrl, cliBaseUrl, ArgumentOptionConstants.HuggingFace.Model, cliModel };

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.HuggingFace.ShouldNotBeNull();
        settings.HuggingFace.BaseUrl.ShouldBe(cliBaseUrl);
        settings.HuggingFace.Model.ShouldBe(cliModel);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("https://config.huggingface.co/api", "config-model",
                "https://env.huggingface.co/api", null)]
    public void Given_Partial_EnvironmentVariables_When_Parse_Invoked_Then_It_Should_Mix_Config_And_Environment(
        string configBaseUrl, string configModel,
        string envBaseUrl, string? envModel)
    {
        // Arrange
        var config = BuildConfigWithHuggingFace(
            configBaseUrl, configModel,
            envBaseUrl, envModel);
        var args = Array.Empty<string>();

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.HuggingFace.ShouldNotBeNull();
        settings.HuggingFace.BaseUrl.ShouldBe(envBaseUrl);  // From environment
        settings.HuggingFace.Model.ShouldBe(configModel);   // From config (no env override)
    }


    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("https://config.huggingface.co/api", "config-model",
                null, "env-model",
                "https://cli.huggingface.co/api", null)]
    public void Given_Mixed_Priority_Sources_When_Parse_Invoked_Then_It_Should_Respect_Priority_Order(
        string configBaseUrl, string configModel,
        string? envBaseUrl, string envModel,
        string cliBaseUrl, string? cliModel)
    {
        // Arrange
        var config = BuildConfigWithHuggingFace(
            configBaseUrl, configModel,
            envBaseUrl, envModel);
        var args = new[] { ArgumentOptionConstants.HuggingFace.BaseUrl, cliBaseUrl, ArgumentOptionConstants.HuggingFace.Model, cliModel };

        // Act
        var settings = ArgumentOptions.Parse(config, args!);

        // Assert
        settings.HuggingFace.ShouldNotBeNull();
        settings.HuggingFace.BaseUrl.ShouldBe(cliBaseUrl);  // CLI wins (highest priority)
        settings.HuggingFace.Model.ShouldBe(envModel);      // Env wins over config (medium priority)
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("https://cli.huggingface.co/api", "cli-model")]
    public void Given_HuggingFace_With_KnownArguments_When_Parse_Invoked_Then_Help_Should_Be_False(string cliBaseUrl, string cliModel)
    {
        // Arrange
        var config = BuildConfigWithHuggingFace();
        var args = new[] { ArgumentOptionConstants.HuggingFace.BaseUrl, cliBaseUrl, ArgumentOptionConstants.HuggingFace.Model, cliModel };

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.Help.ShouldBeFalse();
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData(ArgumentOptionConstants.HuggingFace.BaseUrl)]
    [InlineData(ArgumentOptionConstants.HuggingFace.Model)]
    public void Given_HuggingFace_With_KnownArgument_WithoutValue_When_Parse_Invoked_Then_Help_Should_Be_False(string argument)
    {
        // Arrange
        var config = BuildConfigWithHuggingFace();
        var args = new[] { argument };

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.Help.ShouldBeFalse();
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("https://cli.huggingface.co/api", "--unknown-flag")]
    public void Given_HuggingFace_With_Known_And_Unknown_Argument_When_Parse_Invoked_Then_Help_Should_Be_True(string cliBaseUrl, string argument)
    {
        // Arrange
        var config = BuildConfigWithHuggingFace();
        var args = new[] { ArgumentOptionConstants.HuggingFace.BaseUrl, cliBaseUrl, argument };

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.Help.ShouldBeTrue();
    }


    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("https://env.huggingface.co/api", "env-model")]
    public void Given_EnvironmentVariables_Only_When_Parse_Invoked_Then_Help_Should_Be_False(
        string envBaseUrl, string envModel)
    {
        // Arrange
        var config = BuildConfigWithHuggingFace(
            configBaseUrl: null, configModel: null,
            envBaseUrl: envBaseUrl, envModel: envModel);
        var args = Array.Empty<string>();

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.Help.ShouldBeFalse();
    }


    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("https://cli.huggingface.co/api", "cli-model")]
    public void Given_CLI_Only_When_Parse_Invoked_Then_Help_Should_Be_False(string cliBaseUrl, string cliModel)
    {
        // Arrange
        var config = BuildConfigWithHuggingFace();
        var args = new[] { ArgumentOptionConstants.HuggingFace.BaseUrl, cliBaseUrl, ArgumentOptionConstants.HuggingFace.Model, cliModel };

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.Help.ShouldBeFalse();
    }
}

