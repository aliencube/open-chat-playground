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

1. Make sure you are at the repository root.

    ```bash
    cd $REPOSITORY_ROOT
    ```

1. Get Huggingface models with ollama

    ```bash
    ollama pull "hf.co/Qwen/Qwen3-0.6B-GGUF"
    ```

    **Optionally, if you want to run with a different model**


    ```bash
    # Model name must end with "gguf"
    # e.g. Qwen3-0.6B-GGUF, gemma-3-1b-it-qat-q4_0-gguf
    ollama pull "hf.co/{org}/{model}"
    ```

1. Run the app.

    ```bash
    # bash/zsh
    dotnet run --project $REPOSITORY_ROOT/src/OpenChat.PlaygroundApp \
        -- --connector-type HuggingFace
    ```

    ```powershell
    # PowerShell
    dotnet run --project "$REPOSITORY_ROOT\src\OpenChat.PlaygroundApp" ` 
        -- --connector-type HuggingFace
    ```

    **Optionally, if you want to run with a different model**

    ```bash
    # bash/zsh
    dotnet run --project $REPOSITORY_ROOT/src/OpenChat.PlaygroundApp \
        -- --connector-type HuggingFace \
           --model hf.co/{org}/{model}
    ```

    ```powershell
    # PowerShell
    dotnet run --project "$REPOSITORY_ROOT\src\OpenChat.PlaygroundApp" ` 
        -- --connector-type HuggingFace `
           --model hf.co/{org}/{model}
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

1. Get Huggingface models with ollama

    ```bash
    ollama pull "hf.co/Qwen/Qwen3-0.6B-GGUF"
    ```

    **Optionally, if you want to run with a different model**

    ```bash
    # Model name must end with "gguf"
    # e.g. Qwen3-0.6B-GGUF, gemma-3-1b-it-qat-q4_0-gguf
    ollama pull "hf.co/{org}/{model}"
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

    **Optionally, if you want to run with a different model**

    ```bash
    # bash/zsh - From locally built container with custom model
    docker run -i --rm -p 8080:8080 openchat-playground:latest \
        --connector-type HuggingFace \
        --base-url http://host.docker.internal:11434 \
        --model hf.co/{org}/{model}
    ```

    ```powershell
    # PowerShell - From locally built container with custom model
    docker run -i --rm -p 8080:8080 openchat-playground:latest `
        --connector-type HuggingFace `
        --base-url http://host.docker.internal:11434 `
        --model hf.co/{org}/{model}
    ```

1. Open your web browser, navigate to `http://localhost:8080`, and enter prompts.

