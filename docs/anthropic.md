# OpenChat Playground with Anthropic

This page describes how to run OpenChat Playground (OCP) with [Anthropic models](https://docs.claude.com/en/docs/about-claude/models) integration.

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

1. Add Anthropic API Key for Claude connection. Make sure you should replace `{{ANTHROPIC_API_KEY}}` with your Anthropic API key.

    ```bash
    # bash/zsh
    dotnet user-secrets --project $REPOSITORY_ROOT/src/OpenChat.PlaygroundApp \
        set Anthropic:ApiKey "{{ANTHROPIC_API_KEY}}"
    ```

    ```bash
    # PowerShell
    dotnet user-secrets --project $REPOSITORY_ROOT/src/OpenChat.PlaygroundApp `
        set Anthropic:ApiKey "{{ANTHROPIC_API_KEY}}"
    ```

    > For more details about Anthropic API keys, refer to the doc, [Anthropic API Documentation](https://docs.anthropic.com/claude/reference/getting-started-with-the-api).

1. Run the app. The default model OCP uses is [Claude Sonnet 4.5](https://www.anthropic.com/claude/sonnet).

    ```bash
    # bash/zsh
    dotnet run --project $REPOSITORY_ROOT/src/OpenChat.PlaygroundApp -- \
        --connector-type Anthropic
    ```

    ```powershell
    # PowerShell
    dotnet run --project $REPOSITORY_ROOT/src/OpenChat.PlaygroundApp -- `
        --connector-type Anthropic
    ```

   Alternatively, if you want to run with a different model, say [Claude Opus 4.1](http://www.anthropic.com/claude-opus-4-1-system-card), other than the default one, you can specify it as an argument:

    ```bash
    # bash/zsh
    dotnet run --project $REPOSITORY_ROOT/src/OpenChat.PlaygroundApp -- \
        --connector-type Anthropic \
        --model claude-opus-4-1
    ```

    ```powershell
    # PowerShell
    dotnet run --project $REPOSITORY_ROOT/src/OpenChat.PlaygroundApp -- `
        --connector-type Anthropic `
        --model claude-opus-4-1
    ```

    By default the app limits the model's response to [512 tokens](https://docs.claude.com/en/docs/about-claude/models/overview), other than the default one, you can specify it as an argument:

    ```bash
    # bash/zsh
    dotnet run --project $REPOSITORY_ROOT/src/OpenChat.PlaygroundApp -- \
        --connector-type Anthropic \
        --max-tokens 2048
    ```

    ```powershell
    # PowerShell
    dotnet run --project $REPOSITORY_ROOT/src/OpenChat.PlaygroundApp -- `
        --connector-type Anthropic `
        --max-tokens 2048
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

1. Get Anthropic API Key.

    ```bash
    # bash/zsh
    API_KEY=$(dotnet user-secrets --project ./src/OpenChat.PlaygroundApp list --json | \
              sed -n '/^\/\//d; p' | jq -r '."Anthropic:ApiKey"')
    ```

    ```bash
    # PowerShell
    $API_KEY = (dotnet user-secrets --project ./src/OpenChat.PlaygroundApp list --json | `
                Select-String -NotMatch '^//(BEGIN|END)' | ConvertFrom-Json).'Anthropic:ApiKey'
    ```

1. Run the app. The default model OCP uses is [Claude Sonnet 4.5](https://www.anthropic.com/claude/sonnet).

    ```bash
    # bash/zsh - from locally built container
    docker run -i --rm -p 8080:8080 openchat-playground:latest --connector-type Anthropic \
        --api-key $API_KEY
    ```

    ```powershell
    # PowerShell - from locally built container
    docker run -i --rm -p 8080:8080 openchat-playground:latest --connector-type Anthropic `
        --api-key $API_KEY
    ```

    ```bash
    # bash/zsh - from GitHub Container Registry
    docker run -i --rm -p 8080:8080 ghcr.io/aliencube/open-chat-playground/openchat-playground:latest \
        --connector-type Anthropic \
        --api-key $API_KEY
    ```

    ```powershell
    # PowerShell - from GitHub Container Registry
    docker run -i --rm -p 8080:8080 ghcr.io/aliencube/open-chat-playground/openchat-playground:latest `
        --connector-type Anthropic `
        --api-key $API_KEY
    ```

   Alternatively, if you want to run with a different model, say [Claude Opus 4.1](http://www.anthropic.com/claude-opus-4-1-system-card), other than the default one, you can specify it as an argument:

    ```bash
    # bash/zsh - from locally built container
    docker run -i --rm -p 8080:8080 openchat-playground:latest --connector-type Anthropic \
        --api-key $API_KEY
        --model claude-opus-4-1
    ```

    ```powershell
    # PowerShell - from locally built container
    docker run -i --rm -p 8080:8080 openchat-playground:latest --connector-type Anthropic `
        --api-key $API_KEY
        --model claude-opus-4-1
    ```

    ```bash
    # bash/zsh - from GitHub Container Registry
    docker run -i --rm -p 8080:8080 ghcr.io/aliencube/open-chat-playground/openchat-playground:latest \
        --connector-type Anthropic \
        --api-key $API_KEY
        --model claude-opus-4-1
    ```

    ```powershell
    # PowerShell - from GitHub Container Registry
    docker run -i --rm -p 8080:8080 ghcr.io/aliencube/open-chat-playground/openchat-playground:latest `
        --connector-type Anthropic `
        --api-key $API_KEY
        --model claude-opus-4-1
    ```

    By default the app limits the model's response to [512 tokens](https://docs.claude.com/en/docs/about-claude/models/overview), other than the default one, you can specify it as an argument:

    ```bash
    # bash/zsh - from locally built container
    docker run -i --rm -p 8080:8080 openchat-playground:latest --connector-type Anthropic \
        --api-key $API_KEY
        --max-tokens 2048
    ```

    ```powershell
    # PowerShell - from locally built container
    docker run -i --rm -p 8080:8080 openchat-playground:latest --connector-type Anthropic `
        --api-key $API_KEY
        --max-tokens 2048
    ```

    ```bash
    # bash/zsh - from GitHub Container Registry
    docker run -i --rm -p 8080:8080 ghcr.io/aliencube/open-chat-playground/openchat-playground:latest \
        --connector-type Anthropic \
        --api-key $API_KEY
        --max-tokens 2048
    ```

    ```powershell
    # PowerShell - from GitHub Container Registry
    docker run -i --rm -p 8080:8080 ghcr.io/aliencube/open-chat-playground/openchat-playground:latest `
        --connector-type Anthropic `
        --api-key $API_KEY
        --max-tokens 2048
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

1. Get Anthropic API Key.

    ```bash
    # bash/zsh
    API_KEY=$(dotnet user-secrets --project ./src/OpenChat.PlaygroundApp list --json | \
              sed -n '/^\/\//d; p' | jq -r '."Anthropic:ApiKey"')
    ```

    ```bash
    # PowerShell
    $API_KEY = (dotnet user-secrets --project ./src/OpenChat.PlaygroundApp list --json | `
                Select-String -NotMatch '^//(BEGIN|END)' | ConvertFrom-Json).'Anthropic:ApiKey'
    ```

1. Set Anthropic API Key to azd environment variables.

    ```bash
    azd env set ANTHROPIC_API_KEY $API_KEY
    ```

   The default model OCP uses is [Claude Sonnet 4.5](https://www.anthropic.com/claude/sonnet). If you want to run with a different model, say [Claude Opus 4.1](http://www.anthropic.com/claude-opus-4-1-system-card), other than the default one, add it to azd environment variables.

    ```bash
    azd env set ANTHROPIC_MODEL claude-opus-4-1
    ```

    By default the app limits the model's response to [512 tokens](https://docs.claude.com/en/docs/about-claude/models/overview), other than the default one, add it to azd environment variables.
    ```bash
    azd env set ANTHROPIC_MAX_TOKENS 2048
    ```

1. Set the connector type to `Anthropic`.

    ```bash
    azd env set CONNECTOR_TYPE Anthropic
    ```

1. Run the following commands in order to provision and deploy the app.

    ```bash
    azd up
    ```

    > **NOTE**: You will be asked to provide Azure subscription and location for deployment.

   Once deployed, you will be able to see the deployed OCP app URL.

1. Open your web browser, navigate to the OCP app URL, and enter prompts.

1. Clean up all the resources.

    ```bash
    azd down --force --purge
    ```
