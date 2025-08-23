using Microsoft.Extensions.Configuration;

using OpenChat.PlaygroundApp.Abstractions;
using OpenChat.PlaygroundApp.Connectors;

namespace OpenChat.PlaygroundApp.Tests.Options;

public class AnthropicArgumentOptionsTests
{
    private const string ApiKey = "sk-test-anthropic-key";
    private const string Model = "claude-sonnet-4-20250514";

    private static IConfiguration BuildConfigWithAnthropic(
        string? configApiKey = ApiKey,
        string? configModel = Model,
        string? envApiKey = null,
        string? envModel = null)
    {
        // Base configuration values (lowest priority)
        var configDict = new Dictionary<string, string?>
        {
            ["ConnectorType"] = ConnectorType.Anthropic.ToString()
        };

        if (string.IsNullOrWhiteSpace(configApiKey) == false)
        {
            configDict["Anthropic:ApiKey"] = configApiKey;
        }
        if (string.IsNullOrWhiteSpace(configModel) == false)
        {
            configDict["Anthropic:Model"] = configModel;
        }

        if (string.IsNullOrWhiteSpace(envApiKey) && string.IsNullOrWhiteSpace(envModel))
        {
            return new ConfigurationBuilder()
                       .AddInMemoryCollection(configDict!)
                       .Build();
        }

        // Environment variables (medium priority)
        var envDict = new Dictionary<string, string?>();
        if (string.IsNullOrWhiteSpace(envApiKey) == false)
        {
            envDict["Anthropic:ApiKey"] = envApiKey;
        }
        if (string.IsNullOrWhiteSpace(envModel) == false)
        {
            envDict["Anthropic:Model"] = envModel;
        }

        return new ConfigurationBuilder()
                   .AddInMemoryCollection(configDict!)  // Base configuration (lowest priority)
                   .AddInMemoryCollection(envDict!)     // Environment variables (medium priority)
                   .Build();
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("sk-test-anthropic-key", "claude-sonnet-4-20250514")]
    [InlineData("sk-test-anthropic-key", null)] // Model can be null (will use default)
    public void Given_ConfigValues_And_No_CLI_When_Parse_Invoked_Then_It_Should_Use_Config(string configApiKey, string? configModel)
    {
        // Arrange
        var config = BuildConfigWithAnthropic(configApiKey, configModel);
        var args = Array.Empty<string>();

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.Anthropic.ShouldNotBeNull();
        settings.Anthropic.ApiKey.ShouldBe(configApiKey);
        if (configModel != null)
        {
            settings.Anthropic.Model.ShouldBe(configModel);
        }
        else
        {
            settings.Anthropic.Model.ShouldBe("claude-3-5-sonnet-latest"); // Default value
        }
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("sk-test-anthropic-key", "claude-sonnet-4-20250514",
                "sk-cli-key", "claude-haiku-3-5-latest")]
    public void Given_ConfigValues_And_CLI_When_Parse_Invoked_Then_It_Should_Use_CLI(
        string configApiKey, string configModel,
        string cliApiKey, string cliModel)
    {
        // Arrange
        var config = BuildConfigWithAnthropic(configApiKey, configModel);
        var args = new[] { "--api-key", cliApiKey, "--model", cliModel };

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.Anthropic.ShouldNotBeNull();
        settings.Anthropic.ApiKey.ShouldBe(cliApiKey);
        settings.Anthropic.Model.ShouldBe(cliModel);
    }
}
