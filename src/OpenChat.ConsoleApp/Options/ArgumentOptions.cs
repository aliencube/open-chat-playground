using Microsoft.Extensions.Configuration;

using OpenChat.ConsoleApp.Configurations;

namespace OpenChat.ConsoleApp.Options;

/// <summary>
/// This represents the base argument options settings entity for all arguments options to inherit.
/// </summary>
public class ArgumentOptions
{
    private const string ENDPOINT_ARGUMENT = "--endpoint";
    private const string HELP_ARGUMENT = "--help";
    private const string HELP_ARGUMENT_IN_SHORT = "-h";

    /// <summary>
    /// Parses the command line arguments into the specified options type.
    /// </summary>
    /// <param name="config"><see cref="IConfiguration"/> instance.</param>
    /// <param name="args">List of arguments from the command line.</param>
    /// <returns>The parsed options.</returns>
    public static AppSettings Parse(IConfiguration config, string[] args)
    {
        var settings = new AppSettings();
        config.Bind(settings);

        for (var i = 0; i < args.Length; i++)
        {
            switch (args[i])
            {
                case ENDPOINT_ARGUMENT:
                    if (i + 1 < args.Length)
                    {
                        settings.ApiApp.Endpoint = args[++i];
                    }
                    break;

                case HELP_ARGUMENT:
                case HELP_ARGUMENT_IN_SHORT:
                default:
                    settings.Help = true;
                    break;
            }
        }

        return settings;
    }

    /// <summary>
    /// Displays the help information for the command line arguments.
    /// </summary>
    public static void DisplayHelp()
    {
        var foregroundColor = Console.ForegroundColor;

        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("OpenChat Playground");
        Console.ForegroundColor = foregroundColor;

        Console.WriteLine("Usage: [options]");
        Console.WriteLine();
        Console.WriteLine("Options:");
        Console.WriteLine($"  {ENDPOINT_ARGUMENT}  The API endpoint. Default is http://localhost:5280");
        Console.WriteLine($"  {HELP_ARGUMENT}|{HELP_ARGUMENT_IN_SHORT}   Show this help message.");
    }
}
