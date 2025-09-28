using Microsoft.Extensions.Configuration;

using OpenChat.PlaygroundApp.Abstractions;
using OpenChat.PlaygroundApp.Connectors;

namespace OpenChat.PlaygroundApp.Tests.Options;

public class AmazonBedrockArgumentOptionsTests
{
    private const string AwsAccessKeyId = "test-access-key-id";
    private const string AwsSecretAccessKey = "test-secret-access-key";
    private const string Region = "test-region";
    private const string ModelId = "test-model-id";

    private static IConfiguration BuildConfigWithAmazonBedrock(
        string? AwsAccessKeyId = AwsAccessKeyId,
        string? AwsSecretAccessKey = AwsSecretAccessKey,
        string? configRegion = Region,
        string? configModel = ModelId)
    {
        var configDict = new Dictionary<string, string?>
        {
            ["ConnectorType"] = ConnectorType.AmazonBedrock.ToString()
        };

        if (string.IsNullOrWhiteSpace(AwsAccessKeyId) == false)
        {
            configDict["AmazonBedrock:AccessKeyId"] = AwsAccessKeyId;
        }
        if (string.IsNullOrWhiteSpace(AwsSecretAccessKey) == false)
        {
            configDict["AmazonBedrock:SecretAccessKey"] = AwsSecretAccessKey;
        }
        if (string.IsNullOrWhiteSpace(configRegion) == false)
        {
            configDict["AmazonBedrock:Region"] = configRegion;
        }
        if (string.IsNullOrWhiteSpace(configModel) == false)
        {
            configDict["AmazonBedrock:ModelId"] = configModel;
        }

        // TODO: "envDict" can be added here

        return new ConfigurationBuilder()
            .AddInMemoryCollection(configDict!) // Base configuration (lowest priority)
            .Build();
    }

