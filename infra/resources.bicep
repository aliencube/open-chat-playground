@description('The location used for all deployed resources')
param location string = resourceGroup().location

@description('Tags that will be applied to all resources')
param tags object = {}


param connectorType string = ''

// Amazon Bedrock
// Azure AI Foundry
// GitHub Models
param githubModelsModel string = ''
@secure()
param githubModelsToken string = ''
// Google Vertex AI
// Docker Model Runner
// Foundry Local
// Hugging Face
// Ollama
param ollamaModel string = ''
// Anthropic
// LG
// Naver
// OpenAI
// Upstage

param openchatPlaygroundappExists bool

@description('Id of the user or app to assign application roles')
param principalId string

@description('Principal type of user or app')
param principalType string

var abbrs = loadJsonContent('./abbreviations.json')
var resourceToken = uniqueString(subscription().id, resourceGroup().id, location)

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
// Container registry
module containerRegistry 'br/public:avm/res/container-registry/registry:0.9.3' = {
  name: 'registry'
  params: {
    name: '${abbrs.containerRegistryRegistries}${resourceToken}'
    location: location
    tags: tags
    exportPolicyStatus: 'enabled'
    publicNetworkAccess: 'Enabled'
    roleAssignments:[
      {
        principalId: openchatPlaygroundappIdentity.outputs.principalId
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
    publicNetworkAccess: 'Enabled'
  }
}

module openchatPlaygroundappIdentity 'br/public:avm/res/managed-identity/user-assigned-identity:0.4.1' = {
  name: 'openchatPlaygroundappidentity'
  params: {
    name: '${abbrs.managedIdentityUserAssignedIdentities}openchatPlaygroundapp-${resourceToken}'
    location: location
  }
}
module openchatPlaygroundappFetchLatestImage './modules/fetch-container-image.bicep' = {
  name: 'openchatPlaygroundapp-fetch-image'
  params: {
    exists: openchatPlaygroundappExists
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
// GitHub Models
var envGitHubModels = (connectorType == '' || connectorType == 'GitHubModels') ? concat(githubModelsModel != '' ? [
  {
    name: 'GitHubModels__Model'
    value: githubModelsModel
  }
] : [], [
  {
    name: 'GitHubModels__Token'
    secretRef: 'github-models-token'
  }
]) : []
// Google Vertex AI
// Docker Model Runner
// Foundry Local
// Hugging Face
// Ollama
var envOllama = (connectorType == '' || connectorType == 'Ollama') ? (ollamaModel != '' ? [
  {
    name: 'Ollama__Model'
    value: ollamaModel
  }
] : []) : []
// Anthropic
// LG
// Naver
// OpenAI
// Upstage

module openchatPlaygroundapp 'br/public:avm/res/app/container-app:0.18.1' = {
  name: 'openchatPlaygroundapp'
  params: {
    name: 'openchat-playgroundapp'
    ingressTargetPort: 8080
    scaleSettings: {
      minReplicas: 1
      maxReplicas: 10
    }
    secrets: [
      {
        name: 'github-models-token'
        value: githubModelsToken
      }
    ]
    containers: [
      {
        image: openchatPlaygroundappFetchLatestImage.outputs.?containers[?0].?image ?? 'mcr.microsoft.com/azuredocs/containerapps-helloworld:latest'
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
            value: openchatPlaygroundappIdentity.outputs.clientId
          }
          {
            name: 'PORT'
            value: '8080'
          }],
          envConnectorType,
          envGitHubModels,
          envOllama)
      }
    ]
    managedIdentities:{
      systemAssigned: false
      userAssignedResourceIds: [openchatPlaygroundappIdentity.outputs.resourceId]
    }
    registries:[
      {
        server: containerRegistry.outputs.loginServer
        identity: openchatPlaygroundappIdentity.outputs.resourceId
      }
    ]
    environmentResourceId: containerAppsEnvironment.outputs.resourceId
    location: location
    tags: union(tags, { 'azd-service-name': 'openchat-playgroundapp' })
  }
}

output AZURE_CONTAINER_REGISTRY_ENDPOINT string = containerRegistry.outputs.loginServer
output AZURE_RESOURCE_OPENCHAT_PLAYGROUNDAPP_ID string = openchatPlaygroundapp.outputs.resourceId