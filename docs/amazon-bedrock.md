# OpenChat Playground with Amazon Bedrock

This page describes how to run OpenChat Playground (OCP) with [Amazon Bedrock](https://aws.amazon.com/bedrock/getting-started/) integration.

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

1. Add Amazon Bedrock access details for Amazon Bedrock connection. Make sure you should replace `{{AMAZON_BEDROCK_ACCESS_KEY_ID}}` and `{{AMAZON_BEDROCK_SECRET_ACCESS_KEY}}` with your Amazon Bedrock access key ID and secret access key respectively.

    ```bash
    # bash/zsh
    dotnet user-secrets --project $REPOSITORY_ROOT/src/OpenChat.PlaygroundApp \
        set AmazonBedrock:AccessKeyId "{{AMAZON_BEDROCK_ACCESS_KEY_ID}}"
    dotnet user-secrets --project $REPOSITORY_ROOT/src/OpenChat.PlaygroundApp \
        set AmazonBedrock:SecretAccessKey "{{AMAZON_BEDROCK_SECRET_ACCESS_KEY}}"
    ```

    ```powershell
    # PowerShell
    dotnet user-secrets --project $REPOSITORY_ROOT/src/OpenChat.PlaygroundApp `
        set AmazonBedrock:AccessKeyId "{{AMAZON_BEDROCK_ACCESS_KEY_ID}}"
    dotnet user-secrets --project $REPOSITORY_ROOT/src/OpenChat.PlaygroundApp `
        set AmazonBedrock:SecretAccessKey "{{AMAZON_BEDROCK_SECRET_ACCESS_KEY}}"
    ```

    > To get more details of Amazon Bedrock authenticaiton, refer to the doc, [Generate Amazon Bedrock API keys to easily authenticate to the Amazon Bedrock API](https://docs.aws.amazon.com/bedrock/latest/userguide/api-keys.html).

1. Run the app by passing both region and model parameters, `{{AMAZON_BEDROCK_REGION}}`. The default deployment name OCP uses is [`amazon.nova-micro-v1:0`](https://docs.aws.amazon.com/nova/latest/userguide/what-is-nova.html).

    ```bash
    # bash/zsh
    dotnet run --project $REPOSITORY_ROOT/src/OpenChat.PlaygroundApp -- \
        --connector-type AmazonBedrock \
        --region "{{AMAZON_BEDROCK_REGION}}"
    ```

    ```powershell
    # PowerShell
    dotnet run --project $REPOSITORY_ROOT/src/OpenChat.PlaygroundApp -- `
        --connector-type AmazonBedrock `
        --region "{{AMAZON_BEDROCK_REGION}}"
    ```

   Alternatively, if you want to run with a different deployment, say [`ai21.jamba-1-5-mini-v1:0`](https://docs.aws.amazon.com/bedrock/latest/userguide/model-parameters-jamba.html), other than the default one, you can specify it as an argument.

    ```bash
    # bash/zsh
    dotnet run --project $REPOSITORY_ROOT/src/OpenChat.PlaygroundApp -- \
        --connector-type AmazonBedrock \
        --region "{{AMAZON_BEDROCK_REGION}}" \
        --model-id "ai21.jamba-1-5-mini-v1:0"
    ```

    ```powershell
    # PowerShell
    dotnet run --project $REPOSITORY_ROOT/src/OpenChat.PlaygroundApp -- `
        --connector-type AmazonBedrock `
        --region "{{AMAZON_BEDROCK_REGION}}" `
        --model-id "ai21.jamba-1-5-mini-v1:0"
    ```

1. Open your web browser, navigate to `http://localhost:5280`, and enter prompts.

## Run in local container

1. Make sure that you have the Amazon Bedrock access details including access key ID, secret access key and region.
1. Make sure you are at the repository root.

    ```bash
    cd $REPOSITORY_ROOT
    ```

1. Build a container.

    ```bash
    docker build -f Dockerfile -t openchat-playground:latest .
    ```

1. Get Azure AI Foundry API Key.

    ```bash
    # bash/zsh
    ACCESS_KEY_ID=$(dotnet user-secrets --project ./src/OpenChat.PlaygroundApp list --json | \
                    sed -n '/^\/\//d; p' | jq -r '."AmazonBedrock:AccessKeyId"')
    SECRET_ACCESS_KEY=$(dotnet user-secrets --project ./src/OpenChat.PlaygroundApp list --json | \
                        sed -n '/^\/\//d; p' | jq -r '."AmazonBedrock:SecretAccessKey"')
    ```

    ```powershell
    # PowerShell
    $ACCESS_KEY_ID = (dotnet user-secrets --project ./src/OpenChat.PlaygroundApp list --json | `
                      Select-String -NotMatch '^//(BEGIN|END)' | ConvertFrom-Json).'AmazonBedrock:AccessKeyId'
    $SECRET_ACCESS_KEY = (dotnet user-secrets --project ./src/OpenChat.PlaygroundApp list --json | `
                          Select-String -NotMatch '^//(BEGIN|END)' | ConvertFrom-Json).'AmazonBedrock:SecretAccessKey'
    ```

1. Run the app. The default deployment name OCP uses is [`amazon.nova-micro-v1:0`](https://docs.aws.amazon.com/nova/latest/userguide/what-is-nova.html).

    ```bash
    # bash/zsh - from locally built container
    docker run -i --rm -p 8080:8080 openchat-playground:latest \
        --connector-type AmazonBedrock \
        --access-key-id $ACCESS_KEY_ID \
        --secret-access-key $SECRET_ACCESS_KEY \
        --region "{{AMAZON_BEDROCK_REGION}}"
    ```

    ```powershell
    # PowerShell - from locally built container
    docker run -i --rm -p 8080:8080 openchat-playground:latest `
        --connector-type AmazonBedrock `
        --access-key-id $ACCESS_KEY_ID `
        --secret-access-key $SECRET_ACCESS_KEY `
        --region "{{AMAZON_BEDROCK_REGION}}"
    ```

    ```bash
    # bash/zsh - from GitHub Container Registry
    docker run -i --rm -p 8080:8080 ghcr.io/aliencube/open-chat-playground/openchat-playground:latest \
        --connector-type AmazonBedrock \
        --access-key-id $ACCESS_KEY_ID \
        --secret-access-key $SECRET_ACCESS_KEY \
        --region "{{AMAZON_BEDROCK_REGION}}"
    ```

    ```powershell
    # PowerShell - from GitHub Container Registry
    docker run -i --rm -p 8080:8080 ghcr.io/aliencube/open-chat-playground/openchat-playground:latest `
        --connector-type AmazonBedrock `
        --access-key-id $ACCESS_KEY_ID `
        --secret-access-key $SECRET_ACCESS_KEY `
        --region "{{AMAZON_BEDROCK_REGION}}"
    ```

   Alternatively, if you want to run with a different deployment, say [`ai21.jamba-1-5-mini-v1:0`](https://docs.aws.amazon.com/bedrock/latest/userguide/model-parameters-jamba.html), other than the default one, you can specify it as an argument:

    ```bash
    # bash/zsh - from locally built container
    docker run -i --rm -p 8080:8080 openchat-playground:latest \
        --connector-type AmazonBedrock \
        --access-key-id $ACCESS_KEY_ID \
        --secret-access-key $SECRET_ACCESS_KEY \
        --region "{{AMAZON_BEDROCK_REGION}}" \
        --model-id "ai21.jamba-1-5-mini-v1:0"
    ```

    ```powershell
    # PowerShell - from locally built container
    docker run -i --rm -p 8080:8080 openchat-playground:latest `
        --connector-type AmazonBedrock `
        --access-key-id $ACCESS_KEY_ID `
        --secret-access-key $SECRET_ACCESS_KEY `
        --region "{{AMAZON_BEDROCK_REGION}}" `
        --model-id "ai21.jamba-1-5-mini-v1:0"
    ```

    ```bash
    # bash/zsh - from GitHub Container Registry
    docker run -i --rm -p 8080:8080 ghcr.io/aliencube/open-chat-playground/openchat-playground:latest \
        --connector-type AmazonBedrock \
        --access-key-id $ACCESS_KEY_ID \
        --secret-access-key $SECRET_ACCESS_KEY \
        --region "{{AMAZON_BEDROCK_REGION}}" \
        --model-id "ai21.jamba-1-5-mini-v1:0"
    ```

    ```powershell
    # PowerShell - from GitHub Container Registry
    docker run -i --rm -p 8080:8080 ghcr.io/aliencube/open-chat-playground/openchat-playground:latest `
        --connector-type AmazonBedrock `
        --access-key-id $ACCESS_KEY_ID `
        --secret-access-key $SECRET_ACCESS_KEY `
        --region "{{AMAZON_BEDROCK_REGION}}" `
        --model-id "ai21.jamba-1-5-mini-v1:0"
    ```

1. Open your web browser, navigate to `http://localhost:8080`, and enter prompts.

## Run on Azure

1. Make sure that you have the Azure AI Foundry Endpoint URL.
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

1. Get Amazon Bedrock access details.

    ```bash
    # bash/zsh
    ACCESS_KEY_ID=$(dotnet user-secrets --project ./src/OpenChat.PlaygroundApp list --json | \
                    sed -n '/^\/\//d; p' | jq -r '."AmazonBedrock:AccessKeyId"')
    SECRET_ACCESS_KEY=$(dotnet user-secrets --project ./src/OpenChat.PlaygroundApp list --json | \
                        sed -n '/^\/\//d; p' | jq -r '."AmazonBedrock:SecretAccessKey"')
    ```

    ```powershell
    # PowerShell
    $ACCESS_KEY_ID = (dotnet user-secrets --project ./src/OpenChat.PlaygroundApp list --json | `
                      Select-String -NotMatch '^//(BEGIN|END)' | ConvertFrom-Json).'AmazonBedrock:AccessKeyId'
    $SECRET_ACCESS_KEY = (dotnet user-secrets --project ./src/OpenChat.PlaygroundApp list --json | `
                          Select-String -NotMatch '^//(BEGIN|END)' | ConvertFrom-Json).'AmazonBedrock:SecretAccessKey'
    ```

1. Set Amazon Bedrock configuration to azd environment variables.

    ```bash
    azd env set AMAZON_BEDROCK_ACCESS_KEY_ID $ACCESS_KEY_ID
    azd env set AMAZON_BEDROCK_SECRET_ACCESS_KEY $SECRET_ACCESS_KEY
    azd env set AMAZON_BEDROCK_REGION "{{AMAZON_BEDROCK_REGION}}"
    ```

   The default deployment name OCP uses is [`amazon.nova-micro-v1:0`](https://docs.aws.amazon.com/nova/latest/userguide/what-is-nova.html). If you want to run with a different deployment, say [`ai21.jamba-1-5-mini-v1:0`](https://docs.aws.amazon.com/bedrock/latest/userguide/model-parameters-jamba.html), other than the default one, add it to azd environment variables.

    ```bash
    azd env set AMAZON_BEDROCK_MODEL_ID "ai21.jamba-1-5-mini-v1:0"
    ```

1. Set the connector type to `AmazonBedrock`.

    ```bash
    azd env set CONNECTOR_TYPE AmazonBedrock
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
