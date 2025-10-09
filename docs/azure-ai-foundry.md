# OpenChat Playground with [Azure AI Foundry](https://learn.microsoft.com/azure/ai-foundry/what-is-azure-ai-foundry)

This page describes how to run OpenChat Playground (OCP) with Azure AI Foundry integration.

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

1. Run the app by passing the endpoint parameter `{{AZURE_AI_FOUNDRY_ENDPOINT}}` that was acquired from Azure AI Foundry portal. The default deployment name OCP uses is [`gpt-4o-mini`](https://ai.azure.com/catalog/models/gpt-4o-mini).

    ```bash
    # bash/zsh
    dotnet run --project $REPOSITORY_ROOT/src/OpenChat.PlaygroundApp -- \
        --connector-type AzureAIFoundry \
        --endpoint "{{AZURE_AI_FOUNDRY_ENDPOINT}}"
    ```

    ```powershell
    # PowerShell
    dotnet run --project $REPOSITORY_ROOT/src/OpenChat.PlaygroundApp -- `
        --connector-type AzureAIFoundry `
        --endpoint "{{AZURE_AI_FOUNDRY_ENDPOINT}}"
    ```

   Alternatively, if you want to run with a different deployment, say [`gpt-4`](https://ai.azure.com/catalog/models/gpt-4), other than the default one, you can specify it as an argument. For more details on how to create deployments, refer to [Create and deploy a model](https://learn.microsoft.com/en-us/azure/ai-foundry/foundry-models/how-to/quickstart-ai-project):

    ```bash
    # bash/zsh
    dotnet run --project $REPOSITORY_ROOT/src/OpenChat.PlaygroundApp -- \
        --connector-type AzureAIFoundry \
        --endpoint "{{AZURE_AI_FOUNDRY_ENDPOINT}}" \
        --deployment-name gpt-4
    ```

    ```powershell
    # PowerShell
    dotnet run --project $REPOSITORY_ROOT/src/OpenChat.PlaygroundApp -- `
        --connector-type AzureAIFoundry `
        --endpoint "{{AZURE_AI_FOUNDRY_ENDPOINT}}" `
        --deployment-name gpt-4
    ```

1. Open your web browser, navigate to `http://localhost:5280`, and enter prompts.

## Run in local container

1. Make sure that you have the Azure AI Foundry Endpoint URL.
1. Make sure you are at the repository root.

    ```bash
    cd $REPOSITORY_ROOT
    ```

1. Build a container.

    ```bash
    docker build -f Dockerfile -t openchat-playground:latest .
    ```

1. Get Azure AI Foundry API Key.

    ```bash
    # bash/zsh
    API_KEY=$(dotnet user-secrets --project ./src/OpenChat.PlaygroundApp list --json | \
              sed -n '/^\/\//d; p' | jq -r '."AzureAIFoundry:ApiKey"')
    ```

    ```powershell
    # PowerShell
    $API_KEY = (dotnet user-secrets --project ./src/OpenChat.PlaygroundApp list --json | `
                Select-String -NotMatch '^//(BEGIN|END)' | ConvertFrom-Json).'AzureAIFoundry:ApiKey'
    ```

1. Run the app. The default deployment name OCP uses is [`gpt-4o-mini`](https://ai.azure.com/catalog/models/gpt-4o-mini).

    ```bash
    # bash/zsh - from locally built container
    docker run -i --rm -p 8080:8080 openchat-playground:latest --connector-type AzureAIFoundry \
        --endpoint "{{AZURE_AI_FOUNDRY_ENDPOINT}}" \
        --api-key $API_KEY
    ```

    ```powershell
    # PowerShell - from locally built container
    docker run -i --rm -p 8080:8080 openchat-playground:latest --connector-type AzureAIFoundry `
        --endpoint "{{AZURE_AI_FOUNDRY_ENDPOINT}}" `
        --api-key $API_KEY
    ```

    ```bash
    # bash/zsh - from GitHub Container Registry
    docker run -i --rm -p 8080:8080 ghcr.io/aliencube/open-chat-playground/openchat-playground:latest \
        --connector-type AzureAIFoundry \
        --endpoint "{{AZURE_AI_FOUNDRY_ENDPOINT}}" \
        --api-key $API_KEY
    ```

    ```powershell
    # PowerShell - from GitHub Container Registry
    docker run -i --rm -p 8080:8080 ghcr.io/aliencube/open-chat-playground/openchat-playground:latest `
        --connector-type AzureAIFoundry `
        --endpoint "{{AZURE_AI_FOUNDRY_ENDPOINT}}" `
        --api-key $API_KEY `
    ```

   Alternatively, if you want to run with a different deployment, say [`gpt-4`](https://ai.azure.com/catalog/models/gpt-4), other than the default one, you can specify it as an argument:

    ```bash
    # bash/zsh - from locally built container
    docker run -i --rm -p 8080:8080 openchat-playground:latest --connector-type AzureAIFoundry \
        --endpoint "{{AZURE_AI_FOUNDRY_ENDPOINT}}" \
        --api-key $API_KEY \
        --deployment-name gpt-4
    ```

    ```powershell
    # PowerShell - from locally built container
    docker run -i --rm -p 8080:8080 openchat-playground:latest --connector-type AzureAIFoundry `
        --endpoint "{{AZURE_AI_FOUNDRY_ENDPOINT}}" `
        --api-key $API_KEY `
        --deployment-name gpt-4
    ```

    ```bash
    # bash/zsh - from GitHub Container Registry
    docker run -i --rm -p 8080:8080 ghcr.io/aliencube/open-chat-playground/openchat-playground:latest \
        --connector-type AzureAIFoundry \
        --endpoint "{{AZURE_AI_FOUNDRY_ENDPOINT}}" \
        --api-key $API_KEY \
        --deployment-name gpt-4
    ```

    ```powershell
    # PowerShell - from GitHub Container Registry
    docker run -i --rm -p 8080:8080 ghcr.io/aliencube/open-chat-playground/openchat-playground:latest `
        --connector-type AzureAIFoundry `
        --endpoint "{{AZURE_AI_FOUNDRY_ENDPOINT}}" `
        --api-key $API_KEY `
        --deployment-name gpt-4
    ```

1. Open your web browser, navigate to `http://localhost:8080`, and enter prompts.

## Run on Azure

1. Make sure that you have the Azure AI Foundry Endpoint URL.
1. Make sure you are at the repository root.

    ```bash
    cd $REPOSITORY_ROOT
    ```

1. Login to Azure.

    ```bash
    azd auth login
    ```

1. Check login status.

    ```bash
    azd auth login --check-status
    ```

1. Initialize `azd` template.

    ```bash
    azd init
    ```

   > **NOTE**: You will be asked to provide environment name for provisioning.

1. Get Azure AI Foundry API Key.

    ```bash
    # bash/zsh
    API_KEY=$(dotnet user-secrets --project ./src/OpenChat.PlaygroundApp list --json | \
                 sed -n '/^\/\//d; p' | jq -r '."AzureAIFoundry:ApiKey"')
    ```

    ```bash
    # PowerShell
    $API_KEY = (dotnet user-secrets --project ./src/OpenChat.PlaygroundApp list --json | `
                  Select-String -NotMatch '^//(BEGIN|END)' | ConvertFrom-Json).'AzureAIFoundry:ApiKey'
    ```

1. Set Azure AI Foundry configuration to azd environment variables.

    ```bash
    azd env set AZURE_AI_FOUNDRY_ENDPOINT "{{AZURE_AI_FOUNDRY_ENDPOINT}}"
    azd env set AZURE_AI_FOUNDRY_API_KEY $API_KEY
    ```

   The default deployment name OCP uses is [`gpt-4o-mini`](https://ai.azure.com/catalog/models/gpt-4o-mini). If you want to run with a different deployment, say [`gpt-4`](https://ai.azure.com/catalog/models/gpt-4), other than the default one, add it to azd environment variables.

    ```bash
    azd env set AZURE_AI_FOUNDRY_DEPLOYMENT_NAME gpt-4
    ```

1. Set the connector type to `AzureAIFoundry`.

    ```bash
    azd env set CONNECTOR_TYPE AzureAIFoundry
    ```

1. Run the following commands in order to provision and deploy the app.

    ```bash
    azd up
    ```

   > **NOTE**: You will be asked to provide Azure subscription and location for deployment.

   Once deployed, you will be able to see the deployed OCP app URL.

1. Open your web browser, navigate to the OCP app URL, and enter prompts.

1. Clean up all the resources.

    ```bash
    azd down --force --purge
    ```
