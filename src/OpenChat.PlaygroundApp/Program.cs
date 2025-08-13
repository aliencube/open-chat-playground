using System.ClientModel;

using Microsoft.Extensions.AI;

using OpenChat.PlaygroundApp.Components;

using OpenAI;
using OpenChat.PlaygroundApp.Abstractions;

var builder = WebApplication.CreateBuilder(args);

var config = builder.Configuration;
var settings = ArgumentOptions.Parse(config, args);
if (settings.Help == true)
{
    ArgumentOptions.DisplayHelp();
    return;
}

builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents();

var chatClient = LanguageModelConnector.CreateChatClient(settings);

// // You will need to set the endpoint and key to your own values
// // You can do this using Visual Studio's "Manage User Secrets" UI, or on the command line:
// //   cd this-project-directory
// //   dotnet user-secrets set GitHubModels:Token YOUR-GITHUB-TOKEN
// var credential = new ApiKeyCredential(builder.Configuration["GitHubModels:Token"] ?? throw new InvalidOperationException("Missing configuration: GitHubModels:Token. See the README for details."));
// var openAIOptions = new OpenAIClientOptions()
// {
//     Endpoint = new Uri("https://models.github.ai/inference")
// };

// var ghModelsClient = new OpenAIClient(credential, openAIOptions);
// var chatClient = ghModelsClient.GetChatClient("openai/gpt-4o").AsIChatClient();

builder.Services.AddChatClient(chatClient)
                .UseFunctionInvocation()
                .UseLogging();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseAntiforgery();

app.UseStaticFiles();

app.MapRazorComponents<App>()
   .AddInteractiveServerRenderMode();

app.Run();
