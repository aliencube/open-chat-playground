# OpenChat Playground with [Azure AI Foundry](https://learn.microsoft.com/azure/ai-foundry/what-is-azure-ai-foundry)

This page describes to run OpenChat Playground (OCP) with Azure AI Foundry integration.

## Get the repository root

1. Get the repository root.

    ```bash
    # bash/zsh
    REPOSITORY_ROOT=$(git rev-parse --show-toplevel)
    ```

    ```powershell
    # PowerShell
    $REPOSITORY_ROOT = git rev-parse --show-toplevel
    ```

## Run on local machine

1. Make sure you are at the repository root.

    ```bash
    cd $REPOSITORY_ROOT
    ```

1. Add Azure AI Foundry API Key for Azure AI Foundry connection. Make sure you should replace `{{AZURE_AI_FOUNDRY_API_KEY}}` with your Azure AI Foundry API key.

    ```bash
    # bash/zsh
    dotnet user-secrets --project $REPOSITORY_ROOT/src/OpenChat.PlaygroundApp \
        set AzureAIFoundry:ApiKey "{{AZURE_AI_FOUNDRY_API_KEY}}"
    ```

    ```powershell
    # PowerShell
    dotnet user-secrets --project $REPOSITORY_ROOT/src/OpenChat.PlaygroundApp `
        set AzureAIFoundry:ApiKey "{{AZURE_AI_FOUNDRY_API_KEY}}"
    ```

    > To get an Azure AI Foundry instance, its API endpoint and key, refer to the doc, [Get started with Azure AI Foundry](https://learn.microsoft.com/en-us/azure/ai-foundry/quickstarts/get-started-code?tabs=csharp#set-up-your-environment).

1. Run the app.

    ```bash
    dotnet run --project $REPOSITORY_ROOT/src/OpenChat.PlaygroundApp -- --connector-type AzureAIFoundry
    ```

1. Open your web browser, navigate to `http://localhost:5280`, and enter prompts.

## Run in local container

1. Make sure you are at the repository root.

    ```bash
    cd $REPOSITORY_ROOT
    ```

1. Build a container.

    ```bash
    docker build -f Dockerfile -t openchat-playground:latest .
    ```

1. Get Azure AI Foundry configuration.

    ```bash
    # bash/zsh
    ENDPOINT=$(dotnet user-secrets --project ./src/OpenChat.PlaygroundApp list --json | \
                 sed -n '/^\/\//d; p' | jq -r '."AzureAIFoundry:Endpoint"')
    API_KEY=$(dotnet user-secrets --project ./src/OpenChat.PlaygroundApp list --json | \
                sed -n '/^\/\//d; p' | jq -r '."AzureAIFoundry:ApiKey"')
    DEPLOYMENT_NAME=$(dotnet user-secrets --project ./src/OpenChat.PlaygroundApp list --json | \
                        sed -n '/^\/\//d; p' | jq -r '."AzureAIFoundry:DeploymentName"')
    ```

    ```powershell
    # PowerShell
    $SECRETS = (dotnet user-secrets --project ./src/OpenChat.PlaygroundApp list --json | `
                  Select-String -NotMatch '^//(BEGIN|END)' | ConvertFrom-Json)
    $ENDPOINT = $SECRETS.'AzureAIFoundry:Endpoint'
    $API_KEY = $SECRETS.'AzureAIFoundry:ApiKey'
    $DEPLOYMENT_NAME = $SECRETS.'AzureAIFoundry:DeploymentName'
    ```

1. Run the app.

    ```bash
    # From locally built container
    docker run -i --rm -p 8080:8080 openchat-playground:latest \
        --connector-type AzureAIFoundry \
        --endpoint "$ENDPOINT" \
        --api-key "$API_KEY" \
        --deployment-name "$DEPLOYMENT_NAME"
    ```

    ```bash
    # From GitHub Container Registry
    docker run -i --rm -p 8080:8080 ghcr.io/aliencube/open-chat-playground/openchat-playground:latest \
        --connector-type AzureAIFoundry \
        --endpoint "$ENDPOINT" \
        --api-key "$API_KEY" \
        --deployment-name "$DEPLOYMENT_NAME"
    ```

1. Open your web browser, navigate to `http://localhost:8080`, and enter prompts.

## Run on Azure

1. Make sure you are at the repository root.

    ```bash
    cd $REPOSITORY_ROOT
    ```

1. Login to Azure.

    ```bash
    # Login to Azure Dev CLI
    azd auth login
    ```

1. Check login status.

    ```bash
    # Azure Dev CLI
    azd auth login --check-status
    ```

1. Initialize `azd` template.

    ```bash
    azd init
    ```

   > **NOTE**: You will be asked to provide environment name for provisioning.

1. Get Azure AI Foundry configuration.

    ```bash
    # bash/zsh
    ENDPOINT=$(dotnet user-secrets --project ./src/OpenChat.PlaygroundApp list --json | \
                 sed -n '/^\/\//d; p' | jq -r '."AzureAIFoundry:Endpoint"')
    API_KEY=$(dotnet user-secrets --project ./src/OpenChat.PlaygroundApp list --json | \
                sed -n '/^\/\//d; p' | jq -r '."AzureAIFoundry:ApiKey"')
    DEPLOYMENT_NAME=$(dotnet user-secrets --project ./src/OpenChat.PlaygroundApp list --json | \
                        sed -n '/^\/\//d; p' | jq -r '."AzureAIFoundry:DeploymentName"')
    ```

    ```powershell
    # PowerShell
    $SECRETS = (dotnet user-secrets --project ./src/OpenChat.PlaygroundApp list --json | `
                  Select-String -NotMatch '^//(BEGIN|END)' | ConvertFrom-Json)
    $ENDPOINT = $SECRETS.'AzureAIFoundry:Endpoint'
    $API_KEY = $SECRETS.'AzureAIFoundry:ApiKey'
    $DEPLOYMENT_NAME = $SECRETS.'AzureAIFoundry:DeploymentName'
    ```

1. Set Azure AI Foundry configuration to azd environment variables.

    ```bash
    azd env set AZURE_AI_FOUNDRY_ENDPOINT "$ENDPOINT"
    azd env set AZURE_AI_FOUNDRY_API_KEY "$API_KEY"
    azd env set AZURE_AI_FOUNDRY_DEPLOYMENT_NAME "$DEPLOYMENT_NAME"
    ```

   Optionally, if you want to run with a different model deployment, add it to azd environment variables.

    ```bash
    azd env set AZURE_AI_FOUNDRY_DEPLOYMENT_NAME "gpt-4"
    ```

1. Set the connector type to `AzureAIFoundry`.

    ```bash
    azd env set CONNECTOR_TYPE "AzureAIFoundry"
    ```

1. Run the following commands in order to provision and deploy the app.

    ```bash
    azd up
    ```

   > **NOTE**: You will be asked to provide Azure subscription and location for deployment.

1. Clean up all the resources.

    ```bash
    azd down --force --purge
    ```
