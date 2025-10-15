@description('The location used for all deployed resources')
param location string = resourceGroup().location

@description('Tags that will be applied to all resources')
param tags object = {}

param connectorType string = ''

// Amazon Bedrock
// Azure AI Foundry
param azureAIFoundryEndpoint string = ''
@secure()
param azureAIFoundryApiKey string = ''
param azureAIFoundryDeploymentName string = ''
// GitHub Models
param githubModelsModel string = ''
@secure()
param githubModelsToken string = ''
// Google Vertex AI
// Docker Model Runner
// Foundry Local
// Hugging Face
param huggingFaceModel string = ''
// Ollama
param ollamaModel string = ''
// Anthropic
// LG
param lgModel string = ''
// Naver
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

var abbrs = loadJsonContent('./abbreviations.json')
var resourceToken = uniqueString(subscription().id, resourceGroup().id, location)

var useOllama = connectorType == 'HuggingFace' || connectorType == 'Ollama' || connectorType == 'LG'
var ollamaServerModel = replace(
                          replace(
                            replace(
                              replace(
                                replace(toLower(connectorType == 'LG' ? lgModel : (connectorType == 'Ollama' ? ollamaModel : huggingFaceModel)),
                                '-', ''),
                              '_', ''),
                            ':', ''),
                          '.', ''),
                        '/', '')

// Monitor application with Azure Monitor
module monitoring 'br/public:avm/ptn/azd/monitoring:0.2.1' = {
  name: 'monitoring'
  params: {
    logAnalyticsName: '${abbrs.operationalInsightsWorkspaces}${resourceToken}'
    applicationInsightsName: '${abbrs.insightsComponents}${resourceToken}'
    applicationInsightsDashboardName: '${abbrs.portalDashboards}${resourceToken}'
    location: location
    tags: tags
  }
}

// Storage account
resource storage 'Microsoft.Storage/storageAccounts@2025-01-01' = if (useOllama == true) {
  name: '${abbrs.storageStorageAccounts}${resourceToken}'
  location: location
  kind: 'StorageV2'
  tags: tags
  sku: {
    name: 'Standard_LRS'
  }
  properties: {
    largeFileSharesState: 'Enabled'
  }
}

resource storageFileService 'Microsoft.Storage/storageAccounts/fileServices@2025-01-01' = if (useOllama == true) {
  parent: storage
  name: 'default'
}

resource storageFileShare 'Microsoft.Storage/storageAccounts/fileServices/shares@2025-01-01' = if (useOllama == true) {
  parent: storageFileService
  name: '${toLower(connectorType)}-${ollamaServerModel}'
  properties: {
    shareQuota: 1024
    enabledProtocols: 'SMB'
  }
}

// Container registry
module containerRegistry 'br/public:avm/res/container-registry/registry:0.9.3' = {
  name: 'registry'
  params: {
    name: '${abbrs.containerRegistryRegistries}${resourceToken}'
    location: location
    tags: tags
    exportPolicyStatus: 'enabled'
    publicNetworkAccess: 'Enabled'
    roleAssignments: [
      {
        principalId: openchatPlaygroundAppIdentity.outputs.principalId
        principalType: 'ServicePrincipal'
        // ACR Pull role
        roleDefinitionIdOrName: subscriptionResourceId('Microsoft.Authorization/roleDefinitions', '7f951dda-4ed3-4680-a7ca-43fe172d538d')
      }
    ]
  }
}

// Container apps environment
module containerAppsEnvironment 'br/public:avm/res/app/managed-environment:0.11.3' = {
  name: 'container-apps-environment'
  params: {
    appInsightsConnectionString: monitoring.outputs.applicationInsightsConnectionString
    name: '${abbrs.appManagedEnvironments}${resourceToken}'
    location: location
    zoneRedundant: false
    workloadProfiles: concat([
      {
        workloadProfileType: 'Consumption'
        name: 'Consumption'
        enableFips: false
      }
    ], useOllama == true ? [
      {
        workloadProfileType: 'Consumption-GPU-${gpuProfileName}'
        name: gpuProfileName
        enableFips: false
      }
    ] : [])
    publicNetworkAccess: 'Enabled'
    storages: useOllama == true ? [
      {
        shareName: storageFileShare.name
        storageAccountName: storage.name
        kind: 'SMB'
        accessMode: 'ReadWrite'
      }
    ] : null
  }
}

