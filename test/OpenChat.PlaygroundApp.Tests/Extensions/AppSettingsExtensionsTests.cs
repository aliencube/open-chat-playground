using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using OpenChat.PlaygroundApp.Configurations;
using OpenChat.PlaygroundApp.Connectors;
using OpenChat.PlaygroundApp.Extensions;

namespace OpenChat.PlaygroundApp.Tests.Extensions;

public class AppSettingsExtensionsTests
{
    private static IConfiguration CreateConfiguration(Dictionary<string, string?>? configData = null)
    {
        var data = configData ?? new Dictionary<string, string?>();
        return new ConfigurationBuilder()
            .AddInMemoryCollection(data)
            .Build();
    }

    [Trait("Category", "UnitTest")]
    [Fact]
    public void Given_Null_Configuration_When_AddAppSettings_Invoked_Then_It_Should_Throw_ArgumentNullException()
    {
        // Arrange
        var services = new ServiceCollection();
        IConfiguration configuration = null!;
        var settings = new AppSettings { ConnectorType = ConnectorType.OpenAI };

        // Act
        Action action = () => services.AddAppSettings(configuration, settings);

        // Assert
        action.ShouldThrow<ArgumentNullException>();
    }

    [Trait("Category", "UnitTest")]
    [Fact]
    public void Given_Null_Settings_When_AddAppSettings_Invoked_Then_It_Should_Throw_ArgumentNullException()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = CreateConfiguration();
        AppSettings settings = null!;

        // Act
        Action action = () => services.AddAppSettings(configuration, settings);

        // Assert
        action.ShouldThrow<ArgumentNullException>();
    }

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
    [InlineData(ConnectorType.AzureAIFoundry, "azureai-commandline-model")]
    [InlineData(ConnectorType.FoundryLocal, "foundrylocal-commandline-model")]
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
            case ConnectorType.AzureAIFoundry:
                settings.AzureAIFoundry = new AzureAIFoundrySettings { DeploymentName = commandLineModel };
                break;
            case ConnectorType.FoundryLocal:
                settings.FoundryLocal = new FoundryLocalSettings { Alias = commandLineModel };
                break;
        }

        // Act
        services.AddAppSettings(configuration, settings);

        // Assert
        settings.Model.ShouldBe(commandLineModel);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData(ConnectorType.OpenAI, "Model", "openai-default-model")]
    [InlineData(ConnectorType.AzureAIFoundry, "DeploymentName", "azure-default-model")]
    [InlineData(ConnectorType.FoundryLocal, "Alias", "foundry-default-model")]
    public void Given_No_CommandLine_Model_When_AddAppSettings_Invoked_Then_It_Should_Use_Configuration_Default(
        ConnectorType connectorType, string propertyName, string defaultModel)
    {
        // Arrange
        var services = new ServiceCollection();
        var configData = new Dictionary<string, string?>
        {
            [$"{connectorType}:{propertyName}"] = defaultModel
        };
        var configuration = CreateConfiguration(configData);

        var settings = new AppSettings { ConnectorType = connectorType };
        switch (connectorType)
        {
            case ConnectorType.OpenAI:
                settings.OpenAI = new OpenAISettings();
                break;
            case ConnectorType.AzureAIFoundry:
                settings.AzureAIFoundry = new AzureAIFoundrySettings();
                break;
            case ConnectorType.FoundryLocal:
                settings.FoundryLocal = new FoundryLocalSettings();
                break;
        }

        // Act
        services.AddAppSettings(configuration, settings);

        // Assert
        settings.Model.ShouldBe(defaultModel);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData(ConnectorType.OpenAI, "Model", "openai-default-model", "openai-commandline-model")]
    [InlineData(ConnectorType.AzureAIFoundry, "DeploymentName", "azure-default-model", "azure-commandline-model")]
    [InlineData(ConnectorType.FoundryLocal, "Alias", "foundry-default-model", "foundry-commandline-model")]
    public void Given_CommandLine_Priority_When_AddAppSettings_Invoked_Then_It_Should_Override_Configuration(
        ConnectorType connectorType, string propertyName, string defaultModel, string commandLineModel)
    {
        // Arrange
        var services = new ServiceCollection();
        var configData = new Dictionary<string, string?>
        {
            [$"{connectorType}:{propertyName}"] = defaultModel
        };
        var configuration = CreateConfiguration(configData);

        var settings = new AppSettings { ConnectorType = connectorType };
         switch (connectorType)
        {
            case ConnectorType.OpenAI:
                settings.OpenAI = new OpenAISettings { Model = commandLineModel };
                break;
            case ConnectorType.AzureAIFoundry:
                settings.AzureAIFoundry = new AzureAIFoundrySettings { DeploymentName = commandLineModel };
                break;
            case ConnectorType.FoundryLocal:
                settings.FoundryLocal = new FoundryLocalSettings { Alias = commandLineModel };
                break;
        }

        // Act
        services.AddAppSettings(configuration, settings);

        // Assert
        settings.Model.ShouldBe(commandLineModel);
    }

    [Trait("Category", "UnitTest")]
    [Fact]
    public void Given_Unsupported_ConnectorType_When_AddAppSettings_Invoked_Then_It_Should_Throw_ArgumentException()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = CreateConfiguration();
        var settings = new AppSettings { ConnectorType =  ConnectorType.Unknown };

        // Act
        Action action = () => services.AddAppSettings(configuration, settings);

        // Assert
        action.ShouldThrow<ArgumentException>()
              .Message.ShouldContain("Unsupported ConnectorType");
    }
}