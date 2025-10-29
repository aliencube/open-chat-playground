# OpenChat Playground with Foundry Local

This page describes how to run OpenChat Playground (OCP) with [Foundry Local](https://learn.microsoft.com/azure/ai-foundry/foundry-local/what-is-foundry-local) integration.

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

1. Make sure the Foundry Local server is up and running with the following command.

    ```bash
    foundry service start
    ```

1. Download the Foundry Local model. The default model OCP uses is `phi-4-mini`.

    ```bash
    foundry model download phi-4-mini
    ```

   Alternatively, if you want to run with a different model, say `qwen2.5-7b`, other than the default one, download it first by running the following command.

    ```bash
    foundry model download qwen2.5-7b
    ```

   Make sure to follow the model MUST be selected from the CLI output of `foundry model list`.

1. Make sure you are at the repository root.

    ```bash
    cd $REPOSITORY_ROOT
    ```

1. Run the app.

    ```bash
    # bash/zsh
    dotnet run --project $REPOSITORY_ROOT/src/OpenChat.PlaygroundApp -- \
        --connector-type FoundryLocal
    ```

    ```powershell
    # PowerShell
    dotnet run --project $REPOSITORY_ROOT\src\OpenChat.PlaygroundApp -- `
        --connector-type FoundryLocal
    ```

   Alternatively, if you want to run with a different model, say `qwen2.5-7b`, make sure you've already downloaded the model by running the `foundry model download qwen2.5-7b` command.

    ```bash
    # bash/zsh
    dotnet run --project $REPOSITORY_ROOT/src/OpenChat.PlaygroundApp -- \
        --connector-type FoundryLocal \
        --alias qwen2.5-7b
    ```

    ```powershell
    # PowerShell
    dotnet run --project $REPOSITORY_ROOT\src\OpenChat.PlaygroundApp -- `
        --connector-type FoundryLocal `
        --alias qwen2.5-7b
    ```

1. Open your web browser, navigate to `http://localhost:5280`, and enter prompts.

## Run in local container

1. Make sure the Foundry Local server is up and running.

    ```bash
    foundry service start
    ```

1. Get the Foundry Local service port.

    ```bash
    # bash/zsh
    FL_PORT_NUMBER=$(foundry service set --show true | sed -n '/^{/,/^}/p' | jq -r ".serviceSettings.port")
    ```

    ```powershell
    # PowerShell
    $FL_PORT_NUMBER = (foundry service set --show true | `
        ForEach-Object { `
            if ($_ -match '^{') { $capture = $true } `
            if ($capture) { $_ } `
            if ($_ -match '^}') { $capture = $false } `
        } | Out-String | ConvertFrom-Json).serviceSettings.port
    ```

1. Download the Foundry Local model. The default model OCP uses is `phi-4-mini`.

    ```bash
    foundry model download phi-4-mini
    ```

   Alternatively, if you want to run with a different model, say `qwen2.5-7b`, other than the default one, download it first by running the following command.

    ```bash
    foundry model download qwen2.5-7b
    ```

   Make sure to follow the model MUST be selected from the CLI output of `foundry model list`.

1. Load the Foundry Local model. The default model OCP uses is `phi-4-mini`.

    ```bash
    foundry model load phi-4-mini
    ```

   Alternatively, if you want to run with a different model, say `qwen2.5-7b`, other than the default one, download it first by running the following command.

    ```bash
    foundry model load qwen2.5-7b
    ```

1. Make sure you are at the repository root.

    ```bash
    cd $REPOSITORY_ROOT
    ```

1. Build a container.

    ```bash
    docker build -f Dockerfile -t openchat-playground:latest .
    ```

1. Run the app. The `{{Model ID}}` refers to the `Model ID` shown in the output of the `foundry service list` command.

   > **NOTE**: Make sure it MUST be the model ID, instead of alias.

    ```bash
    # bash/zsh - from locally built container
    docker run -i --rm -p 8080:8080 openchat-playground:latest \
        --connector-type FoundryLocal \
        --base-url http://host.docker.internal:$FL_PORT_NUMBER/ \
        --model "Phi-4-mini-instruct-generic-gpu:4" \
        --disable-foundrylocal-manager
    ```

    ```powershell
    # PowerShell - from locally built container
    docker run -i --rm -p 8080:8080 openchat-playground:latest `
        --connector-type FoundryLocal `
        --base-url http://host.docker.internal:$FL_PORT_NUMBER/ `
        --model {{Model ID}} `
        --disable-foundrylocal-manager 
    ```

    ```bash
    # bash/zsh - from GitHub Container Registry
    docker run -i --rm -p 8080:8080 ghcr.io/aliencube/open-chat-playground/openchat-playground:latest \
        --connector-type FoundryLocal \
        --base-url http://host.docker.internal:$FL_PORT_NUMBER/ \
        --model {{Model ID}} \
        --disable-foundrylocal-manager
    ```

    ```powershell
    # PowerShell - from GitHub Container Registry
    docker run -i --rm -p 8080:8080 ghcr.io/aliencube/open-chat-playground/openchat-playground:latest `
        --connector-type FoundryLocal `
        --base-url http://host.docker.internal:$FL_PORT_NUMBER/ `
        --model {{Model ID}} `
        --disable-foundrylocal-manager 
    ```

1. Open your web browser, navigate to `http://localhost:8080`, and enter prompts.
