# OpenChat Playground with Upstage

This page describes how to run OpenChat Playground (OCP) with [Upstage Solar](https://www.upstage.ai/solar-llm) models integration.

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

1. Add Upstage API Key for Upstage Solar connection. Make sure you should replace `{{UPSTAGE_API_KEY}}` with your Upstage API key.

    ```bash
    # bash/zsh
    dotnet user-secrets --project $REPOSITORY_ROOT/src/OpenChat.PlaygroundApp \
        set Upstage:ApiKey "{{UPSTAGE_API_KEY}}"
    ```

    ```powershell
    # PowerShell
    dotnet user-secrets --project $REPOSITORY_ROOT/src/OpenChat.PlaygroundApp `
        set Upstage:ApiKey "{{UPSTAGE_API_KEY}}"
    ```

    > For more details about Upstage API Keys, refer to the doc, [Upstage API key](https://console.upstage.ai/docs/getting-started#get-an-api-key).

1. Run the app. The default model OCP uses is [solar-mini]( https://console.upstage.ai/docs/models/solar-mini).

    ```bash
    # bash/zsh
    dotnet run --project $REPOSITORY_ROOT/src/OpenChat.PlaygroundApp -- \
        --connector-type Upstage
    ```

    ```powershell
    # PowerShell
    dotnet run --project $REPOSITORY_ROOT/src/OpenChat.PlaygroundApp -- `
        --connector-type Upstage
    ```

   Alternatively, if you want to run with a different model, say [solar-pro2](https://console.upstage.ai/docs/models/solar-pro-2), other than the default one, you can specify it as an argument:

    ```bash
    # bash/zsh
    dotnet run --project $REPOSITORY_ROOT/src/OpenChat.PlaygroundApp -- \
        --connector-type Upstage \
        --model solar-pro2
    ```

    ```powershell
    # PowerShell
    dotnet run --project $REPOSITORY_ROOT/src/OpenChat.PlaygroundApp -- `
        --connector-type Upstage `
        --model solar-pro2
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

1. Get Upstage API Key.

    ```bash
    # bash/zsh
    API_KEY=$(dotnet user-secrets --project ./src/OpenChat.PlaygroundApp list --json | \
              sed -n '/^\/\//d; p' | jq -r '."Upstage:ApiKey"')
    ```

    ```powershell
    # PowerShell
    $API_KEY = (dotnet user-secrets --project ./src/OpenChat.PlaygroundApp list --json | `
                Select-String -NotMatch '^//(BEGIN|END)' | ConvertFrom-Json).'Upstage:ApiKey'
    ```

1. Run the app. The default model OCP uses is [solar-mini](https://console.upstage.ai/docs/models/solar-mini).

    ```bash
    # bash/zsh - from locally built container
    docker run -i --rm -p 8080:8080 openchat-playground:latest --connector-type Upstage \
        --api-key $API_KEY
    ```

    ```powershell
    # PowerShell - from locally built container
    docker run -i --rm -p 8080:8080 openchat-playground:latest --connector-type Upstage `
        --api-key $API_KEY
    ```

    ```bash
    # bash/zsh - from GitHub Container Registry
    docker run -i --rm -p 8080:8080 ghcr.io/aliencube/open-chat-playground/openchat-playground:latest \
        --connector-type Upstage \
        --api-key $API_KEY
    ```

    ```powershell
    # PowerShell - from GitHub Container Registry
    docker run -i --rm -p 8080:8080 ghcr.io/aliencube/open-chat-playground/openchat-playground:latest `
        --connector-type Upstage `
        --api-key $API_KEY
    ```

   Alternatively, if you want to run with a different model, say [solar-pro2](https://console.upstage.ai/docs/models/solar-pro-2), other than the default one, you can specify it as an argument:

    ```bash
    # bash/zsh - from locally built container with custom model
    docker run -i --rm -p 8080:8080 openchat-playground:latest --connector-type Upstage \
        --api-key $API_KEY \
        --model solar-pro2
    ```

    ```powershell
    # PowerShell - from locally built container with custom model
    docker run -i --rm -p 8080:8080 openchat-playground:latest --connector-type Upstage `
        --api-key $API_KEY `
        --model solar-pro2
    ```

1. Open your web browser, navigate to `http://localhost:8080`, and enter prompts.

## Run on Azure

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

1. Get Upstage API Key.

    ```bash
    # bash/zsh
    API_KEY=$(dotnet user-secrets --project ./src/OpenChat.PlaygroundApp list --json | \
              sed -n '/^\/\//d; p' | jq -r '."Upstage:ApiKey"')
    ```

    ```powershell
    # PowerShell
    $API_KEY = (dotnet user-secrets --project ./src/OpenChat.PlaygroundApp list --json | `
                Select-String -NotMatch '^//(BEGIN|END)' | ConvertFrom-Json).'Upstage:ApiKey'
    ```

1. Set Upstage API Key to azd environment variables.

    ```bash
    azd env set UPSTAGE_API_KEY $API_KEY
    ```

   The default model OCP uses is [solar-mini](https://console.upstage.ai/docs/models/solar-mini). If you want to run with a different model, say [solar-pro2](https://console.upstage.ai/docs/models/solar-pro-2), other than the default one, add it to azd environment variables.

    ```bash
    azd env set UPSTAGE_MODEL "solar-pro2"
    ```

1. Set the connector type to `Upstage`.

    ```bash
    azd env set CONNECTOR_TYPE "Upstage"
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