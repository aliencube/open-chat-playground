# OpenChat Playground with GitHub Models

This page describes to run OpenChat Playground (OCP) with GitHub Models integration.

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

1. Add GitHub Personal Access Token (PAT) for GitHub Models connection. Make sure you should replace `{{GITHUB_TOKEN}}` with your GitHub PAT.

    ```bash
    # bash/zsh
    dotnet user-secrets --project $REPOSITORY_ROOT/src/OpenChat.PlaygroundApp \
        set GitHubModels:Token "{{GITHUB_TOKEN}}"
    ```

    ```bash
    # PowerShell
    dotnet user-secrets --project $REPOSITORY_ROOT/src/OpenChat.PlaygroundApp `
        set GitHubModels:Token "{{GITHUB_TOKEN}}"
    ```

    > For more details about GitHub PAT, refer to the doc, [Managing your personal access tokens](https://docs.github.com/authentication/keeping-your-account-and-data-secure/managing-your-personal-access-tokens).

1. Run the app.

    ```bash
    dotnet run --project $REPOSITORY_ROOT/src/OpenChat.PlaygroundApp -- --connector-type GitHubModels
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

1. Get GitHub PAT.

    ```bash
    # bash/zsh
    TOKEN=$(dotnet user-secrets --project ./src/OpenChat.PlaygroundApp list --json | \
              sed -n '/^\/\//d; p' | jq -r '."GitHubModels:Token"')
    ```

    ```bash
    # PowerShell
    $TOKEN = (dotnet user-secrets --project ./src/OpenChat.PlaygroundApp list --json | `
                Select-String -NotMatch '^//(BEGIN|END)' | ConvertFrom-Json).'GitHubModels:Token'
    ```

1. Run the app.

    ```bash
    # From locally built container
    docker run -i --rm -p 8080:8080 openchat-playground:latest --connector-type GitHubModels --token $TOKEN
    ```

    ```bash
    # From GitHub Container Registry
    docker run -i --rm -p 8080:8080 ghcr.io/aliencube/open-chat-playground/openchat-playground:latest --connector-type GitHubModels --token $TOKEN
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

1. Get GitHub PAT.

    ```bash
    # bash/zsh
    TOKEN=$(dotnet user-secrets --project ./src/OpenChat.PlaygroundApp list --json | \
              sed -n '/^\/\//d; p' | jq -r '."GitHubModels:Token"')
    ```

    ```bash
    # PowerShell
    $TOKEN = (dotnet user-secrets --project ./src/OpenChat.PlaygroundApp list --json | `
                Select-String -NotMatch '^//(BEGIN|END)' | ConvertFrom-Json).'GitHubModels:Token'
    ```

1. Set GitHub PAT to azd environment variables.

    ```bash
    azd env set GH_MODELS_TOKEN $TOKEN
    ```

   Optionally, if you want to run with a different model, say [Grok 3 mini](https://github.com/marketplace/models/azureml-xai/grok-3-mini), other than the default one, add it to azd environment variables.

    ```bash
    azd env set GH_MODELS_MODEL "xai/grok-3-mini"
    ```

1. Set the connector type to `GitHubModels`.

    ```bash
    azd env set CONNECTOR_TYPE "GitHubModels"
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
