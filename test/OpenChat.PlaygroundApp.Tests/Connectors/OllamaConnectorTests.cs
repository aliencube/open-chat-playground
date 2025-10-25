using Microsoft.Extensions.AI;

using OpenChat.PlaygroundApp.Abstractions;
using OpenChat.PlaygroundApp.Configurations;
using OpenChat.PlaygroundApp.Connectors;

namespace OpenChat.PlaygroundApp.Tests.Connectors;

public class OllamaConnectorTests
{
	private const string BaseUrl = "http://localhost:11434";
	private const string Model = "llama3.2";

	private static AppSettings BuildAppSettings(string? baseUrl = BaseUrl, string? model = Model)
	{
		return new AppSettings
		{
			ConnectorType = ConnectorType.Ollama,
			Ollama = new OllamaSettings
			{
				BaseUrl = baseUrl,
				Model = model
			}
		};
	}

	[Trait("Category", "UnitTest")]
	[Theory]
	[InlineData(typeof(LanguageModelConnector), typeof(OllamaConnector), true)]
	[InlineData(typeof(OllamaConnector), typeof(LanguageModelConnector), false)]
	public void Given_BaseType_Then_It_Should_Be_AssignableFrom_DerivedType(Type baseType, Type derivedType, bool expected)
	{
		// Act
		var result = baseType.IsAssignableFrom(derivedType);

		// Assert
		result.ShouldBe(expected);
	}
	
	[Trait("Category", "UnitTest")]
    [Fact]
    public void Given_Null_Settings_When_Instantiated_Then_It_Should_Throw()
    {
        // Act
        Action action = () => new OllamaConnector(null!);

        // Assert
        action.ShouldThrow<ArgumentNullException>()
              .Message.ShouldContain("settings");
    }

    [Trait("Category", "UnitTest")]
    [InlineData("Ollama")]
    [Theory]
	public void Given_Settings_Is_Null_When_EnsureLanguageModelSettingsValid_Invoked_Then_It_Should_Throw(string expectedMessage)
	{
		// Arrange
		var settings = new AppSettings
		{
			ConnectorType = ConnectorType.Ollama,
			Ollama = null
		};
		var connector = new OllamaConnector(settings);

		// Act
		Action action = () => connector.EnsureLanguageModelSettingsValid();

		// Assert
		action.ShouldThrow<InvalidOperationException>()
		      .Message.ShouldContain(expectedMessage);
	}

	[Trait("Category", "UnitTest")]
	[Fact]
	public void Given_Settings_When_Instantiated_Then_It_Should_Return()
	{
		// Arrange
		var settings = BuildAppSettings();

		// Act
		var result = new OllamaConnector(settings);

		// Assert
		result.ShouldNotBeNull();
	}

    [Trait("Category", "UnitTest")]
    [InlineData("Ollama")]
    [Theory]
    public void Given_Null_Settings_When_EnsureLanguageModelSettingsValid_Invoked_Then_It_Should_Throw(string expectedMessage)
    {
        // Arrange
        var settings = new AppSettings
        {
            ConnectorType = ConnectorType.Ollama,
            Ollama = null
        };
        var connector = new OllamaConnector(settings);

        // Act
        Action action = () => connector.EnsureLanguageModelSettingsValid();

        // Assert
        action.ShouldThrow<InvalidOperationException>()
              .Message.ShouldContain(expectedMessage);
    }

	[Trait("Category", "UnitTest")]
	[Theory]
	[InlineData(null, typeof(NullReferenceException), "Object reference not set to an instance of an object")]
	[InlineData("", typeof(InvalidOperationException), "Ollama:BaseUrl")]
	[InlineData("   ", typeof(InvalidOperationException), "Ollama:BaseUrl")]
	[InlineData("\t\n\r", typeof(InvalidOperationException), "Ollama:BaseUrl")]
	public void Given_Invalid_BaseUrl_When_EnsureLanguageModelSettingsValid_Invoked_Then_It_Should_Throw(string? baseUrl, Type expectedType, string expectedMessage)
	{
		// Arrange
		var settings = BuildAppSettings(baseUrl: baseUrl);
		var connector = new OllamaConnector(settings);

		// Act
		Action action = () => connector.EnsureLanguageModelSettingsValid();

		// Assert
		action.ShouldThrow(expectedType)
		      .Message.ShouldContain(expectedMessage);
	}

	[Trait("Category", "UnitTest")]
	[Theory]
	[InlineData(null, typeof(NullReferenceException), "Object reference not set to an instance of an object")]
	[InlineData("", typeof(InvalidOperationException), "Ollama:Model")]
	[InlineData("   ", typeof(InvalidOperationException), "Ollama:Model")]
	[InlineData("\t\n\r", typeof(InvalidOperationException), "Ollama:Model")]
	public void Given_Invalid_Model_When_EnsureLanguageModelSettingsValid_Invoked_Then_It_Should_Throw(string? model, Type expectedType, string expectedMessage)
	{
		// Arrange
		var settings = BuildAppSettings(model: model);
        var connector = new OllamaConnector(settings);

		// Act
		Action action = () => connector.EnsureLanguageModelSettingsValid();

		// Assert
		action.ShouldThrow(expectedType)
		      .Message.ShouldContain(expectedMessage);
	}