module openchatPlaygroundAppIdentity 'br/public:avm/res/managed-identity/user-assigned-identity:0.4.1' = {
  name: 'openchatPlaygroundAppIdentity'
  params: {
    name: '${abbrs.managedIdentityUserAssignedIdentities}${resourceToken}'
    location: location
  }
}

module openchatPlaygroundAppFetchLatestImage './modules/fetch-container-image.bicep' = {
  name: 'openchatPlaygroundApp-fetch-image'
  params: {
    exists: openchatPlaygroundAppExists
    name: 'openchat-playgroundapp'
  }
}

var envConnectorType = connectorType != '' ? [
  {
    name: 'ConnectorType'
    value: connectorType
  }
] : []

// Amazon Bedrock
// Azure AI Foundry
var envAzureAIFoundry = connectorType == 'AzureAIFoundry' ? concat(azureAIFoundryEndpoint != '' ? [
  {
    name: 'AzureAIFoundry__Endpoint'
    value: azureAIFoundryEndpoint
  }
] : [], azureAIFoundryDeploymentName != '' ? [
  {
    name: 'AzureAIFoundry__DeploymentName'
    value: azureAIFoundryDeploymentName
  }
] : [], azureAIFoundryApiKey != '' ? [
  {
    name: 'AzureAIFoundry__ApiKey'
    secretRef: 'azure-ai-foundry-api-key'
  }
]: []) : []
// GitHub Models
var envGitHubModels = (connectorType == '' || connectorType == 'GitHubModels') ? concat(githubModelsModel != '' ? [
  {
    name: 'GitHubModels__Model'
    value: githubModelsModel
  }
] : [], githubModelsToken != '' ? [
  {
    name: 'GitHubModels__Token'
    secretRef: 'github-models-token'
  }
] : []) : []
// Google Vertex AI
// Docker Model Runner
// Foundry Local
// Hugging Face
var envHuggingFace = connectorType == 'HuggingFace' ? concat(huggingFaceModel != '' ? [
  {
    name: 'HuggingFace__Model'
    value: huggingFaceModel
  }
] : []) : []
// Ollama
var envOllama = connectorType == 'Ollama' ? concat(ollamaModel != '' ? [
  {
    name: 'Ollama__Model'
    value: ollamaModel
  }
] : []) : []
// Anthropic
// LG
var envLG = connectorType == 'LG' ? concat(lgModel != '' ? [
  {
    name: 'LG__Model'
    value: lgModel
  }
] : []) : []
// Naver
// OpenAI
var envOpenAI = connectorType == 'OpenAI' ? concat(openAIModel != '' ? [
  {
    name: 'OpenAI__Model'
    value: openAIModel
  }
] : [], openAIApiKey != '' ? [
  {
    name: 'OpenAI__ApiKey'
    secretRef: 'openai-api-key'
  }
] : []) : []
// Upstage
var envUpstage = connectorType == 'Upstage' ? concat(upstageModel != '' ? [
  {
    name: 'Upstage__Model'
    value: upstageModel
  }
] : [], upstageBaseUrl != '' ? [
  {
    name: 'Upstage__BaseUrl'
    value: upstageBaseUrl
  }
] : [], upstageApiKey != '' ? [
  {
    name: 'Upstage__ApiKey'
    secretRef: 'upstage-api-key'
  }
] : []) : []

