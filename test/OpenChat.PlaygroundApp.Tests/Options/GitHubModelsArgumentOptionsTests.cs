using Microsoft.Extensions.Configuration;

using OpenChat.PlaygroundApp.Abstractions;
using OpenChat.PlaygroundApp.Configurations;
using OpenChat.PlaygroundApp.Connectors;

namespace OpenChat.PlaygroundApp.Tests.Options;

public class GitHubModelsArgumentOptionsTests
{
    private const string Endpoint = "https://github-models/inference";
    private const string Token = "github-pat";
    private const string Model = "github-model-name";

	private static IConfiguration BuildConfigFrom(params (string Key, string Value)[] pairs)
    {
        var dict = pairs.ToDictionary(p => p.Key, p => (string?)p.Value);
        var config = new ConfigurationBuilder()
                         .AddInMemoryCollection(dict!)
                         .Build();

        return config;
    }

	private static IConfiguration BuildConfigWithGitHubModels(
		string? endpoint = Endpoint,
		string? token = Token,
		string? model = Model)
	{
		return BuildConfigFrom(
			("ConnectorType", ConnectorType.GitHubModels.ToString()),
			("GitHubModels:Endpoint", endpoint!),
			("GitHubModels:Token", token!),
			("GitHubModels:Model", model!)
		);
	}

	private static AppSettings Parse(IConfiguration config, params string[] args)
	{
		var fullArgs = new List<string>();
		if (args.Any(a => a.Equals("--connector-type", StringComparison.CurrentCultureIgnoreCase) || a.Equals("-c", StringComparison.CurrentCultureIgnoreCase)) == false)
		{
			fullArgs.AddRange(["--connector-type", ConnectorType.GitHubModels.ToString()]);
		}
		fullArgs.AddRange(args);
		return ArgumentOptions.Parse(config, [.. fullArgs]);
	}

	[Trait("Category", "UnitTest")]
	[Theory]
	[InlineData("https://example.test/inference")]
	public void Given_Endpoint_When_Parse_Invoked_Then_It_Should_Set_Endpoint(string endpoint)
	{
		var config = BuildConfigWithGitHubModels();
		var settings = Parse(config, "--endpoint", endpoint);

		settings.ConnectorType.ShouldBe(ConnectorType.GitHubModels);
		settings.GitHubModels.ShouldNotBeNull();
		settings.GitHubModels.Endpoint.ShouldBe(endpoint);
		settings.GitHubModels.Token.ShouldBe(Token);
		settings.GitHubModels.Model.ShouldBe(Model);
	}

	[Trait("Category", "UnitTest")]
	[Theory]
	[InlineData("test-token")]
	public void Given_Token_When_Parse_Invoked_Then_It_Should_Set_Token(string token)
	{
		var config = BuildConfigWithGitHubModels();
		var settings = Parse(config, "--token", token);

		settings.GitHubModels.ShouldNotBeNull();
		settings.GitHubModels.Endpoint.ShouldBe(Endpoint);
		settings.GitHubModels.Token.ShouldBe(token);
		settings.GitHubModels.Model.ShouldBe(Model);
	}

	[Trait("Category", "UnitTest")]
	[Theory]
	[InlineData("test-model")]
	public void Given_Model_When_Parse_Invoked_Then_It_Should_Set_Model(string model)
	{
		var config = BuildConfigWithGitHubModels();

		var settings = Parse(config, "--model", model);

		settings.GitHubModels.ShouldNotBeNull();
		settings.GitHubModels.Endpoint.ShouldBe(Endpoint);
		settings.GitHubModels.Token.ShouldBe(Token);
		settings.GitHubModels.Model.ShouldBe(model);
	}

	[Trait("Category", "UnitTest")]
	[Theory]
	[InlineData("https://example.test/inference", "test-token", "openai/gpt-5-mini")]
	public void Given_AllArguments_When_Parse_Invoked_Then_It_Should_Set_All(string endpoint, string token, string model)
	{
		var config = BuildConfigWithGitHubModels();
		var settings = Parse(config, "--endpoint", endpoint, "--token", token, "--model", model);

		settings.GitHubModels.ShouldNotBeNull();
		settings.GitHubModels.Endpoint.ShouldBe(endpoint);
		settings.GitHubModels.Token.ShouldBe(token);
		settings.GitHubModels.Model.ShouldBe(model);
	}

	[Trait("Category", "UnitTest")]
	[Theory]
	[InlineData("--endpoint")]
	[InlineData("--token")]
	[InlineData("--model")]
	public void Given_ArgumentWithoutValue_When_Parse_Invoked_Then_It_Should_Not_Set_Property(string argument)
	{
		var config = BuildConfigFrom(("ConnectorType", ConnectorType.GitHubModels.ToString()));
		var settings = Parse(config, argument);

        settings.GitHubModels.ShouldNotBeNull();
        settings.GitHubModels.Endpoint.ShouldBeNull();
        settings.GitHubModels.Token.ShouldBeNull();
        settings.GitHubModels.Model.ShouldBeNull();
	}

