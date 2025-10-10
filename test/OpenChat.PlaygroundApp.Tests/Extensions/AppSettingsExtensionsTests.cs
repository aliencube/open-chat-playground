using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using OpenChat.PlaygroundApp.Configurations;
using OpenChat.PlaygroundApp.Connectors;
using OpenChat.PlaygroundApp.Extensions;

namespace OpenChat.PlaygroundApp.Tests.Extensions;

public class AppSettingsExtensionsTests
{
    [Trait("Category", "UnitTest")]
    [Fact]
    public void Given_Valid_Parameters_When_AddAppSettings_Invoked_Then_It_Should_Return_Services()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = CreateConfiguration();
        var settings = new AppSettings { ConnectorType = ConnectorType.OpenAI };

        // Act
        var result = services.AddAppSettings(configuration, settings);

        // Assert
        result.ShouldNotBeNull();
        result.ShouldBeSameAs(services);
    }

    [Trait("Category", "UnitTest")]
    [Fact]
    public void Given_Valid_Parameters_When_AddAppSettings_Invoked_Then_It_Should_Register_AppSettings_As_Singleton()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = CreateConfiguration();
        var settings = new AppSettings { ConnectorType = ConnectorType.OpenAI };

        // Act
        services.AddAppSettings(configuration, settings);
        var descriptor = services.FirstOrDefault(sd => sd.ServiceType == typeof(AppSettings));

        // Assert
        descriptor.ShouldNotBeNull();
        descriptor.Lifetime.ShouldBe(ServiceLifetime.Singleton);
        descriptor.ImplementationInstance.ShouldBeSameAs(settings);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData(ConnectorType.OpenAI, "openai-commandline-model")]
    [InlineData(ConnectorType.GitHubModels, "github-commandline-model")]
    [InlineData(ConnectorType.AzureAIFoundry, "azureai-commandline-model")]
    [InlineData(ConnectorType.LG, "lg-commandline-model")]
    public void Given_CommandLine_Model_When_AddAppSettings_Invoked_Then_It_Should_Use_CommandLine_Value(
        ConnectorType connectorType, string commandLineModel)
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = CreateConfiguration();
        var settings = new AppSettings { ConnectorType = connectorType };

        switch (connectorType)
        {
            case ConnectorType.OpenAI:
                settings.OpenAI = new OpenAISettings { Model = commandLineModel };
                break;
            case ConnectorType.GitHubModels:
                settings.GitHubModels = new GitHubModelsSettings { Model = commandLineModel };
                break;
            case ConnectorType.AzureAIFoundry:
                settings.AzureAIFoundry = new AzureAIFoundrySettings { DeploymentName = commandLineModel };
                break;
            case ConnectorType.LG:
                settings.LG = new LGSettings { Model = commandLineModel };
                break;
        }

        // Act
        services.AddAppSettings(configuration, settings);

        // Assert
        settings.Model.ShouldBe(commandLineModel);
    }

    [Trait("Category", "UnitTest")]
    [Fact]
    public void Given_No_CommandLine_Model_When_AddAppSettings_Invoked_Then_It_Should_Use_Configuration_Default()
    {
        // Arrange
        var services = new ServiceCollection();
        var configData = new Dictionary<string, string?>
        {
            ["OpenAI:Model"] = "gpt-4.1-mini"
        };
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(configData)
            .Build();

        var settings = new AppSettings 
        { 
            ConnectorType = ConnectorType.OpenAI,
            OpenAI = new OpenAISettings { Model = null }
        };

        // Act
        services.AddAppSettings(configuration, settings);

        // Assert
        settings.Model.ShouldBe("gpt-4.1-mini");
    }

    [Trait("Category", "UnitTest")]
    [Fact]
    public void Given_CommandLine_Priority_When_AddAppSettings_Invoked_Then_It_Should_Override_Configuration()
    {
        // Arrange
        var services = new ServiceCollection();
        var configData = new Dictionary<string, string?>
        {
            ["OpenAI:Model"] = "gpt-4.1-mini"
        };
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(configData)
            .Build();

        var settings = new AppSettings 
        { 
            ConnectorType = ConnectorType.OpenAI,
            OpenAI = new OpenAISettings { Model = "commandline-model" }
        };

        // Act
        services.AddAppSettings(configuration, settings);

        // Assert
        settings.Model.ShouldBe("commandline-model");
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData(999)]
    [InlineData(-1)]
    [InlineData(int.MaxValue)]
    public void Given_Unsupported_ConnectorType_When_AddAppSettings_Invoked_Then_It_Should_Throw_ArgumentException(int invalidConnectorType)
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = CreateConfiguration();
        var settings = new AppSettings { ConnectorType = (ConnectorType)invalidConnectorType };

        // Act
        Action action = () => services.AddAppSettings(configuration, settings);

        // Assert
        action.ShouldThrow<ArgumentException>();
    }

    private static IConfiguration CreateConfiguration(Dictionary<string, string?>? configData = null)
    {
        var data = configData ?? new Dictionary<string, string?>();
        return new ConfigurationBuilder()
            .AddInMemoryCollection(data)
            .Build();
    }
}