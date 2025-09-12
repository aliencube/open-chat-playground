using Microsoft.Extensions.Configuration;

using OpenChat.PlaygroundApp.Abstractions;
using OpenChat.PlaygroundApp.Connectors;
using OpenChat.PlaygroundApp.Options;

namespace OpenChat.PlaygroundApp.Tests.Options;

public class UpstageArgumentOptionsTests
{
    private const string BaseUrl = "https://test.upstage.ai/api/v1";
    private const string ApiKey = "upstage-api-key";
    private const string Model = "upstage-model-name";

    private static IConfiguration BuildConfigWithUpstage(
        string? configBaseUrl = BaseUrl,
        string? configApiKey = ApiKey,
        string? configModel = Model)
    {
        // Base configuration values
        var configDict = new Dictionary<string, string?>
        {
            ["ConnectorType"] = ConnectorType.Upstage.ToString()
        };

        if (string.IsNullOrWhiteSpace(configBaseUrl) == false)
        {
            configDict["Upstage:BaseUrl"] = configBaseUrl;
        }
        if (string.IsNullOrWhiteSpace(configApiKey) == false)
        {
            configDict["Upstage:ApiKey"] = configApiKey;
        }
        if (string.IsNullOrWhiteSpace(configModel) == false)
        {
            configDict["Upstage:Model"] = configModel;
        }

        return new ConfigurationBuilder()
                   .AddInMemoryCollection(configDict!)
                   .Build();
    }

    [Trait("Category", "UnitTest")]
    [Fact]
    public void Given_UpstageArgumentOptions_When_Instantiated_Then_It_Should_Inherit_From_ArgumentOptions()
    {
        // Arrange & Act
        var upstageOptions = new UpstageArgumentOptions();

        // Assert
        upstageOptions.ShouldBeAssignableTo<ArgumentOptions>();
        upstageOptions.ShouldBeOfType<UpstageArgumentOptions>();
    }

    [Trait("Category", "UnitTest")]
    [Fact]
    public void Given_UpstageArgumentOptions_When_Created_Then_It_Should_Have_Default_Properties()
    {
        // Arrange & Act
        var upstageOptions = new UpstageArgumentOptions();

        // Assert
        upstageOptions.BaseUrl.ShouldBeNull();
        upstageOptions.ApiKey.ShouldBeNull();
        upstageOptions.Model.ShouldBeNull();
        upstageOptions.Help.ShouldBeFalse(); // Inherited from ArgumentOptions
    }

    [Trait("Category", "UnitTest")]
    [Fact]
    public void Given_UpstageArgumentOptions_When_Properties_Set_Then_It_Should_Store_Values()
    {
        // Arrange
        var upstageOptions = new UpstageArgumentOptions();
        var expectedBaseUrl = "https://test.upstage.ai/v1";
        var expectedApiKey = "test-api-key";
        var expectedModel = "test-model";

        // Act
        upstageOptions.BaseUrl = expectedBaseUrl;
        upstageOptions.ApiKey = expectedApiKey;
        upstageOptions.Model = expectedModel;

        // Assert
        upstageOptions.BaseUrl.ShouldBe(expectedBaseUrl);
        upstageOptions.ApiKey.ShouldBe(expectedApiKey);
        upstageOptions.Model.ShouldBe(expectedModel);
    }

    [Trait("Category", "UnitTest")]
    [Fact]
    public void Given_UpstageArgumentOptions_When_Accessed_Through_Base_Type_Then_It_Should_Work()
    {
        // Arrange
        ArgumentOptions baseOptions = new UpstageArgumentOptions();

        // Act & Assert
        baseOptions.ShouldBeOfType<UpstageArgumentOptions>();
        baseOptions.Help.ShouldBeFalse(); // Can access inherited properties

        // Cast to specific type to access Upstage-specific properties
        var upstageOptions = baseOptions as UpstageArgumentOptions;
        upstageOptions.ShouldNotBeNull();
        upstageOptions!.BaseUrl.ShouldBeNull();
        upstageOptions.ApiKey.ShouldBeNull();
        upstageOptions.Model.ShouldBeNull();
    }

