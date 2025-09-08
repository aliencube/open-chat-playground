using Microsoft.Extensions.Configuration;

using OpenChat.PlaygroundApp.Abstractions;
using OpenChat.PlaygroundApp.Connectors;
using OpenChat.PlaygroundApp.Options;

namespace OpenChat.PlaygroundApp.Tests.Options;

public class AnthropicArgumentOptionsTests
{
    private const string ApiKey = "test-api-key";
    private const string Model = "test-model";

    private static IConfiguration BuildConfigWithAnthropic(
        string? configApiKey = ApiKey,
        string? configModel = Model)
    {
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

        return new ConfigurationBuilder()
                   .AddInMemoryCollection(configDict!)
                   .Build();
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData(typeof(ArgumentOptions), typeof(AnthropicArgumentOptions), true)]
    [InlineData(typeof(AnthropicArgumentOptions), typeof(ArgumentOptions), false)]
    public void Given_AnthropicArgumentOptions_When_Checking_Inheritance_Then_Should_Inherit_From_ArgumentOptions(Type baseType, Type derivedType, bool expectedResult)
    {
        // Act
        var isSubclass = derivedType.IsSubclassOf(baseType);
        
        // Assert
        if (expectedResult)
        {
            isSubclass.ShouldBeTrue();
        }
        else
        {
            isSubclass.ShouldBeFalse();
        }
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("test-api-key", "test-model")]
    public void Given_Nothing_When_Parse_Invoked_Then_It_Should_Set_Config(string expectedApiKey, string expectedModel)
    {
        // Arrange
        var config = BuildConfigWithAnthropic(expectedApiKey, expectedModel);
        var args = Array.Empty<string>();

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.Anthropic.ShouldNotBeNull();
        settings.Anthropic.ApiKey.ShouldBe(expectedApiKey);
        settings.Anthropic.Model.ShouldBe(expectedModel);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("cli-api-key")]
    public void Given_CLI_ApiKey_When_Parse_Invoked_Then_It_Should_Use_CLI_ApiKey(string cliApiKey)
    {
        // Arrange
        var config = BuildConfigWithAnthropic();
        var args = new[] { "--api-key", cliApiKey };

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.Anthropic.ShouldNotBeNull();
        settings.Anthropic.ApiKey.ShouldBe(cliApiKey);
        settings.Anthropic.Model.ShouldBe(Model);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("claude-3-5-sonnet-latest")]
    public void Given_CLI_Model_When_Parse_Invoked_Then_It_Should_Use_CLI_Model(string cliModel)
    {
        // Arrange
        var config = BuildConfigWithAnthropic();
        var args = new[] { "--model", cliModel };

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.Anthropic.ShouldNotBeNull();
        settings.Anthropic.ApiKey.ShouldBe(ApiKey);
        settings.Anthropic.Model.ShouldBe(cliModel);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("cli-api-key", "claude-3-5-sonnet-latest")]
    public void Given_All_CLI_Arguments_When_Parse_Invoked_Then_It_Should_Use_CLI(string cliApiKey, string cliModel)
    {
        // Arrange
        var config = BuildConfigWithAnthropic();
        var args = new[] { "--api-key", cliApiKey, "--model", cliModel };

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.Anthropic.ShouldNotBeNull();
        settings.Anthropic.ApiKey.ShouldBe(cliApiKey);
        settings.Anthropic.Model.ShouldBe(cliModel);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("--api-key")]
    [InlineData("--model")]
    public void Given_CLI_ArgumentWithoutValue_When_Parse_Invoked_Then_It_Should_Use_Config(string argument)
    {
        // Arrange
        var config = BuildConfigWithAnthropic();
        var args = new[] { argument };

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.Anthropic.ShouldNotBeNull();
        settings.Anthropic.ApiKey.ShouldBe(ApiKey);
        settings.Anthropic.Model.ShouldBe(Model);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("--something", "else", "--another", "value")]
    public void Given_Unrelated_CLI_Arguments_When_Parse_Invoked_Then_It_Should_Use_Config(params string[] args)
    {
        // Arrange
        var config = BuildConfigWithAnthropic();

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.Anthropic.ShouldNotBeNull();
        settings.Anthropic.ApiKey.ShouldBe(ApiKey);
        settings.Anthropic.Model.ShouldBe(Model);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("--strange-model-name")]
    public void Given_Anthropic_With_ModelName_StartingWith_Dashes_When_Parse_Invoked_Then_It_Should_Treat_As_Value(string model)
    {
        // Arrange
        var config = BuildConfigWithAnthropic();
        var args = new[] { "--model", model };

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.Anthropic.ShouldNotBeNull();
        settings.Anthropic.Model.ShouldBe(model);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("config-api-key", "config-model")]
    public void Given_ConfigValues_And_No_CLI_When_Parse_Invoked_Then_It_Should_Use_Config(string configApiKey, string configModel)
    {
        // Arrange
        var config = BuildConfigWithAnthropic(configApiKey, configModel);
        var args = Array.Empty<string>();

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.Anthropic.ShouldNotBeNull();
        settings.Anthropic.ApiKey.ShouldBe(configApiKey);
        settings.Anthropic.Model.ShouldBe(configModel);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("config-api-key", "config-model", "cli-api-key", "cli-model")]
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

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("cli-api-key", "cli-model")]
    public void Given_Anthropic_With_KnownArguments_When_Parse_Invoked_Then_Help_ShouldBe_False(string cliApiKey, string cliModel)
    {
        // Arrange
        var config = BuildConfigWithAnthropic(ApiKey, Model);
        var args = new[] { "--api-key", cliApiKey, "--model", cliModel };

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.Help.ShouldBeFalse();
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("--api-key")]
    [InlineData("--model")]
    public void Given_Anthropic_With_KnownArgument_WithoutValue_When_Parse_Invoked_Then_Help_ShouldBe_False(string argument)
    {
        // Arrange
        var config = BuildConfigWithAnthropic();
        var args = new[] { argument };

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.Help.ShouldBeFalse();
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("cli-api-key", "--unknown-flag")]
    public void Given_Anthropic_With_Known_And_Unknown_Argument_When_Parse_Invoked_Then_Help_ShouldBe_True(string cliApiKey, string argument)
    {
        // Arrange
        var config = BuildConfigWithAnthropic();
        var args = new[] { "--api-key", cliApiKey, argument };

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.Help.ShouldBeTrue();
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("cli-api-key", "cli-model")]
    public void Given_CLI_Only_When_Parse_Invoked_Then_Help_Should_Be_False(string cliApiKey, string cliModel)
    {
        // Arrange
        var config = BuildConfigWithAnthropic();
        var args = new[] { "--api-key", cliApiKey, "--model", cliModel };

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.Help.ShouldBeFalse();
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData(null, null, ConnectorType.Unknown, false)]
    public void Given_AnthropicArgumentOptions_When_Creating_Instance_Then_Should_Have_Correct_Properties(string? expectedApiKey, string? expectedModel, ConnectorType expectedConnectorType, bool expectedHelp)
    {
        // Act
        var options = new AnthropicArgumentOptions();
        
        // Assert
        options.ShouldNotBeNull();
        options.ApiKey.ShouldBe(expectedApiKey);
        options.Model.ShouldBe(expectedModel);
        options.ConnectorType.ShouldBe(expectedConnectorType);
        options.Help.ShouldBe(expectedHelp);
    }
}