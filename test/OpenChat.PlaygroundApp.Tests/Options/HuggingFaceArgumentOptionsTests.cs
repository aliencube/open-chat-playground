using Microsoft.Extensions.Configuration;

using OpenChat.PlaygroundApp.Abstractions;
using OpenChat.PlaygroundApp.Connectors;

namespace OpenChat.PlaygroundApp.Tests.Options;

public class HuggingFaceArgumentOptionsTests
{
    private const string BaseUrl = "https://huggingface.co/api";
    private const string Model = "hf-model-name";

    private static IConfiguration BuildConfigWithHuggingFace(
        string? configBaseUrl = BaseUrl,
        string? configModel = Model)
    {
        // Base configuration values (lowest priority)
        var configDict = new Dictionary<string, string?>
        {
            ["ConnectorType"] = ConnectorType.HuggingFace.ToString()
        };

        if (string.IsNullOrWhiteSpace(configBaseUrl) == false)
        {
            configDict["HuggingFace:BaseUrl"] = configBaseUrl;
        }
        if (string.IsNullOrWhiteSpace(configModel) == false)
        {
            configDict["HuggingFace:Model"] = configModel;
        }

        return new ConfigurationBuilder()
                               .AddInMemoryCollection(configDict!)
                               .Build();
    }

    [Trait("Category", "UnitTest")]
    [Fact]
    public void Given_Nothing_When_Parse_Invoked_Then_It_Should_Set_Config()
    {
        // Arrange
        var config = BuildConfigWithHuggingFace();
        var args = Array.Empty<string>();

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.HuggingFace.ShouldNotBeNull();
        settings.HuggingFace.BaseUrl.ShouldBe(BaseUrl);
        settings.HuggingFace.Model.ShouldBe(Model);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("https://example.test/api")]
    public void Given_CLI_BaseUrl_When_Parse_Invoked_Then_It_Should_Use_CLI_BaseUrl(string cliBaseUrl)
    {
        // Arrange
        var config = BuildConfigWithHuggingFace();
        var args = new[] { "--base-url", cliBaseUrl };

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.HuggingFace.ShouldNotBeNull();
        settings.HuggingFace.BaseUrl.ShouldBe(cliBaseUrl);
        settings.HuggingFace.Model.ShouldBe(Model);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("test-model")]
    public void Given_CLI_Model_When_Parse_Invoked_Then_It_Should_Use_CLI_Model(string cliModel)
    {
        // Arrange
        var config = BuildConfigWithHuggingFace();
        var args = new[] { "--model", cliModel };

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.HuggingFace.ShouldNotBeNull();
        settings.HuggingFace.BaseUrl.ShouldBe(BaseUrl);
        settings.HuggingFace.Model.ShouldBe(cliModel);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("https://cli.example/api", "cli-model")]
    public void Given_All_CLI_Arguments_When_Parse_Invoked_Then_It_Should_Use_CLI(string cliBaseUrl, string cliModel)
    {
        // Arrange
        var config = BuildConfigWithHuggingFace();
        var args = new[] { "--base-url", cliBaseUrl, "--model", cliModel };

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.HuggingFace.ShouldNotBeNull();
        settings.HuggingFace.BaseUrl.ShouldBe(cliBaseUrl);
        settings.HuggingFace.Model.ShouldBe(cliModel);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("--base-url")]
    [InlineData("--model")]
    public void Given_CLI_ArgumentWithoutValue_When_Parse_Invoked_Then_It_Should_Use_Config(string argument)
    {
        // Arrange
        var config = BuildConfigWithHuggingFace();
        var args = new[] { argument };

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.HuggingFace.ShouldNotBeNull();
        settings.HuggingFace.BaseUrl.ShouldBe(BaseUrl);
        settings.HuggingFace.Model.ShouldBe(Model);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("--something", "else", "--another", "value")]
    public void Given_Unrelated_CLI_Arguments_When_Parse_Invoked_Then_It_Should_Use_Config(params string[] args)
    {
        // Arrange
        var config = BuildConfigWithHuggingFace();

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.HuggingFace.ShouldNotBeNull();
        settings.HuggingFace.BaseUrl.ShouldBe(BaseUrl);
        settings.HuggingFace.Model.ShouldBe(Model);
    }


    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("--strange-model-name")]
    public void Given_HuggingFace_With_ModelName_StartingWith_Dashes_When_Parse_Invoked_Then_It_Should_Treat_As_Value(string model)
    {
        // Arrange
        var config = BuildConfigWithHuggingFace();
        var args = new[] { "--model", model };

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.HuggingFace.ShouldNotBeNull();
        settings.HuggingFace.Model.ShouldBe(model);
    }


    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("https://models.hf.ai/api", "hf-model")]
    public void Given_HuggingFace_With_KnownArguments_When_Parse_Invoked_Then_Help_ShouldBe_False(string cliBaseUrl, string cliModel)
    {
        // Arrange
        var config = BuildConfigWithHuggingFace(BaseUrl, Model);
        var args = new[] { "--base-url", cliBaseUrl, "--model", cliModel };

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.Help.ShouldBeFalse();
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("--base-url")]
    [InlineData("--model")]
    public void Given_HuggingFace_With_KnownArgument_WithoutValue_When_Parse_Invoked_Then_Help_ShouldBe_False(string argument)
    {
        // Arrange
        var config = BuildConfigWithHuggingFace();
        var args = new[] { argument };

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.Help.ShouldBeFalse();
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("https://models.hf.ai/api", "--unknown-flag")]
    public void Given_HuggingFace_With_Known_And_Unknown_Argument_When_Parse_Invoked_Then_Help_ShouldBe_True(string cliBaseUrl, string argument)
    {
        // Arrange
        var config = BuildConfigWithHuggingFace();
        var args = new[] { "--base-url", cliBaseUrl, argument };

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.Help.ShouldBeTrue();
    }


    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("https://cli.example/api", "cli-model")]
    public void Given_CLI_Only_When_Parse_Invoked_Then_Help_Should_Be_False(string cliBaseUrl, string cliModel)
    {
        // Arrange
        var config = BuildConfigWithHuggingFace();
        var args = new[] { "--base-url", cliBaseUrl, "--model", cliModel };

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.Help.ShouldBeFalse();
    }
}

