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

## Getting started

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

1. Navigate to the cloned repository.

    ```bash
    cd open-chat-playground
    ```

1. Get the repository root.

    ```bash
    # bash/zsh
    REPOSITORY_ROOT=$(git rev-parse --show-toplevel)
    ```

    ```powershell
    # PowerShell
    $REPOSITORY_ROOT = git rev-parse --show-toplevel
    ```

### Run on local machine

#### Use GitHub Models

1. Make sure you are at the repository root.

    ```bash
    cd $REPOSITORY_ROOT
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
    dotnet run --project $REPOSITORY_ROOT/src/OpenChat.PlaygroundApp -- --connector-type GitHubModels
    ```

1. Open your web browser, navigate to `http://localhost:5280`, and enter prompts.

### Run in container

#### Build container

1. Make sure you are at the repository root.

    ```bash
    cd $REPOSITORY_ROOT
    ```

1. Build a container.

    ```bash
    docker build -f Dockerfile -t openchat-playground:latest .
    ```

#### Run container

##### Use GitHub Models

1. Get GitHub PAT.

    ```bash
    # bash/zsh
    TOKEN=$(dotnet user-secrets --project ./src/OpenChat.PlaygroundApp list --json | \
              sed -n '/^\/\//d; p' | jq -r '."GitHubModels:Token"')
    ```

    ```bash
    # PowerShell
    $TOKEN = (dotnet user-secrets --project ./src/OpenChat.PlaygroundApp list --json | `
                Select-String -NotMatch '^//(BEGIN|END)' | ConvertFrom-Json).'GitHubModels:Token'
    ```

1. Run the app.

    ```bash
    # From locally built container
    docker run -i --rm -p 8080:8080 -e GitHubModels__Token=$TOKEN openchat-playground:latest --connector-type GitHubModels
    ```

    ```bash
    # From GitHub Container Registry
    docker run -i --rm -p 8080:8080 -e GitHubModels__Token=$TOKEN ghcr.io/aliencube/open-chat-playground/openchat-playground:latest --connector-type GitHubModels
    ```

1. Open your web browser, navigate to `http://localhost:8080`, and enter prompts.

### Run tests

#### Build app

1. Make sure you are at the repository root.

    ```bash
    cd $REPOSITORY_ROOT
    ```

1. Build the app.

    ```bash
    dotnet restore && dotnet build
    ```

#### Unit tests

1. Make sure you are at the repository root.

    ```bash
    cd $REPOSITORY_ROOT
    ```

1. Run tests.

    ```bash
    dotnet test --filter "Category=UnitTest"
    ```

#### Integration tests

1. Make sure you are at the repository root.

    ```bash
    cd $REPOSITORY_ROOT
    ```

1. Install playwright.

    ```bash
    pwsh $REPOSITORY_ROOT/test/OpenChat.PlaygroundApp.Tests/bin/Debug/net{YOUR_VERSION}/playwright.ps1 install
    ```

1. Run the app.

    ```bash
    dotnet run --project $REPOSITORY_ROOT/src/OpenChat.PlaygroundApp
    ```

1. Run tests.

    ```bash
    # With LLM provider
    dotnet test --filter "Category=IntegrationTest"
    ```

    ```bash
    # Without LLM provider
    dotnet test --filter "Category=IntegrationTest & Category!=LLMRequired"
    ```

### Run on Azure

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

### Configure GitHub Actions for CI/CD Pipeline

1. Make sure you are at the repository root.

    ```bash
    cd $REPOSITORY_ROOT
    ```

1. Make sure you've logged in to Azure.

    ```bash
    azd auth login --check-status
    ```

1. Run pipeline config.

    ```bash
    azd pipeline config
    ```

1. Answer the question below:

   - `? Select how to authenticate the pipeline to Azure` 👉 `Federated Service Principal (SP + OIDC)`
   - `? Would you like to commit and push your local changes to start the configured CI pipeline?` 👉 `No`

1. Once the configuration is done, push a new commit to GitHub to run the GitHub Actions workflow.
