using Microsoft.Extensions.Configuration;

using OpenChat.PlaygroundApp.Abstractions;
using OpenChat.PlaygroundApp.Connectors;
using OpenChat.PlaygroundApp.Constants;
using OpenChat.PlaygroundApp.Options;

namespace OpenChat.PlaygroundApp.Tests.Options;

public class FoundryLocalArgumentOptionsTests
{
    private const string Alias = "test-foundry-local-alias";
    private const string AliasConfigKey = "FoundryLocal:Alias";

    private static IConfiguration BuildConfigWithFoundryLocal(
        string? configAlias = Alias,
        string? envAlias = null)
    {
        // Base configuration values (lowest priority)
        var configDict = new Dictionary<string, string?>
        {
            [AppSettingConstants.ConnectorType] = ConnectorType.FoundryLocal.ToString(),
        };

        if (string.IsNullOrWhiteSpace(configAlias) == false)
        {
            configDict[AliasConfigKey] = configAlias;
        }

        if (string.IsNullOrWhiteSpace(envAlias) == true)
        {
            return new ConfigurationBuilder()
                       .AddInMemoryCollection(configDict!)
                       .Build();
        }

        // Environment variables (medium priority)
        var envDict = new Dictionary<string, string?>();
        if (string.IsNullOrWhiteSpace(envAlias) == false)
        {
            envDict[AliasConfigKey] = envAlias;
        }

        return new ConfigurationBuilder()
                   .AddInMemoryCollection(configDict!)  // Base configuration (lowest priority)
                   .AddInMemoryCollection(envDict!)     // Environment variables (medium priority)
                   .Build();
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData(typeof(ArgumentOptions), typeof(FoundryLocalArgumentOptions), true)]
    [InlineData(typeof(FoundryLocalArgumentOptions), typeof(ArgumentOptions), false)]
    public void Given_BaseType_Then_It_Should_Be_AssignableFrom_DerivedType(Type baseType, Type derivedType, bool expected)
    {
        // Act
        var result = baseType.IsAssignableFrom(derivedType);

        // Assert
        result.ShouldBe(expected);
    }

