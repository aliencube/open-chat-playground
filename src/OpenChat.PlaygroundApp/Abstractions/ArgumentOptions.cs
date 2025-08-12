using OpenChat.PlaygroundApp.Configurations;
using OpenChat.PlaygroundApp.Connectors;

namespace OpenChat.PlaygroundApp.Abstractions;

/// <summary>
/// This represents the base argument options settings entity for all arguments options to inherit.
/// </summary>
public abstract class ArgumentOptions
{
    /// <summary>
    /// Gets or sets the connector type to use.
    /// </summary>
    public ConnectorType ConnectorType { get; set; }

    /// <summary>
    /// Gets or sets the value indicating whether to display help information or not.
    /// </summary>
    public bool Help { get; set; }

    /// <summary>
    /// Verifies the connector type from the configuration and command line arguments.
    /// </summary>
    /// <param name="config"><see cref="IConfiguration"/> instance.</param>
    /// <param name="args">List of arguments from the command line.</param>
    /// <returns>The verified <see cref="ConnectorType"/> value.</returns>
    public static ConnectorType VerifyConnectorType(IConfiguration config, string[] args)
    {
        var connectorType = Enum.TryParse<ConnectorType>(config["ConnectorType"], ignoreCase: true, out var result) ? result : ConnectorType.Unknown;
        for (var i = 0; i < args.Length; i++)
        {
            if (string.Equals(args[i], "--connector-type", StringComparison.InvariantCultureIgnoreCase) ||
                string.Equals(args[i], "-c", StringComparison.InvariantCultureIgnoreCase))
            {
                if (i + 1 < args.Length && Enum.TryParse<ConnectorType>(args[i + 1], ignoreCase: true, out result))
                {
                    connectorType = result;
                }
                break;
            }
        }

        return connectorType;
    }

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

        var connectorType = VerifyConnectorType(config, args);
        if (connectorType == ConnectorType.Unknown)
        {
            settings.ConnectorType = connectorType;
            settings.Help = true;

            return settings;
        }

        var expectedName = connectorType + "ArgumentOptions";

        var assembly = typeof(ArgumentOptions).Assembly;
        var optionsType = (assembly?.GetTypes()
                                   .SingleOrDefault(t => typeof(ArgumentOptions).IsAssignableFrom(t) &&
                                                         string.Equals(t.Name, expectedName, StringComparison.InvariantCultureIgnoreCase)))
                          ?? throw new InvalidOperationException($"Options type '{expectedName}' not found for connector type '{connectorType}'.");

        var options = (ArgumentOptions)Activator.CreateInstance(optionsType)!;
        options.ConnectorType = connectorType;

        options.ParseOptions(config, args);

        for (var i = 0; i < args.Length; i++)
        {
            switch (args[i])
            {
                case "--connector-type":
                case "-c":
                    if (i + 1 < args.Length)
                    {
                        if (Enum.TryParse<ConnectorType>(args[i + 1], ignoreCase: true, out var result))
                        {
                            options.ConnectorType = result;
                            i++;
                        }
                    }
                    break;

                case "--help":
                case "-h":
                default:
                    options.Help = true;
                    break;
            }
        }

        settings.ConnectorType = options.ConnectorType;
        settings.Help = options.ShouldDisplayHelp(optionsType);

        return settings;
    }

    /// <summary>
    /// Displays the help information for the command line arguments.
    /// </summary>
    public void DisplayHelp()
    {
        Console.WriteLine("Usage: [options]");
        Console.WriteLine("Options:");
        Console.WriteLine("  --connector-type|-c  Specify the connector type.");
        Console.WriteLine("  --help|-h            Show this help message.");
    }

    /// <summary>
    /// Parses the command line arguments into the specified options type.
    /// </summary>
    /// <param name="config"><see cref="IConfiguration"/> instance.</param>
    /// <param name="args">List of arguments from the command line.</param>
    protected virtual void ParseOptions(IConfiguration config, string[] args)
    {
        return;
    }

    /// <summary>
    /// Determines whether to display help information based on the options provided.
    /// </summary>
    /// <param name="type">The type of the options to parse into.</param>
    /// <returns></returns>
    protected virtual bool ShouldDisplayHelp(Type type)
    {
        return this.ConnectorType == ConnectorType.Unknown || this.Help;
    }
}
