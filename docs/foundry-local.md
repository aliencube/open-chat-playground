# OpenChat Playground with Foundry Local

This page describes how to run OpenChat Playground (OCP) with Foundry Local models integration.

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

1. Make sure the Foundry Local server is up and running.

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

1. Set the Foundry Local service port. The default port OCP uses is `55438`.

    ```bash
    foundry service set --port 55438
    ```

   Alternatively, if you want to run with a different port, say `63997`, other than the default one, set it first by running the following command.

    ```bash
    foundry service set --port 63997
    ```

1. Make sure the Foundry Local server is up and running.

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

    ```bash
    # bash/zsh - from locally built container
    docker run -i --rm -p 8080:8080 openchat-playground:latest \
        --connector-type FoundryLocal \
        --alias {{Model ID}} \
        --endpoint http://host.docker.internal:55438/v1 \
        --disable-foundrylocal-manager
    ```

    ```powershell
    # PowerShell - from locally built container
    docker run -i --rm -p 8080:8080 openchat-playground:latest `
        --connector-type FoundryLocal `
        --alias {{Model ID}} `
        --endpoint http://host.docker.internal:55438/v1 `
        --disable-foundrylocal-manager 
    ```

    ```bash
    # bash/zsh - from GitHub Container Registry
    docker run -i --rm -p 8080:8080 ghcr.io/aliencube/open-chat-playground/openchat-playground:latest \
        --connector-type FoundryLocal \
        --alias {{Model ID}} \
        --endpoint http://host.docker.internal:55438/v1 \
        --disable-foundrylocal-manager
    ```

    ```powershell
    # PowerShell - from GitHub Container Registry
    docker run -i --rm -p 8080:8080 ghcr.io/aliencube/open-chat-playground/openchat-playground:latest `
        --connector-type FoundryLocal `
        --alias {{Model ID}} `
        --endpoint http://host.docker.internal:55438/v1 `
        --disable-foundrylocal-manager 
    ```

1. Open your web browser, navigate to `http://localhost:8080`, and enter prompts.
