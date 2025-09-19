using Microsoft.Extensions.Configuration;

using OpenChat.PlaygroundApp.Abstractions;
using OpenChat.PlaygroundApp.Connectors;

namespace OpenChat.PlaygroundApp.Tests.Options;

public class AmazonBedrockArgumentOptionsTests
{
    private const string Region = "test-region";
    private const string Model = "test-model";

    private static IConfiguration BuildConfigWithAmazonBedrock(
        string? configRegion = Region,
        string? configModel = Model)
    {
        var configDict = new Dictionary<string, string?>
        {
            ["ConnectorType"] = ConnectorType.AmazonBedrock.ToString()
        };

        if (string.IsNullOrWhiteSpace(configRegion) == false)
        {
            configDict["AmazonBedrock:Region"] = configRegion;
        }
        if (string.IsNullOrWhiteSpace(configModel) == false)
        {
            configDict["AmazonBedrock:Model"] = configModel;
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
        settings.AmazonBedrock.Region.ShouldBe(Region);
        settings.AmazonBedrock.Model.ShouldBe(Model);
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
        settings.AmazonBedrock.Region.ShouldBe(cliRegion);
        settings.AmazonBedrock.Model.ShouldBe(Model);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("cli-model")]
    public void Given_CLI_Model_When_Parse_Invoked_Then_It_Should_Use_CLI_Model(string cliModel)
    {
        // Arrange
        var config = BuildConfigWithAmazonBedrock();
        var args = new[] { "--model", cliModel };

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.AmazonBedrock.ShouldNotBeNull();
        settings.AmazonBedrock.Region.ShouldBe(Region);
        settings.AmazonBedrock.Model.ShouldBe(cliModel);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("cli-region", "cli-model")]
    public void Given_All_CLI_Arguments_When_Parse_Invoked_Then_It_Should_Use_CLI(string cliRegion, string cliModel)
    {
        // Arrange
        var config = BuildConfigWithAmazonBedrock();
        var args = new[] { "--region", cliRegion, "--model", cliModel };

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.AmazonBedrock.ShouldNotBeNull();
        settings.AmazonBedrock.Region.ShouldBe(cliRegion);
        settings.AmazonBedrock.Model.ShouldBe(cliModel);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("--region")]
    [InlineData("--model")]
    public void Given_CLI_ArgumentWithoutValue_When_Parse_Invoked_Then_It_Should_Use_Config(string argument)
    {
        // Arrange
        var config = BuildConfigWithAmazonBedrock();
        var args = new[] { argument };

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.AmazonBedrock.ShouldNotBeNull();
        settings.AmazonBedrock.Region.ShouldBe(Region);
        settings.AmazonBedrock.Model.ShouldBe(Model);
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
        settings.AmazonBedrock.Region.ShouldBe(Region);
        settings.AmazonBedrock.Model.ShouldBe(Model);
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
    [InlineData("{{AWS_REGION}}", "anthropic.claude-sonnet-4-20250514-v1:0")]
    public void Given_ConfigValues_And_No_CLI_When_Parse_Invoked_Then_It_Should_Use_Config(string configRegion, string configModel)
    {
        // Arrange
        var config = BuildConfigWithAmazonBedrock(configRegion, configModel);
        var args = Array.Empty<string>();

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.AmazonBedrock.ShouldNotBeNull();
        settings.AmazonBedrock.Region.ShouldBe(configRegion);
        settings.AmazonBedrock.Model.ShouldBe(configModel);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("config-region", "config-model", "cli-region", "cli-model")]
    public void Given_ConfigValues_And_CLI_When_Parse_Invoked_Then_It_Should_Use_CLI(
        string configRegion, string configModel,
        string cliRegion, string cliModel)
    {
        // Arrange
        var config = BuildConfigWithAmazonBedrock(configRegion, configModel);
        var args = new[] { "--region", cliRegion, "--model", cliModel };

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.AmazonBedrock.ShouldNotBeNull();
        settings.AmazonBedrock.Region.ShouldBe(cliRegion);
        settings.AmazonBedrock.Model.ShouldBe(cliModel);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("cli-region", "cli-model")]
    public void Given_AmazonBedrock_With_KnownArguments_When_Parse_Invoked_Then_Help_ShouldBe_False(string cliRegion, string cliModel)
    {
        // Arrange
        var config = BuildConfigWithAmazonBedrock();
        var args = new[] { "--region", cliRegion, "--model", cliModel };

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.Help.ShouldBeFalse();
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("--region")]
    [InlineData("--model")]
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