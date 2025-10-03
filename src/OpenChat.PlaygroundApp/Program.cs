using Microsoft.Extensions.AI;

using OpenChat.PlaygroundApp.Abstractions;
using OpenChat.PlaygroundApp.Components;

var builder = WebApplication.CreateBuilder(args);

var config = builder.Configuration;
var settings = ArgumentOptions.Parse(config, args);
if (settings.Help == true)
{
    ArgumentOptions.DisplayHelp();
    return;
}

var section = config.GetSection(settings.ConnectorType.ToString());
settings.Model = settings.ConnectorType switch
{
    OpenChat.PlaygroundApp.Connectors.ConnectorType.AzureAIFoundry => section.GetValue<string>("DeploymentName"),
    OpenChat.PlaygroundApp.Connectors.ConnectorType.FoundryLocal => section.GetValue<string>("Alias"),
    _ => section.GetValue<string>("Model")
};

builder.Services.AddSingleton(settings);

builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents();

var chatClient = await LanguageModelConnector.CreateChatClientAsync(settings);

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

await app.RunAsync();
