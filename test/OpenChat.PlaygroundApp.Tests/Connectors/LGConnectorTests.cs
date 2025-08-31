using OpenChat.PlaygroundApp.Configurations;
using OpenChat.PlaygroundApp.Connectors;
using Xunit;

namespace OpenChat.PlaygroundApp.Tests.Connectors;

/// <summary>
/// This represents the test entity for <see cref="LGConnector"/>.
/// </summary>
public class LGConnectorTests
{
    [Fact]
    public void LGConnector_WithValidSettings_ShouldCreateInstance()
    {
        // Arrange
        var settings = new AppSettings
        {
            ConnectorType = ConnectorType.LG,
            LG = new LGSettings
            {
                BaseUrl = "http://localhost:11434",
                Model = "lg-model:latest"
            }
        };

        // Act
        var connector = new LGConnector(settings);

        // Assert
        Assert.NotNull(connector);
    }

    [Fact]
    public async Task LGConnector_WithMissingBaseUrl_ShouldThrowException()
    {
        // Arrange
        var settings = new AppSettings
        {
            ConnectorType = ConnectorType.LG,
            LG = new LGSettings
            {
                BaseUrl = null, // Missing BaseUrl
                Model = "lg-model:latest"
            }
        };

        var connector = new LGConnector(settings);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => connector.GetChatClientAsync());
        
        Assert.Contains("Missing configuration: LG:BaseUrl", exception.Message);
    }

    [Fact]
    public async Task LGConnector_WithMissingModel_ShouldThrowException()
    {
        // Arrange
        var settings = new AppSettings
        {
            ConnectorType = ConnectorType.LG,
            LG = new LGSettings
            {
                BaseUrl = "http://localhost:11434",
                Model = null // Missing Model
            }
        };

        var connector = new LGConnector(settings);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => connector.GetChatClientAsync());
        
        Assert.Contains("Missing configuration: LG:Model", exception.Message);
    }
}