    [Trait("Category", "UnitTest")]
    [Fact]
    public void Given_Nothing_When_Parse_Invoked_Then_It_Should_Set_Config()
    {
        // Arrange
        var config = BuildConfigWithAmazonBedrock();
        var args = Array.Empty<string>();

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.AmazonBedrock.ShouldNotBeNull();
        settings.AmazonBedrock.AccessKeyId.ShouldBe(AwsAccessKeyId);
        settings.AmazonBedrock.SecretAccessKey.ShouldBe(AwsSecretAccessKey);
        settings.AmazonBedrock.Region.ShouldBe(Region);
        settings.AmazonBedrock.ModelId.ShouldBe(ModelId);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("cli-access-key-id")]
    public void Given_CLI_accessKeyId_When_Parse_Invoked_Then_It_Should_Use_CLI_accessKeyId(string cliAccessKeyId)
    {
        // Arrange
        var config = BuildConfigWithAmazonBedrock();
        var args = new[] { "--access-key-id", cliAccessKeyId };

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.AmazonBedrock.ShouldNotBeNull();
        settings.AmazonBedrock.AccessKeyId.ShouldBe(cliAccessKeyId);
        settings.AmazonBedrock.SecretAccessKey.ShouldBe(AwsSecretAccessKey);
        settings.AmazonBedrock.Region.ShouldBe(Region);
        settings.AmazonBedrock.ModelId.ShouldBe(ModelId);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("cli-secret-access-key")]
    public void Given_CLI_secretAccessKey_When_Parse_Invoked_Then_It_Should_Use_CLI_secretAccessKey(string cliSecretAccessKey)
    {
        // Arrange
        var config = BuildConfigWithAmazonBedrock();
        var args = new[] { "--secret-access-key", cliSecretAccessKey };

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.AmazonBedrock.ShouldNotBeNull();
        settings.AmazonBedrock.AccessKeyId.ShouldBe(AwsAccessKeyId);
        settings.AmazonBedrock.SecretAccessKey.ShouldBe(cliSecretAccessKey);
        settings.AmazonBedrock.Region.ShouldBe(Region);
        settings.AmazonBedrock.ModelId.ShouldBe(ModelId);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("cli-region")]
    public void Given_CLI_Region_When_Parse_Invoked_Then_It_Should_Use_CLI_Region(string cliRegion)
    {
        // Arrange
        var config = BuildConfigWithAmazonBedrock();
        var args = new[] { "--region", cliRegion };

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.AmazonBedrock.ShouldNotBeNull();
        settings.AmazonBedrock.AccessKeyId.ShouldBe(AwsAccessKeyId);
        settings.AmazonBedrock.SecretAccessKey.ShouldBe(AwsSecretAccessKey);
        settings.AmazonBedrock.Region.ShouldBe(cliRegion);
        settings.AmazonBedrock.ModelId.ShouldBe(ModelId);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("cli-model-id")]
    public void Given_CLI_Model_When_Parse_Invoked_Then_It_Should_Use_CLI_Model(string cliModelId)
    {
        // Arrange
        var config = BuildConfigWithAmazonBedrock();
        var args = new[] { "--model-id", cliModelId };

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.AmazonBedrock.ShouldNotBeNull();
        settings.AmazonBedrock.AccessKeyId.ShouldBe(AwsAccessKeyId);
        settings.AmazonBedrock.SecretAccessKey.ShouldBe(AwsSecretAccessKey);
        settings.AmazonBedrock.Region.ShouldBe(Region);
        settings.AmazonBedrock.ModelId.ShouldBe(cliModelId);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("cli-access-key-id", "cli-secret-access-key", "cli-region", "cli-model-id")]
    public void Given_All_CLI_Arguments_When_Parse_Invoked_Then_It_Should_Use_CLI(string cliAccessKeyId, string cliSecretAccessKey, string cliRegion, string cliModelId)
    {
        // Arrange
        var config = BuildConfigWithAmazonBedrock();
        var args = new[] { "--access-key-id", cliAccessKeyId, "--secret-access-key", cliSecretAccessKey, "--region", cliRegion, "--model-id", cliModelId };

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.AmazonBedrock.ShouldNotBeNull();
        settings.AmazonBedrock.AccessKeyId.ShouldBe(cliAccessKeyId);
        settings.AmazonBedrock.SecretAccessKey.ShouldBe(cliSecretAccessKey);
        settings.AmazonBedrock.Region.ShouldBe(cliRegion);
        settings.AmazonBedrock.ModelId.ShouldBe(cliModelId);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("--access-key-id")]
    [InlineData("--secret-access-key")]
    [InlineData("--region")]
    [InlineData("--model-id")]
    public void Given_CLI_ArgumentWithoutValue_When_Parse_Invoked_Then_It_Should_Use_Config(string argument)
    {
        // Arrange
        var config = BuildConfigWithAmazonBedrock();
        var args = new[] { argument };

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.AmazonBedrock.ShouldNotBeNull();
        settings.AmazonBedrock.AccessKeyId.ShouldBe(AwsAccessKeyId);
        settings.AmazonBedrock.SecretAccessKey.ShouldBe(AwsSecretAccessKey);
        settings.AmazonBedrock.Region.ShouldBe(Region);
        settings.AmazonBedrock.ModelId.ShouldBe(ModelId);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("--something", "else", "--another", "value")]
    public void Given_Unrelated_CLI_Arguments_When_Parse_Invoked_Then_It_Should_Use_Config(params string[] args)
    {
        // Arrange
        var config = BuildConfigWithAmazonBedrock();

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.AmazonBedrock.ShouldNotBeNull();
        settings.AmazonBedrock.AccessKeyId.ShouldBe(AwsAccessKeyId);
        settings.AmazonBedrock.SecretAccessKey.ShouldBe(AwsSecretAccessKey);
        settings.AmazonBedrock.Region.ShouldBe(Region);
        settings.AmazonBedrock.ModelId.ShouldBe(ModelId);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("--strange-region")]
    public void Given_AmazonBedrock_With_Region_StartingWith_Dashes_When_Parse_Invoked_Then_It_Should_Treat_As_Value(string region)
    {
        // Arrange
        var config = BuildConfigWithAmazonBedrock();
        var args = new[] { "--region", region };

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.AmazonBedrock.ShouldNotBeNull();
        settings.AmazonBedrock.Region.ShouldBe(region);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("config-access-key-id", "config-secret-access-key", "config-region", "config-model-id")]
    public void Given_ConfigValues_And_No_CLI_When_Parse_Invoked_Then_It_Should_Use_Config(string configAccessKeyId, string configSecretAccessKey, string configRegion, string configModelId)
    {
        // Arrange
        var config = BuildConfigWithAmazonBedrock(configAccessKeyId, configSecretAccessKey, configRegion, configModelId);
        var args = Array.Empty<string>();

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.AmazonBedrock.ShouldNotBeNull();
        settings.AmazonBedrock.AccessKeyId.ShouldBe(configAccessKeyId);
        settings.AmazonBedrock.SecretAccessKey.ShouldBe(configSecretAccessKey);
        settings.AmazonBedrock.Region.ShouldBe(configRegion);
        settings.AmazonBedrock.ModelId.ShouldBe(configModelId);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("config-access-key-id", "config-secret-access-key", "config-region", "config-model-id",
                "cli-access-key-id", "cli-secret-access-key", "cli-region", "cli-model-id")]
    public void Given_ConfigValues_And_CLI_When_Parse_Invoked_Then_It_Should_Use_CLI(
        string configAccessKeyId, string configSecretAccessKey, string configRegion, string configModelId,
        string cliAccessKeyId, string cliSecretAccessKey, string cliRegion, string cliModelId)
    {
        // Arrange
        var config = BuildConfigWithAmazonBedrock(configAccessKeyId, configSecretAccessKey, configRegion, configModelId);
        var args = new[] { "--access-key-id", cliAccessKeyId, "--secret-access-key", cliSecretAccessKey, "--region", cliRegion, "--model-id", cliModelId };

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.AmazonBedrock.ShouldNotBeNull();
        settings.AmazonBedrock.AccessKeyId.ShouldBe(cliAccessKeyId);
        settings.AmazonBedrock.SecretAccessKey.ShouldBe(cliSecretAccessKey);
        settings.AmazonBedrock.Region.ShouldBe(cliRegion);
        settings.AmazonBedrock.ModelId.ShouldBe(cliModelId);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("cli-access-key-id", "cli-secret-access-key", "cli-region", "cli-model-id")]
    public void Given_AmazonBedrock_With_KnownArguments_When_Parse_Invoked_Then_Help_ShouldBe_False(string cliAccessKeyId, string cliSecretAccessKey, string cliRegion, string cliModelId)
    {
        // Arrange
        var config = BuildConfigWithAmazonBedrock(AwsAccessKeyId, AwsSecretAccessKey, Region, ModelId);
        var args = new[] { "--access-key-id", cliAccessKeyId, "--secret-access-key", cliSecretAccessKey, "--region", cliRegion, "--model-id", cliModelId };

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.Help.ShouldBeFalse();
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("--access-key-id")]
    [InlineData("--secret-access-key")]
    [InlineData("--region")]
    [InlineData("--model-id")]
    public void Given_AmazonBedrock_With_KnownArgument_WithoutValue_When_Parse_Invoked_Then_Help_ShouldBe_False(string argument)
    {
        // Arrange
        var config = BuildConfigWithAmazonBedrock();
        var args = new[] { argument };

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.Help.ShouldBeFalse();
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("cli-region", "--unknown-flag")]
    public void Given_AmazonBedrock_With_Known_And_Unknown_Argument_When_Parse_Invoked_Then_Help_ShouldBe_True(string cliRegion, string argument)
    {
        // Arrange
        var config = BuildConfigWithAmazonBedrock();
        var args = new[] { "--region", cliRegion, argument };

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.Help.ShouldBeTrue();
    }
}