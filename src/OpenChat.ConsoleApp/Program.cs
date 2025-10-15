using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using OpenChat.ConsoleApp.Clients;
using OpenChat.ConsoleApp.Options;
using OpenChat.ConsoleApp.Services;

var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();
var settings = ArgumentOptions.Parse(config, args);
if (settings.Help == true)
{
    ArgumentOptions.DisplayHelp();
    return;
}

var host = Host.CreateDefaultBuilder(args)
               .UseConsoleLifetime()
               .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.AddConsole();
                    logging.SetMinimumLevel(LogLevel.Warning);
                })
               .ConfigureServices(services =>
                {
                    services.AddSingleton(settings);
                    services.AddHttpClient<IApiClient, ApiClient>(client =>
                    {
                        client.BaseAddress = new Uri(settings.ApiApp.Endpoint!);
                    });
                    services.AddSingleton<IPlaygroundService, PlaygroundService>();
                })
               .Build();

var service = host.Services.GetRequiredService<IPlaygroundService>();
await service.RunAsync();
