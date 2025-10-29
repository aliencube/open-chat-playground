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

    > Note: code and tests in this repository use the config keys `GoogleVertexAI:ApiKey` and `GoogleVertexAI:Model`.

1. Run the app with the `GoogleVertexAI` connector. Optionally pass `--model` to override the configured model/deployment name:

    ```bash
    # bash/zsh
    dotnet run --project $REPOSITORY_ROOT/src/OpenChat.PlaygroundApp -- \
        --connector-type GoogleVertexAI \
        --api-key $GOOGLE_VERTEX_AI_API_KEY \
        --model your-vertex-model-name
    ```

    ```powershell
    # PowerShell
    dotnet run --project $REPOSITORY_ROOT/src/OpenChat.PlaygroundApp -- `
        --connector-type GoogleVertexAI `
        --api-key $env:GOOGLE_VERTEX_AI_API_KEY `
        --model your-vertex-model-name
    ```

1. Open your web browser at `http://localhost:5280` and start entering prompts.

## Run in local container

1. Make sure you are at the repository root.

    ```bash
    cd $REPOSITORY_ROOT
    ```

1. Build a container image.

    ```bash
    docker build -f Dockerfile -t openchat-playground:latest .
    ```

1. Get the API key from user secrets (example):

    ```bash
    API_KEY=$(dotnet user-secrets --project ./src/OpenChat.PlaygroundApp list --json | \
              sed -n '/^\/\//d; p' | jq -r '."GoogleVertexAI:ApiKey"')
    ```

1. Run the app with the built image, passing the connector flags:

    ```bash
    # bash/zsh - from locally built container
    docker run -i --rm -p 8080:8080 openchat-playground:latest --connector-type GoogleVertexAI \
        --api-key $API_KEY \
        --model your-vertex-model-name
    ```

    ```powershell
    # PowerShell - from locally built container
    docker run -i --rm -p 8080:8080 openchat-playground:latest --connector-type GoogleVertexAI `
        --api-key $API_KEY `
        --model your-vertex-model-name
    ```

You can also run a published image (for example from a registry) the same way, passing the flags above.

## Run on Azure

1. Make sure you are at the repository root.

    ```bash
    cd $REPOSITORY_ROOT
    ```

1. Login to Azure:

    ```bash
    azd auth login
    ```

1. Initialize `azd` template if needed:

    ```bash
    azd init
    ```

1. Configure azd environment variables. Example: read the API key from user-secrets and set it in `azd`:

    ```bash
    API_KEY=$(dotnet user-secrets --project ./src/OpenChat.PlaygroundApp list --json | \
              sed -n '/^\/\//d; p' | jq -r '."GoogleVertexAI:ApiKey"')

    azd env set GOOGLE_VERTEX_AI_API_KEY $API_KEY
    # optionally set model
    azd env set GOOGLE_VERTEX_AI_MODEL your-vertex-model-name

    # set connector type
    azd env set CONNECTOR_TYPE GoogleVertexAI
    ```

1. Provision and deploy:

    ```bash
    azd up
    ```

1. Clean up:

    ```bash
    azd down --force --purge
    ```
