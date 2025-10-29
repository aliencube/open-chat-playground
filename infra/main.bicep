targetScope = 'subscription'

@minLength(1)
@maxLength(64)
@description('Name of the environment that can be used as part of naming resource convention')
param environmentName string

@minLength(1)
@description('Primary location for all resources')
param location string

param connectorType string = ''

// Amazon Bedrock
@secure()
param amazonBedrockAccessKeyId string = ''
@secure()
param amazonBedrockSecretAccessKey string = ''
param amazonBedrockRegion string = ''
param amazonBedrockModelId string = ''
// Azure AI Foundry
param azureAIFoundryEndpoint string = ''
@secure()
param azureAIFoundryApiKey string = ''
param azureAIFoundryDeploymentName string = ''
// GitHub Models
@secure()
param githubModelsToken string = ''
param githubModelsModel string = ''
// Google Vertex AI
// Docker Model Runner - NOT SUPPORTED
// Foundry Local - NOT SUPPORTED
// Hugging Face
param huggingFaceModel string = ''
// Ollama
param ollamaModel string = ''
// Anthropic
@secure()
param anthropicApiKey string = ''
param anthropicModel string = ''
@minValue(1)
param anthropicMaxTokens int
// LG
param lgModel string = ''
// Naver - NOT SUPPORTED
// OpenAI
param openAIModel string = ''
@secure()
param openAIApiKey string = ''
// Upstage
param upstageModel string = ''
param upstageBaseUrl string = ''
@secure()
param upstageApiKey string = ''

@allowed([
  'NC24-A100'
  'NC8as-T4'
])
@description('The GPU profile name for Container Apps environment when using Ollama, Hugging Face or LG connectors. Supported values are NC24-A100 and NC8as-T4.')
param gpuProfileName string = 'NC8as-T4'

param openchatPlaygroundAppExists bool

@description('Id of the user or app to assign application roles')
param principalId string

@description('Principal type of user or app')
param principalType string

// Tags that should be applied to all resources.
// 
// Note that 'azd-service-name' tags should be applied separately to service host resources.
// Example usage:
//   tags: union(tags, { 'azd-service-name': <service name in azure.yaml> })
var tags = {
  'azd-env-name': environmentName
}

// Organize resources in a resource group
resource rg 'Microsoft.Resources/resourceGroups@2021-04-01' = {
  name: 'rg-${environmentName}'
  location: location
  tags: tags
}

module resources 'resources.bicep' = {
  scope: rg
  name: 'resources'
  params: {
    location: location
    tags: tags
    principalId: principalId
    principalType: principalType
    connectorType: connectorType
    amazonBedrockAccessKeyId: amazonBedrockAccessKeyId
    amazonBedrockSecretAccessKey: amazonBedrockSecretAccessKey
    amazonBedrockRegion: amazonBedrockRegion
    amazonBedrockModelId: amazonBedrockModelId
    azureAIFoundryEndpoint: azureAIFoundryEndpoint
    azureAIFoundryApiKey: azureAIFoundryApiKey
    azureAIFoundryDeploymentName: azureAIFoundryDeploymentName
    huggingFaceModel: huggingFaceModel
    githubModelsToken: githubModelsToken
    githubModelsModel: githubModelsModel
    ollamaModel: ollamaModel
    anthropicModel: anthropicModel
    anthropicApiKey: anthropicApiKey
    anthropicMaxTokens: anthropicMaxTokens
    lgModel: lgModel
    openAIModel: openAIModel
    openAIApiKey: openAIApiKey
    upstageModel: upstageModel
    upstageBaseUrl: upstageBaseUrl
    upstageApiKey: upstageApiKey
    gpuProfileName: gpuProfileName
    openchatPlaygroundAppExists: openchatPlaygroundAppExists
  }
}

output AZURE_CONTAINER_REGISTRY_ENDPOINT string = resources.outputs.AZURE_CONTAINER_REGISTRY_ENDPOINT
output AZURE_RESOURCE_OPENCHAT_PLAYGROUNDAPP_ID string = resources.outputs.AZURE_RESOURCE_OPENCHAT_PLAYGROUNDAPP_ID
