using Microsoft.Extensions.Configuration;

using OpenChat.PlaygroundApp.Abstractions;
using OpenChat.PlaygroundApp.Connectors;

namespace OpenChat.PlaygroundApp.Tests.Options;

public class AzureAIFoundryArgumentOptionsTests
{
    private const string Endpoint = "https://test.azure-ai-foundry/inference";
    private const string ApiKey = "azure-api-key";
    private const string DeploymentName = "azure-deployment-name";

    private static IConfiguration BuildConfigWithAzureAIFoundry(
        string? configEndpoint = Endpoint,
        string? configApiKey = ApiKey,
        string? configDeploymentName = DeploymentName,
        string? envEndpoint = null,
        string? envApiKey = null,
        string? envDeploymentName = null)
    {
        var configDict = new Dictionary<string, string?>
        {
            ["ConnectorType"] = ConnectorType.AzureAIFoundry.ToString()
        };

        if (string.IsNullOrWhiteSpace(configEndpoint) == false)
        {
            configDict["AzureAIFoundry:Endpoint"] = configEndpoint;
        }
        if (string.IsNullOrWhiteSpace(configApiKey) == false)
        {
            configDict["AzureAIFoundry:ApiKey"] = configApiKey;
        }
        if (string.IsNullOrWhiteSpace(configDeploymentName) == false)
        {
            configDict["AzureAIFoundry:DeploymentName"] = configDeploymentName;
        }

        if (string.IsNullOrWhiteSpace(envEndpoint) && string.IsNullOrWhiteSpace(envApiKey) && string.IsNullOrWhiteSpace(envDeploymentName))
        {
            return new ConfigurationBuilder()
                       .AddInMemoryCollection(configDict!)
                       .Build();
        }

        // Environment variables (medium priority)
        var envDict = new Dictionary<string, string?>();
        if (string.IsNullOrWhiteSpace(envEndpoint) == false)
        {
            envDict["AzureAIFoundry:Endpoint"] = envEndpoint;
        }
        if (string.IsNullOrWhiteSpace(envApiKey) == false)
        {
            envDict["AzureAIFoundry:ApiKey"] = envApiKey;
        }
        if (string.IsNullOrWhiteSpace(envDeploymentName) == false)
        {
            envDict["AzureAIFoundry:DeploymentName"] = envDeploymentName;
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
        var config = BuildConfigWithAzureAIFoundry();
        var args = Array.Empty<string>();

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.AzureAIFoundry.ShouldNotBeNull();
        settings.AzureAIFoundry.Endpoint.ShouldBe(Endpoint);
        settings.AzureAIFoundry.ApiKey.ShouldBe(ApiKey);
        settings.AzureAIFoundry.DeploymentName.ShouldBe(DeploymentName);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("https://cli.azure-ai-foundry/inference")]
    public void Given_CLI_Endpoint_When_Parse_Invoked_Then_It_Should_Use_CLI_Endpoint(string cliEndpoint)
    {
        // Arrange
        var config = BuildConfigWithAzureAIFoundry();
        var args = new[] { "--endpoint", cliEndpoint };

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.AzureAIFoundry.ShouldNotBeNull();
        settings.AzureAIFoundry.Endpoint.ShouldBe(cliEndpoint);
        settings.AzureAIFoundry.ApiKey.ShouldBe(ApiKey);
        settings.AzureAIFoundry.DeploymentName.ShouldBe(DeploymentName);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("cli-api-key")]
    public void Given_CLI_ApiKey_When_Parse_Invoked_Then_It_Should_Use_CLI_ApiKey(string cliApiKey)
    {
        // Arrange
        var config = BuildConfigWithAzureAIFoundry();
        var args = new[] { "--api-key", cliApiKey };

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.AzureAIFoundry.ShouldNotBeNull();
        settings.AzureAIFoundry.Endpoint.ShouldBe(Endpoint);
        settings.AzureAIFoundry.ApiKey.ShouldBe(cliApiKey);
        settings.AzureAIFoundry.DeploymentName.ShouldBe(DeploymentName);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("cli-deployment-name")]
    public void Given_CLI_DeploymentName_When_Parse_Invoked_Then_It_Should_Use_CLI_DeploymentName(string cliDeploymentName)
    {
        // Arrange
        var config = BuildConfigWithAzureAIFoundry();
        var args = new[] { "--deployment-name", cliDeploymentName };

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.AzureAIFoundry.ShouldNotBeNull();
        settings.AzureAIFoundry.Endpoint.ShouldBe(Endpoint);
        settings.AzureAIFoundry.ApiKey.ShouldBe(ApiKey);
        settings.AzureAIFoundry.DeploymentName.ShouldBe(cliDeploymentName);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("https://cli.azure-ai-foundry/inference", "cli-api-key", "cli-deployment-name")]
    public void Given_All_CLI_Arguments_When_Parse_Invoked_Then_It_Should_Use_CLI(string cliEndpoint, string cliApiKey, string cliDeploymentName)
    {
        // Arrange
        var config = BuildConfigWithAzureAIFoundry();
        var args = new[] { "--endpoint", cliEndpoint, "--api-key", cliApiKey, "--deployment-name", cliDeploymentName };

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.AzureAIFoundry.ShouldNotBeNull();
        settings.AzureAIFoundry.Endpoint.ShouldBe(cliEndpoint);
        settings.AzureAIFoundry.ApiKey.ShouldBe(cliApiKey);
        settings.AzureAIFoundry.DeploymentName.ShouldBe(cliDeploymentName);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("--endpoint")]
    [InlineData("--api-key")]
    [InlineData("--deployment-name")]
    public void Given_CLI_ArgumentWithoutValue_When_Parse_Invoked_Then_It_Should_Use_Config(string argument)
    {
        // Arrange
        var config = BuildConfigWithAzureAIFoundry();
        var args = new[] { argument };

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.AzureAIFoundry.ShouldNotBeNull();
        settings.AzureAIFoundry.Endpoint.ShouldBe(Endpoint);
        settings.AzureAIFoundry.ApiKey.ShouldBe(ApiKey);
        settings.AzureAIFoundry.DeploymentName.ShouldBe(DeploymentName);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("--something", "else", "--another", "value")]
    public void Given_Unrelated_CLI_Arguments_When_Parse_Invoked_Then_It_Should_Use_Config(params string[] args)
    {
        // Arrange
        var config = BuildConfigWithAzureAIFoundry();

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.AzureAIFoundry.ShouldNotBeNull();
        settings.AzureAIFoundry.Endpoint.ShouldBe(Endpoint);
        settings.AzureAIFoundry.ApiKey.ShouldBe(ApiKey);
        settings.AzureAIFoundry.DeploymentName.ShouldBe(DeploymentName);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("--strange-deployment-name")]
    public void Given_AzureAIFoundry_With_DeploymentName_StartingWith_Dashes_When_Parse_Invoked_Then_It_Should_Treat_As_Value(string deploymentName)
    {
        // Arrange
        var config = BuildConfigWithAzureAIFoundry();
        var args = new[] { "--deployment-name", deploymentName };

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.AzureAIFoundry.ShouldNotBeNull();
        settings.AzureAIFoundry.DeploymentName.ShouldBe(deploymentName);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("https://config.azure-ai-foundry/inference", "config-api-key", "config-deployment-name")]
    public void Given_ConfigValues_And_No_CLI_When_Parse_Invoked_Then_It_Should_Use_Config(string configEndpoint, string configApiKey, string configDeploymentName)
    {
        // Arrange
        var config = BuildConfigWithAzureAIFoundry(configEndpoint, configApiKey, configDeploymentName);
        var args = Array.Empty<string>();

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.AzureAIFoundry.ShouldNotBeNull();
        settings.AzureAIFoundry.Endpoint.ShouldBe(configEndpoint);
        settings.AzureAIFoundry.ApiKey.ShouldBe(configApiKey);
        settings.AzureAIFoundry.DeploymentName.ShouldBe(configDeploymentName);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("https://config.azure-ai-foundry/inference", "config-api-key", "config-deployment-name",
                "https://cli.azure-ai-foundry/inference", "cli-api-key", "cli-deployment-name")]
    public void Given_ConfigValues_And_CLI_When_Parse_Invoked_Then_It_Should_Use_CLI(
        string configEndpoint, string configApiKey, string configDeploymentName,
        string cliEndpoint, string cliApiKey, string cliDeploymentName)
    {
        // Arrange
        var config = BuildConfigWithAzureAIFoundry(configEndpoint, configApiKey, configDeploymentName);
        var args = new[] { "--endpoint", cliEndpoint, "--api-key", cliApiKey, "--deployment-name", cliDeploymentName };

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.AzureAIFoundry.ShouldNotBeNull();
        settings.AzureAIFoundry.Endpoint.ShouldBe(cliEndpoint);
        settings.AzureAIFoundry.ApiKey.ShouldBe(cliApiKey);
        settings.AzureAIFoundry.DeploymentName.ShouldBe(cliDeploymentName);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("https://env.azure-ai-foundry/inference", "env-api-key", "env-deployment-name")]
    public void Given_EnvironmentVariables_And_No_Config_When_Parse_Invoked_Then_It_Should_Use_EnvironmentVariables(
        string envEndpoint, string envApiKey, string envDeploymentName)
    {
        // Arrange
        var config = BuildConfigWithAzureAIFoundry(
            configEndpoint: null, configApiKey: null, configDeploymentName: null,
            envEndpoint: envEndpoint, envApiKey: envApiKey, envDeploymentName: envDeploymentName);
        var args = Array.Empty<string>();

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.AzureAIFoundry.ShouldNotBeNull();
        settings.AzureAIFoundry.Endpoint.ShouldBe(envEndpoint);
        settings.AzureAIFoundry.ApiKey.ShouldBe(envApiKey);
        settings.AzureAIFoundry.DeploymentName.ShouldBe(envDeploymentName);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("https://config.azure-ai-foundry/inference", "config-api-key", "config-deployment-name",
                "https://env.azure-ai-foundry/inference", "env-api-key", "env-deployment-name")]
    public void Given_ConfigValues_And_EnvironmentVariables_When_Parse_Invoked_Then_It_Should_Use_EnvironmentVariables(
        string configEndpoint, string configApiKey, string configDeploymentName,
        string envEndpoint, string envApiKey, string envDeploymentName)
    {
        // Arrange
        var config = BuildConfigWithAzureAIFoundry(
            configEndpoint, configApiKey, configDeploymentName,
            envEndpoint, envApiKey, envDeploymentName);
        var args = Array.Empty<string>();

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.AzureAIFoundry.ShouldNotBeNull();
        settings.AzureAIFoundry.Endpoint.ShouldBe(envEndpoint);
        settings.AzureAIFoundry.ApiKey.ShouldBe(envApiKey);
        settings.AzureAIFoundry.DeploymentName.ShouldBe(envDeploymentName);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("https://config.azure-ai-foundry/inference", "config-api-key", "config-deployment-name",
                "https://env.azure-ai-foundry/inference", "env-api-key", "env-deployment-name",
                "https://cli.azure-ai-foundry/inference", "cli-api-key", "cli-deployment-name")]
    public void Given_ConfigValues_And_EnvironmentVariables_And_CLI_When_Parse_Invoked_Then_It_Should_Use_CLI(
        string configEndpoint, string configApiKey, string configDeploymentName,
        string envEndpoint, string envApiKey, string envDeploymentName,
        string cliEndpoint, string cliApiKey, string cliDeploymentName)
    {
        // Arrange
        var config = BuildConfigWithAzureAIFoundry(
            configEndpoint, configApiKey, configDeploymentName,
            envEndpoint, envApiKey, envDeploymentName);
        var args = new[] { "--endpoint", cliEndpoint, "--api-key", cliApiKey, "--deployment-name", cliDeploymentName };

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.AzureAIFoundry.ShouldNotBeNull();
        settings.AzureAIFoundry.Endpoint.ShouldBe(cliEndpoint);
        settings.AzureAIFoundry.ApiKey.ShouldBe(cliApiKey);
        settings.AzureAIFoundry.DeploymentName.ShouldBe(cliDeploymentName);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("https://config.azure-ai-foundry/inference", "config-api-key", "config-deployment-name",
                "https://env.azure-ai-foundry/inference", null, "env-deployment-name")]
    public void Given_Partial_EnvironmentVariables_When_Parse_Invoked_Then_It_Should_Mix_Config_And_Environment(
        string configEndpoint, string configApiKey, string configDeploymentName,
        string envEndpoint, string? envApiKey, string envDeploymentName)
    {
        // Arrange
        var config = BuildConfigWithAzureAIFoundry(
            configEndpoint, configApiKey, configDeploymentName,
            envEndpoint, envApiKey, envDeploymentName);
        var args = Array.Empty<string>();

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.AzureAIFoundry.ShouldNotBeNull();
        settings.AzureAIFoundry.Endpoint.ShouldBe(envEndpoint); // From environment
        settings.AzureAIFoundry.ApiKey.ShouldBe(configApiKey);  // From config (no env override)
        settings.AzureAIFoundry.DeploymentName.ShouldBe(envDeploymentName); // From environment
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("https://config.azure-ai-foundry/inference", "config-api-key", "config-deployment-name",
                null, "env-api-key", null,
                "https://cli.azure-ai-foundry/inference", null, null)]
    public void Given_Mixed_Priority_Sources_When_Parse_Invoked_Then_It_Should_Respect_Priority_Order(
        string configEndpoint, string configApiKey, string configDeploymentName,
        string? envEndpoint, string envApiKey, string? envDeploymentName,
        string cliEndpoint, string? cliApiKey, string? cliDeploymentName)
    {
        // Arrange
        var config = BuildConfigWithAzureAIFoundry(
            configEndpoint, configApiKey, configDeploymentName,
            envEndpoint, envApiKey, envDeploymentName);
        var args = new[] { "--endpoint", cliEndpoint, "--api-key", cliApiKey, "--deployment-name", cliDeploymentName };

        // Act
        var settings = ArgumentOptions.Parse(config, args!);

        // Assert
        settings.AzureAIFoundry.ShouldNotBeNull();
        settings.AzureAIFoundry.Endpoint.ShouldBe(cliEndpoint);  // CLI wins (highest priority)
        settings.AzureAIFoundry.ApiKey.ShouldBe(envApiKey);      // Env wins over config (medium priority)
        settings.AzureAIFoundry.DeploymentName.ShouldBe(configDeploymentName); // Config only (lowest priority)
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("https://cli.azure-ai-foundry/inference", "cli-api-key", "cli-deployment-name")]
    public void Given_AzureAIFoundry_With_KnownArguments_When_Parse_Invoked_Then_Help_ShouldBe_False(string cliEndpoint, string cliApiKey, string cliDeploymentName)
    {
        // Arrange
        var config = BuildConfigWithAzureAIFoundry(Endpoint, ApiKey, DeploymentName);
        var args = new[] { "--endpoint", cliEndpoint, "--api-key", cliApiKey, "--deployment-name", cliDeploymentName };

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.Help.ShouldBeFalse();
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("--endpoint")]
    [InlineData("--api-key")]
    [InlineData("--deployment-name")]
    public void Given_AzureAIFoundry_With_KnownArgument_WithoutValue_When_Parse_Invoked_Then_Help_ShouldBe_False(string argument)
    {
        // Arrange
        var config = BuildConfigWithAzureAIFoundry();
        var args = new[] { argument };

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.Help.ShouldBeFalse();
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("https://cli.azure-ai-foundry/inference", "--unknown-flag")]
    public void Given_AzureAIFoundry_With_Known_And_Unknown_Argument_When_Parse_Invoked_Then_Help_ShouldBe_True(string cliEndpoint, string argument)
    {
        // Arrange
        var config = BuildConfigWithAzureAIFoundry();
        var args = new[] { "--endpoint", cliEndpoint, argument };

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.Help.ShouldBeTrue();
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("https://env.azure-ai-foundry/inference", "env-api-key", "env-deployment-name")]
    public void Given_EnvironmentVariables_Only_When_Parse_Invoked_Then_Help_Should_Be_False(
        string envEndpoint, string envApiKey, string envDeploymentName)
    {
        // Arrange
        var config = BuildConfigWithAzureAIFoundry(
            configEndpoint: null, configApiKey: null, configDeploymentName: null,
            envEndpoint: envEndpoint, envApiKey: envApiKey, envDeploymentName: envDeploymentName);
        var args = Array.Empty<string>();

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.Help.ShouldBeFalse();
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("https://cli.azure-ai-foundry/inference", "cli-api-key", "cli-deployment-name")]
    public void Given_CLI_Only_When_Parse_Invoked_Then_Help_Should_Be_False(string cliEndpoint, string cliApiKey, string cliDeploymentName)
    {
        // Arrange
        var config = BuildConfigWithAzureAIFoundry();
        var args = new[] { "--endpoint", cliEndpoint, "--api-key", cliApiKey, "--deployment-name", cliDeploymentName };

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.Help.ShouldBeFalse();
    }
}