	[Trait("Category", "UnitTest")]
	[Fact]
	public void Given_Valid_Settings_When_EnsureLanguageModelSettingsValid_Invoked_Then_It_Should_Return_True()
	{
		// Arrange
		var settings = BuildAppSettings();
		var connector = new OllamaConnector(settings);

		// Act
		var result = connector.EnsureLanguageModelSettingsValid();

		// Assert
		result.ShouldBeTrue();
	}

	[Trait("Category", "UnitTest")]
	[Theory]
	[InlineData(null, typeof(ArgumentNullException), "null")]
	[InlineData("", typeof(UriFormatException), "empty")]
	[InlineData("   ", typeof(UriFormatException), "Invalid URI: The format of the URI could not be determined.")]
	[InlineData("\t\n\r", typeof(UriFormatException), "Invalid URI:")]
	[InlineData("invalid-uri-format", typeof(UriFormatException), "Invalid URI: The format of the URI could not be determined.")]
	[InlineData("not-a-url", typeof(UriFormatException), "Invalid URI: The format of the URI could not be determined.")]
	public void Given_Invalid_BaseUrl_When_GetChatClientAsync_Invoked_Then_It_Should_Throw(string? baseUrl, Type expected, string message)
	{
		// Arrange
		var settings = BuildAppSettings(baseUrl: baseUrl);
		var connector = new OllamaConnector(settings);

		// Act
		Func<Task> func = async () => await connector.GetChatClientAsync();

		// Assert
		func.ShouldThrow(expected)
			.Message.ShouldContain(message);
	}
	
	[Trait("Category", "UnitTest")]
    [Theory]
    [InlineData(null, typeof(NullReferenceException), "Object reference not set to an instance of an object")]
    public void Given_Null_Model_When_GetChatClientAsync_Invoked_Then_It_Should_Throw(string? model, Type expected, string message)
    {
        // Arrange        
        var settings = BuildAppSettings(model: model);
        var connector = new HuggingFaceConnector(settings);

        // Act
        Func<Task> func = async () => await connector.GetChatClientAsync();

        // Assert
        func.ShouldThrow(expected)
            .Message.ShouldContain(message);
    }

    [Trait("Category", "IntegrationTest")]
    [Trait("Category", "LLMRequired")]
    [Fact]
    public async Task Given_Valid_Settings_When_GetChatClientAsync_Invoked_Then_It_Should_Return_ChatClient()
    {
        // Arrange
        var settings = BuildAppSettings();
        var connector = new OllamaConnector(settings);

        // Act
        var client = await connector.GetChatClientAsync();

        // Assert
        client.ShouldNotBeNull();
        client.ShouldBeAssignableTo<IChatClient>();
    }

    [Trait("Category", "UnitTest")]
    [InlineData(typeof(InvalidOperationException), "Ollama")]
    [Theory]
    public async Task Given_Null_Settings_When_CreateChatClientAsync_Invoked_Then_It_Should_Throw(Type expected, string expectedMessage)
    {
        // Arrange
        var settings = new AppSettings
        {
            ConnectorType = ConnectorType.Ollama,
            Ollama = null
        };

        // Act
        Func<Task> func = async () => await LanguageModelConnector.CreateChatClientAsync(settings);

        // Assert  
        func.ShouldThrow(expected)
            .Message.ShouldContain(expectedMessage);
    }
	
	[Trait("Category", "UnitTest")]
    [Theory]
	[InlineData(null, null, typeof(NullReferenceException), "Object reference not set to an instance of an object")]
	[InlineData(null, Model, typeof(NullReferenceException), "Object reference not set to an instance of an object")]
	[InlineData("", Model, typeof(InvalidOperationException), "Missing configuration: Ollama")]
	[InlineData("   ", Model, typeof(InvalidOperationException), "Missing configuration: Ollama")]
	[InlineData(BaseUrl, null, typeof(NullReferenceException), "Object reference not set to an instance of an object")]
	[InlineData(BaseUrl, "", typeof(InvalidOperationException), "Missing configuration: Ollama")]
	[InlineData(BaseUrl, "  ", typeof(InvalidOperationException), "Missing configuration: Ollama")]
    public void Given_Invalid_Settings_When_CreateChatClientAsync_Invoked_Then_It_Should_Throw(string? baseUrl, string? model, Type expected, string expectedMessage)
    {
        // Arrange
        var settings = new AppSettings
        {
            ConnectorType = ConnectorType.Ollama,
            Ollama = new OllamaSettings
            {
                BaseUrl = baseUrl,
                Model = model
            }
        };

        // Act
        Func<Task> func = async () => await LanguageModelConnector.CreateChatClientAsync(settings);

        // Assert  
        func.ShouldThrow(expected)
            .Message.ShouldContain(expectedMessage);
    }

	[Trait("Category", "IntegrationTest")]
	[Trait("Category", "LLMRequired")]
    [Fact]
    public async Task Given_Valid_Settings_When_CreateChatClientAsync_Invoked_Then_It_Should_Return_IChatClient()
    {
        // Arrange
        var settings = BuildAppSettings();

        // Act
        var result = await LanguageModelConnector.CreateChatClientAsync(settings);

        // Assert
        result.ShouldNotBeNull();
        result.ShouldBeAssignableTo<IChatClient>();
    }
}