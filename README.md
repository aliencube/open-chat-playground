# AI Open Chat Playground

This provides a web UI for AI chat playground that is able to connect virtually any LLM from any platform.

## Prerequisites

- [Azure Subscription](https://azure.microsoft.com/free)
- [.NET SDK 9](https://dotnet.microsoft.com/download/dotnet/9.0)
- [Visual Studio Code](https://code.visualstudio.com/) + [C# DevKit](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.csdevkit)
- [Docker Desktop](https://docs.docker.com/desktop/)
- [PowerShell 7.4+](https://learn.microsoft.com/powershell/scripting/install/installing-powershell)
- [Azure Developer CLI](https://learn.microsoft.com/azure/developer/azure-developer-cli/install-azd)
- [Azure CLI](https://learn.microsoft.com/cli/azure/install-azure-cli) + [Container Apps extension](https://learn.microsoft.com/cli/azure/azure-cli-extensions-overview)
- [GitHub CLI](https://cli.github.com/)

## Getting Started

### Get the repository ready

1. Login to GitHub.

    ```bash
    gh auth login
    ```

1. Check login status.

    ```bash
    gh auth status
    ```

1. Fork this repository to your account and clone the forked repository to your local machine.

    ```bash
    gh repo fork aliencube/open-chat-playground --clone --default-branch-only
    ```

### Run it locally

1. Get the repository root.

    ```bash
    # bash/zsh
    REPOSITORY_ROOT=$(git rev-parse --show-toplevel)
    ```

    ```powershell
    # PowerShell
    $REPOSITORY_ROOT = git rev-parse --show-toplevel
    ```

1. Add GitHub Personal Access Token (PAT) for GitHub Models connection. Make sure you should replace `{{YOUR_TOKEN}}` with your GitHub PAT.

    ```bash
    # bash/zsh
    dotnet user-secrets --project $REPOSITORY_ROOT/src/OpenChat.PlaygroundApp \
        set GitHubModels:Token "{{YOUR_TOKEN}}"
    ```

    ```bash
    # PowerShell
    dotnet user-secrets --project $REPOSITORY_ROOT/src/OpenChat.PlaygroundApp `
        set GitHubModels:Token "{{YOUR_TOKEN}}"
    ```

    > For more details about GitHub PAT, refer to the doc, [Managing your personal access tokens](https://docs.github.com/authentication/keeping-your-account-and-data-secure/managing-your-personal-access-tokens).

1. Run the app.

    ```bash
    dotnet run --project $REPOSITORY_ROOT/src/OpenChat.PlaygroundApp
    ```

1. Open your web browser and navigate to `http://localhost:8080` and enter prompts.

### Run on Azure

1. Get the repository root.

    ```bash
    # bash/zsh
    REPOSITORY_ROOT=$(git rev-parse --show-toplevel)
    ```

    ```powershell
    # PowerShell
    $REPOSITORY_ROOT = git rev-parse --show-toplevel
    ```

1. Make sure you are at the repository root.

    ```bash
    cd $REPOSITORY_ROOT
    ```

1. Login to Azure.

    ```bash
    # Login to Azure Dev CLI
    azd auth login
    
    # Login to Azure CLI
    az login
    ```

1. Check login status.

    ```bash
    # Azure Dev CLI
    azd auth login --check-status
    
    # Azure CLI
    az account show
    ```

1. Run the following commands in order to provision and deploy the app.

    ```bash
    azd up
    ```

   > **NOTE**: You will be asked to provide Azure subscription and location for deployment.

## Run Test

1. Get the repository root.

    ```bash
    # bash/zsh
    REPOSITORY_ROOT=$(git rev-parse --show-toplevel)
    ```

    ```powershell
    # PowerShell
    $REPOSITORY_ROOT = git rev-parse --show-toplevel
    ```

1. Restore packages and install playwright.

    ```bash
    cd $REPOSITORY_ROOT && dotnet restore
    pwsh $REPOSITORY_ROOT/test/OpenChat.PlaygroundApp.Tests/bin/Debug/net{YOUR_VERSION}/playwright.ps1 install
    ```

1. Run the app.

    ```bash
    dotnet run --project $REPOSITORY_ROOT/src/OpenChat.PlaygroundApp
    ```

1. Run tests.

    ```bash
    # With LLM provider
    cd $REPOSITORY_ROOT && dotnet test

    # Without LLM provider
    cd $REPOSITORY_ROOT && dotnet test --filter "Category!=LLMRequired"
    ```