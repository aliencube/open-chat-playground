using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var config = builder.Configuration;

var openai = builder.AddConnectionString("openai");
var ollama = builder.AddOllama("ollama")
                    .WithImageTag("0.5.13")
                    .WithDataVolume()
                    // .WithContainerRuntimeArgs("--gpus=all")
                    // .WithOpenWebUI()
                    .AddModel("phi4-mini");
var hface = builder.AddOllama("hface")
                   .WithImageTag("0.5.13")
                   .WithDataVolume()
                   // .WithContainerRuntimeArgs("--gpus=all")
                //    .WithOpenWebUI()
                   .AddHuggingFaceModel("exaone", "LGAI-EXAONE/EXAONE-3.5-7.8B-Instruct-GGUF");

var apiapp = builder.AddProject<OpenChat_ApiApp>("apiapp")
                    .WithReference(openai)
                    .WithReference(ollama)
                    .WithReference(hface)
                    .WithEnvironment("SemanticKernel__ServiceId", config["SemanticKernel:ServiceId"]!)
                    .WithEnvironment("GitHub__Models__ModelId", config["GitHub:Models:ModelId"]!)
                    .WaitFor(ollama)
                    .WaitFor(hface);

var webapp = builder.AddProject<OpenChat_PlaygroundApp>("playgroundapp")
                    .WithReference(apiapp)
                    .WaitFor(apiapp);

builder.Build().Run();