module openchatPlaygroundApp 'br/public:avm/res/app/container-app:0.18.1' = {
  name: 'openchatPlaygroundApp'
  params: {
    name: 'openchat-playgroundapp'
    ingressTargetPort: 8080
    scaleSettings: {
      minReplicas: 1
      maxReplicas: 10
    }
    secrets: concat(azureAIFoundryApiKey != '' ? [
      {
        name: 'azure-ai-foundry-api-key'
        value: azureAIFoundryApiKey
      }
    ] : [], githubModelsToken != '' ? [
      {
        name: 'github-models-token'
        value: githubModelsToken
      }
    ] : [], openAIApiKey != '' ? [
      {
        name: 'openai-api-key'
        value: openAIApiKey
      }
    ] : [], upstageApiKey != '' ? [
      {
        name: 'upstage-api-key'
        value: upstageApiKey
      }
    ] : [])
    containers: [
      {
        image: openchatPlaygroundAppFetchLatestImage.outputs.?containers[?0].?image ?? 'mcr.microsoft.com/azuredocs/containerapps-helloworld:latest'
        name: 'main'
        resources: {
          cpu: json('0.5')
          memory: '1.0Gi'
        }
        env: concat([
          {
            name: 'APPLICATIONINSIGHTS_CONNECTION_STRING'
            value: monitoring.outputs.applicationInsightsConnectionString
          }
          {
            name: 'AZURE_CLIENT_ID'
            value: openchatPlaygroundAppIdentity.outputs.clientId
          }
          {
            name: 'PORT'
            value: '8080'
          }
        ],
        envConnectorType,
        envAzureAIFoundry,
        envGitHubModels,
        envHuggingFace,
        envOllama,
        envLG,
        envOpenAI,
        envUpstage, 
        useOllama == true ? [
          {
            name: connectorType == 'LG' ? 'LG__BaseUrl' : (connectorType == 'Ollama' ? 'Ollama__BaseUrl' : 'HuggingFace__BaseUrl')
            value: 'https://${ollama.outputs.fqdn}'
          }
        ] : [])
      }
    ]
    managedIdentities: {
      systemAssigned: false
      userAssignedResourceIds: [
        openchatPlaygroundAppIdentity.outputs.resourceId
      ]
    }
    registries: [
      {
        server: containerRegistry.outputs.loginServer
        identity: openchatPlaygroundAppIdentity.outputs.resourceId
      }
    ]
    environmentResourceId: containerAppsEnvironment.outputs.resourceId
    location: location
    tags: union(tags, { 'azd-service-name': 'openchat-playgroundapp' })
  }
}

module ollama 'br/public:avm/res/app/container-app:0.18.1' = if (useOllama == true) {
  name: 'ollama'
  params: {
    name: 'ollama'
    ingressTargetPort: 11434
    ingressTransport: 'http'
    scaleSettings: {
      minReplicas: 0
      maxReplicas: 10
      cooldownPeriod: 300
      pollingInterval: 30
    }
    workloadProfileName: gpuProfileName
    secrets: []
    volumes: [
      {
        name: '${toLower(connectorType)}-${ollamaServerModel}'
        storageType: 'AzureFile'
        storageName: storageFileShare.name
      }
    ]
    containers: [
      {
        image: 'docker.io/ollama/ollama:latest'
        name: 'main'
        resources: {
          cpu: gpuProfileName == 'NC8as-T4' ? json('8') : json('24')
          memory: gpuProfileName == 'NC8as-T4' ? '56Gi' : '220Gi'
        }
        env: [
          {
            name: 'APPLICATIONINSIGHTS_CONNECTION_STRING'
            value: monitoring.outputs.applicationInsightsConnectionString
          }
          {
            name: 'AZURE_CLIENT_ID'
            value: openchatPlaygroundAppIdentity.outputs.clientId
          }
          {
            name: 'PORT'
            value: '11434'
          }
        ]
        volumeMounts: [
          {
            volumeName: '${toLower(connectorType)}-${ollamaServerModel}'
            mountPath: '/root/.ollama'
          }
        ]
      }
    ]
    managedIdentities: {
      systemAssigned: false
      userAssignedResourceIds: [
        openchatPlaygroundAppIdentity.outputs.resourceId
      ]
    }
    environmentResourceId: containerAppsEnvironment.outputs.resourceId
    location: location
    tags: union(tags, { 'azd-service-name': 'ollama' })
  }
}

output AZURE_CONTAINER_REGISTRY_ENDPOINT string = containerRegistry.outputs.loginServer
output AZURE_RESOURCE_OPENCHAT_PLAYGROUNDAPP_ID string = openchatPlaygroundApp.outputs.resourceId
