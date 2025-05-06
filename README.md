# AI Open Chat Playground

This provides a web UI for AI chat playground that is able to connect virtually any LLM from any platform.

## Prerequisites

- [.NET SDK 9](https://dotnet.microsoft.com/download/dotnet/9.0)
- [Visual Studio Code](https://code.visualstudio.com/) + [C# DevKit](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.csdevkit)
- [Docker Desktop](https://docs.docker.com/desktop/) or [Podman](https://podman.io/docs/installation)
- [PowerShell 7.4+](https://learn.microsoft.com/powershell/scripting/install/installing-powershell)
- [Azure Developer CLI](https://learn.microsoft.com/azure/developer/azure-developer-cli/install-azd)
- [Azure CLI](https://learn.microsoft.com/cli/azure/install-azure-cli) + [Container Apps extension](https://learn.microsoft.com/cli/azure/azure-cli-extensions-overview)
- [GitHub CLI](https://cli.github.com/)

## Getting Started

### Get the repository ready

1. Login to GitHub.

    ```bash
    gh auth login
    ```

1. Check login status.

    ```bash
    gh auth status
    ```

1. Fork this repository to your account and clone the forked repository to your local machine.

    ```bash
    gh repo fork aliencube/open-chat-playground --clone --default-branch-only
    ```

### Run it locally

1. Get the repository root.

    ```bash
    # bash/zsh
    REPOSITORY_ROOT=$(git rev-parse --show-toplevel)
    ```

    ```powershell
    # PowerShell
    $REPOSITORY_ROOT = git rev-parse --show-toplevel
    ```

1. Run the app.

    ```bash
    dotnet run --project $REPOSITORY_ROOT/src/OpenChat.AppHost
    ```

1. If you want to change the model, pass the following arguments:

    ```bash
    dotnet run --project $REPOSITORY_ROOT/src/OpenChat.AppHost -- [OPTIONS]
    ```

   Here are the list of options:

   - `--llm-provider`: Choose `openai`, `ollama` or `hface`. Default is `openai`.
   - `--openai-deployment`: Provide the deployment name, if you choose `openai` as the LLM provider. Default is `gpt-4o`.
   - `--openai-connection-string`: Provide the connection string to OpenAI. It must be provided if you choose `openai` as the LLM provider. It must follow the format like `Endpoint=xxxxx;Key=xxxxx`.
   - `--ollama-image-tag`: Provide the Ollama container version, if you choose either `ollama` or `hface` as the LLM provider. Default is `0.6.8`.
   - `--ollama-use-gpu`: Provide the value whether to use the GPU acceleration or not, if you choose either `ollama` or `hface` as the LLM provider. Default is `false`.
   - `--ollama-deployment`: Provide the deployment name, if you choose `ollama` as the LLM provider. Default is `llama`.
   - `--ollama-model`: Provide the model name, if you choose `ollama` as the LLM provider. Default is `llama3.2`.
   - `--huggingface-deployment`: Provide the deployment name, if you choose `hface` as the LLM provider. Default is `qwen3`.
   - `--huggingface-model`: Provide the model name, if you choose `hface` as the LLM provider. Default is `Qwen/Qwen3-14B-GGUF`.
   - `--help`: Display the help message.

### Run on Azure

1. Get the repository root.

    ```bash
    # bash/zsh
    REPOSITORY_ROOT=$(git rev-parse --show-toplevel)
    ```

    ```powershell
    # PowerShell
    $REPOSITORY_ROOT = git rev-parse --show-toplevel
    ```

1. Make sure you are at the repository root.

    ```bash
    cd $REPOSITORY_ROOT
    ```

1. Login to Azure.

    ```bash
    # Login to Azure Dev CLI
    azd auth login
    
    # Login to Azure CLI
    az login
    ```

1. Check login status.

    ```bash
    # Azure Dev CLI
    azd auth login --check-status
    
    # Azure CLI
    az account show
    ```

1. Update `appsettings.json` on the `OpenChat.AppHost` project. The JSON object below shows the default values.

    ```jsonc
    {
      "LLM": {
        // Set the LLM provider.
        "Provider": "openai"
      },
    
      "OpenAI": {
        // Set the deployment name, if you choose 'openai' as the LLM provider.
        "DeploymentName": "gpt-4o"
      },
    
      "Ollama": {
        // Set the Ollama container image tag.
        "ImageTag": "0.6.8",
        // Set to either 'true' or 'false' depending on the usage of the GPU acceleration.
        "UseGPU": false,
        // Set the deployment name, if you choose either `ollama` or `hface` as the LLM provider.
        "DeploymentName": "llama",
        // Set the model name, if you choose either `ollama` or `hface` as the LLM provider.
        // Make sure the model name must include '/' in the middle and 'GGUF' at the end, if you choose 'hface' as the LLM provider.
        "ModelName": "llama3.2"
      }
    }
    ```

1. Add OpenAI connection string, if you want to use OpenAI as the LLM provider.

    ```bash
    # bash/zsh
    dotnet user-secrets --project $REPOSITORY_ROOT/src/OpenChat.AppHost \
        set ConnectionStrings:openai "Endpoint={{API_ENDPOINT}};Key={{API_KEY}}"
    ```

    ```bash
    # PowerShell
    dotnet user-secrets --project $REPOSITORY_ROOT/src/OpenChat.AppHost `
        set ConnectionStrings:openai "Endpoint={{API_ENDPOINT}};Key={{API_KEY}}"
    ```

1. Run the following commands in order to provision and deploy the app.

    ```bash
    azd up
    ```

   > **NOTE**: You will be asked to provide Azure subscription and location for deployment.
