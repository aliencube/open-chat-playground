using Microsoft.Extensions.Configuration;

using OpenChat.PlaygroundApp.Abstractions;
using OpenChat.PlaygroundApp.Connectors;
using OpenChat.PlaygroundApp.Constants;
using OpenChat.PlaygroundApp.Options;

namespace OpenChat.PlaygroundApp.Tests.Options;

public class GitHubModelsArgumentOptionsTests
{
    private const string Endpoint = "https://test.github-models/inference";
    private const string Token = "github-pat";
    private const string Model = "github-model-name";
    private const string EndpointConfigKey = "GitHubModels:Endpoint";
    private const string TokenConfigKey = "GitHubModels:Token";
    private const string ModelConfigKey = "GitHubModels:Model";

    private static IConfiguration BuildConfigWithGitHubModels(
        string? configEndpoint = Endpoint,
        string? configToken = Token,
        string? configModel = Model,
        string? envEndpoint = null,
        string? envToken = null,
        string? envModel = null)
    {
        // Base configuration values (lowest priority)
        var configDict = new Dictionary<string, string?>
        {
            [AppSettingConstants.ConnectorType] = ConnectorType.GitHubModels.ToString()
        };

        if (string.IsNullOrWhiteSpace(configEndpoint) == false)
        {
            configDict[EndpointConfigKey] = configEndpoint;
        }
        if (string.IsNullOrWhiteSpace(configToken) == false)
        {
            configDict[TokenConfigKey] = configToken;
        }
        if (string.IsNullOrWhiteSpace(configModel) == false)
        {
            configDict[ModelConfigKey] = configModel;
        }

        if (string.IsNullOrWhiteSpace(envEndpoint) == true &&
            string.IsNullOrWhiteSpace(envToken) == true &&
            string.IsNullOrWhiteSpace(envModel) == true)
        {
            return new ConfigurationBuilder()
                       .AddInMemoryCollection(configDict!)
                       .Build();
        }

        // Environment variables (medium priority)
        var envDict = new Dictionary<string, string?>();
        if (string.IsNullOrWhiteSpace(envEndpoint) == false)
        {
            envDict[EndpointConfigKey] = envEndpoint;
        }
        if (string.IsNullOrWhiteSpace(envToken) == false)
        {
            envDict[TokenConfigKey] = envToken;
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
    [Theory]
    [InlineData(typeof(ArgumentOptions), typeof(GitHubModelsArgumentOptions), true)]
    [InlineData(typeof(GitHubModelsArgumentOptions), typeof(ArgumentOptions), false)]
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
        var config = BuildConfigWithGitHubModels();
        var args = Array.Empty<string>();

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.GitHubModels.ShouldNotBeNull();
        settings.GitHubModels.Endpoint.ShouldBe(Endpoint);
        settings.GitHubModels.Token.ShouldBe(Token);
        settings.GitHubModels.Model.ShouldBe(Model);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("https://cli.github-models/inference")]
    public void Given_CLI_Endpoint_When_Parse_Invoked_Then_It_Should_Use_CLI_Endpoint(string cliEndpoint)
    {
        // Arrange
        var config = BuildConfigWithGitHubModels();
        var args = new[]
        {
            ArgumentOptionConstants.GitHubModels.Endpoint, cliEndpoint
        };

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.GitHubModels.ShouldNotBeNull();
        settings.GitHubModels.Endpoint.ShouldBe(cliEndpoint);
        settings.GitHubModels.Token.ShouldBe(Token);
        settings.GitHubModels.Model.ShouldBe(Model);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("cli-token")]
    public void Given_CLI_Token_When_Parse_Invoked_Then_It_Should_Use_CLI_Token(string cliToken)
    {
        // Arrange
        var config = BuildConfigWithGitHubModels();
        var args = new[]
        {
            ArgumentOptionConstants.GitHubModels.Token, cliToken
        };

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.GitHubModels.ShouldNotBeNull();
        settings.GitHubModels.Endpoint.ShouldBe(Endpoint);
        settings.GitHubModels.Token.ShouldBe(cliToken);
        settings.GitHubModels.Model.ShouldBe(Model);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("cli-model")]
    public void Given_CLI_Model_When_Parse_Invoked_Then_It_Should_Use_CLI_Model(string cliModel)
    {
        // Arrange
        var config = BuildConfigWithGitHubModels();
        var args = new[]
        {
            ArgumentOptionConstants.GitHubModels.Model, cliModel
        };

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.GitHubModels.ShouldNotBeNull();
        settings.GitHubModels.Endpoint.ShouldBe(Endpoint);
        settings.GitHubModels.Token.ShouldBe(Token);
        settings.GitHubModels.Model.ShouldBe(cliModel);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("https://cli.github-models/inference", "cli-token", "cli-model")]
    public void Given_All_CLI_Arguments_When_Parse_Invoked_Then_It_Should_Use_CLI(
        string cliEndpoint, string cliToken, string cliModel)
    {
        // Arrange
        var config = BuildConfigWithGitHubModels();
        var args = new[]
        {
            ArgumentOptionConstants.GitHubModels.Endpoint, cliEndpoint,
            ArgumentOptionConstants.GitHubModels.Token, cliToken,
            ArgumentOptionConstants.GitHubModels.Model, cliModel
        };

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.GitHubModels.ShouldNotBeNull();
        settings.GitHubModels.Endpoint.ShouldBe(cliEndpoint);
        settings.GitHubModels.Token.ShouldBe(cliToken);
        settings.GitHubModels.Model.ShouldBe(cliModel);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData(ArgumentOptionConstants.GitHubModels.Endpoint)]
    [InlineData(ArgumentOptionConstants.GitHubModels.Token)]
    [InlineData(ArgumentOptionConstants.GitHubModels.Model)]
    public void Given_CLI_ArgumentWithoutValue_When_Parse_Invoked_Then_It_Should_Use_Config(string argument)
    {
        // Arrange
        var config = BuildConfigWithGitHubModels();
        var args = new[] { argument };

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.GitHubModels.ShouldNotBeNull();
        settings.GitHubModels.Endpoint.ShouldBe(Endpoint);
        settings.GitHubModels.Token.ShouldBe(Token);
        settings.GitHubModels.Model.ShouldBe(Model);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("--something", "else", "--another", "value")]
    public void Given_Unrelated_CLI_Arguments_When_Parse_Invoked_Then_It_Should_Use_Config(params string[] args)
    {
        // Arrange
        var config = BuildConfigWithGitHubModels();

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.GitHubModels.ShouldNotBeNull();
        settings.GitHubModels.Endpoint.ShouldBe(Endpoint);
        settings.GitHubModels.Token.ShouldBe(Token);
        settings.GitHubModels.Model.ShouldBe(Model);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("--strange-model-name")]
    public void Given_GitHubModels_With_ModelName_StartingWith_Dashes_When_Parse_Invoked_Then_It_Should_Treat_As_Value(string model)
    {
        // Arrange
        var config = BuildConfigWithGitHubModels();
        var args = new[]
        {
            ArgumentOptionConstants.GitHubModels.Model, model
        };

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.GitHubModels.ShouldNotBeNull();
        settings.GitHubModels.Model.ShouldBe(model);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("https://config.github-models/inference", "config-token", "config-model")]
    public void Given_ConfigValues_And_No_CLI_When_Parse_Invoked_Then_It_Should_Use_Config(
        string configEndpoint, string configToken, string configModel)
    {
        // Arrange
        var config = BuildConfigWithGitHubModels(configEndpoint, configToken, configModel);
        var args = Array.Empty<string>();

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.GitHubModels.ShouldNotBeNull();
        settings.GitHubModels.Endpoint.ShouldBe(configEndpoint);
        settings.GitHubModels.Token.ShouldBe(configToken);
        settings.GitHubModels.Model.ShouldBe(configModel);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("https://config.github-models/inference", "config-token", "config-model",
                "https://cli.github-models/inference", "cli-token", "cli-model")]
    public void Given_ConfigValues_And_CLI_When_Parse_Invoked_Then_It_Should_Use_CLI(
        string configEndpoint, string configToken, string configModel,
        string cliEndpoint, string cliToken, string cliModel)
    {
        // Arrange
        var config = BuildConfigWithGitHubModels(configEndpoint, configToken, configModel);
        var args = new[]
        {
            ArgumentOptionConstants.GitHubModels.Endpoint, cliEndpoint,
            ArgumentOptionConstants.GitHubModels.Token, cliToken,
            ArgumentOptionConstants.GitHubModels.Model, cliModel
        };

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.GitHubModels.ShouldNotBeNull();
        settings.GitHubModels.Endpoint.ShouldBe(cliEndpoint);
        settings.GitHubModels.Token.ShouldBe(cliToken);
        settings.GitHubModels.Model.ShouldBe(cliModel);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("https://env.github-models/inference", "env-token", "env-model")]
    public void Given_EnvironmentVariables_And_No_Config_When_Parse_Invoked_Then_It_Should_Use_EnvironmentVariables(
        string envEndpoint, string envToken, string envModel)
    {
        // Arrange
        var config = BuildConfigWithGitHubModels(
            configEndpoint: null, configToken: null, configModel: null,
            envEndpoint: envEndpoint, envToken: envToken, envModel: envModel);
        var args = Array.Empty<string>();

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.GitHubModels.ShouldNotBeNull();
        settings.GitHubModels.Endpoint.ShouldBe(envEndpoint);
        settings.GitHubModels.Token.ShouldBe(envToken);
        settings.GitHubModels.Model.ShouldBe(envModel);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("https://config.github-models/inference", "config-token", "config-model",
                "https://env.github-models/inference", "env-token", "env-model")]
    public void Given_ConfigValues_And_EnvironmentVariables_When_Parse_Invoked_Then_It_Should_Use_EnvironmentVariables(
        string configEndpoint, string configToken, string configModel,
        string envEndpoint, string envToken, string envModel)
    {
        // Arrange
        var config = BuildConfigWithGitHubModels(
            configEndpoint, configToken, configModel,
            envEndpoint, envToken, envModel);
        var args = Array.Empty<string>();

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.GitHubModels.ShouldNotBeNull();
        settings.GitHubModels.Endpoint.ShouldBe(envEndpoint);
        settings.GitHubModels.Token.ShouldBe(envToken);
        settings.GitHubModels.Model.ShouldBe(envModel);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("https://config.github-models/inference", "config-token", "config-model",
                "https://env.github-models/inference", "env-token", "env-model",
                "https://cli.github-models/inference", "cli-token", "cli-model")]
    public void Given_ConfigValues_And_EnvironmentVariables_And_CLI_When_Parse_Invoked_Then_It_Should_Use_CLI(
        string configEndpoint, string configToken, string configModel,
        string envEndpoint, string envToken, string envModel,
        string cliEndpoint, string cliToken, string cliModel)
    {
        // Arrange
        var config = BuildConfigWithGitHubModels(
            configEndpoint, configToken, configModel,
            envEndpoint, envToken, envModel);
        var args = new[]
        {
            ArgumentOptionConstants.GitHubModels.Endpoint, cliEndpoint,
            ArgumentOptionConstants.GitHubModels.Token, cliToken,
            ArgumentOptionConstants.GitHubModels.Model, cliModel
        };

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.GitHubModels.ShouldNotBeNull();
        settings.GitHubModels.Endpoint.ShouldBe(cliEndpoint);
        settings.GitHubModels.Token.ShouldBe(cliToken);
        settings.GitHubModels.Model.ShouldBe(cliModel);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("https://config.github-models/inference", "config-token", "config-model",
                "https://env.github-models/inference", null, "env-model")]
    public void Given_Partial_EnvironmentVariables_When_Parse_Invoked_Then_It_Should_Mix_Config_And_Environment(
        string configEndpoint, string configToken, string configModel,
        string envEndpoint, string? envToken, string envModel)
    {
        // Arrange
        var config = BuildConfigWithGitHubModels(
            configEndpoint, configToken, configModel,
            envEndpoint, envToken, envModel);
        var args = Array.Empty<string>();

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.GitHubModels.ShouldNotBeNull();
        settings.GitHubModels.Endpoint.ShouldBe(envEndpoint); // From environment
        settings.GitHubModels.Token.ShouldBe(configToken);    // From config (no env override)
        settings.GitHubModels.Model.ShouldBe(envModel);       // From environment
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("https://config.github-models/inference", "config-token", "config-model",
                null, "env-token", null,
                "https://cli.github-models/inference", null, null)]
    public void Given_Mixed_Priority_Sources_When_Parse_Invoked_Then_It_Should_Respect_Priority_Order(
        string configEndpoint, string configToken, string configModel,
        string? envEndpoint, string envToken, string? envModel,
        string cliEndpoint, string? cliToken, string? cliModel)
    {
        // Arrange
        var config = BuildConfigWithGitHubModels(
            configEndpoint, configToken, configModel,
            envEndpoint, envToken, envModel);
        var args = new[]
        {
            ArgumentOptionConstants.GitHubModels.Endpoint, cliEndpoint,
            ArgumentOptionConstants.GitHubModels.Token, cliToken,
            ArgumentOptionConstants.GitHubModels.Model, cliModel
        };

        // Act
        var settings = ArgumentOptions.Parse(config, args!);

        // Assert
        settings.GitHubModels.ShouldNotBeNull();
        settings.GitHubModels.Endpoint.ShouldBe(cliEndpoint);  // CLI wins (highest priority)
        settings.GitHubModels.Token.ShouldBe(envToken);        // Env wins over config (medium priority)
        settings.GitHubModels.Model.ShouldBe(configModel);     // Config only (lowest priority)
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("https://cli.github-models/inference", "cli-token", "cli-model")]
    public void Given_GitHubModels_With_KnownArguments_When_Parse_Invoked_Then_Help_ShouldBe_False(
        string cliEndpoint, string cliToken, string cliModel)
    {
        // Arrange
        var config = BuildConfigWithGitHubModels(Endpoint, Token, Model);
        var args = new[]
        {
            ArgumentOptionConstants.GitHubModels.Endpoint, cliEndpoint,
            ArgumentOptionConstants.GitHubModels.Token, cliToken,
            ArgumentOptionConstants.GitHubModels.Model, cliModel
        };

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.Help.ShouldBeFalse();
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData(ArgumentOptionConstants.GitHubModels.Endpoint)]
    [InlineData(ArgumentOptionConstants.GitHubModels.Token)]
    [InlineData(ArgumentOptionConstants.GitHubModels.Model)]
    public void Given_GitHubModels_With_KnownArgument_WithoutValue_When_Parse_Invoked_Then_Help_ShouldBe_False(string argument)
    {
        // Arrange
        var config = BuildConfigWithGitHubModels();
        var args = new[] { argument };

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.Help.ShouldBeFalse();
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("https://cli.github-models/inference", "--unknown-flag")]
    public void Given_GitHubModels_With_Known_And_Unknown_Argument_When_Parse_Invoked_Then_Help_ShouldBe_True(
        string cliEndpoint, string argument)
    {
        // Arrange
        var config = BuildConfigWithGitHubModels();
        var args = new[]
        {
            ArgumentOptionConstants.GitHubModels.Endpoint, cliEndpoint,
            argument
        };

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.Help.ShouldBeTrue();
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("https://env.github-models/inference", "env-token", "env-model")]
    public void Given_EnvironmentVariables_Only_When_Parse_Invoked_Then_Help_Should_Be_False(
        string envEndpoint, string envToken, string envModel)
    {
        // Arrange
        var config = BuildConfigWithGitHubModels(
            configEndpoint: null, configToken: null, configModel: null,
            envEndpoint: envEndpoint, envToken: envToken, envModel: envModel);
        var args = Array.Empty<string>();

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.Help.ShouldBeFalse();
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("https://env.github-models/inference", "cli-token", "cli-model")]
    public void Given_CLI_Only_When_Parse_Invoked_Then_Help_Should_Be_False(string cliEndpoint, string cliToken, string cliModel)
    {
        // Arrange
        var config = BuildConfigWithGitHubModels();
        var args = new[]
        {
            ArgumentOptionConstants.GitHubModels.Endpoint, cliEndpoint,
            ArgumentOptionConstants.GitHubModels.Token, cliToken,
            ArgumentOptionConstants.GitHubModels.Model, cliModel
        };

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.Help.ShouldBeFalse();
    }
}
