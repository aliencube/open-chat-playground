# OpenChat Playground with Hugging Face

This page describes how to run OpenChat Playground (OCP) with Hugging Face integration.

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

1. Download the Hugging Face model. The default model OCP uses is `Qwen/Qwen3-0.6B-GGUF`.

    ```bash
    ollama pull hf.co/Qwen/Qwen3-0.6B-GGUF
    ```

    Optionally, if you want to run with a different model, say [microsoft/phi-4-gguf](https://huggingface.co/microsoft/phi-4-gguf), other than the default one, download it first by running the following command.

    ```bash
    ollama pull hf.co/microsoft/phi-4-gguf
    ```

    Make sure to follow the exact format like `hf.co/{{org}}/{{model}}` and the model MUST include `GGUF`.

1. Make sure you are at the repository root.

    ```bash
    cd $REPOSITORY_ROOT
    ```

1. Run the app.

    ```bash
    # bash/zsh
    dotnet run --project $REPOSITORY_ROOT/src/OpenChat.PlaygroundApp -- \
        --connector-type HuggingFace
    ```

    ```powershell
    # PowerShell
    dotnet run --project $REPOSITORY_ROOT\src\OpenChat.PlaygroundApp -- `
        --connector-type HuggingFace
    ```

    Optionally, if you want to run with a different model, say [microsoft/phi-4-gguf](https://huggingface.co/microsoft/phi-4-gguf), other than the default one, download it first by running the following command.

    ```bash
    # bash/zsh
    dotnet run --project $REPOSITORY_ROOT/src/OpenChat.PlaygroundApp -- \
        --connector-type HuggingFace \
        --model hf.co/microsoft/phi-4-gguf
    ```

    ```powershell
    # PowerShell
    dotnet run --project $REPOSITORY_ROOT\src\OpenChat.PlaygroundApp -- `
        --connector-type HuggingFace `
        --model hf.co/microsoft/phi-4-gguf
    ```

1. Open your web browser, navigate to `http://localhost:5280`, and enter prompts.

## Run in local container

1. Make sure the Ollama server is up and running.

    ```bash
    ollama serve
    ```

1. Download the Hugging Face model. The default model OCP uses is `Qwen/Qwen3-0.6B-GGUF`.

    ```bash
    ollama pull hf.co/Qwen/Qwen3-0.6B-GGUF
    ```

    Optionally, if you want to run with a different model, say [microsoft/phi-4-gguf](https://huggingface.co/microsoft/phi-4-gguf), other than the default one, download it first by running the following command.

    ```bash
    ollama pull hf.co/microsoft/phi-4-gguf
    ```

    Make sure to follow the exact format like `hf.co/{{org}}/{{model}}` and the model MUST include `GGUF`.

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
    # bash/zsh - From locally built container
    docker run -i --rm -p 8080:8080 openchat-playground:latest \
        --connector-type HuggingFace \
        --base-url http://host.docker.internal:11434
    ```

    ```powershell
    # PowerShell - From locally built container
    docker run -i --rm -p 8080:8080 openchat-playground:latest `
        --connector-type HuggingFace `
        --base-url http://host.docker.internal:11434
    ```

    ```bash
    # bash/zsh - From GitHub Container Registry
    docker run -i --rm -p 8080:8080 ghcr.io/aliencube/open-chat-playground/openchat-playground:latest \
        --connector-type HuggingFace \
        --base-url http://host.docker.internal:11434
    ```

    ```powershell
    # PowerShell - From GitHub Container Registry
    docker run -i --rm -p 8080:8080 ghcr.io/aliencube/open-chat-playground/openchat-playground:latest `
        --connector-type HuggingFace `
        --base-url http://host.docker.internal:11434
    ```

    Optionally, if you want to run with a different model, say [microsoft/phi-4-gguf](https://huggingface.co/microsoft/phi-4-gguf), other than the default one, download it first by running the following command.

    ```bash
    # bash/zsh - From locally built container with custom model
    docker run -i --rm -p 8080:8080 openchat-playground:latest \
        --connector-type HuggingFace \
        --base-url http://host.docker.internal:11434 \
        --model hf.co/microsoft/phi-4-gguf
    ```

    ```powershell
    # PowerShell - From locally built container with custom model
    docker run -i --rm -p 8080:8080 openchat-playground:latest `
        --connector-type HuggingFace `
        --base-url http://host.docker.internal:11434 `
        --model hf.co/microsoft/phi-4-gguf
    ```

1. Open your web browser, navigate to `http://localhost:8080`, and enter prompts.