	[Trait("Category", "UnitTest")]
	[Theory]
	[InlineData("--something", "else", "--another", "value")]
	public void Given_UnrelatedArguments_When_Parse_Invoked_Then_It_Should_Ignore_Them(params string[] args)
	{
		var config = BuildConfigFrom(("ConnectorType", ConnectorType.GitHubModels.ToString()));
		var settings = Parse(config, args);

        settings.GitHubModels.ShouldNotBeNull();
        settings.GitHubModels.Endpoint.ShouldBeNull();
        settings.GitHubModels.Token.ShouldBeNull();
        settings.GitHubModels.Model.ShouldBeNull();
	}

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("https://models.github.ai/inference", "{{GITHUB_PAT}}", "openai/gpt-5-mini")]
	public void Given_ConfigValues_And_No_CLI_When_Parse_Invoked_Then_It_Should_Use_Config(string endpoint, string token, string model)
    {
        var config = BuildConfigWithGitHubModels(endpoint, token, model);
        var settings = Parse(config);

        settings.GitHubModels.ShouldNotBeNull();
        settings.GitHubModels.Endpoint.ShouldBe(endpoint);
        settings.GitHubModels.Token.ShouldBe(token);
        settings.GitHubModels.Model.ShouldBe(model);
    }

	[Trait("Category", "UnitTest")]
	[Theory]
	[InlineData("https://models.github.ai/inference", "{{GITHUB_PAT}}", "openai/gpt-5-mini",
		        "https://cli.example/inference", "cli-token", "openai/gpt-5-large")]
	public void Given_ConfigValues_And_CLI_When_Parse_Invoked_Then_CLI_Should_Override_Config(
        string configEndpoint, string configToken, string configModel,
        string cliEndpoint, string cliToken, string cliModel)
	{
        var config = BuildConfigWithGitHubModels(configEndpoint, configToken, configModel);
		var settings = Parse(config, "--endpoint", cliEndpoint, "--token", cliToken, "--model", cliModel);

		settings.GitHubModels.ShouldNotBeNull();
		settings.GitHubModels.Endpoint.ShouldBe(cliEndpoint);
		settings.GitHubModels.Token.ShouldBe(cliToken);
		settings.GitHubModels.Model.ShouldBe(cliModel);
	}

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("https://models.github.ai/inference", "pat", "openai/gpt-5-mini")]
    public void Given_GitHubModels_With_KnownArguments_When_Parse_Invoked_Then_Help_ShouldBe_False(string endpoint, string token, string model)
    {
        var config = BuildConfigWithGitHubModels(Endpoint, Token, Model);
        var args = new[] { "--endpoint", endpoint, "--token", token, "--model", model };
        var settings = ArgumentOptions.Parse(config, args);

        settings.Help.ShouldBeFalse();
        settings.ConnectorType.ShouldBe(ConnectorType.GitHubModels);
        settings.GitHubModels.ShouldNotBeNull();
        settings.GitHubModels.Endpoint.ShouldBe(endpoint);
        settings.GitHubModels.Token.ShouldBe(token);
        settings.GitHubModels.Model.ShouldBe(model);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("--endpoint")]
    [InlineData("--token")]
    [InlineData("--model")]
    public void Given_GitHubModels_With_KnownArgument_WithoutValue_When_Parse_Invoked_Then_Help_ShouldBe_False(string argument)
    {
        var config = BuildConfigWithGitHubModels();
        var args = new[] { argument };

        var settings = ArgumentOptions.Parse(config, args);

        settings.Help.ShouldBeFalse();
        settings.GitHubModels.ShouldNotBeNull();
        settings.GitHubModels.Endpoint.ShouldBe(Endpoint);
        settings.GitHubModels.Token.ShouldBe(Token);
        settings.GitHubModels.Model.ShouldBe(Model);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("https://models.github.ai/inference", "--unknown-flag")]
    public void Given_GitHubModels_With_Known_And_Unknown_Argument_When_Parse_Invoked_Then_Help_ShouldBe_True(string endpoint, string argument)
    {
        var config = BuildConfigWithGitHubModels();
        var args = new[] { "--endpoint", endpoint, argument };

        var settings = ArgumentOptions.Parse(config, args);

        settings.Help.ShouldBeTrue();
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("--strange-model-name")]
    public void Given_GitHubModels_With_ModelName_StartingWith_Dashes_When_Parse_Invoked_Then_It_Should_Treat_As_Value(string model)
    {
        var config = BuildConfigWithGitHubModels();
        var args = new[] { "--model", model };

        var settings = ArgumentOptions.Parse(config, args);

        settings.Help.ShouldBeFalse();
        settings.GitHubModels.ShouldNotBeNull();
        settings.GitHubModels.Model.ShouldBe(model);
    }
}