    [Trait("Category", "UnitTest")]
    [Fact]
    public void Given_Nothing_When_Parse_Invoked_Then_It_Should_Set_Config()
    {
        // Arrange
        var config = BuildConfigWithFoundryLocal();
        var args = Array.Empty<string>();

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.FoundryLocal.ShouldNotBeNull();
        settings.FoundryLocal.Alias.ShouldBe(Alias);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("cli-foundry-local-alias")]
    public void Given_CLI_Alias_When_Parse_Invoked_Then_It_Should_Use_CLI_Alias(string cliAlias)
    {
        // Arrange
        var config = BuildConfigWithFoundryLocal();
        var args = new[]
        {
            ArgumentOptionConstants.FoundryLocal.Alias, cliAlias
        };

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.FoundryLocal.ShouldNotBeNull();
        settings.FoundryLocal.Alias.ShouldBe(cliAlias);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData(ArgumentOptionConstants.FoundryLocal.Alias)]
    public void Given_CLI_ArgumentWithoutValue_When_Parse_Invoked_Then_It_Should_Use_Config(string argument)
    {
        // Arrange
        var config = BuildConfigWithFoundryLocal();
        var args = new[] { argument };

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.FoundryLocal.ShouldNotBeNull();
        settings.FoundryLocal.Alias.ShouldBe(Alias);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("--something", "else", "--another", "value")]
    public void Given_Unrelated_CLI_Arguments_When_Parse_Invoked_Then_It_Should_Use_Config(params string[] args)
    {
        // Arrange
        var config = BuildConfigWithFoundryLocal();

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.FoundryLocal.ShouldNotBeNull();
        settings.FoundryLocal.Alias.ShouldBe(Alias);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("--strange-alias-name")]
    public void Given_FoundryLocal_With_AliasName_StartingWith_Dashes_When_Parse_Invoked_Then_It_Should_Treat_As_Value(string alias)
    {
        // Arrange
        var config = BuildConfigWithFoundryLocal();
        var args = new[]
        {
            ArgumentOptionConstants.FoundryLocal.Alias, alias
        };

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.FoundryLocal.ShouldNotBeNull();
        settings.FoundryLocal.Alias.ShouldBe(alias);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("config-foundry-local-alias")]
    public void Given_ConfigValues_And_No_CLI_When_Parse_Invoked_Then_It_Should_Use_Config(string configAlias)
    {
        // Arrange
        var config = BuildConfigWithFoundryLocal(configAlias);
        var args = Array.Empty<string>();

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.FoundryLocal.ShouldNotBeNull();
        settings.FoundryLocal.Alias.ShouldBe(configAlias);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("config-foundry-local-alias", "cli-foundry-local-alias")]
    public void Given_ConfigValues_And_CLI_When_Parse_Invoked_Then_It_Should_Use_CLI(
        string configAlias, string cliAlias)
    {
        // Arrange
        var config = BuildConfigWithFoundryLocal(configAlias);
        var args = new[]
        {
            ArgumentOptionConstants.FoundryLocal.Alias, cliAlias
        };

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.FoundryLocal.ShouldNotBeNull();
        settings.FoundryLocal.Alias.ShouldBe(cliAlias);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("env-foundry-local-alias")]
    public void Given_EnvironmentVariables_And_No_Config_When_Parse_Invoked_Then_It_Should_Use_EnvironmentVariables(string envAlias)
    {
        // Arrange
        var config = BuildConfigWithFoundryLocal(
            configAlias: null,
            envAlias: envAlias);
        var args = Array.Empty<string>();

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.FoundryLocal.ShouldNotBeNull();
        settings.FoundryLocal.Alias.ShouldBe(envAlias);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("config-foundry-local-alias", "env-foundry-local-alias")]
    public void Given_ConfigValues_And_EnvironmentVariables_When_Parse_Invoked_Then_It_Should_Use_EnvironmentVariables(string configAlias, string envAlias)
    {
        // Arrange
        var config = BuildConfigWithFoundryLocal(configAlias, envAlias);
        var args = Array.Empty<string>();

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.FoundryLocal.ShouldNotBeNull();
        settings.FoundryLocal.Alias.ShouldBe(envAlias);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("config-foundry-local-alias", "env-foundry-local-alias", "cli-foundry-local-alias")]
    public void Given_ConfigValues_And_EnvironmentVariables_And_CLI_When_Parse_Invoked_Then_It_Should_Use_CLI(
        string configAlias, string envAlias, string cliAlias)
    {
        // Arrange
        var config = BuildConfigWithFoundryLocal(configAlias, envAlias);
        var args = new[]
        {
            ArgumentOptionConstants.FoundryLocal.Alias, cliAlias
        };

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.FoundryLocal.ShouldNotBeNull();
        settings.FoundryLocal.Alias.ShouldBe(cliAlias);
    }


    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("cli-foundry-local-alias")]
    public void Given_FoundryLocal_With_KnownArguments_When_Parse_Invoked_Then_Help_Should_Be_False(string cliAlias)
    {
        // Arrange
        var config = BuildConfigWithFoundryLocal();
        var args = new[]
        {
            ArgumentOptionConstants.FoundryLocal.Alias, cliAlias
        };

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.Help.ShouldBeFalse();
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData(ArgumentOptionConstants.FoundryLocal.Alias)]
    public void Given_FoundryLocal_With_KnownArgument_WithoutValue_When_Parse_Invoked_Then_Help_Should_Be_False(string argument)
    {
        // Arrange
        var config = BuildConfigWithFoundryLocal();
        var args = new[] { argument };

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.Help.ShouldBeFalse();
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("cli-foundry-local-alias", "--unknown-flag")]
    public void Given_FoundryLocal_With_Known_And_Unknown_Argument_When_Parse_Invoked_Then_Help_Should_Be_True(
        string cliAlias, string argument)
    {
        // Arrange
        var config = BuildConfigWithFoundryLocal();
        var args = new[]
        {
            ArgumentOptionConstants.FoundryLocal.Alias, cliAlias,
            argument
        };

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.Help.ShouldBeTrue();
    }


    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("env-foundry-local-alias")]
    public void Given_EnvironmentVariables_Only_When_Parse_Invoked_Then_Help_Should_Be_False(string envAlias)
    {
        // Arrange
        var config = BuildConfigWithFoundryLocal(
            configAlias: null,
            envAlias: envAlias);
        var args = Array.Empty<string>();

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.Help.ShouldBeFalse();
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("cli-foundry-local-alias")]
    public void Given_CLI_Only_When_Parse_Invoked_Then_Help_Should_Be_False(string cliAlias)
    {
        // Arrange
        var config = BuildConfigWithFoundryLocal();
        var args = new[]
        {
            ArgumentOptionConstants.FoundryLocal.Alias, cliAlias
        };

        // Act
        var settings = ArgumentOptions.Parse(config, args);

        // Assert
        settings.Help.ShouldBeFalse();
    }
}