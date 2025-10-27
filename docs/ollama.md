# OpenChat Playground with Ollama

This page describes how to run OpenChat Playground (OCP) with [Ollama](https://ollama.com/search) integration.

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

1. Make sure Ollama is installed and running on your local machine. If not, install Ollama from [ollama.com](https://ollama.com/search) and start the service.

    ```bash
    ollama serve
    ```

1. Pull the model you want to use. The default model OCP uses is [llama3.2](https://ollama.com/library/llama3.2).

    ```bash
    ollama pull llama3.2
    ```

   Alternatively, if you want to run with a different model, say [qwen3](https://ollama.com/library/qwen3) other than the default one, download it first by running the following command.

    ```bash
    ollama pull qwen3
    ```

1. Make sure you are at the repository root.

    ```bash
    cd $REPOSITORY_ROOT
    ```

1. Run the app.

    ```bash
    # bash/zsh
    dotnet run --project $REPOSITORY_ROOT/src/OpenChat.PlaygroundApp -- \
        --connector-type Ollama
    ```

    ```powershell
    # PowerShell
    dotnet run --project $REPOSITORY_ROOT\src\OpenChat.PlaygroundApp -- `
        --connector-type Ollama
    ```

   Alternatively, if you want to run with a different model, say [qwen3](https://ollama.com/library/qwen3), make sure you've already downloaded the model by running the `ollama pull qwen3` command.

    ```bash
    # bash/zsh
    dotnet run --project $REPOSITORY_ROOT/src/OpenChat.PlaygroundApp -- \
        --connector-type Ollama \
        --model qwen3
    ```

    ```powershell
    # PowerShell
    dotnet run --project $REPOSITORY_ROOT\src\OpenChat.PlaygroundApp -- `
        --connector-type Ollama `
        --model qwen3
    ```

1. Open your web browser, navigate to `http://localhost:5280`, and enter prompts.

## Run on local container

1. Make sure the Ollama server is up and running.

    ```bash
    ollama serve
    ```

1. Download the Ollama model. The default model OCP uses is [llama3.2](https://ollama.com/library/llama3.2).

    ```bash
    ollama pull llama3.2
    ```

   Alternatively, if you want to run with a different model, say [qwen3](https://ollama.com/library/qwen3), other than the default one, download it first by running the following command.

    ```bash
    ollama pull qwen3
    ```

1. Make sure you are at the repository root.

    ```bash
    cd $REPOSITORY_ROOT
    ```

1. Build a container.

    ```bash
    docker build -f Dockerfile -t openchat-playground:latest .
    ```

1. Run the app.

    ```bash
    # bash/zsh - from locally built container
    docker run -i --rm -p 8080:8080 openchat-playground:latest \
      --connector-type Ollama \
      --base-url http://host.docker.internal:11434 \
    ```

    ```powershell
    # PowerShell - from locally built container
    docker run -i --rm -p 8080:8080 openchat-playground:latest `
        --connector-type Ollama `
        --base-url http://host.docker.internal:11434
    ```

    ```bash
    # bash/zsh - from GitHub Container Registry
    docker run -i --rm -p 8080:8080 ghcr.io/aliencube/open-chat-playground/openchat-playground:latest \
        --connector-type Ollama \
        --base-url http://host.docker.internal:11434
    ```

    ```powershell
    # PowerShell - from GitHub Container Registry
    docker run -i --rm -p 8080:8080 ghcr.io/aliencube/open-chat-playground/openchat-playground:latest `
        --connector-type Ollama `
        --base-url http://host.docker.internal:11434
    ```

   Alternatively, if you want to run with a different model, say [qwen3](https://ollama.com/library/qwen), make sure you've already downloaded the model by running the `ollama pull qwen3` command.

    ```bash
    ollama pull qwen3
    ```

    ```bash
    # bash/zsh - from locally built container
    docker run -i --rm -p 8080:8080 openchat-playground:latest \
        --connector-type Ollama \
        --base-url http://host.docker.internal:11434 \
        --model qwen3
    ```

    ```powershell
    # PowerShell - from locally built container
    docker run -i --rm -p 8080:8080 openchat-playground:latest `
        --connector-type Ollama `
        --base-url http://host.docker.internal:11434 `
        --model qwen3
    ```

    ```bash
    # bash/zsh - from GitHub Container Registry
    docker run -i --rm -p 8080:8080 ghcr.io/aliencube/open-chat-playground/openchat-playground:latest \
        --connector-type Ollama \
        --base-url http://host.docker.internal:11434 \
        --model qwen3
    ```

    ```powershell
    # PowerShell - from GitHub Container Registry
    docker run -i --rm -p 8080:8080 ghcr.io/aliencube/open-chat-playground/openchat-playground:latest `
        --connector-type Ollama `
        --base-url http://host.docker.internal:11434 `
        --model qwen3
    ```

   > **NOTE**: Use `host.docker.internal:11434` to connect to Ollama running on the host machine from inside the container.

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

1. Set the connector type to `Ollama`.

    ```bash
    azd env set CONNECTOR_TYPE "Ollama"
    ```

   The default model OCP uses is [llama3.2](https://ollama.com/library/llama3.2). If you want to run with a different model, say [qwen3](https://ollama.com/library/qwen3) other than the default one, add it to azd environment variables.

    ```bash
    azd env set OLLAMA_MODEL "qwen3"
    ```

1. As a default, the app uses a Serverless GPU with NVIDIA T4 (`NC8as-T4`). If you want to use NVIDIA A100, set the GPU profile.

    ```bash
    azd env set GPU_PROFILE_NAME "NC24-A100"
    ```

   If you want to know more about Serverless GPU, visit [Using serverless GPUs in Azure Container Apps](https://learn.microsoft.com/azure/container-apps/gpu-serverless-overview#use-serverless-gpus).

1. Run the following commands in order to provision and deploy the app.

    ```bash
    azd up
    ```

   > **NOTE**: You will be asked to provide Azure subscription and location for deployment.
   > **IMPORTANT**: Due to the limitation for GPU support, the available regions are limited to `Australia East`, `Sweden Central` and `West US 3`. For more details, visit [Using serverless GPUs in Azure Container Apps](https://learn.microsoft.com/azure/container-apps/gpu-serverless-overview#supported-regions).

   Once deployed, you will be able to see the deployed OCP app URL.

1. Open your web browser, navigate to the OCP app URL, and enter prompts.

1. Clean up all the resources.

    ```bash
    azd down --force --purge
    ```