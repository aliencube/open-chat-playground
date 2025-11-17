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

1. Make sure the Docker Model Runner is up and running, and ready to accept requests with the following command.

    ```bash
    docker model status
    ```

   It should say `Docker Model Runner is running`.

    ```bash
    # bash/zsh
    curl http://localhost:12434
    ```

    ```powershell
    # Powershell
    Invoke-WebRequest http://localhost:12434
    ```

    It should say `The Service is running`

    > If it says `Connection refused`, turn on "Enable host-side TCP support" option in [Docker Desktop Settings](https://docs.docker.com/ai/model-runner/get-started/#docker-desktop).

1. Download the model. The default model OCP uses is [ai/smollm2](https://hub.docker.com/r/ai/smollm2).

    ```bash
    docker model pull ai/smollm2
    ```

   Alternatively, if you want to run with a different model, say [ai/qwen3](https://hub.docker.com/r/ai/qwen3), other than the default one, download it first by running the following command.

    ```bash
    docker model pull ai/qwen3
    ```

1. Make sure you are at the repository root.

    ```bash
    cd $REPOSITORY_ROOT
    ```

1. Run the app.

    ```bash
    # bash/zsh
    dotnet run --project $REPOSITORY_ROOT/src/OpenChat.PlaygroundApp -- \
        --connector-type DockerModelRunner
    ```

    ```powershell
    # PowerShell
    dotnet run --project $REPOSITORY_ROOT\src\OpenChat.PlaygroundApp -- `
       --connector-type DockerModelRunner
    ```

1. Open your web browser, navigate to `http://localhost:5280`, and enter prompts.

## Run in local container

1. Make sure the Docker Model Runner is up and running, and ready to accept requests with the following command.

    ```bash
    docker model status
    ```

   It should say `Docker Model Runner is running`.

    ```bash
    # bash/zsh
    curl http://localhost:12434
    ```

    ```powershell
    # Powershell
    Invoke-WebRequest http://localhost:12434
    ```

    It should say `The Service is running`

    > If it says `Connection refused`, turn on "Enable host-side TCP support" option in [Docker Desktop Settings](https://docs.docker.com/ai/model-runner/get-started/#docker-desktop).

1. Download the model. The default model OCP uses is [ai/smollm2](https://hub.docker.com/r/ai/smollm2).

    ```bash
    docker model pull ai/smollm2
    ```

   Alternatively, if you want to run with a different model, say [ai/qwen3](https://hub.docker.com/r/ai/qwen3), other than the default one, download it first by running the following command.

    ```bash
    docker model pull ai/qwen3
    ```

1. Make sure you are at the repository root.

    ```bash
    cd $REPOSITORY_ROOT
    ```

1. Build a container.

    ```bash
    docker build -f Dockerfile -t openchat-playground:latest .
    ```

1. Run the app. The default model OCP uses is [ai/smollm2](https://hub.docker.com/r/ai/smollm2).

    ```bash
    # bash/zsh - from locally built container
    docker run -i --rm -p 8080:8080 openchat-playground:latest \
        --connector-type DockerModelRunner \
        --base-url http://host.docker.internal:12434
    ```

    ```powershell
    # PowerShell - from locally built container
    docker run -i --rm -p 8080:8080 openchat-playground:latest `
        --connector-type DockerModelRunner `
        --base-url http://host.docker.internal:12434
    ```

    ```bash
    # bash/zsh - from GitHub Container Registry
    docker run -i --rm -p 8080:8080 ghcr.io/aliencube/open-chat-playground/openchat-playground:latest \
        --connector-type DockerModelRunner \
        --base-url http://host.docker.internal:12434
    ```

    ```powershell
    # PowerShell - from GitHub Container Registry
    docker run -i --rm -p 8080:8080 ghcr.io/aliencube/open-chat-playground/openchat-playground:latest `
        --connector-type DockerModelRunner `
        --base-url http://host.docker.internal:12434
    ```

   Alternatively, if you want to run with a different model, say [ai/qwen3](https://hub.docker.com/r/ai/qwen3), make sure you've already downloaded the model by running the `docker model pull ai/qwen3` command.

    ```bash
    # bash/zsh - from locally built container
    docker run -i --rm -p 8080:8080 openchat-playground:latest \
        --connector-type DockerModelRunner \
        --base-url http://host.docker.internal:12434 \
        --model ai/qwen3
    ```

    ```powershell
    # PowerShell - from locally built container
    docker run -i --rm -p 8080:8080 openchat-playground:latest `
        --connector-type DockerModelRunner `
        --base-url http://host.docker.internal:12434 `
        --model ai/qwen3
    ```

    ```bash
    # bash/zsh - from GitHub Container Registry
    docker run -i --rm -p 8080:8080 ghcr.io/aliencube/open-chat-playground/openchat-playground:latest \
        --connector-type DockerModelRunner \
        --base-url http://host.docker.internal:12434 \
        --model ai/qwen3
    ```

    ```powershell
    # PowerShell - from GitHub Container Registry
    docker run -i --rm -p 8080:8080 ghcr.io/aliencube/open-chat-playground/openchat-playground:latest `
        --connector-type DockerModelRunner `
        --base-url http://host.docker.internal:12434 `
        --model ai/qwen3
    ```

1. Open your web browser, navigate to `http://localhost:8080`, and enter prompts.
