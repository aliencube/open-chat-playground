# OpenChat Playground with Ollama

This page describes how to run OpenChat Playground (OCP) with Ollama integration.

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

1. Make sure Ollama is installed and running on your local machine. If not, install Ollama from [ollama.com](https://ollama.com/) and start the service.

    ```bash
    # Start Ollama service
    ollama serve
    ```

1. Pull the model you want to use. Replace `{{MODEL_NAME}}` with your desired model.

    ```bash
    # Example: Pull llama3.2 model
    ollama pull llama3.2
    
    # Or pull other models
    ollama pull mistral
    ollama pull phi3
    ollama pull qwen
    ```

1. Run the app.

    ```bash
    dotnet run --project $REPOSITORY_ROOT/src/OpenChat.PlaygroundApp -- --connector-type Ollama --model llama3.2
    ```

1. Open your web browser, navigate to `http://localhost:5280`, and enter prompts.

## Run on local container

This approach runs OpenChat Playground in a container while connecting to Ollama running on the host machine.

1. Make sure you are at the repository root.

    ```bash
    cd $REPOSITORY_ROOT
    ```

1. Configure Ollama to accept connections from containers.

    ```powershell
    # PowerShell (Windows)
    $env:OLLAMA_HOST = "0.0.0.0:11434"
    # Start Ollama service
    ollama serve
    ```

    ```bash
    # bash/zsh 
    export OLLAMA_HOST=0.0.0.0:11434
    ollama serve
    ```

1. Pull the model you want to use.

    ```bash
    # Pull llama3.2 model (recommended)
    ollama pull llama3.2
    
    # Verify Ollama is accessible
    curl http://localhost:11434/api/version
    ```

1. Build a container.

    ```bash
    docker build -f Dockerfile -t openchat-playground:latest .
    ```

1. Run the app.

    ```bash
    # bash/zsh - from locally built container
    docker run -i --rm -p 8080:8080 \
      openchat-playground:latest \
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
   > **NOTE**: If you want to use other ollama models besides the basic model, first download the model using the command

    ```bash
    ollama pull qwen
    ```

    ```bash
    # bash/zsh - from locally built container
    docker run -i --rm -p 8080:8080 \
      openchat-playground:latest \
      --connector-type Ollama \
      --base-url http://host.docker.internal:11434 \
      --model qwen
    ```

    ```powershell
    # PowerShell - from locally built container (with a different model)
    docker run -i --rm -p 8080:8080 openchat-playground:latest `
        --connector-type Ollama `
        --base-url http://host.docker.internal:11434 `
        --model qwen
    ```

   > **NOTE**: Use `host.docker.internal:11434` to connect to Ollama running on the host machine from inside the container. Make sure `OLLAMA_HOST=0.0.0.0:11434` is set on the host.

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

1. Set Ollama configuration to azd environment variables.
    
    **Azure-hosted Ollama**

    ```bash
    # Set connector type to Ollama
    azd env set CONNECTOR_TYPE "Ollama"
    
    # Use placeholder URL that will be replaced when Ollama is deployed in Azure
    azd env set OLLAMA_BASE_URL "https://{{OLLAMA_URL}}:11434"
    
    # Set a specific model
    azd env set OLLAMA_MODEL "llama3.2"
    ```
    
    > **NOTE**: Replace {{OLLAMA_URL}} with the URL of your Ollama instance deployed on Azure. Automated deployment and URL configuration are currently in preparation.

1. Run the following commands in order to provision and deploy the app.

    ```bash
    azd up
    ```

   > **NOTE**: You will be asked to provide Azure subscription and location for deployment.

1. Clean up all the resources.

    ```bash
    azd down --force --purge
    ```
