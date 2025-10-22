# OpenChat Playground with Docker Model Runner

This page describes how to run OpenChat Playground (OCP) with [Docker Model Runner](https://docs.docker.com/ai/model-runner/) integration.

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

1. Download the model. The default model OCP uses is [ai/smollm2](https://hub.docker.com/r/ai/smollm2).

    ```bash
    docker model pull ai/smollm2
    ```

1. Make sure you are at the repository root.

    ```bash
    cd $REPOSITORY_ROOT
    ```

1. Run the app.

    ```bash
    # bash/zsh
    dotnet run --project $REPOSITORY_ROOT/src/OpenChat.
    PlaygroundApp -- \
        --connector-type DockerModelRunner
    ```

    ```powershell
    # PowerShell
    dotnet run --project $REPOSITORY_ROOT\src\OpenChat.
    PlaygroundApp -- `
       --connector-type DockerModelRunner
    ```

1. Open your web browser, navigate to `http://localhost:5280`, and enter prompts.
