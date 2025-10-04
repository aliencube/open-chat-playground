using Microsoft.Extensions.Configuration;

using OpenChat.PlaygroundApp.Abstractions;
using OpenChat.PlaygroundApp.Connectors;
using OpenChat.PlaygroundApp.Options;

namespace OpenChat.PlaygroundApp.Tests.Options;

public class DockerModelRunnerArgumentOptionsTests
{
    private const string BaseUrl = "http://localhost:12434";
    private const string Model = "ai/smollm2";

    private static IConfiguration BuildConfigWithDockerModelRunner(
        string? configBaseUrl = BaseUrl,
        string? configModel = Model)
    {
        var configDict = new Dictionary<string, string?>
        {
            ["ConnectorType"] = ConnectorType.DockerModelRunner.ToString()
        };

        if (string.IsNullOrWhiteSpace(configBaseUrl) == false)
        {
            configDict["DockerModelRunner:BaseUrl"] = configBaseUrl;
        }
        if (string.IsNullOrWhiteSpace(configModel) == false)
        {
            configDict["DockerModelRunner:Model"] = configModel;
        }

        return new ConfigurationBuilder()
                   .AddInMemoryCollection(configDict!)
                   .Build();
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData(typeof(ArgumentOptions), typeof(DockerModelRunnerArgumentOptions), true)]
    [InlineData(typeof(DockerModelRunnerArgumentOptions), typeof(ArgumentOptions), false)]
    public void Given_BaseType_Then_It_Should_Be_AssignableFrom_DerivedType(Type baseType, Type derivedType, bool expected)
    {
        // Act
        var result = baseType.IsAssignableFrom(derivedType);

        // Assert
        result.ShouldBe(expected);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("http://localhost:12434", "ai/smollm2")]
    public void Given_Nothing_When_Parse_Invoked_Then_It_Should_Set_Config(string expectedBaseUrl, string expectedModel)
    {
        // Arrange
        var config = BuildConfigWithDockerModelRunner(expectedBaseUrl, expectedModel);
        var args = Array.Empty<string>();

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.DockerModelRunner.ShouldNotBeNull();
        settings.DockerModelRunner.BaseUrl.ShouldBe(expectedBaseUrl);
        settings.DockerModelRunner.Model.ShouldBe(expectedModel);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("http://localhost:9999")]
    public void Given_CLI_BaseUrl_When_Parse_Invoked_Then_It_Should_Use_CLI_BaseUrl(string cliBaseUrl)
    {
        // Arrange
        var config = BuildConfigWithDockerModelRunner();
        var args = new[] { "--base-url", cliBaseUrl };

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.DockerModelRunner.ShouldNotBeNull();
        settings.DockerModelRunner.BaseUrl.ShouldBe(cliBaseUrl);
        settings.DockerModelRunner.Model.ShouldBe(Model);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("ai/phi-4")]
    public void Given_CLI_Model_When_Parse_Invoked_Then_It_Should_Use_CLI_Model(string cliModel)
    {
        // Arrange
        var config = BuildConfigWithDockerModelRunner();
        var args = new[] { "--model", cliModel };

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.DockerModelRunner.ShouldNotBeNull();
        settings.DockerModelRunner.BaseUrl.ShouldBe(BaseUrl);
        settings.DockerModelRunner.Model.ShouldBe(cliModel);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("http://192.168.1.100:8080", "ai/custom-model")]
    public void Given_All_CLI_Arguments_When_Parse_Invoked_Then_It_Should_Use_CLI(string cliBaseUrl, string cliModel)
    {
        // Arrange
        var config = BuildConfigWithDockerModelRunner();
        var args = new[] { "--base-url", cliBaseUrl, "--model", cliModel };

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.DockerModelRunner.ShouldNotBeNull();
        settings.DockerModelRunner.BaseUrl.ShouldBe(cliBaseUrl);
        settings.DockerModelRunner.Model.ShouldBe(cliModel);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("--base-url")]
    [InlineData("--model")]
    public void Given_CLI_ArgumentWithoutValue_When_Parse_Invoked_Then_It_Should_Use_Config(string argument)
    {
        // Arrange
        var config = BuildConfigWithDockerModelRunner();
        var args = new[] { argument };

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.DockerModelRunner.ShouldNotBeNull();
        settings.DockerModelRunner.BaseUrl.ShouldBe(BaseUrl);
        settings.DockerModelRunner.Model.ShouldBe(Model);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("--something", "else", "--another", "value")]
    public void Given_Unrelated_CLI_Arguments_When_Parse_Invoked_Then_It_Should_Use_Config(params string[] args)
    {
        // Arrange
        var config = BuildConfigWithDockerModelRunner();

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.DockerModelRunner.ShouldNotBeNull();
        settings.DockerModelRunner.BaseUrl.ShouldBe(BaseUrl);
        settings.DockerModelRunner.Model.ShouldBe(Model);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("--strange-model-name")]
    public void Given_DockerModelRunner_With_ModelName_StartingWith_Dashes_When_Parse_Invoked_Then_It_Should_Treat_As_Value(string model)
    {
        // Arrange
        var config = BuildConfigWithDockerModelRunner();
        var args = new[] { "--model", model };

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.DockerModelRunner.ShouldNotBeNull();
        settings.DockerModelRunner.Model.ShouldBe(model);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("http://config-host:7777", "config-model")]
    public void Given_ConfigValues_And_No_CLI_When_Parse_Invoked_Then_It_Should_Use_Config(string configBaseUrl, string configModel)
    {
        // Arrange
        var config = BuildConfigWithDockerModelRunner(configBaseUrl, configModel);
        var args = Array.Empty<string>();

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.DockerModelRunner.ShouldNotBeNull();
        settings.DockerModelRunner.BaseUrl.ShouldBe(configBaseUrl);
        settings.DockerModelRunner.Model.ShouldBe(configModel);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("http://config:1234", "config-model", "http://cli:5678", "cli-model")]
    public void Given_ConfigValues_And_CLI_When_Parse_Invoked_Then_It_Should_Use_CLI(
        string configBaseUrl, string configModel,
        string cliBaseUrl, string cliModel)
    {
        // Arrange
        var config = BuildConfigWithDockerModelRunner(configBaseUrl, configModel);
        var args = new[] { "--base-url", cliBaseUrl, "--model", cliModel };

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.DockerModelRunner.ShouldNotBeNull();
        settings.DockerModelRunner.BaseUrl.ShouldBe(cliBaseUrl);
        settings.DockerModelRunner.Model.ShouldBe(cliModel);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("http://localhost:8080", "ai/test-model")]
    public void Given_DockerModelRunner_With_KnownArguments_When_Parse_Invoked_Then_Help_ShouldBe_False(string cliBaseUrl, string cliModel)
    {
        // Arrange
        var config = BuildConfigWithDockerModelRunner(BaseUrl, Model);
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
    public void Given_DockerModelRunner_With_KnownArgument_WithoutValue_When_Parse_Invoked_Then_Help_ShouldBe_False(string argument)
    {
        // Arrange
        var config = BuildConfigWithDockerModelRunner();
        var args = new[] { argument };

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.Help.ShouldBeFalse();
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("http://localhost:8080", "--unknown-flag")]
    public void Given_DockerModelRunner_With_Known_And_Unknown_Argument_When_Parse_Invoked_Then_Help_ShouldBe_True(string cliBaseUrl, string argument)
    {
        // Arrange
        var config = BuildConfigWithDockerModelRunner();
        var args = new[] { "--base-url", cliBaseUrl, argument };

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.Help.ShouldBeTrue();
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("http://localhost:9090", "ai/new-model")]
    public void Given_CLI_Only_When_Parse_Invoked_Then_Help_Should_Be_False(string cliBaseUrl, string cliModel)
    {
        // Arrange
        var config = BuildConfigWithDockerModelRunner();
        var args = new[] { "--base-url", cliBaseUrl, "--model", cliModel };

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.Help.ShouldBeFalse();
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData(null, null, ConnectorType.Unknown, false)]
    public void Given_DockerModelRunnerArgumentOptions_When_Creating_Instance_Then_Should_Have_Correct_Properties(string? expectedBaseUrl, string? expectedModel, ConnectorType expectedConnectorType, bool expectedHelp)
    {
        // Act
        var options = new DockerModelRunnerArgumentOptions();

        // Assert
        options.ShouldNotBeNull();
        options.BaseUrl.ShouldBe(expectedBaseUrl);
        options.Model.ShouldBe(expectedModel);
        options.ConnectorType.ShouldBe(expectedConnectorType);
        options.Help.ShouldBe(expectedHelp);
    }
}