    [Trait("Category", "UnitTest")]
    [Fact]
    public void Given_Nothing_When_Parse_Invoked_Then_It_Should_Set_Config()
    {
        // Arrange
        var config = BuildConfigWithUpstage();
        var args = Array.Empty<string>();

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.Upstage.ShouldNotBeNull();
        settings.Upstage.BaseUrl.ShouldBe(BaseUrl);
        settings.Upstage.ApiKey.ShouldBe(ApiKey);
        settings.Upstage.Model.ShouldBe(Model);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("https://cli.upstage.ai/api/v1")]
    public void Given_CLI_BaseUrl_When_Parse_Invoked_Then_It_Should_Use_CLI_BaseUrl(string cliBaseUrl)
    {
        // Arrange
        var config = BuildConfigWithUpstage();
        var args = new[] { "--base-url", cliBaseUrl };

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.Upstage.ShouldNotBeNull();
        settings.Upstage.BaseUrl.ShouldBe(cliBaseUrl);
        settings.Upstage.ApiKey.ShouldBe(ApiKey);
        settings.Upstage.Model.ShouldBe(Model);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("cli-api-key")]
    public void Given_CLI_ApiKey_When_Parse_Invoked_Then_It_Should_Use_CLI_ApiKey(string cliApiKey)
    {
        // Arrange
        var config = BuildConfigWithUpstage();
        var args = new[] { "--api-key", cliApiKey };

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.Upstage.ShouldNotBeNull();
        settings.Upstage.BaseUrl.ShouldBe(BaseUrl);
        settings.Upstage.ApiKey.ShouldBe(cliApiKey);
        settings.Upstage.Model.ShouldBe(Model);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("cli-model")]
    public void Given_CLI_Model_When_Parse_Invoked_Then_It_Should_Use_CLI_Model(string cliModel)
    {
        // Arrange
        var config = BuildConfigWithUpstage();
        var args = new[] { "--model", cliModel };

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.Upstage.ShouldNotBeNull();
        settings.Upstage.BaseUrl.ShouldBe(BaseUrl);
        settings.Upstage.ApiKey.ShouldBe(ApiKey);
        settings.Upstage.Model.ShouldBe(cliModel);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("https://cli.upstage.ai/api/v1", "cli-api-key", "cli-model")]
    public void Given_All_CLI_Arguments_When_Parse_Invoked_Then_It_Should_Use_CLI(string cliBaseUrl, string cliApiKey, string cliModel)
    {
        // Arrange
        var config = BuildConfigWithUpstage();
        var args = new[] { "--base-url", cliBaseUrl, "--api-key", cliApiKey, "--model", cliModel };

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.Upstage.ShouldNotBeNull();
        settings.Upstage.BaseUrl.ShouldBe(cliBaseUrl);
        settings.Upstage.ApiKey.ShouldBe(cliApiKey);
        settings.Upstage.Model.ShouldBe(cliModel);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("--base-url")]
    [InlineData("--api-key")]
    [InlineData("--model")]
    public void Given_CLI_ArgumentWithoutValue_When_Parse_Invoked_Then_It_Should_Use_Config(string argument)
    {
        // Arrange
        var config = BuildConfigWithUpstage();
        var args = new[] { argument };

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.Upstage.ShouldNotBeNull();
        settings.Upstage.BaseUrl.ShouldBe(BaseUrl);
        settings.Upstage.ApiKey.ShouldBe(ApiKey);
        settings.Upstage.Model.ShouldBe(Model);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("--something", "else", "--another", "value")]
    public void Given_Unrelated_CLI_Arguments_When_Parse_Invoked_Then_It_Should_Use_Config(params string[] args)
    {
        // Arrange
        var config = BuildConfigWithUpstage();

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.Upstage.ShouldNotBeNull();
        settings.Upstage.BaseUrl.ShouldBe(BaseUrl);
        settings.Upstage.ApiKey.ShouldBe(ApiKey);
        settings.Upstage.Model.ShouldBe(Model);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("--strange-model-name")]
    public void Given_Upstage_With_ModelName_StartingWith_Dashes_When_Parse_Invoked_Then_It_Should_Treat_As_Value(string model)
    {
        // Arrange
        var config = BuildConfigWithUpstage();
        var args = new[] { "--model", model };

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.Upstage.ShouldNotBeNull();
        settings.Upstage.Model.ShouldBe(model);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("https://config.upstage.ai/api/v1", "config-api-key", "config-model")]
    public void Given_ConfigValues_And_No_CLI_When_Parse_Invoked_Then_It_Should_Use_Config(string configBaseUrl, string configApiKey, string configModel)
    {
        // Arrange
        var config = BuildConfigWithUpstage(configBaseUrl, configApiKey, configModel);
        var args = Array.Empty<string>();

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.Upstage.ShouldNotBeNull();
        settings.Upstage.BaseUrl.ShouldBe(configBaseUrl);
        settings.Upstage.ApiKey.ShouldBe(configApiKey);
        settings.Upstage.Model.ShouldBe(configModel);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("https://config.upstage.ai/api/v1", "config-api-key", "config-model",
                "https://cli.upstage.ai/api/v1", "cli-api-key", "cli-model")]
    public void Given_ConfigValues_And_CLI_When_Parse_Invoked_Then_It_Should_Use_CLI(
        string configBaseUrl, string configApiKey, string configModel,
        string cliBaseUrl, string cliApiKey, string cliModel)
    {
        // Arrange
        var config = BuildConfigWithUpstage(configBaseUrl, configApiKey, configModel);
        var args = new[] { "--base-url", cliBaseUrl, "--api-key", cliApiKey, "--model", cliModel };

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.Upstage.ShouldNotBeNull();
        settings.Upstage.BaseUrl.ShouldBe(cliBaseUrl);
        settings.Upstage.ApiKey.ShouldBe(cliApiKey);
        settings.Upstage.Model.ShouldBe(cliModel);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("https://cli.upstage.ai/api/v1", "cli-api-key", "cli-model")]
    public void Given_Upstage_With_KnownArguments_When_Parse_Invoked_Then_Help_ShouldBe_False(string cliBaseUrl, string cliApiKey, string cliModel)
    {
        // Arrange
        var config = BuildConfigWithUpstage(BaseUrl, ApiKey, Model);
        var args = new[] { "--base-url", cliBaseUrl, "--api-key", cliApiKey, "--model", cliModel };

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.Help.ShouldBeFalse();
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("--base-url")]
    [InlineData("--api-key")]
    [InlineData("--model")]
    public void Given_Upstage_With_KnownArgument_WithoutValue_When_Parse_Invoked_Then_Help_ShouldBe_False(string argument)
    {
        // Arrange
        var config = BuildConfigWithUpstage();
        var args = new[] { argument };

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.Help.ShouldBeFalse();
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("https://cli.upstage.ai/api/v1", "--unknown-flag")]
    public void Given_Upstage_With_Known_And_Unknown_Argument_When_Parse_Invoked_Then_Help_ShouldBe_True(string cliBaseUrl, string argument)
    {
        // Arrange
        var config = BuildConfigWithUpstage();
        var args = new[] { "--base-url", cliBaseUrl, argument };

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.Help.ShouldBeTrue();
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("https://cli.upstage.ai/api/v1", "cli-api-key", "cli-model")]
    public void Given_CLI_Only_When_Parse_Invoked_Then_Help_Should_Be_False(string cliBaseUrl, string cliApiKey, string cliModel)
    {
        // Arrange
        var config = BuildConfigWithUpstage();
        var args = new[] { "--base-url", cliBaseUrl, "--api-key", cliApiKey, "--model", cliModel };

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.Help.ShouldBeFalse();
    }
}
