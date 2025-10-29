# OpenChat Playground with Google Vertex AI

This page describes how to run OpenChat Playground (OCP) with Google Vertex AI integration.

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

1. Add Google Vertex AI API Key. Replace `{{GOOGLE_VERTEX_AI_API_KEY}}` with your key.

    ```bash
    # bash/zsh
    dotnet user-secrets --project $REPOSITORY_ROOT/src/OpenChat.PlaygroundApp \
        set GoogleVertexAI:ApiKey "{{GOOGLE_VERTEX_AI_API_KEY}}"
    ```

    ```powershell
    # PowerShell
    dotnet user-secrets --project $REPOSITORY_ROOT/src/OpenChat.PlaygroundApp `
        set GoogleVertexAI:ApiKey "{{GOOGLE_VERTEX_AI_API_KEY}}"
    ```

    > To get an API Key, refer to the doc [Using Gemini API keys](https://ai.google.dev/gemini-api/docs/api-key#api-keys).

1. Run the app. The default model OCP uses is [Gemini 2.5 Flash Lite](https://ai.google.dev/gemini-api/docs/models#gemini-2.5-flash-lite).

    ```bash
    # bash/zsh
    dotnet run --project $REPOSITORY_ROOT/src/OpenChat.PlaygroundApp -- \
        --connector-type GoogleVertexAI
    ```

    ```powershell
    # PowerShell
    dotnet run --project $REPOSITORY_ROOT/src/OpenChat.PlaygroundApp -- `
        --connector-type GoogleVertexAI
    ```

    Alternatively, if you want to run with a different deployment, say [`gemini-2.5-pro`](https://ai.google.dev/gemini-api/docs/models#gemini-2.5-pro), other than the default one, you can specify it as an argument.

    ```bash
    # bash/zsh
    dotnet run --project $REPOSITORY_ROOT/src/OpenChat.PlaygroundApp -- \
        --connector-type GoogleVertexAI \
        --model gemini-2.5-pro
    ```

    ```powershell
    # PowerShell
    dotnet run --project $REPOSITORY_ROOT/src/OpenChat.PlaygroundApp -- `
        --connector-type GoogleVertexAI `
        --model gemini-2.5-pro
    ```

1. Open your web browser at `http://localhost:5280` and start entering prompts.

## Run in local container

1. Make sure you are at the repository root.

    ```bash
    cd $REPOSITORY_ROOT
    ```

1. Build a container.

    ```bash
    docker build -f Dockerfile -t openchat-playground:latest .
    ```

1. Get the Google Vertex AI key.

    ```bash
    API_KEY=$(dotnet user-secrets --project ./src/OpenChat.PlaygroundApp list --json | \
              sed -n '/^\/\//d; p' | jq -r '."GoogleVertexAI:ApiKey"')
    ```

    ```powershell
    # PowerShell
    $API_KEY = (dotnet user-secrets --project ./src/OpenChat.PlaygroundApp list --json | `
                Select-String -NotMatch '^//(BEGIN|END)' | ConvertFrom-Json).'GoogleVertexAI:ApiKey'
    ```

1. Run the app. The default model OCP uses is [Gemini 2.5 Flash Lite](https://ai.google.dev/gemini-api/docs/models#gemini-2.5-flash-lite).

    ```bash
    # bash/zsh - from locally built container
    docker run -i --rm -p 8080:8080 openchat-playground:latest \ 
        --connector-type GoogleVertexAI \
        --api-key $API_KEY
    ```

    ```powershell
    # PowerShell - from locally built container
    docker run -i --rm -p 8080:8080 openchat-playground:latest --connector-type GoogleVertexAI `
        --api-key $API_KEY
    ```

    ```bash
    # bash/zsh - from GitHub Container Registry
    docker run -i --rm -p 8080:8080 ghcr.io/aliencube/open-chat-playground/openchat-playground:latest \
        --connector-type GoogleVertexAI \
        --api-key $API_KEY
    ```

    ```powershell
    # PowerShell - from GitHub Container Registry
    docker run -i --rm -p 8080:8080 ghcr.io/aliencube/open-chat-playground/openchat-playground:latest `
        --connector-type GoogleVertexAI `
        --api-key $API_KEY `
    ```

    Alternatively, if you want to run with a different deployment, say [`gemini-2.5-pro`](https://ai.google.dev/gemini-api/docs/models#gemini-2.5-pro), other than the default one, you can specify it as an argument.

    ```bash
    # bash/zsh - from locally built container
    docker run -i --rm -p 8080:8080 openchat-playground:latest \ 
        --connector-type GoogleVertexAI \
        --api-key $API_KEY \
        --model gemini-2.5-pro
    ```

    ```powershell
    # PowerShell - from locally built container
    docker run -i --rm -p 8080:8080 openchat-playground:latest --connector-type GoogleVertexAI `
        --api-key $API_KEY `
        --model gemini-2.5-pro
    ```

    ```bash
    # bash/zsh - from GitHub Container Registry
    docker run -i --rm -p 8080:8080 ghcr.io/aliencube/open-chat-playground/openchat-playground:latest \
        --connector-type GoogleVertexAI \
        --api-key $API_KEY \
        --model gemini-2.5-pro
    ```

    ```powershell
    # PowerShell - from GitHub Container Registry
    docker run -i --rm -p 8080:8080 ghcr.io/aliencube/open-chat-playground/openchat-playground:latest `
        --connector-type GoogleVertexAI `
        --api-key $API_KEY `
        --model gemini-2.5-pro
    ```

1. Open your web browser, navigate to `http://localhost:8080`, and enter prompts.

## Run on Azure

1. Make sure you are at the repository root.

    ```bash
    cd $REPOSITORY_ROOT
    ```

1. Login to Azure:

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

1. Get Google Vertex AI API Key.

    ```bash
    API_KEY=$(dotnet user-secrets --project ./src/OpenChat.PlaygroundApp list --json | \
              sed -n '/^\/\//d; p' | jq -r '."GoogleVertexAI:ApiKey"')
    ```

    ```powershell
    # PowerShell
    $API_KEY = (dotnet user-secrets --project ./src/OpenChat.PlaygroundApp list --json | `
                Select-String -NotMatch '^//(BEGIN|END)' | ConvertFrom-Json).'GoogleVertexAI:ApiKey'
    ```

1. Set Google Vertex AI configuration to azd environment variables.

    ```bash
    azd env set GOOGLE_VERTEX_AI_API_KEY $API_KEY
    ```

    The default model OCP uses is [Gemini 2.5 Flash Lite](https://ai.google.dev/gemini-api/docs/models#gemini-2.5-flash-lite). If you want to run with a different deployment, say [`gemini-2.5-pro`](https://ai.google.dev/gemini-api/docs/models#gemini-2.5-pro), other than the default one, add it to azd environment variables.

    ```bash
    azd env set GOOGLE_VERTEX_AI_MODEL gemini-2.5-pro
    ```

1. Set the connector type to `GoogleVertexAI`

    ```bash
    azd env set CONNECTOR_TYPE GoogleVertexAI
    ```

1. Provision and deploy:

    ```bash
    azd up
    ```

    > **NOTE**: You will be asked to provide Azure subscription and location for deployment.

    Once deployed, you will be able to see the deployed OCP app URL.

1. Open your web browser, navigate to the OCP app URL, and enter prompts.

1. Clean up:

    ```bash
    azd down --force --purge
    ```
