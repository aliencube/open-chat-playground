using System.Collections.Generic;

using Microsoft.Extensions.Configuration;

using OpenChat.PlaygroundApp.Abstractions;
using OpenChat.PlaygroundApp.Configurations;
using OpenChat.PlaygroundApp.Connectors;

namespace OpenChat.PlaygroundApp.Tests.Options;

public class GitHubModelsArgumentOptionsTests
{
    private const string Endpoint = "https://github-models/inference";
    private const string Token = "github-pat";
    private const string Model = "github-model-name";

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
            ["ConnectorType"] = ConnectorType.GitHubModels.ToString()
        };

        if (string.IsNullOrWhiteSpace(configEndpoint) == false)
        {
            configDict["GitHubModels:Endpoint"] = configEndpoint;
        }
        if (string.IsNullOrWhiteSpace(configToken) == false)
        {
            configDict["GitHubModels:Token"] = configToken;
        }
        if (string.IsNullOrWhiteSpace(configModel) == false)
        {
            configDict["GitHubModels:Model"] = configModel;
        }

        if (string.IsNullOrWhiteSpace(envEndpoint) && string.IsNullOrWhiteSpace(envToken) && string.IsNullOrWhiteSpace(envModel))
        {
            return new ConfigurationBuilder()
                       .AddInMemoryCollection(configDict!)
                       .Build();
        }

        // Environment variables (medium priority)
        var envDict = new Dictionary<string, string?>();
        if (string.IsNullOrWhiteSpace(envEndpoint) == false)
        {
            envDict["GitHubModels:Endpoint"] = envEndpoint;
        }
        if (string.IsNullOrWhiteSpace(envToken) == false)
        {
            envDict["GitHubModels:Token"] = envToken;
        }
        if (string.IsNullOrWhiteSpace(envModel) == false)
        {
            envDict["GitHubModels:Model"] = envModel;
        }

        return new ConfigurationBuilder()
                   .AddInMemoryCollection(configDict!)  // Base configuration (lowest priority)
                   .AddInMemoryCollection(envDict!)     // Environment variables (medium priority)
                   .Build();
    }



    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("https://example.test/inference")]
    public void Given_Endpoint_When_Parse_Invoked_Then_It_Should_Set_Endpoint(string endpoint)
    {
        // Arrange
        var config = BuildConfigWithGitHubModels();
        var args = new[] { "--connector-type", ConnectorType.GitHubModels.ToString(), "--endpoint", endpoint };

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.ConnectorType.ShouldBe(ConnectorType.GitHubModels);
        settings.GitHubModels.ShouldNotBeNull();
        settings.GitHubModels.Endpoint.ShouldBe(endpoint);
        settings.GitHubModels.Token.ShouldBe(Token);
        settings.GitHubModels.Model.ShouldBe(Model);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("test-token")]
    public void Given_Token_When_Parse_Invoked_Then_It_Should_Set_Token(string token)
    {
        // Arrange
        var config = BuildConfigWithGitHubModels();
        var args = new[] { "--connector-type", ConnectorType.GitHubModels.ToString(), "--token", token };

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.GitHubModels.ShouldNotBeNull();
        settings.GitHubModels.Endpoint.ShouldBe(Endpoint);
        settings.GitHubModels.Token.ShouldBe(token);
        settings.GitHubModels.Model.ShouldBe(Model);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("test-model")]
    public void Given_Model_When_Parse_Invoked_Then_It_Should_Set_Model(string model)
    {
        // Arrange
        var config = BuildConfigWithGitHubModels();
        var args = new[] { "--connector-type", ConnectorType.GitHubModels.ToString(), "--model", model };

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.GitHubModels.ShouldNotBeNull();
        settings.GitHubModels.Endpoint.ShouldBe(Endpoint);
        settings.GitHubModels.Token.ShouldBe(Token);
        settings.GitHubModels.Model.ShouldBe(model);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("https://example.test/inference", "test-token", "openai/gpt-4o-mini")]
    public void Given_AllArguments_When_Parse_Invoked_Then_It_Should_Set_All(string endpoint, string token, string model)
    {
        // Arrange
        var config = BuildConfigWithGitHubModels();
        var args = new[] { "--connector-type", ConnectorType.GitHubModels.ToString(), "--endpoint", endpoint, "--token", token, "--model", model };

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.GitHubModels.ShouldNotBeNull();
        settings.GitHubModels.Endpoint.ShouldBe(endpoint);
        settings.GitHubModels.Token.ShouldBe(token);
        settings.GitHubModels.Model.ShouldBe(model);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("--endpoint")]
    [InlineData("--token")]
    [InlineData("--model")]
    public void Given_ArgumentWithoutValue_When_Parse_Invoked_Then_It_Should_Not_Set_Property(string argument)
    {
        // Arrange
        var config = BuildConfigWithGitHubModels(configEndpoint: null, configToken: null, configModel: null);
        var args = new[] { "--connector-type", ConnectorType.GitHubModels.ToString(), argument };

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.GitHubModels.ShouldNotBeNull();
        settings.GitHubModels.Endpoint.ShouldBeNull();
        settings.GitHubModels.Token.ShouldBeNull();
        settings.GitHubModels.Model.ShouldBeNull();
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("--something", "else", "--another", "value")]
    public void Given_UnrelatedArguments_When_Parse_Invoked_Then_It_Should_Ignore_Them(params string[] args)
    {
        // Arrange
        var config = BuildConfigWithGitHubModels(configEndpoint: null, configToken: null, configModel: null);
        var fullArgs = new List<string> { "--connector-type", ConnectorType.GitHubModels.ToString() };
        fullArgs.AddRange(args);

        // Act
        var settings = ArgumentOptions.Parse(config, [.. fullArgs]);

        // Assert
        settings.GitHubModels.ShouldNotBeNull();
        settings.GitHubModels.Endpoint.ShouldBeNull();
        settings.GitHubModels.Token.ShouldBeNull();
        settings.GitHubModels.Model.ShouldBeNull();
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("https://models.github.ai/inference", "{{GITHUB_PAT}}", "openai/gpt-4o-mini")]
    public void Given_ConfigValues_And_No_CLI_When_Parse_Invoked_Then_It_Should_Use_Config(string endpoint, string token, string model)
    {
        // Arrange
        var config = BuildConfigWithGitHubModels(endpoint, token, model);
        var args = new[] { "--connector-type", ConnectorType.GitHubModels.ToString() };

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.GitHubModels.ShouldNotBeNull();
        settings.GitHubModels.Endpoint.ShouldBe(endpoint);
        settings.GitHubModels.Token.ShouldBe(token);
        settings.GitHubModels.Model.ShouldBe(model);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("https://models.github.ai/inference", "{{GITHUB_PAT}}", "openai/gpt-4o-mini",
                "https://cli.example/inference", "cli-token", "openai/gpt-5-large")]
    public void Given_ConfigValues_And_CLI_When_Parse_Invoked_Then_CLI_Should_Override_Config(
        string configEndpoint, string configToken, string configModel,
        string cliEndpoint, string cliToken, string cliModel)
    {
        // Arrange
        var config = BuildConfigWithGitHubModels(configEndpoint, configToken, configModel);
        var args = new[] { "--connector-type", ConnectorType.GitHubModels.ToString(), "--endpoint", cliEndpoint, "--token", cliToken, "--model", cliModel };

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
    [InlineData("https://models.github.ai/inference", "pat", "openai/gpt-4o-mini")]
    public void Given_GitHubModels_With_KnownArguments_When_Parse_Invoked_Then_Help_ShouldBe_False(string endpoint, string token, string model)
    {
        // Arrange
        var config = BuildConfigWithGitHubModels(Endpoint, Token, Model);
        var args = new[] { "--endpoint", endpoint, "--token", token, "--model", model };

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.Help.ShouldBeFalse();
        settings.ConnectorType.ShouldBe(ConnectorType.GitHubModels);
        settings.GitHubModels.ShouldNotBeNull();
        settings.GitHubModels.Endpoint.ShouldBe(endpoint);
        settings.GitHubModels.Token.ShouldBe(token);
        settings.GitHubModels.Model.ShouldBe(model);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("--endpoint")]
    [InlineData("--token")]
    [InlineData("--model")]
    public void Given_GitHubModels_With_KnownArgument_WithoutValue_When_Parse_Invoked_Then_Help_ShouldBe_False(string argument)
    {
        // Arrange
        var config = BuildConfigWithGitHubModels();
        var args = new[] { argument };

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.Help.ShouldBeFalse();
        settings.GitHubModels.ShouldNotBeNull();
        settings.GitHubModels.Endpoint.ShouldBe(Endpoint);
        settings.GitHubModels.Token.ShouldBe(Token);
        settings.GitHubModels.Model.ShouldBe(Model);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("https://models.github.ai/inference", "--unknown-flag")]
    public void Given_GitHubModels_With_Known_And_Unknown_Argument_When_Parse_Invoked_Then_Help_ShouldBe_True(string endpoint, string argument)
    {
        // Arrange
        var config = BuildConfigWithGitHubModels();
        var args = new[] { "--endpoint", endpoint, argument };

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.Help.ShouldBeTrue();
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("--strange-model-name")]
    public void Given_GitHubModels_With_ModelName_StartingWith_Dashes_When_Parse_Invoked_Then_It_Should_Treat_As_Value(string model)
    {
        // Arrange
        var config = BuildConfigWithGitHubModels();
        var args = new[] { "--model", model };

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.Help.ShouldBeFalse();
        settings.GitHubModels.ShouldNotBeNull();
        settings.GitHubModels.Model.ShouldBe(model);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("https://env.example/inference", "env-token", "env-model")]
    public void Given_EnvironmentVariables_And_No_Config_When_Parse_Invoked_Then_It_Should_Use_EnvironmentVariables(
        string envEndpoint, string envToken, string envModel)
    {
        // Arrange
        var config = BuildConfigWithGitHubModels(
            configEndpoint: null, configToken: null, configModel: null,
            envEndpoint: envEndpoint, envToken: envToken, envModel: envModel);
        var args = new[] { "--connector-type", ConnectorType.GitHubModels.ToString() };

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
    [InlineData("https://config.example/inference", "config-token", "config-model",
                "https://env.example/inference", "env-token", "env-model")]
    public void Given_ConfigValues_And_EnvironmentVariables_When_Parse_Invoked_Then_EnvironmentVariables_Should_Override_Config(
        string configEndpoint, string configToken, string configModel,
        string envEndpoint, string envToken, string envModel)
    {
        // Arrange
        var config = BuildConfigWithGitHubModels(
            configEndpoint, configToken, configModel,
            envEndpoint, envToken, envModel);
        var args = new[] { "--connector-type", ConnectorType.GitHubModels.ToString() };

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
    [InlineData("https://config.example/inference", "config-token", "config-model",
                "https://env.example/inference", "env-token", "env-model",
                "https://cli.example/inference", "cli-token", "cli-model")]
    public void Given_ConfigValues_And_EnvironmentVariables_And_CLI_When_Parse_Invoked_Then_CLI_Should_Override_All(
        string configEndpoint, string configToken, string configModel,
        string envEndpoint, string envToken, string envModel,
        string cliEndpoint, string cliToken, string cliModel)
    {
        // Arrange
        var config = BuildConfigWithGitHubModels(
            configEndpoint, configToken, configModel,
            envEndpoint, envToken, envModel);
        var args = new[] { "--connector-type", ConnectorType.GitHubModels.ToString(), "--endpoint", cliEndpoint, "--token", cliToken, "--model", cliModel };

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
    [InlineData("https://config.example/inference", "config-token", "config-model",
                "https://env.example/inference", null, "env-model")]
    public void Given_Partial_EnvironmentVariables_When_Parse_Invoked_Then_It_Should_Mix_Config_And_Environment(
        string configEndpoint, string configToken, string configModel,
        string envEndpoint, string? envToken, string envModel)
    {
        // Arrange
        var config = BuildConfigWithGitHubModels(
            configEndpoint, configToken, configModel,
            envEndpoint, envToken, envModel);
        var args = new[] { "--connector-type", ConnectorType.GitHubModels.ToString() };

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
    [InlineData("https://config.example/inference", "config-token", "config-model",
                null, "env-token", null,
                "https://cli.example/inference")]
    public void Given_Mixed_Priority_Sources_When_Parse_Invoked_Then_It_Should_Respect_Priority_Order(
        string configEndpoint, string configToken, string configModel,
        string? envEndpoint, string envToken, string? envModel,
        string cliEndpoint)
    {
        // Arrange
        var config = BuildConfigWithGitHubModels(
            configEndpoint, configToken, configModel,
            envEndpoint, envToken, envModel);
        var args = new[] { "--connector-type", ConnectorType.GitHubModels.ToString(), "--endpoint", cliEndpoint };

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.GitHubModels.ShouldNotBeNull();
        settings.GitHubModels.Endpoint.ShouldBe(cliEndpoint);  // CLI wins (highest priority)
        settings.GitHubModels.Token.ShouldBe(envToken);        // Env wins over config (medium priority)
        settings.GitHubModels.Model.ShouldBe(configModel);     // Config only (lowest priority)
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("https://env.example/inference", "env-token", "env-model")]
    public void Given_EnvironmentVariables_Only_When_Parse_Invoked_Then_Help_Should_Be_False(
        string envEndpoint, string envToken, string envModel)
    {
        // Arrange
        var config = BuildConfigWithGitHubModels(
            configEndpoint: null, configToken: null, configModel: null,
            envEndpoint: envEndpoint, envToken: envToken, envModel: envModel);
        var args = new[] { "--connector-type", ConnectorType.GitHubModels.ToString() };

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.Help.ShouldBeFalse();
        settings.ConnectorType.ShouldBe(ConnectorType.GitHubModels);
        settings.GitHubModels.ShouldNotBeNull();
        settings.GitHubModels.Endpoint.ShouldBe(envEndpoint);
        settings.GitHubModels.Token.ShouldBe(envToken);
        settings.GitHubModels.Model.ShouldBe(envModel);
    }
}
