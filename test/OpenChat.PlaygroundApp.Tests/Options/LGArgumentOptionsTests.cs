using Microsoft.Extensions.Configuration;

using OpenChat.PlaygroundApp.Abstractions;
using OpenChat.PlaygroundApp.Connectors;

namespace OpenChat.PlaygroundApp.Tests.Options;

public class LGArgumentOptionsTests
{
    private const string BaseUrl = "http://localhost:11434";
    private const string Model = "exaone3.5:latest";

    private static IConfiguration BuildConfigWithLG(
        string? configBaseUrl = BaseUrl,
        string? configModel = Model,
        string? envBaseUrl = null,
        string? envModel = null)
    {
        // Base configuration values (lowest priority)
        var configDict = new Dictionary<string, string?>
        {
            ["ConnectorType"] = ConnectorType.LG.ToString()
        };

        if (string.IsNullOrWhiteSpace(configBaseUrl) == false)
        {
            configDict["LG:BaseUrl"] = configBaseUrl;
        }
        if (string.IsNullOrWhiteSpace(configModel) == false)
        {
            configDict["LG:Model"] = configModel;
        }

        if (string.IsNullOrWhiteSpace(envBaseUrl) && string.IsNullOrWhiteSpace(envModel))
        {
            return new ConfigurationBuilder()
                       .AddInMemoryCollection(configDict!)
                       .Build();
        }

        // Environment variables (medium priority)
        var envDict = new Dictionary<string, string?>();
        if (string.IsNullOrWhiteSpace(envBaseUrl) == false)
        {
            envDict["LG:BaseUrl"] = envBaseUrl;
        }
        if (string.IsNullOrWhiteSpace(envModel) == false)
        {
            envDict["LG:Model"] = envModel;
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
        var config = BuildConfigWithLG();
        var args = Array.Empty<string>();

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.LG.ShouldNotBeNull();
        settings.LG.BaseUrl.ShouldBe(BaseUrl);
        settings.LG.Model.ShouldBe(Model);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("http://test.example.com:8080")]
    public void Given_CLI_BaseUrl_When_Parse_Invoked_Then_It_Should_Use_CLI_BaseUrl(string cliBaseUrl)
    {
        // Arrange
        var config = BuildConfigWithLG();
        var args = new[] { "--base-url", cliBaseUrl };

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.LG.ShouldNotBeNull();
        settings.LG.BaseUrl.ShouldBe(cliBaseUrl);
        settings.LG.Model.ShouldBe(Model);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("exaone3.5:latest")]
    public void Given_CLI_Model_When_Parse_Invoked_Then_It_Should_Use_CLI_Model(string cliModel)
    {
        // Arrange
        var config = BuildConfigWithLG();
        var args = new[] { "--model", cliModel };

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.LG.ShouldNotBeNull();
        settings.LG.BaseUrl.ShouldBe(BaseUrl);
        settings.LG.Model.ShouldBe(cliModel);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("http://test.example.com:8080", "exaone3.5:latest")]
    public void Given_All_CLI_Arguments_When_Parse_Invoked_Then_It_Should_Use_CLI(string cliBaseUrl, string cliModel)
    {
        // Arrange
        var config = BuildConfigWithLG();
        var args = new[] { "--base-url", cliBaseUrl, "--model", cliModel };

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.LG.ShouldNotBeNull();
        settings.LG.BaseUrl.ShouldBe(cliBaseUrl);
        settings.LG.Model.ShouldBe(cliModel);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("--base-url")]
    [InlineData("--model")]
    public void Given_CLI_ArgumentWithoutValue_When_Parse_Invoked_Then_It_Should_Use_Config(string argument)
    {
        // Arrange
        var config = BuildConfigWithLG();
        var args = new[] { argument };

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.LG.ShouldNotBeNull();
        settings.LG.BaseUrl.ShouldBe(BaseUrl);
        settings.LG.Model.ShouldBe(Model);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("--something", "else", "--another", "value")]
    public void Given_Unrelated_CLI_Arguments_When_Parse_Invoked_Then_It_Should_Use_Config(params string[] args)
    {
        // Arrange
        var config = BuildConfigWithLG();

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.LG.ShouldNotBeNull();
        settings.LG.BaseUrl.ShouldBe(BaseUrl);
        settings.LG.Model.ShouldBe(Model);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("http://localhost:11434", "exaone3.5:latest")]
    public void Given_ConfigValues_And_No_CLI_When_Parse_Invoked_Then_It_Should_Use_Config(string configBaseUrl, string configModel)
    {
        // Arrange
        var config = BuildConfigWithLG(configBaseUrl, configModel);
        var args = Array.Empty<string>();

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.LG.ShouldNotBeNull();
        settings.LG.BaseUrl.ShouldBe(configBaseUrl);
        settings.LG.Model.ShouldBe(configModel);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("http://localhost:11434", "exaone3.5:latest",
                "http://cli.example:8080", "exaone3.5:latest")]
    public void Given_ConfigValues_And_CLI_When_Parse_Invoked_Then_It_Should_Use_CLI(
        string configBaseUrl, string configModel,
        string cliBaseUrl, string cliModel)
    {
        // Arrange
        var config = BuildConfigWithLG(configBaseUrl, configModel);
        var args = new[] { "--base-url", cliBaseUrl, "--model", cliModel };

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.LG.ShouldNotBeNull();
        settings.LG.BaseUrl.ShouldBe(cliBaseUrl);
        settings.LG.Model.ShouldBe(cliModel);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("--strange-model-name")]
    public void Given_LG_With_ModelName_StartingWith_Dashes_When_Parse_Invoked_Then_It_Should_Treat_As_Value(string model)
    {
        // Arrange
        var config = BuildConfigWithLG();
        var args = new[] { "--model", model };

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.LG.ShouldNotBeNull();
        settings.LG.Model.ShouldBe(model);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("http://env.example:9090", "exaone3.5:latest")]
    public void Given_EnvironmentVariables_And_No_Config_When_Parse_Invoked_Then_It_Should_Use_EnvironmentVariables(
        string envBaseUrl, string envModel)
    {
        // Arrange
        var config = BuildConfigWithLG(
            configBaseUrl: null, configModel: null,
            envBaseUrl: envBaseUrl, envModel: envModel);
        var args = Array.Empty<string>();

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.LG.ShouldNotBeNull();
        settings.LG.BaseUrl.ShouldBe(envBaseUrl);
        settings.LG.Model.ShouldBe(envModel);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("http://config.example:7070", "exaone3.5:latest",
                "http://env.example:8080", "exaone3.5:latest")]
    public void Given_ConfigValues_And_EnvironmentVariables_When_Parse_Invoked_Then_It_Should_Use_EnvironmentVariables(
        string configBaseUrl, string configModel,
        string envBaseUrl, string envModel)
    {
        // Arrange
        var config = BuildConfigWithLG(
            configBaseUrl, configModel,
            envBaseUrl, envModel);
        var args = Array.Empty<string>();

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.LG.ShouldNotBeNull();
        settings.LG.BaseUrl.ShouldBe(envBaseUrl);
        settings.LG.Model.ShouldBe(envModel);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("http://config.example:7070", "exaone3.5:latest",
                "http://env.example:8080", "exaone3.5:latest",
                "http://cli.example:9090", "exaone3.5:latest")]
    public void Given_ConfigValues_And_EnvironmentVariables_And_CLI_When_Parse_Invoked_Then_It_Should_Use_CLI(
        string configBaseUrl, string configModel,
        string envBaseUrl, string envModel,
        string cliBaseUrl, string cliModel)
    {
        // Arrange
        var config = BuildConfigWithLG(
            configBaseUrl, configModel,
            envBaseUrl, envModel);
        var args = new[] { "--base-url", cliBaseUrl, "--model", cliModel };

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.LG.ShouldNotBeNull();
        settings.LG.BaseUrl.ShouldBe(cliBaseUrl);
        settings.LG.Model.ShouldBe(cliModel);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("http://config.example:7070", "exaone3.5:latest",
                "http://env.example:8080", null)]
    public void Given_Partial_EnvironmentVariables_When_Parse_Invoked_Then_It_Should_Mix_Config_And_Environment(
        string configBaseUrl, string configModel,
        string envBaseUrl, string? envModel)
    {
        // Arrange
        var config = BuildConfigWithLG(
            configBaseUrl, configModel,
            envBaseUrl, envModel);
        var args = Array.Empty<string>();

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.LG.ShouldNotBeNull();
        settings.LG.BaseUrl.ShouldBe(envBaseUrl); // From environment
        settings.LG.Model.ShouldBe(configModel);  // From config (no env override)
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("http://config.example:7070", "exaone3.5:latest",
                null, "exaone3.5:latest",
                "http://cli.example:9090")]
    public void Given_Mixed_Priority_Sources_When_Parse_Invoked_Then_It_Should_Respect_Priority_Order(
        string configBaseUrl, string configModel,
        string? envBaseUrl, string envModel,
        string cliBaseUrl)
    {
        // Arrange
        var config = BuildConfigWithLG(
            configBaseUrl, configModel,
            envBaseUrl, envModel);
        var args = new[] { "--base-url", cliBaseUrl };

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.LG.ShouldNotBeNull();
        settings.LG.BaseUrl.ShouldBe(cliBaseUrl);    // CLI wins (highest priority)
        settings.LG.Model.ShouldBe(envModel);        // Env wins over config (medium priority)
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("http://localhost:11434", "exaone3.5:latest")]
    public void Given_LG_With_KnownArguments_When_Parse_Invoked_Then_Help_ShouldBe_False(string cliBaseUrl, string cliModel)
    {
        // Arrange
        var config = BuildConfigWithLG(BaseUrl, Model);
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
    public void Given_LG_With_KnownArgument_WithoutValue_When_Parse_Invoked_Then_Help_ShouldBe_False(string argument)
    {
        // Arrange
        var config = BuildConfigWithLG();
        var args = new[] { argument };

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.Help.ShouldBeFalse();
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("http://localhost:11434", "--unknown-flag")]
    public void Given_LG_With_Known_And_Unknown_Argument_When_Parse_Invoked_Then_Help_ShouldBe_True(string cliBaseUrl, string argument)
    {
        // Arrange
        var config = BuildConfigWithLG();
        var args = new[] { "--base-url", cliBaseUrl, argument };

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.Help.ShouldBeTrue();
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("http://env.example:8080", "exaone3.5:latest")]
    public void Given_EnvironmentVariables_Only_When_Parse_Invoked_Then_Help_Should_Be_False(
        string envBaseUrl, string envModel)
    {
        // Arrange
        var config = BuildConfigWithLG(
            configBaseUrl: null, configModel: null,
            envBaseUrl: envBaseUrl, envModel: envModel);
        var args = Array.Empty<string>();

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.Help.ShouldBeFalse();
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("http://cli.example:9090", "exaone3.5:latest")]
    public void Given_CLI_Only_When_Parse_Invoked_Then_Help_Should_Be_False(string cliBaseUrl, string cliModel)
    {
        // Arrange
        var config = BuildConfigWithLG();
        var args = new[] { "--base-url", cliBaseUrl, "--model", cliModel };

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.Help.ShouldBeFalse();
    }
}
