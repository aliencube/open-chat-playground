using Microsoft.Extensions.Configuration;

using OpenChat.PlaygroundApp.Abstractions;
using OpenChat.PlaygroundApp.Connectors;
using OpenChat.PlaygroundApp.Constants;
using OpenChat.PlaygroundApp.Options;

namespace OpenChat.PlaygroundApp.Tests.Options;

public class DockerModelRunnerArgumentOptionsTests
{
    private const string BaseUrl = "http://test-dmr";
    private const string Model = "test-dmr-model";
    private const string BaseUrlConfigKey = "DockerModelRunner:BaseUrl";
    private const string ModelConfigKey = "DockerModelRunner:Model";

    private static IConfiguration BuildConfigWithDockerModelRunner(
        string? configBaseUrl = BaseUrl,
        string? configModel = Model,
        string? envBaseUrl = null,
        string? envModel = null
    )
    {
        // Base configuration (lowest priority)
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
        if (string.IsNullOrWhiteSpace(envBaseUrl) == true &&
            string.IsNullOrWhiteSpace(envModel) == true)
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
                   .AddInMemoryCollection(configDict!)   // Base configuration (lowest priority)
                   .AddInMemoryCollection(envDict!)      // Environment variables (medium priority)
                   .Build();
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData(typeof(ArgumentOptions), typeof(DockerModelRunnerArgumentOptions), true)]
    [InlineData(typeof(DockerModelRunnerArgumentOptions), typeof(ArgumentOptions), false)]
    public void Given_BaseType_Then_It_Should_Be_AssignableFrom_DerivedType(Type baseType, Type derivedType, bool expected)
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
    [InlineData("http://cli-dmr")]
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
    [InlineData("http://cli-dmr", "cli-model")]
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
    [InlineData("http://config-dmr", "config-model")]
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
    [InlineData("http://config-dmr", "config-model", 
                "http://cli-dmr", "cli-model")]
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

    
    [Theory]
    [InlineData("http://cli-dmr", "cli-model")]
    public void Given_DockerModelRunner_With_KnownArguments_When_Parse_Invoked_Then_Help_ShouldBe_False(string cliBaseUrl, string cliModel)
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
    public void Given_DockerModelRunner_With_KnownArgument_WithoutValue_When_Parse_Invoked_Then_Help_ShouldBe_False(string argument)
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
    [InlineData("http://cli-dmr", "--unknown-flag")]
    public void Given_DockerModelRunner_With_Known_And_Unknown_Argument_When_Parse_Invoked_Then_Help_ShouldBe_True(
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
    [InlineData("http://cli-dmr", "cli-model")]
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

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("http://env-dmr", "env-model")]
    public void Given_EnvironmentVariables_And_No_Config_When_Parse_Invoked_Then_It_Should_Use_EnvironmentVariables(
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
        settings.DockerModelRunner.ShouldNotBeNull();
        settings.DockerModelRunner.BaseUrl.ShouldBe(envBaseUrl);
        settings.DockerModelRunner.Model.ShouldBe(envModel);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("http://config-dmr", "config-model", "http://env-dmr", "env-model")]
    public void Given_ConfigValues_And_EnvironmentVariables_When_Parse_Invoked_Then_It_Should_Use_EnvironmentVariables(
        string configBaseUrl, string configModel,
        string envBaseUrl, string envModel)
    {
        // Arrange
        var config = BuildConfigWithDockerModelRunner(configBaseUrl, configModel, envBaseUrl, envModel);
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
    [InlineData("http://config-dmr", "config-model", "http://env-dmr", "env-model", "http://cli-dmr", "cli-model")]
    public void Given_ConfigValues_And_EnvironmentVariables_And_CLI_When_Parse_Invoked_Then_It_Should_Use_CLI(
        string configBaseUrl, string configModel,
        string envBaseUrl, string envModel,
        string cliBaseUrl, string cliModel)
    {
        // Arrange
        var config = BuildConfigWithDockerModelRunner(configBaseUrl, configModel, envBaseUrl, envModel);
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
    [InlineData("http://env-dmr", null)]
    [InlineData(null, "env-model")]
    public void Given_Partial_EnvironmentVariables_When_Parse_Invoked_Then_It_Should_Mix_Config_And_Environment(
        string? envBaseUrl, string? envModel)
    {
        // Arrange
        var config = BuildConfigWithDockerModelRunner(
            configBaseUrl: BaseUrl, configModel: Model,
            envBaseUrl: envBaseUrl, envModel: envModel);
        var args = Array.Empty<string>();

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.DockerModelRunner.ShouldNotBeNull();
        settings.DockerModelRunner.BaseUrl.ShouldBe(envBaseUrl ?? BaseUrl);
        settings.DockerModelRunner.Model.ShouldBe(envModel ?? Model);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("http://env-dmr", "env-model")]
    public void Given_EnvironmentVariables_Only_When_Parse_Invoked_Then_Help_Should_Be_False(string envBaseUrl, string envModel)
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
}