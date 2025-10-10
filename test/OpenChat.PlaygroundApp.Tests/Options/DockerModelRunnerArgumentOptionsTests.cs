using Microsoft.Extensions.Configuration;

using OpenChat.PlaygroundApp.Abstractions;
using OpenChat.PlaygroundApp.Connectors;
using OpenChat.PlaygroundApp.Constants;

namespace OpenChat.PlaygroundApp.Tests.Options;

public class DockerModelRunnerArgumentOptionsTests
{
    private const string BaseUrl = "http://test-docker-model-runner";
    private const string Model = "docker-model-name";
    private const string BaseUrlConfigKey = "DockerModelRunner:BaseUrl";
    private const string ModelConfigKey = "DockerModelRunner:Model";

    private static IConfiguration BuildConfigWithDockerModelRunner(
        string? configBaseUrl = BaseUrl,
        string? configModel = Model,
        string? envBaseUrl = null,
        string? envModel = null
    )
    {
        // Base configuration values (lowest priority)
        var configDict = new Dictionary<string, string?>
        {
            [AppSettingConstants.ConnectorType] = ConnectorType.DockerModelRunner.ToString(),
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
        var config = BuildConfigWithDockerModelRunner();
        var args = Array.Empty<string>();

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.DockerModelRunner.ShouldNotBeNull();
        settings.DockerModelRunner.BaseUrl.ShouldBe(BaseUrl);
        settings.DockerModelRunner.Model.ShouldBe(Model);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("http://cli-docker-model-runner")]
    public void Given_CLI_BaseUrl_When_Parse_Invoked_Then_It_Should_Use_CLI_BaseUrl(string cliBaseUrl)
    {
        // Arrange
        var config = BuildConfigWithDockerModelRunner();
        var args = new[]
        {
            ArgumentOptionConstants.DockerModelRunner.BaseUrl, cliBaseUrl
        };

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.DockerModelRunner.ShouldNotBeNull();
        settings.DockerModelRunner.BaseUrl.ShouldBe(cliBaseUrl);
        settings.DockerModelRunner.Model.ShouldBe(Model);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("cli-model")]
    public void Given_CLI_Model_When_Parse_Invoked_Then_It_Should_Use_CLI_Model(string cliModel)
    {
        // Arrange
        var config = BuildConfigWithDockerModelRunner();
        var args = new[]
        {
            ArgumentOptionConstants.DockerModelRunner.Model, cliModel
        };

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.DockerModelRunner.ShouldNotBeNull();
        settings.DockerModelRunner.BaseUrl.ShouldBe(BaseUrl);
        settings.DockerModelRunner.Model.ShouldBe(cliModel);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("http://cli-docker-model-runner", "cli-model")]
    public void Given_All_CLI_Arguments_When_Parse_Invoked_Then_It_Should_Use_CLI(string cliBaseUrl, string cliModel)
    {
        // Arrange
        var config = BuildConfigWithDockerModelRunner();
        var args = new[]
        {
            ArgumentOptionConstants.DockerModelRunner.BaseUrl, cliBaseUrl,
            ArgumentOptionConstants.DockerModelRunner.Model, cliModel
        };

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.DockerModelRunner.ShouldNotBeNull();
        settings.DockerModelRunner.BaseUrl.ShouldBe(cliBaseUrl);
        settings.DockerModelRunner.Model.ShouldBe(cliModel);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData(ArgumentOptionConstants.DockerModelRunner.BaseUrl)]
    [InlineData(ArgumentOptionConstants.DockerModelRunner.Model)]
    public void Given_CLI_ArgumentWithoutValue_When_Parse_Invoked_Then_It_Should_Use_Config(string argument)
    {
        // Arrange
        var config = BuildConfigWithDockerModelRunner();
        var args = new[] { argument };

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.DockerModelRunner.ShouldNotBeNull();
        settings.DockerModelRunner.BaseUrl.ShouldBe(BaseUrl);
        settings.DockerModelRunner.Model.ShouldBe(Model);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("--something", "else", "--another", "value")]
    public void Given_Unrelated_CLI_Arguments_When_Parse_Invoked_Then_It_Should_Use_Config(params string[] args)
    {
        // Arrange
        var config = BuildConfigWithDockerModelRunner();

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.DockerModelRunner.ShouldNotBeNull();
        settings.DockerModelRunner.BaseUrl.ShouldBe(BaseUrl);
        settings.DockerModelRunner.Model.ShouldBe(Model);
    }


    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("--strange-model-name")]
    public void Given_DockerModelRunner_With_ModelName_StartingWith_Dashes_When_Parse_Invoked_Then_It_Should_Treat_As_Value(string model)
    {
        // Arrange
        var config = BuildConfigWithDockerModelRunner();
        var args = new[]
        {
            ArgumentOptionConstants.DockerModelRunner.Model, model
        };

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.DockerModelRunner.ShouldNotBeNull();
        settings.DockerModelRunner.Model.ShouldBe(model);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("http://config-docker-model-runner", "config-model")]
    public void Given_ConfigValues_And_No_CLI_When_Parse_Invoked_Then_It_Should_Use_Config(string configBaseUrl, string configModel)
    {
        // Arrange
        var config = BuildConfigWithDockerModelRunner(configBaseUrl, configModel);
        var args = Array.Empty<string>();

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.DockerModelRunner.ShouldNotBeNull();
        settings.DockerModelRunner.BaseUrl.ShouldBe(configBaseUrl);
        settings.DockerModelRunner.Model.ShouldBe(configModel);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("http://config-docker-model-runner", "config-model",
                "http://cli-docker-model-runner", "cli-model")]
    public void Given_ConfigValues_And_CLI_When_Parse_Invoked_Then_It_Should_Use_CLI(
        string configBaseUrl, string configModel,
        string cliBaseUrl, string cliModel)
    {
        // Arrange
        var config = BuildConfigWithDockerModelRunner(configBaseUrl, configModel);
        var args = new[]
        {
            ArgumentOptionConstants.DockerModelRunner.BaseUrl, cliBaseUrl,
            ArgumentOptionConstants.DockerModelRunner.Model, cliModel
        };

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.DockerModelRunner.ShouldNotBeNull();
        settings.DockerModelRunner.BaseUrl.ShouldBe(cliBaseUrl);
        settings.DockerModelRunner.Model.ShouldBe(cliModel);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("http://env-docker-model-runner", "env-model")]
    public void Given_EnvironmentVariables_And_No_Config_When_Parse_Invoked_Then_It_Should_Use_EnvironmentVariables(
        string envBaseUrl, string envModel)
    {
        // Arrange
        var config = BuildConfigWithDockerModelRunner(
            configBaseUrl: null, configModel: null,
            envBaseUrl: envBaseUrl, envModel: envModel
        );
        var args = Array.Empty<string>();

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.DockerModelRunner.ShouldNotBeNull();
        settings.DockerModelRunner.BaseUrl.ShouldBe(envBaseUrl);
        settings.DockerModelRunner.Model.ShouldBe(envModel);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("http://config-docker-model-runner", "config-model",
                "http://env-docker-model-runner", "env-model")]
    public void Given_ConfigValues_And_EnvironmentVariables_When_Parse_Invoked_Then_It_Should_Use_EnvironmentVariables(
        string configBaseUrl, string configModel,
        string envBaseUrl, string envModel)
    {
        // Arrange
        var config = BuildConfigWithDockerModelRunner(
            configBaseUrl, configModel,
            envBaseUrl, envModel);
        var args = Array.Empty<string>();

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.DockerModelRunner.ShouldNotBeNull();
        settings.DockerModelRunner.BaseUrl.ShouldBe(envBaseUrl);
        settings.DockerModelRunner.Model.ShouldBe(envModel);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("http://config-docker-model-runner", "config-model",
                "http://env-docker-model-runner", "env-model",
                "http://cli-docker-model-runner", "cli-model")]
    public void Given_ConfigValues_And_EnvironmentVariables_And_CLI_When_Parse_Invoked_Then_It_Should_Use_CLI(
        string configBaseUrl, string configModel,
        string envBaseUrl, string envModel,
        string cliBaseUrl, string cliModel)
    {
        // Arrange
        var config = BuildConfigWithDockerModelRunner(
            configBaseUrl, configModel,
            envBaseUrl, envModel);
        var args = new[]
        {
            ArgumentOptionConstants.DockerModelRunner.BaseUrl, cliBaseUrl,
            ArgumentOptionConstants.DockerModelRunner.Model, cliModel
        };

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.DockerModelRunner.ShouldNotBeNull();
        settings.DockerModelRunner.BaseUrl.ShouldBe(cliBaseUrl);
        settings.DockerModelRunner.Model.ShouldBe(cliModel);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("http://config-docker-model-runner", "config-model",
                "http://env-docker-model-runner", null)]
    public void Given_Partial_EnvironmentVariables_When_Parse_Invoked_Then_It_Should_Mix_Config_And_Environment(
        string configBaseUrl, string configModel,
        string envBaseUrl, string? envModel)
    {
        // Arrange
        var config = BuildConfigWithDockerModelRunner(
            configBaseUrl, configModel,
            envBaseUrl, envModel);
        var args = Array.Empty<string>();

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.DockerModelRunner.ShouldNotBeNull();
        settings.DockerModelRunner.BaseUrl.ShouldBe(envBaseUrl);  // From environment
        settings.DockerModelRunner.Model.ShouldBe(configModel);   // From config (no env override)
    }


    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("http://config-docker-model-runner", "config-model",
                null, "env-model",
                "http://cli-docker-model-runner", null)]
    public void Given_Mixed_Priority_Sources_When_Parse_Invoked_Then_It_Should_Respect_Priority_Order(
        string configBaseUrl, string configModel,
        string? envBaseUrl, string envModel,
        string cliBaseUrl, string? cliModel)
    {
        // Arrange
        var config = BuildConfigWithDockerModelRunner(
            configBaseUrl, configModel,
            envBaseUrl, envModel);
        var args = new[]
        {
            ArgumentOptionConstants.DockerModelRunner.BaseUrl, cliBaseUrl,
            ArgumentOptionConstants.DockerModelRunner.Model, cliModel
        };

        // Act
        var settings = ArgumentOptions.Parse(config, args!);

        // Assert
        settings.DockerModelRunner.ShouldNotBeNull();
        settings.DockerModelRunner.BaseUrl.ShouldBe(cliBaseUrl);  // CLI wins (highest priority)
        settings.DockerModelRunner.Model.ShouldBe(envModel);      // Env wins over config (medium priority)
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("http://cli-docker-model-runner", "cli-model")]
    public void Given_DockerModelRunner_With_KnownArguments_When_Parse_Invoked_Then_Help_Should_Be_False(
        string cliBaseUrl, string cliModel)
    {
        // Arrange
        var config = BuildConfigWithDockerModelRunner();
        var args = new[]
        {
            ArgumentOptionConstants.DockerModelRunner.BaseUrl, cliBaseUrl,
            ArgumentOptionConstants.DockerModelRunner.Model, cliModel
        };

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.Help.ShouldBeFalse();
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData(ArgumentOptionConstants.DockerModelRunner.BaseUrl)]
    [InlineData(ArgumentOptionConstants.DockerModelRunner.Model)]
    public void Given_DockerModelRunner_With_KnownArgument_WithoutValue_When_Parse_Invoked_Then_Help_Should_Be_False(string argument)
    {
        // Arrange
        var config = BuildConfigWithDockerModelRunner();
        var args = new[] { argument };

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.Help.ShouldBeFalse();
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("http://cli-docker-model-runner", "--unknown-flag")]
    public void Given_DockerModelRunner_With_Known_And_Unknown_Argument_When_Parse_Invoked_Then_Help_Should_Be_True(
        string cliBaseUrl, string argument)
    {
        // Arrange
        var config = BuildConfigWithDockerModelRunner();
        var args = new[]
        {
            ArgumentOptionConstants.DockerModelRunner.BaseUrl, cliBaseUrl,
            argument
        };

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.Help.ShouldBeTrue();
    }


    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("http://env-docker-model-runner", "env-model")]
    public void Given_EnvironmentVariables_Only_When_Parse_Invoked_Then_Help_Should_Be_False(
        string envBaseUrl, string envModel)
    {
        // Arrange
        var config = BuildConfigWithDockerModelRunner(
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
    [InlineData("http://cli-docker-model-runner", "cli-model")]
    public void Given_CLI_Only_When_Parse_Invoked_Then_Help_Should_Be_False(string cliBaseUrl, string cliModel)
    {
        // Arrange
        var config = BuildConfigWithDockerModelRunner();
        var args = new[]
        {
            ArgumentOptionConstants.DockerModelRunner.BaseUrl, cliBaseUrl,
            ArgumentOptionConstants.DockerModelRunner.Model, cliModel
        };

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.Help.ShouldBeFalse();
    }
}

