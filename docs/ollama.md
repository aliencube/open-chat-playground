# OpenChat Playground with Ollama

This page describes to run OpenChat Playground (OCP) with Ollama integration.

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
    ```

1. Run the app.

    ```bash
    dotnet run --project $REPOSITORY_ROOT/src/OpenChat.PlaygroundApp -- --connector-type Ollama
    ```

1. Open your web browser, navigate to `http://localhost:5280`, and enter prompts.

## Run in local container

1. Make sure you are at the repository root.

    ```bash
    cd $REPOSITORY_ROOT
    ```

1. Build a container.

    ```
    docker build -f Dockerfile -t openchat-playground:latest .
    ```

1. Run the app.

    ```
    # From locally built container
    docker run -i --rm -p 8080:8080 openchat-playground:latest --connector-type Ollama --base-url http://host.docker.internal:11434 --model llama3.2
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

1. Set Ollama configuration to azd environment variables. 
    ```bash
    # Set connector type to Ollama
    azd env set CONNECTOR_TYPE "Ollama"
    
    # Optionally, set a specific model (default is llama3.2)
    azd env set OLLAMA_MODEL "llama3.2"
    ```

1. Run the following commands in order to provision and deploy the app.

    ```bash
    azd up
    ```

   > **NOTE**: You will be asked to provide Azure subscription and location for deployment.

1. Clean up all the resources.

    ```bash
    azd down --force --purge
    ```
