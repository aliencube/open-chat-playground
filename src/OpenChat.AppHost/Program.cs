using Microsoft.Extensions.DependencyInjection;

using OpenChat.Common.Configurations;

using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var config = AppSettings.Parse(builder.Configuration, args);
builder.Services.AddSingleton(config);

var openai = default(IResourceBuilder<IResourceWithConnectionString>);
if (config.LLM.Provider == LLMProviderType.OpenAI)
{
    openai = builder.AddConnectionString(config.LLM.Provider.ToString());
}

var ollama = default(IResourceBuilder<OllamaResource>);
var model = default(IResourceBuilder<OllamaModelResource>);
if (config.LLM.Provider == LLMProviderType.Ollama || config.LLM.Provider == LLMProviderType.HuggingFace)
{
    ollama = builder.AddOllama(config.LLM.Provider.ToString())
                    .WithImageTag(config.Ollama.ImageTag)
                    .WithDataVolume();
    if (config.Ollama.UseGPU == true)
    {
        ollama.WithContainerRuntimeArgs("--gpus=all");
    }
    model = config.LLM.Provider == LLMProviderType.Ollama
            ? ollama.AddModel(config.Ollama.DeploymentName, config.Ollama.ModelName)
            : ollama.AddHuggingFaceModel(config.Ollama.DeploymentName, config.Ollama.ModelName);
}

var webapp = builder.AddProject<OpenChat_PlaygroundApp>("playgroundapp")
                    .WithEnvironment("LLM__Provider", config.LLM.Provider.ToString());
if (config.LLM.Provider == LLMProviderType.OpenAI)
{
    webapp.WithReference(openai!)
          .WaitFor(openai!)
          .WithEnvironment("OpenAI__DeploymentName", config.OpenAI.DeploymentName);
}
if (config.LLM.Provider == LLMProviderType.Ollama || config.LLM.Provider == LLMProviderType.HuggingFace)
{
    webapp.WithReference(model!)
          .WaitFor(model!)
          .WithEnvironment("Ollama__ImageTag", config.Ollama.ImageTag)
          .WithEnvironment("Ollama__UseGPU", config.Ollama.UseGPU.ToString().ToLowerInvariant())
          .WithEnvironment("Ollama__UseHuggingFaceModel", config.Ollama.UseHuggingFaceModel.ToString().ToLowerInvariant())
          .WithEnvironment("Ollama__DeploymentName", config.Ollama.DeploymentName)
          .WithEnvironment("Ollama__ModelName", config.Ollama.ModelName);
}

builder.Build().Run();
