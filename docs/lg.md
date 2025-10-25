# OpenChat Playground with LG

This page describes how to run OpenChat Playground (OCP) with [LG models on Hugging Face](https://huggingface.co/LGAI-EXAONE) integration.

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

1. Make sure the Ollama server is up and running.

    ```bash
    ollama serve
    ```

1. Download the LG model from Hugging Face. The default model OCP uses is [`LGAI-EXAONE/EXAONE-4.0-1.2B-GGUF`](https://huggingface.co/LGAI-EXAONE/EXAONE-4.0-1.2B-GGUF).

    ```bash
    ollama pull hf.co/LGAI-EXAONE/EXAONE-4.0-1.2B-GGUF
    ```

   Alternatively, if you want to run with a different model, say [`LGAI-EXAONE/EXAONE-4.0-32B-GGUF`](https://huggingface.co/LGAI-EXAONE/EXAONE-4.0-32B-GGUF), other than the default one, download it first by running the following command.

    ```bash
    ollama pull hf.co/LGAI-EXAONE/EXAONE-4.0-32B-GGUF
    ```

   Make sure to follow the exact format like `hf.co/LGAI-EXAONE/EXAONE-{{MODEL}}-GGUF` and the model MUST include `GGUF`.

1. Make sure you are at the repository root.

    ```bash
    cd $REPOSITORY_ROOT
    ```

1. Run the app.

    ```bash
    # bash/zsh
    dotnet run --project $REPOSITORY_ROOT/src/OpenChat.PlaygroundApp -- \
        --connector-type LG
    ```

    ```powershell
    # PowerShell
    dotnet run --project $REPOSITORY_ROOT\src\OpenChat.PlaygroundApp -- `
        --connector-type LG
    ```

   Alternatively, if you want to run with a different model, say [`LGAI-EXAONE/EXAONE-4.0-32B-GGUF`](https://huggingface.co/LGAI-EXAONE/EXAONE-4.0-32B-GGUF), make sure you've already downloaded the model by running the `ollama pull hf.co/LGAI-EXAONE/EXAONE-4.0-32B-GGUF` command.

    ```bash
    # bash/zsh
    dotnet run --project $REPOSITORY_ROOT/src/OpenChat.PlaygroundApp -- \
        --connector-type LG \
        --model hf.co/LGAI-EXAONE/EXAONE-4.0-32B-GGUF
    ```

    ```powershell
    # PowerShell
    dotnet run --project $REPOSITORY_ROOT\src\OpenChat.PlaygroundApp -- `
        --connector-type LG `
        --model hf.co/LGAI-EXAONE/EXAONE-4.0-32B-GGUF
    ```

1. Open your web browser, navigate to `http://localhost:5280`, and enter prompts.

## Run in local container

1. Make sure the Ollama server is up and running.

    ```bash
    ollama serve
    ```

1. Download the LG model from Hugging Face. The default model OCP uses is [`LGAI-EXAONE/EXAONE-4.0-1.2B-GGUF`](https://huggingface.co/LGAI-EXAONE/EXAONE-4.0-1.2B-GGUF).

    ```bash
    ollama pull hf.co/LGAI-EXAONE/EXAONE-4.0-1.2B-GGUF
    ```

   Alternatively, if you want to run with a different model, say [`LGAI-EXAONE/EXAONE-4.0-32B-GGUF`](https://huggingface.co/LGAI-EXAONE/EXAONE-4.0-32B-GGUF), other than the default one, download it first by running the following command.

    ```bash
    ollama pull hf.co/LGAI-EXAONE/EXAONE-4.0-32B-GGUF
    ```

   Make sure to follow the exact format like `hf.co/LGAI-EXAONE/EXAONE-{{MODEL}}-GGUF` and the model MUST include `GGUF`.

1. Make sure you are at the repository root.

    ```bash
    cd $REPOSITORY_ROOT
    ```

1. Build a container.

    ```bash
    docker build -f Dockerfile -t openchat-playground:latest .
    ```

1. Run the app. The default model OCP uses is [`LGAI-EXAONE/EXAONE-4.0-1.2B-GGUF`](https://huggingface.co/LGAI-EXAONE/EXAONE-4.0-1.2B-GGUF).

    ```bash
    # bash/zsh - from locally built container
    docker run -i --rm -p 8080:8080 openchat-playground:latest \
        --connector-type LG \
        --base-url http://host.docker.internal:11434
    ```

    ```powershell
    # PowerShell - from locally built container
    docker run -i --rm -p 8080:8080 openchat-playground:latest `
        --connector-type LG `
        --base-url http://host.docker.internal:11434
    ```

    ```bash
    # bash/zsh - from GitHub Container Registry
    docker run -i --rm -p 8080:8080 ghcr.io/aliencube/open-chat-playground/openchat-playground:latest \
        --connector-type LG \
        --base-url http://host.docker.internal:11434
    ```

    ```powershell
    # PowerShell - from GitHub Container Registry
    docker run -i --rm -p 8080:8080 ghcr.io/aliencube/open-chat-playground/openchat-playground:latest `
        --connector-type LG `
        --base-url http://host.docker.internal:11434
    ```

   Alternatively, if you want to run with a different model, say [`LGAI-EXAONE/EXAONE-4.0-32B-GGUF`](https://huggingface.co/LGAI-EXAONE/EXAONE-4.0-32B-GGUF), make sure you've already downloaded the model by running the `ollama pull hf.co/LGAI-EXAONE/EXAONE-4.0-32B-GGUF` command.

    ```bash
    # bash/zsh - from locally built container
    docker run -i --rm -p 8080:8080 openchat-playground:latest \
        --connector-type LG \
        --base-url http://host.docker.internal:11434 \
        --model hf.co/LGAI-EXAONE/EXAONE-4.0-32B-GGUF
    ```

    ```powershell
    # PowerShell - from locally built container
    docker run -i --rm -p 8080:8080 openchat-playground:latest `
        --connector-type LG `
        --base-url http://host.docker.internal:11434 `
        --model hf.co/LGAI-EXAONE/EXAONE-4.0-32B-GGUF
    ```

    ```bash
    # bash/zsh - from GitHub Container Registry
    docker run -i --rm -p 8080:8080 ghcr.io/aliencube/open-chat-playground/openchat-playground:latest \
        --connector-type LG \
        --base-url http://host.docker.internal:11434 \
        --model hf.co/LGAI-EXAONE/EXAONE-4.0-32B-GGUF
    ```

    ```powershell
    # PowerShell - from GitHub Container Registry
    docker run -i --rm -p 8080:8080 ghcr.io/aliencube/open-chat-playground/openchat-playground:latest `
        --connector-type LG `
        --base-url http://host.docker.internal:11434 `
        --model hf.co/LGAI-EXAONE/EXAONE-4.0-32B-GGUF
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

1. Set the connector type to `LG`.

    ```bash
    azd env set CONNECTOR_TYPE "LG"
    ```

   The default model OCP uses is [`LGAI-EXAONE/EXAONE-4.0-1.2B-GGUF`](https://huggingface.co/LGAI-EXAONE/EXAONE-4.0-1.2B-GGUF). If you want to run with a different model, say [`LGAI-EXAONE/EXAONE-4.0-32B-GGUF`](https://huggingface.co/LGAI-EXAONE/EXAONE-4.0-32B-GGUF), other than the default one, add it to azd environment variables.

    ```bash
    azd env set LG_MODEL "LGAI-EXAONE/EXAONE-4.0-32B-GGUF"
    ```

   Make sure to follow the exact format like `LGAI-EXAONE/EXAONE-{{MODEL}}-GGUF` and the model MUST include `GGUF`.

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
