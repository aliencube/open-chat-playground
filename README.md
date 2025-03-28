# AI Open Chat Playground

This provides a web UI for AI chat playground that is able to connect virtually any LLM from any platform.

## Prerequisites

- [.NET SDK 9](https://dotnet.microsoft.com/download/dotnet/9.0)
- [Visual Studio Code](https://code.visualstudio.com/) + [C# DevKit](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.csdevkit)
- [Docker Desktop](https://docs.docker.com/desktop/) or [Podman](https://podman.io/docs/installation)
- [PowerShell 7.4+](https://learn.microsoft.com/powershell/scripting/install/installing-powershell)
- [Azure Developer CLI](https://learn.microsoft.com/azure/developer/azure-developer-cli/install-azd)
- [Azure CLI](https://learn.microsoft.com/cli/azure/install-azure-cli) + [Container Apps extension](https://learn.microsoft.com/cli/azure/azure-cli-extensions-overview)
- [GitHub CLI](https://cli.github.com/)

## Getting Started

1. Login to Azure and GitHub.

    ```bash
    # Login to Azure Dev CLI
    azd auth login
    
    # Login to Azure CLI
    az login
    
    # Login to GitHub
    gh auth login
    ```

1. Check login status.

    ```bash
    # Azure Dev CLI
    azd auth login --check-status
    
    # Azure CLI
    az account show
    
    # GitHub CLI
    gh auth status
    ```

1. Fork this repository to your account and clone the forked repository to your local machine.

    ```bash
    gh repo fork aliencube/open-chat-playground --clone --default-branch-only
    ```

1. Run the following commands in order to provision and deploy the app.

    ```bash
    # zsh/bash
    AZURE_ENV_NAME="proxy$((RANDOM%9000+1000))"
    azd init -e $AZURE_ENV_NAME
    azd up
    
    # PowerShell
    $AZURE_ENV_NAME = "proxy$(Get-Random -Minimum 1000 -Maximum 9999)"
    azd init -e $AZURE_ENV_NAME
    azd up
    ```

   > **NOTE**: The `AZURE_ENV_NAME` variable is an arbitrary name for the Azure environment. You can change it to your preferred one.

1. Run the following command to provision Azure resources that are required for the app.

    ```bash
    # zsh/bash
    AZURE_RESOURCE_GROUP="rg-$AZURE_ENV_NAME"
    AZURE_LOCATION=$(azd env get-value AZURE_LOCATION)
    az deployment group create \
        -g $AZURE_RESOURCE_GROUP \
        --template-file ./infra/aspire.bicep \
        --parameters environmentName=$AZURE_ENV_NAME \
        --parameters location=$AZURE_LOCATION

    # PowerShell
    $AZURE_RESOURCE_GROUP = "rg-$AZURE_ENV_NAME"
    $AZURE_LOCATION = azd env get-value AZURE_LOCATION
    az deployment group create `
        -g $AZURE_RESOURCE_GROUP `
        --template-file ./infra/aspire.bicep `
        --parameters environmentName=$AZURE_ENV_NAME `
        --parameters location=$AZURE_LOCATION
    ```

## Unit Testing & Integration Testing & Linting OpenAPI Document

1. Run the following command to run both unit tests and integration tests at once on your local machine.

    ```bash
    # zsh/bash
    ./scripts/run-tests.sh -c Debug -k || ./scripts/run-tests.sh -c Debug

    # PowerShell
    ./scripts/Run-Tests.ps1 -Configuration Debug -KillPorts
    ```
