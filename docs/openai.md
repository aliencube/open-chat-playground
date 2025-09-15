# OpenChat Playground with OpenAI GPT

This page describes how to run OpenChat Playground (OCP) with OpenAI GPT integration.

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

1. Add OpenAI API Key for OpenAI GPT connection. Make sure you should replace `{{OPENAI_API_KEY}}` with your OpenAI API key.

    ```bash
    # bash/zsh
    dotnet user-secrets --project $REPOSITORY_ROOT/src/OpenChat.PlaygroundApp \
        set OpenAI:ApiKey "{{OPENAI_API_KEY}}"
    ```

    ```bash
    # PowerShell
    dotnet user-secrets --project $REPOSITORY_ROOT/src/OpenChat.PlaygroundApp `
        set OpenAI:ApiKey "{{OPENAI_API_KEY}}"
    ```

    > For more details about OpenAI API Keys, refer to the doc, [API Keys](https://platform.openai.com/api-keys).

1. Run the app.

    ```bash
    # bash/zsh
    dotnet run --project $REPOSITORY_ROOT/src/OpenChat.PlaygroundApp -- \
        --connector-type OpenAI
    ```

    ```powershell
    # PowerShell
    dotnet run --project $REPOSITORY_ROOT/src/OpenChat.PlaygroundApp -- `
        --connector-type OpenAI
    ```

    Optionally, if you want to run with a different model, say [GPT-4o](https://platform.openai.com/docs/models/gpt-4o), other than the default one, you can specify it via command line:

    ```bash
    # bash/zsh
    dotnet run --project $REPOSITORY_ROOT/src/OpenChat.PlaygroundApp -- \
        --connector-type OpenAI \
        --model gpt-4o
    ```

    ```powershell
    # PowerShell
    dotnet run --project $REPOSITORY_ROOT/src/OpenChat.PlaygroundApp -- `
        --connector-type OpenAI `
        --model gpt-4o
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

1. Get OpenAI API Key.

    ```bash
    # bash/zsh
    API_KEY=$(dotnet user-secrets --project ./src/OpenChat.PlaygroundApp list --json | \
              sed -n '/^\/\//d; p' | jq -r '."OpenAI:ApiKey"')
    ```

    ```bash
    # PowerShell
    $API_KEY = (dotnet user-secrets --project ./src/OpenChat.PlaygroundApp list --json | `
                Select-String -NotMatch '^//(BEGIN|END)' | ConvertFrom-Json).'OpenAI:ApiKey'
    ```

1. Run the app.

    ```bash
    # bash/zsh - From locally built container
    docker run -i --rm -p 8080:8080 openchat-playground:latest \
        --connector-type OpenAI --api-key $API_KEY
    ```

    ```powershell
    # PowerShell - From locally built container
    docker run -i --rm -p 8080:8080 openchat-playground:latest `
        --connector-type OpenAI --api-key $API_KEY
    ```

    ```bash
    # bash/zsh - From GitHub Container Registry
    docker run -i --rm -p 8080:8080 ghcr.io/aliencube/open-chat-playground/openchat-playground:latest \
        --connector-type OpenAI --api-key $API_KEY
    ```

    ```powershell
    # PowerShell - From GitHub Container Registry
    docker run -i --rm -p 8080:8080 ghcr.io/aliencube/open-chat-playground/openchat-playground:latest `
        --connector-type OpenAI --api-key $API_KEY
    ```

    Optionally, if you want to run with a different model, say [GPT-4o](https://platform.openai.com/docs/models/gpt-4o), other than the default one, you can specify it via command line:

    ```bash
    # bash/zsh - From locally built container with custom model
    docker run -i --rm -p 8080:8080 openchat-playground:latest --connector-type OpenAI \
        --api-key $API_KEY --model gpt-4o
    ```

    ```powershell
    # PowerShell - From locally built container with custom model
    docker run -i --rm -p 8080:8080 openchat-playground:latest --connector-type OpenAI `
        --api-key $API_KEY --model gpt-4o
    ```

1. Open your web browser, navigate to `http://localhost:8080`, and enter prompts.

## Run on Azure

1. Make sure you are at the repository root.

    ```bash
    cd $REPOSITORY_ROOT
    ```

1. Login to Azure.

    ```bash
    azd auth login
    ```

1. Check login status.

    ```bash
    azd auth login --check-status
    ```

1. Initialize `azd` template.

    ```bash
    azd init
    ```

    > **NOTE**: You will be asked to provide environment name for provisioning.

1. Get OpenAI API Key.

    ```bash
    # bash/zsh
    API_KEY=$(dotnet user-secrets --project ./src/OpenChat.PlaygroundApp list --json | \
              sed -n '/^\/\//d; p' | jq -r '."OpenAI:ApiKey"')
    ```

    ```bash
    # PowerShell
    $API_KEY = (dotnet user-secrets --project ./src/OpenChat.PlaygroundApp list --json | `
                Select-String -NotMatch '^//(BEGIN|END)' | ConvertFrom-Json).'OpenAI:ApiKey'
    ```

1. Set OpenAI API Key to azd environment variables.

    ```bash
    azd env set OPENAI_API_KEY $API_KEY
    ```

    Optionally, if you want to run with a different model, say [gpt-4o](https://platform.openai.com/docs/models/gpt-4o), other than the default one, add it to azd environment variables.

    ```bash
    azd env set OPENAI_MODEL gpt-4o
    ```

1. Set the connector type to `OpenAI`.

    ```bash
    azd env set CONNECTOR_TYPE OpenAI
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
