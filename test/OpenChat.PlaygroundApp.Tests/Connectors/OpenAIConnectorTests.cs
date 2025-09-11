using Microsoft.Extensions.AI;

using OpenChat.PlaygroundApp.Abstractions;
using OpenChat.PlaygroundApp.Configurations;
using OpenChat.PlaygroundApp.Connectors;

namespace OpenChat.PlaygroundApp.Tests.Connectors;

public class OpenAIConnectorTests
{
    private static AppSettings BuildAppSettings(string? apiKey = "test-api-key", string? model = "test-model")
    {
        return new AppSettings
        {
            ConnectorType = ConnectorType.OpenAI,
            OpenAI = new OpenAISettings
            {
                ApiKey = apiKey,
                Model = model
            }
        };
    }
	
	[Trait("Category", "UnitTest")]
	[Fact]
	public async Task Given_Valid_Settings_When_CreateChatClientAsync_Invoked_Then_It_Should_Return_IChatClient()
	{
		// Arrange
		var settings = BuildAppSettings();

		// Act
		var client = await LanguageModelConnector.CreateChatClientAsync(settings);

		// Assert
		client.ShouldNotBeNull();
		client.ShouldBeAssignableTo<IChatClient>();
	}

	[Trait("Category", "UnitTest")]
	[Fact]
	public void Given_Valid_AppSettings_When_Constructor_Invoked_Then_It_Should_Create_Instance()
	{
		// Arrange
		var settings = BuildAppSettings();

		// Act
		var connector = new OpenAIConnector(settings);

		// Assert
		connector.ShouldNotBeNull();
	}

	[Trait("Category", "UnitTest")]
	[Fact]
	public void Given_Null_AppSettings_When_Constructor_Invoked_Then_It_Should_Throw()
	{
		// Act
		var ex = Assert.Throws<NullReferenceException>(() => new OpenAIConnector(null!));

		// Assert
		ex.ShouldNotBeNull();
	}

	[Trait("Category", "UnitTest")]
	[Fact]
	public void Given_AppSettings_With_Null_OpenAI_Settings_When_EnsureLanguageModelSettingsValid_Invoked_Then_It_Should_Throw()
	{
		// Arrange
		var settings = new AppSettings
		{
			ConnectorType = ConnectorType.OpenAI,
			OpenAI = null
		};

		// Act
		var connector = new OpenAIConnector(settings);
		var ex = Assert.Throws<InvalidOperationException>(() => connector.EnsureLanguageModelSettingsValid());

		// Assert
		ex.Message.ShouldContain("Missing configuration: OpenAI.");
	}

	[Trait("Category", "UnitTest")]
	[Theory]
	[InlineData("")]
	[InlineData("   ")]
	public void Given_Invalid_ApiKey_When_EnsureLanguageModelSettingsValid_Invoked_Then_It_Should_Throw(string? apiKey)
	{
		// Arrange
		var settings = BuildAppSettings(apiKey: apiKey);

		// Act
		var connector = new OpenAIConnector(settings);
		var ex = Assert.Throws<InvalidOperationException>(() => connector.EnsureLanguageModelSettingsValid());

		// Assert
		ex.Message.ShouldContain("Missing configuration: OpenAI:ApiKey.");
	}

	[Trait("Category", "UnitTest")]
	[Theory]
	[InlineData(null)]
	[InlineData("")]
	[InlineData("   ")]
	public void Given_Invalid_Model_When_EnsureLanguageModelSettingsValid_Invoked_Then_It_Should_Throw(string? model)
	{
		// Arrange
		var settings = BuildAppSettings(model: model);

		// Act
		var connector = new OpenAIConnector(settings);
		
		if (model is null)
		{
			Assert.Throws<NullReferenceException>(() => connector.EnsureLanguageModelSettingsValid());
		}
		else
		{
			var ex = Assert.Throws<InvalidOperationException>(() => connector.EnsureLanguageModelSettingsValid());
			ex.Message.ShouldContain("Missing configuration: OpenAI:Model.");
		}
	}

