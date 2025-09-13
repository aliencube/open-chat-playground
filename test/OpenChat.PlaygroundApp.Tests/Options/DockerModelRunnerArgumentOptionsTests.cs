using OpenChat.PlaygroundApp.Abstractions;
using OpenChat.PlaygroundApp.Options;

namespace OpenChat.PlaygroundApp.Tests.Options;

public class DockerModelRunnerArgumentOptionsTests
{
    [Trait("Category", "UnitTest")]
    [Fact]
    public void DockerModelRunnerArgumentOptions_Should_Inherit_From_ArgumentOptions()
    {
        // Arrange & Act
        var dockerModelRunnerOptions = new DockerModelRunnerArgumentOptions();

        // Assert
        dockerModelRunnerOptions.ShouldBeAssignableTo<ArgumentOptions>();
    }
}