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
        string? configDeploymentName = DeploymentName)
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

        return new ConfigurationBuilder()
                   .AddInMemoryCollection(configDict!)  // Base configuration (lowest priority)
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
    [Trait("Category", "AzureAIFoundry")]
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
}
