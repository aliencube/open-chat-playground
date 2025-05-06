using Microsoft.Extensions.DependencyInjection;

using OpenChat.Common.Configurations;

using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var config = AppSettings.Parse(builder.Configuration, args);
if (config.Help == true)
{
    Console.WriteLine("Usage: dotnet run -- [options]");
    return;
}
if (config.LLM.ProviderType == LLMProviderType.Undefined)
{
    Console.WriteLine("Usage: dotnet run -- [options]");
    return;
}

builder.Services.AddSingleton(config);

// Add OpenAI
var openai = default(IResourceBuilder<IResourceWithConnectionString>);
if (config.LLM.ProviderType == LLMProviderType.OpenAI)
{
    openai = builder.AddConnectionString(config.LLM.ProviderType.ToString().ToLowerInvariant());
}

// Add Ollama or Hugging Face
var ollama = default(IResourceBuilder<OllamaResource>);
var model = default(IResourceBuilder<OllamaModelResource>);
if (config.LLM.ProviderType == LLMProviderType.Ollama || config.LLM.ProviderType == LLMProviderType.HuggingFace)
{
    ollama = builder.AddOllama(config.LLM.ProviderType.ToString().ToLowerInvariant())
                    .WithImageTag(config.Ollama.ImageTag)
                    .WithDataVolume();
    if (config.Ollama.UseGPU == true)
    {
        ollama.WithContainerRuntimeArgs("--gpus=all");
    }
    model = config.LLM.ProviderType == LLMProviderType.Ollama
            ? ollama.AddModel(config.Ollama.DeploymentName, config.Ollama.ModelName)
            : ollama.AddHuggingFaceModel(config.Ollama.DeploymentName, config.Ollama.ModelName);
}

// Add the playground web application
var webapp = builder.AddProject<OpenChat_PlaygroundApp>("playgroundapp")
                    .WithEnvironment("LLM__Provider", config.LLM.ProviderType.ToString().ToLowerInvariant());
if (config.LLM.ProviderType == LLMProviderType.OpenAI)
{
    webapp.WithReference(openai!)
          .WaitFor(openai!)
          .WithEnvironment("OpenAI__DeploymentName", config.OpenAI.DeploymentName);
}
if (config.LLM.ProviderType == LLMProviderType.Ollama || config.LLM.ProviderType == LLMProviderType.HuggingFace)
{
    webapp.WithReference(model!)
          .WaitFor(model!)
          .WithEnvironment("Ollama__ImageTag", config.Ollama.ImageTag)
          .WithEnvironment("Ollama__UseGPU", config.Ollama.UseGPU.ToString().ToLowerInvariant())
          .WithEnvironment("Ollama__DeploymentName", config.Ollama.DeploymentName)
          .WithEnvironment("Ollama__ModelName", config.Ollama.ModelName);
}

builder.Build().Run();