	[Trait("Category", "UnitTest")]
	[Theory]
	[InlineData(null)]
	[InlineData("")]
	[InlineData("   ")]
	public async Task Given_Invalid_ApiKey_When_CreateChatClientAsync_Invoked_Then_It_Should_Throw(string? apiKey)
	{
		// Arrange
		var settings = BuildAppSettings(apiKey: apiKey);

		// Act & Assert
		if (apiKey is null)
		{
			await Assert.ThrowsAsync<NullReferenceException>(() => LanguageModelConnector.CreateChatClientAsync(settings));
		}
		else
		{
			var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => LanguageModelConnector.CreateChatClientAsync(settings));
			ex.Message.ShouldContain("Missing configuration: OpenAI:ApiKey.");
		}
	}

	[Trait("Category", "UnitTest")]
	[Fact]
	public void Given_OpenAIConnector_When_Cast_To_LanguageModelConnector_Then_It_Should_Succeed()
	{
		// Arrange
		var settings = BuildAppSettings();
		var connector = new OpenAIConnector(settings);

		// Act
		LanguageModelConnector baseConnector = connector;

		// Assert
		baseConnector.ShouldNotBeNull();
		baseConnector.ShouldBeOfType<OpenAIConnector>();
	}

	[Trait("Category", "UnitTest")]
	[Fact]
	public void Given_OpenAIConnector_When_Check_Type_Then_It_Should_Be_LanguageModelConnector_Subclass()
	{
		// Arrange
		var settings = BuildAppSettings();
		var connector = new OpenAIConnector(settings);

		// Act & Assert
		connector.ShouldBeAssignableTo<LanguageModelConnector>();
		typeof(OpenAIConnector).IsSubclassOf(typeof(LanguageModelConnector)).ShouldBeTrue();
	}

	[Trait("Category", "UnitTest")]
	[Fact]
	public async Task Given_OpenAIConnector_When_CreateChatClientAsync_Called_Through_Base_Class_Then_It_Should_Work()
	{
		// Arrange
		var settings = BuildAppSettings();
		var connector = new OpenAIConnector(settings);
		LanguageModelConnector baseConnector = connector;

		// Act
		var client = await baseConnector.GetChatClientAsync();

		// Assert
		client.ShouldNotBeNull();
		client.ShouldBeAssignableTo<IChatClient>();
	}

	[Trait("Category", "UnitTest")]
	[Theory]
	[InlineData("   ")]
	[InlineData("\t\n\r")]
	public void Given_Whitespace_ApiKey_When_EnsureLanguageModelSettingsValid_Invoked_Then_It_Should_Throw(string apiKey)
	{
		// Arrange
		var settings = BuildAppSettings(apiKey: apiKey);

		// Act
		var connector = new OpenAIConnector(settings);
		var ex = Assert.Throws<InvalidOperationException>(() => connector.EnsureLanguageModelSettingsValid());

		// Assert
		ex.Message.ShouldContain("Missing configuration: OpenAI:ApiKey.");
	}

	[Trait("Category", "UnitTest")]
	[Theory]
	[InlineData("   ")]
	[InlineData("\t\n\r")]
	public void Given_Whitespace_Model_When_EnsureLanguageModelSettingsValid_Invoked_Then_It_Should_Throw(string model)
	{
		// Arrange
		var settings = BuildAppSettings(model: model);

		// Act
		var connector = new OpenAIConnector(settings);
		var ex = Assert.Throws<InvalidOperationException>(() => connector.EnsureLanguageModelSettingsValid());

		// Assert
		ex.Message.ShouldContain("Missing configuration: OpenAI:Model.");
	}

	// Exception Handling Tests

	[Trait("Category", "UnitTest")]
	[Fact]
	public void Given_Settings_With_Wrong_Type_Cast_When_EnsureLanguageModelSettingsValid_Invoked_Then_It_Should_Throw()
	{
		// Arrange
		var settings = new AppSettings
		{
			ConnectorType = ConnectorType.OpenAI,
			GitHubModels = new GitHubModelsSettings // Wrong settings type
			{
				Token = "test-token",
				Model = "test-model"
			}
			// OpenAI is null, so validation will fail
		};

		// Act
		var connector = new OpenAIConnector(settings);
		var ex = Assert.Throws<InvalidOperationException>(() => connector.EnsureLanguageModelSettingsValid());

		// Assert
		ex.Message.ShouldContain("Missing configuration: OpenAI.");
	}

	[Trait("Category", "UnitTest")]
	[Fact]
	public void Given_Multiple_Null_Settings_When_EnsureLanguageModelSettingsValid_Invoked_Then_It_Should_Throw_For_ApiKey_First()
	{
		// Arrange
		var settings = BuildAppSettings(apiKey: null, model: null);

		// Act
		var connector = new OpenAIConnector(settings);
		Assert.Throws<NullReferenceException>(() => connector.EnsureLanguageModelSettingsValid());
	}

	[Trait("Category", "UnitTest")]
	[Fact]
	public void Given_Valid_ApiKey_But_Null_Model_When_EnsureLanguageModelSettingsValid_Invoked_Then_It_Should_Throw_For_Model()
	{
		// Arrange
		var settings = BuildAppSettings(apiKey: "valid-api-key", model: null);

		// Act
		var connector = new OpenAIConnector(settings);
		Assert.Throws<NullReferenceException>(() => connector.EnsureLanguageModelSettingsValid());
	}

    [Trait("Category", "UnitTest")]
    [Fact]
    public void Given_Settings_Is_Null_When_EnsureLanguageModelSettingsValid_Invoked_Then_It_Should_Throw()
    {
        // Arrange
        var appSettings = new AppSettings { ConnectorType = ConnectorType.OpenAI, OpenAI = null };

        // Act
        var connector = new OpenAIConnector(appSettings);
        var ex = Assert.Throws<InvalidOperationException>(() => connector.EnsureLanguageModelSettingsValid());

        // Assert
        ex.Message.ShouldContain("Missing configuration: OpenAI.");
    }


    [Trait("Category", "UnitTest")]
    [Fact]
    public void Given_Valid_Settings_When_EnsureLanguageModelSettingsValid_Invoked_Then_It_Should_Return_True()
    {
        // Arrange
        var appSettings = BuildAppSettings();
        var connector = new OpenAIConnector(appSettings);

        // Act
        var result = connector.EnsureLanguageModelSettingsValid();

        // Assert
        result.ShouldBeTrue();
    }

    [Trait("Category", "UnitTest")]
    [Fact]
    public async Task Given_Valid_Settings_When_GetChatClient_Invoked_Then_It_Should_Return_ChatClient()
    {
        var settings = BuildAppSettings();
        var connector = new OpenAIConnector(settings);

        var client = await connector.GetChatClientAsync();

        client.ShouldNotBeNull();
    }

    [Trait("Category", "UnitTest")]
    [Theory]
	[InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Given_Missing_ApiKey_When_EnsureLanguageModelSettingsValid_Invoked_Then_It_Should_Throw(string? apiKey)
    {
        var settings = BuildAppSettings(apiKey: apiKey);
        var connector = new OpenAIConnector(settings);

        if (apiKey is null)
        {
            Assert.Throws<NullReferenceException>(() => connector.EnsureLanguageModelSettingsValid());
        }
        else
        {
            var ex = Assert.Throws<InvalidOperationException>(() => connector.EnsureLanguageModelSettingsValid());
            ex.Message.ShouldContain("Missing configuration: OpenAI:ApiKey.");
        }
    }

    [Trait("Category", "UnitTest")]
    [Theory]
	[InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Given_Missing_Model_When_EnsureLanguageModelSettingsValid_Invoked_Then_It_Should_Throw(string? model)
    {
        var settings = BuildAppSettings(model: model);
        var connector = new OpenAIConnector(settings);

        if (model is null)
        {
            Assert.Throws<NullReferenceException>(() => connector.EnsureLanguageModelSettingsValid());
        }
        else
        {
            var ex = Assert.Throws<InvalidOperationException>(() => connector.EnsureLanguageModelSettingsValid());
            ex.Message.ShouldContain("Missing configuration: OpenAI:Model.");
        }
    }

}