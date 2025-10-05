using Microsoft.Extensions.Configuration;

using OpenChat.PlaygroundApp.Abstractions;
using OpenChat.PlaygroundApp.Connectors;
using OpenChat.PlaygroundApp.Constants;
using OpenChat.PlaygroundApp.Options;

namespace OpenChat.PlaygroundApp.Tests.Options;

public class OllamaArgumentOptionsTests
{
    private const string BaseUrl = "http://test-ollama";
    private const string Model = "test-model";

    private static IConfiguration BuildConfigWithOllama(
        string? configBaseUrl = BaseUrl,
        string? configModel = Model,
        string? envBaseUrl = null,
        string? envModel = null
    )
    {
        // Base configuration values (lowest priority)
        var configDict = new Dictionary<string, string?>
        {
            ["ConnectorType"] = ConnectorType.Ollama.ToString(),
        };

        if (string.IsNullOrWhiteSpace(configBaseUrl) == false)
        {
            configDict["Ollama:BaseUrl"] = configBaseUrl;
        }
        if (string.IsNullOrWhiteSpace(configModel) == false)
        {
            configDict["Ollama:Model"] = configModel;
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
            envDict["Ollama:BaseUrl"] = envBaseUrl;
        }
        if (string.IsNullOrWhiteSpace(envModel) == false)
        {
            envDict["Ollama:Model"] = envModel;
        }

        return new ConfigurationBuilder()
                   .AddInMemoryCollection(configDict!)   // Base configuration (lowest priority)
                   .AddInMemoryCollection(envDict!)      // Environment variables (medium priority)
                   .Build();
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData(typeof(ArgumentOptions), typeof(OllamaArgumentOptions), true)]
    [InlineData(typeof(OllamaArgumentOptions), typeof(ArgumentOptions), false)]
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
        var config = BuildConfigWithOllama();
        var args = Array.Empty<string>();

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.Ollama.ShouldNotBeNull();
        settings.Ollama.BaseUrl.ShouldBe(BaseUrl);
        settings.Ollama.Model.ShouldBe(Model);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("http://cli-ollama")]
    public void Given_CLI_BaseUrl_When_Parse_Invoked_Then_It_Should_Use_CLI_BaseUrl(string cliBaseUrl)
    {
        // Arrange
        var config = BuildConfigWithOllama();
        var args = new[] { CommandLineArguments.Ollama.BaseUrl, cliBaseUrl };

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.Ollama.ShouldNotBeNull();
        settings.Ollama.BaseUrl.ShouldBe(cliBaseUrl);
        settings.Ollama.Model.ShouldBe(Model);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("cli-model")]
    public void Given_CLI_Model_When_Parse_Invoked_Then_It_Should_Use_CLI_Model(string cliModel)
    {
        // Arrange
        var config = BuildConfigWithOllama();
        var args = new[] { CommandLineArguments.Ollama.Model, cliModel };

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.Ollama.ShouldNotBeNull();
        settings.Ollama.BaseUrl.ShouldBe(BaseUrl);
        settings.Ollama.Model.ShouldBe(cliModel);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("http://cli-ollama", "cli-model")]
    public void Given_All_CLI_Arguments_When_Parse_Invoked_Then_It_Should_Use_CLI(string cliBaseUrl, string cliModel)
    {
        // Arrange
        var config = BuildConfigWithOllama();
        var args = new[] { CommandLineArguments.Ollama.BaseUrl, cliBaseUrl, CommandLineArguments.Ollama.Model, cliModel };

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.Ollama.ShouldNotBeNull();
        settings.Ollama.BaseUrl.ShouldBe(cliBaseUrl);
        settings.Ollama.Model.ShouldBe(cliModel);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData(CommandLineArguments.Ollama.BaseUrl)]
    [InlineData(CommandLineArguments.Ollama.Model)]
    public void Given_CLI_ArgumentWithoutValue_When_Parse_Invoked_Then_It_Should_Use_Config(string argument)
    {
        // Arrange
        var config = BuildConfigWithOllama();
        var args = new[] { argument };

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.Ollama.ShouldNotBeNull();
        settings.Ollama.BaseUrl.ShouldBe(BaseUrl);
        settings.Ollama.Model.ShouldBe(Model);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("--something", "else", "--another", "value")]
    public void Given_Unrelated_CLI_Arguments_When_Parse_Invoked_Then_It_Should_Use_Config(params string[] args)
    {
        // Arrange
        var config = BuildConfigWithOllama();

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.Ollama.ShouldNotBeNull();
        settings.Ollama.BaseUrl.ShouldBe(BaseUrl);
        settings.Ollama.Model.ShouldBe(Model);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("--strange-model-name")]
    public void Given_Ollama_With_ModelName_StartingWith_Dashes_When_Parse_Invoked_Then_It_Should_Treat_As_Value(string model)
    {
        // Arrange
        var config = BuildConfigWithOllama();
        var args = new[] { CommandLineArguments.Ollama.Model, model };

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.Ollama.ShouldNotBeNull();
        settings.Ollama.Model.ShouldBe(model);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("http://config-ollama", "config-model")]
    public void Given_ConfigValues_And_No_CLI_When_Parse_Invoked_Then_It_Should_Use_Config(string configBaseUrl, string configModel)
    {
        // Arrange
        var config = BuildConfigWithOllama(configBaseUrl, configModel);
        var args = Array.Empty<string>();

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.Ollama.ShouldNotBeNull();
        settings.Ollama.BaseUrl.ShouldBe(configBaseUrl);
        settings.Ollama.Model.ShouldBe(configModel);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("http://config-ollama", "config-model",
                "http://cli-ollama", "cli-model")]
    public void Given_ConfigValues_And_CLI_When_Parse_Invoked_Then_It_Should_Use_CLI(
        string configBaseUrl, string configModel,
        string cliBaseUrl, string cliModel)
    {
        // Arrange
        var config = BuildConfigWithOllama(configBaseUrl, configModel);
        var args = new[] { CommandLineArguments.Ollama.BaseUrl, cliBaseUrl, CommandLineArguments.Ollama.Model, cliModel };

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.Ollama.ShouldNotBeNull();
        settings.Ollama.BaseUrl.ShouldBe(cliBaseUrl);
        settings.Ollama.Model.ShouldBe(cliModel);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("http://cli-ollama", "cli-model")]
    public void Given_Ollama_With_KnownArguments_When_Parse_Invoked_Then_Help_Should_Be_False(string cliBaseUrl, string cliModel)
    {
        // Arrange
        var config = BuildConfigWithOllama();
        var args = new[] { CommandLineArguments.Ollama.BaseUrl, cliBaseUrl, CommandLineArguments.Ollama.Model, cliModel };

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.Help.ShouldBeFalse();
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData(CommandLineArguments.Ollama.BaseUrl)]
    [InlineData(CommandLineArguments.Ollama.Model)]
    public void Given_Ollama_With_KnownArgument_WithoutValue_When_Parse_Invoked_Then_Help_Should_Be_False(string argument)
    {
        // Arrange
        var config = BuildConfigWithOllama();
        var args = new[] { argument };

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.Help.ShouldBeFalse();
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("http://cli-ollama", "--unknown-flag")]
    public void Given_Ollama_With_Known_And_Unknown_Argument_When_Parse_Invoked_Then_Help_Should_Be_True(string cliBaseUrl, string argument)
    {
        // Arrange
        var config = BuildConfigWithOllama();
        var args = new[] { CommandLineArguments.Ollama.BaseUrl, cliBaseUrl, argument };

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.Help.ShouldBeTrue();
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("http://cli-ollama", "cli-model")]
    public void Given_CLI_Only_When_Parse_Invoked_Then_Help_Should_Be_False(string cliBaseUrl, string cliModel)
    {
        // Arrange
        var config = BuildConfigWithOllama();
        var args = new[] { CommandLineArguments.Ollama.BaseUrl, cliBaseUrl, CommandLineArguments.Ollama.Model, cliModel };

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.Help.ShouldBeFalse();
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("http://env-ollama", "env-model")]
    public void Given_EnvironmentVariables_And_No_Config_When_Parse_Invoked_Then_It_Should_Use_EnvironmentVariables(string envBaseUrl, string envModel)
    {
        // Arrange
        var config = BuildConfigWithOllama(
            configBaseUrl: null, configModel: null,
            envBaseUrl: envBaseUrl, envModel: envModel);
        var args = Array.Empty<string>();

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.Ollama.ShouldNotBeNull();
        settings.Ollama.BaseUrl.ShouldBe(envBaseUrl);
        settings.Ollama.Model.ShouldBe(envModel);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("http://config-ollama", "config-model", "http://env-ollama", "env-model")]
    public void Given_ConfigValues_And_EnvironmentVariables_When_Parse_Invoked_Then_It_Should_Use_EnvironmentVariables(
        string configBaseUrl, string configModel,
        string envBaseUrl, string envModel)
    {
        // Arrange
        var config = BuildConfigWithOllama(configBaseUrl, configModel, envBaseUrl, envModel);
        var args = Array.Empty<string>();

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.Ollama.ShouldNotBeNull();
        settings.Ollama.BaseUrl.ShouldBe(envBaseUrl);
        settings.Ollama.Model.ShouldBe(envModel);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("http://config-ollama", "config-model", "http://env-ollama", "env-model", "http://cli-ollama", "cli-model")]
    public void Given_ConfigValues_And_EnvironmentVariables_And_CLI_When_Parse_Invoked_Then_It_Should_Use_CLI(
        string configBaseUrl, string configModel,
        string envBaseUrl, string envModel,
        string cliBaseUrl, string cliModel)
    {
        // Arrange
        var config = BuildConfigWithOllama(configBaseUrl, configModel, envBaseUrl, envModel);
        var args = new[] { CommandLineArguments.Ollama.BaseUrl, cliBaseUrl, CommandLineArguments.Ollama.Model, cliModel };

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.Ollama.ShouldNotBeNull();
        settings.Ollama.BaseUrl.ShouldBe(cliBaseUrl);
        settings.Ollama.Model.ShouldBe(cliModel);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("http://env-ollama", null)]
    [InlineData(null, "env-model")]
    public void Given_Partial_EnvironmentVariables_When_Parse_Invoked_Then_It_Should_Mix_Config_And_Environment(
        string? envBaseUrl, string? envModel)
    {
        // Arrange
        var config = BuildConfigWithOllama(
            configBaseUrl: BaseUrl, configModel: Model,
            envBaseUrl: envBaseUrl, envModel: envModel);
        var args = Array.Empty<string>();

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.Ollama.ShouldNotBeNull();
        settings.Ollama.BaseUrl.ShouldBe(envBaseUrl ?? BaseUrl);
        settings.Ollama.Model.ShouldBe(envModel ?? Model);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("http://env-ollama", "env-model")]
    public void Given_EnvironmentVariables_Only_When_Parse_Invoked_Then_Help_Should_Be_False(string envBaseUrl, string envModel)
    {
        // Arrange
        var config = BuildConfigWithOllama(
            configBaseUrl: null, configModel: null,
            envBaseUrl: envBaseUrl, envModel: envModel);
        var args = Array.Empty<string>();

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.Help.ShouldBeFalse();
    }
}