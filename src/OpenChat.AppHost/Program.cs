using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var config = builder.Configuration;

var ollama = builder.AddOllama("ollama")
                    .WithImageTag("0.5.13")
                    .WithDataVolume()
                    // .WithContainerRuntimeArgs("--gpus=all")
                    .AddModel("phi4-mini");

var apiapp = builder.AddProject<OpenChat_ApiApp>("apiapp")
                    .WithReference(ollama)
                    .WithEnvironment("SemanticKernel__ServiceId", config["SemanticKernel:ServiceId"]!)
                    .WithEnvironment("GitHub__Models__ModelId", config["GitHub:Models:ModelId"]!)
                    .WaitFor(ollama);

var webapp = builder.AddProject<OpenChat_PlaygroundApp>("playgroundapp")
                    .WithReference(apiapp)
                    .WaitFor(apiapp);

builder.Build().Run();
