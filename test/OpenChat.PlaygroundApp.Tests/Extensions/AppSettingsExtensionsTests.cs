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
    public void Given_Null_Services_When_AddAppSettings_Invoked_Then_It_Should_Throw_ArgumentNullException()
    {
        // Arrange
        IServiceCollection services = null!;
        var configuration = CreateConfiguration();
        var settings = new AppSettings { ConnectorType = ConnectorType.OpenAI };

        // Act
        Action action = () => services.AddAppSettings(configuration, settings);

        // Assert
        action.ShouldThrow<ArgumentNullException>();
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
    [Fact]
    public void Given_OpenAI_CommandLine_Model_When_AddAppSettings_Invoked_Then_It_Should_Use_CommandLine_Value()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = CreateConfiguration();
        var commandLineModel = "openai-commandline-model";
        var settings = new AppSettings
        {
            ConnectorType = ConnectorType.OpenAI,
            OpenAI = new OpenAISettings { Model = commandLineModel }
        };

        // Act
        services.AddAppSettings(configuration, settings);

        // Assert
        settings.Model.ShouldBe(commandLineModel);
    }

    [Trait("Category", "UnitTest")]
    [Fact]
    public void Given_AzureAIFoundry_CommandLine_Model_When_AddAppSettings_Invoked_Then_It_Should_Use_CommandLine_Value()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = CreateConfiguration();
        var commandLineModel = "azureai-commandline-model";
        var settings = new AppSettings
        {
            ConnectorType = ConnectorType.AzureAIFoundry,
            AzureAIFoundry = new AzureAIFoundrySettings { DeploymentName = commandLineModel }
        };

        // Act
        services.AddAppSettings(configuration, settings);

        // Assert
        settings.Model.ShouldBe(commandLineModel);
    }

    [Trait("Category", "UnitTest")]
    [Fact]
    public void Given_FoundryLocal_CommandLine_Model_When_AddAppSettings_Invoked_Then_It_Should_Use_CommandLine_Value()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = CreateConfiguration();
        var commandLineModel = "foundrylocal-commandline-model";
        var settings = new AppSettings
        {
            ConnectorType = ConnectorType.FoundryLocal,
            FoundryLocal = new FoundryLocalSettings { Alias = commandLineModel }
        };

        // Act
        services.AddAppSettings(configuration, settings);

        // Assert
        settings.Model.ShouldBe(commandLineModel);
    }

    [Trait("Category", "UnitTest")]
    [Fact]
    public void Given_OpenAI_No_CommandLine_Model_When_AddAppSettings_Invoked_Then_It_Should_Use_Configuration_Default()
    {
        // Arrange
        var services = new ServiceCollection();
        var defaultModel = "openai-default-model";
        var configData = new Dictionary<string, string?>
        {
            ["OpenAI:Model"] = defaultModel
        };
        var configuration = CreateConfiguration(configData);

        var settings = new AppSettings
        {
            ConnectorType = ConnectorType.OpenAI,
            OpenAI = new OpenAISettings()
        };

        // Act
        services.AddAppSettings(configuration, settings);

        // Assert
        settings.Model.ShouldBe(defaultModel);
    }

    [Trait("Category", "UnitTest")]
    [Fact]
    public void Given_AzureAIFoundry_No_CommandLine_Model_When_AddAppSettings_Invoked_Then_It_Should_Use_Configuration_Default()
    {
        // Arrange
        var services = new ServiceCollection();
        var defaultModel = "azure-default-model";
        var configData = new Dictionary<string, string?>
        {
            ["AzureAIFoundry:DeploymentName"] = defaultModel
        };
        var configuration = CreateConfiguration(configData);

        var settings = new AppSettings
        {
            ConnectorType = ConnectorType.AzureAIFoundry,
            AzureAIFoundry = new AzureAIFoundrySettings()
        };

        // Act
        services.AddAppSettings(configuration, settings);

        // Assert
        settings.Model.ShouldBe(defaultModel);
    }

    [Trait("Category", "UnitTest")]
    [Fact]
    public void Given_FoundryLocal_No_CommandLine_Model_When_AddAppSettings_Invoked_Then_It_Should_Use_Configuration_Default()
    {
        // Arrange
        var services = new ServiceCollection();
        var defaultModel = "foundry-default-model";
        var configData = new Dictionary<string, string?>
        {
            ["FoundryLocal:Alias"] = defaultModel
        };
        var configuration = CreateConfiguration(configData);

        var settings = new AppSettings
        {
            ConnectorType = ConnectorType.FoundryLocal,
            FoundryLocal = new FoundryLocalSettings()
        };

        // Act
        services.AddAppSettings(configuration, settings);

        // Assert
        settings.Model.ShouldBe(defaultModel);
    }

    [Trait("Category", "UnitTest")]
    [Fact]
    public void Given_OpenAI_CommandLine_Priority_When_AddAppSettings_Invoked_Then_It_Should_Override_Configuration()
    {
        // Arrange
        var services = new ServiceCollection();
        var defaultModel = "openai-default-model";
        var commandLineModel = "openai-commandline-model";
        var configData = new Dictionary<string, string?>
        {
            ["OpenAI:Model"] = defaultModel
        };
        var configuration = CreateConfiguration(configData);

        var settings = new AppSettings
        {
            ConnectorType = ConnectorType.OpenAI,
            OpenAI = new OpenAISettings { Model = commandLineModel }
        };

        // Act
        services.AddAppSettings(configuration, settings);

        // Assert
        settings.Model.ShouldBe(commandLineModel);
    }

    [Trait("Category", "UnitTest")]
    [Fact]
    public void Given_AzureAIFoundry_CommandLine_Priority_When_AddAppSettings_Invoked_Then_It_Should_Override_Configuration()
    {
        // Arrange
        var services = new ServiceCollection();
        var defaultModel = "azure-default-model";
        var commandLineModel = "azure-commandline-model";
        var configData = new Dictionary<string, string?>
        {
            ["AzureAIFoundry:DeploymentName"] = defaultModel
        };
        var configuration = CreateConfiguration(configData);

        var settings = new AppSettings
        {
            ConnectorType = ConnectorType.AzureAIFoundry,
            AzureAIFoundry = new AzureAIFoundrySettings { DeploymentName = commandLineModel }
        };

        // Act
        services.AddAppSettings(configuration, settings);

        // Assert
        settings.Model.ShouldBe(commandLineModel);
    }

    [Trait("Category", "UnitTest")]
    [Fact]
    public void Given_FoundryLocal_CommandLine_Priority_When_AddAppSettings_Invoked_Then_It_Should_Override_Configuration()
    {
        // Arrange
        var services = new ServiceCollection();
        var defaultModel = "foundry-default-model";
        var commandLineModel = "foundry-commandline-model";
        var configData = new Dictionary<string, string?>
        {
            ["FoundryLocal:Alias"] = defaultModel
        };
        var configuration = CreateConfiguration(configData);

        var settings = new AppSettings
        {
            ConnectorType = ConnectorType.FoundryLocal,
            FoundryLocal = new FoundryLocalSettings { Alias = commandLineModel }
        };

        // Act
        services.AddAppSettings(configuration, settings);

        // Assert
        settings.Model.ShouldBe(commandLineModel);
    }    
}