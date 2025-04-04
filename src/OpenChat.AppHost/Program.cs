using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var config = builder.Configuration;

var hface = builder.AddOllama("hface")
                   .WithImageTag("0.5.13")
                   .WithDataVolume()
                   // .WithContainerRuntimeArgs("--gpus=all")
                   .AddHuggingFaceModel("exaone", "LGAI-EXAONE/EXAONE-3.5-7.8B-Instruct-GGUF");

var apiapp = builder.AddProject<OpenChat_ApiApp>("apiapp")
                    .WithReference(hface)
                    .WithEnvironment("SemanticKernel__ServiceId", config["SemanticKernel:ServiceId"]!)
                    .WithEnvironment("GitHub__Models__ModelId", config["GitHub:Models:ModelId"]!)
                    .WaitFor(hface);

var webapp = builder.AddProject<OpenChat_PlaygroundApp>("playgroundapp")
                    .WithReference(apiapp)
                    .WaitFor(apiapp);

builder.Build().Run();
