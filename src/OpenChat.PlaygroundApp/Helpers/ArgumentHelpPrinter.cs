using OpenChat.PlaygroundApp.Constants;

namespace OpenChat.PlaygroundApp.Helpers;

/// <summary>
/// Provides helper methods for displaying command line argument help information.
/// </summary>
public static class ArgumentHelpPrinter
{
    /// <summary>
    /// Displays the application banner.
    /// </summary>
    public static void DisplayBanner()
    {
        string cyan = "\x1b[38;5;51m";
        string blue = "\x1b[38;5;33m";
        string purple = "\x1b[38;5;141m";
        string pink = "\x1b[38;5;201m";
        string green = "\x1b[38;5;48m";
        string reset = "\x1b[0m";

        Console.OutputEncoding = System.Text.Encoding.UTF8;

        Console.Clear();
        Console.WriteLine($@"{cyan}   ██████╗ ██████╗ ███████╗███╗   ██╗   ██████╗██╗  ██╗ █████╗ ████████╗{reset}");
        Console.WriteLine($@"{blue}  ██╔═══██╗██╔══██╗██╔════╝████╗  ██║  ██╔════╝██║  ██║██╔══██╗╚══██╔══╝{reset}");
        Console.WriteLine($@"{purple}  ██║   ██║██████╔╝█████╗  ██╔██╗ ██║  ██║     ███████║███████║   ██║   {reset}");
        Console.WriteLine($@"{pink}  ██║   ██║██╔═══╝ ██╔══╝  ██║╚██╗██║  ██║     ██╔══██║██╔══██║   ██║   {reset}");
        Console.WriteLine($@"{cyan}  ╚██████╔╝██║     ███████╗██║ ╚████║  ╚██████╗██║  ██║██║  ██║   ██║   {reset}");
        Console.WriteLine($@"{blue}   ╚═════╝ ╚═╝     ╚══════╝╚═╝  ╚═══╝   ╚═════╝╚═╝  ╚═╝╚═╝  ╚═╝   ╚═╝   {reset}");

        Console.WriteLine();

        Console.WriteLine($@"{green}  ██████╗ ██╗      █████╗ ██╗   ██╗   ██████╗ ██████╗  ██████╗ ██╗   ██╗███╗   ██╗██████╗ {reset}");
        Console.WriteLine($@"{green}  ██╔══██╗██║     ██╔══██╗╚██╗ ██╔╝  ██╔════╝ ██╔══██╗██╔═══██╗██║   ██║████╗  ██║██╔══██╗{reset}");
        Console.WriteLine($@"{cyan}  ██████╔╝██║     ███████║ ╚████╔╝   ██║  ███╗██████╔╝██║   ██║██║   ██║██╔██╗ ██║██║  ██║{reset}");
        Console.WriteLine($@"{blue}  ██╔═══╝ ██║     ██╔══██║  ╚██╔╝    ██║   ██║██╔══██╗██║   ██║██║   ██║██║╚██╗██║██║  ██║{reset}");
        Console.WriteLine($@"{purple}  ██║     ███████╗██║  ██║   ██║     ╚██████╔╝██║  ██║╚██████╔╝╚██████╔╝██║ ╚████║██████╔╝{reset}");
        Console.WriteLine($@"{pink}  ╚═╝     ╚══════╝╚═╝  ╚═╝   ╚═╝      ╚═════╝ ╚═╝  ╚═╝ ╚═════╝  ╚═════╝ ╚═╝  ╚═══╝╚═════╝ {reset}");

        Console.WriteLine();
    }

    /// <summary>
    /// Displays the help information for the command line arguments.
    /// </summary>
    public static void DisplayHelp()
    {
        DisplayHelpForAmazonBedrock();
        DisplayHelpForAzureAIFoundry();
        DisplayHelpForGitHubModels();
        DisplayHelpForGoogleVertexAI();
        DisplayHelpForDockerModelRunner();
        DisplayHelpForFoundryLocal();
        DisplayHelpForHuggingFace();
        DisplayHelpForOllama();
        DisplayHelpForAnthropic();
        DisplayHelpForLG();
        DisplayHelpForNaver();
        DisplayHelpForOpenAI();
        DisplayHelpForUpstage();
        Console.WriteLine($"  {ArgumentOptionConstants.Help}|{ArgumentOptionConstants.HelpInShort}            Show this help message.");
    }

    private static void DisplayHelpForAmazonBedrock()
    {
        var foregroundColor = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Magenta;
        Console.WriteLine("  ** Amazon Bedrock: **");
        Console.ForegroundColor = foregroundColor;

        Console.WriteLine($"  {ArgumentOptionConstants.AmazonBedrock.AccessKeyId}     The AWSCredentials Access Key ID.");
        Console.WriteLine($"  {ArgumentOptionConstants.AmazonBedrock.SecretAccessKey} The AWSCredentials Secret Access Key.");
        Console.WriteLine($"  {ArgumentOptionConstants.AmazonBedrock.Region}            The AWS region.");
        Console.WriteLine($"  {ArgumentOptionConstants.AmazonBedrock.ModelId}          The model ID. Default to 'anthropic.claude-sonnet-4-20250514-v1:0'");
        Console.WriteLine();
    }

    private static void DisplayHelpForAzureAIFoundry()
    {
        var foregroundColor = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Magenta;
        Console.WriteLine("  ** Azure AI Foundry: **");
        Console.ForegroundColor = foregroundColor;

        Console.WriteLine($"  {ArgumentOptionConstants.AzureAIFoundry.Endpoint}           The Azure AI Foundry endpoint.");
        Console.WriteLine($"  {ArgumentOptionConstants.AzureAIFoundry.ApiKey}            The Azure AI Foundry API key.");
        Console.WriteLine($"  {ArgumentOptionConstants.AzureAIFoundry.DeploymentName}    The deployment name. Default to 'gpt-4o-mini'");
        Console.WriteLine();
    }

    private static void DisplayHelpForGitHubModels()
    {
        var foregroundColor = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Magenta;
        Console.WriteLine("  ** GitHub Models: **");
        Console.ForegroundColor = foregroundColor;

        Console.WriteLine($"  {ArgumentOptionConstants.GitHubModels.Endpoint}           The endpoint URL. Default to 'https://models.github.ai/inference'");
        Console.WriteLine($"  {ArgumentOptionConstants.GitHubModels.Token}              The GitHub PAT.");
        Console.WriteLine($"  {ArgumentOptionConstants.GitHubModels.Model}              The model name. Default to 'openai/gpt-4o-mini'");
        Console.WriteLine();
    }

    private static void DisplayHelpForGoogleVertexAI()
    {
        var foregroundColor = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Magenta;
        Console.WriteLine("  ** Google Vertex AI: **");
        Console.ForegroundColor = foregroundColor;

        Console.WriteLine("  TBD");
        Console.WriteLine();
    }

    private static void DisplayHelpForDockerModelRunner()
    {
        var foregroundColor = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.DarkYellow;
        Console.WriteLine("  ** Docker Model Runner: **");
        Console.ForegroundColor = foregroundColor;

        Console.WriteLine("  TBD");
        Console.WriteLine();
    }

    private static void DisplayHelpForFoundryLocal()
    {
        var foregroundColor = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.DarkYellow;
        Console.WriteLine("  ** Foundry Local: **");
        Console.ForegroundColor = foregroundColor;

        Console.WriteLine("  TBD");
        Console.WriteLine();
    }

    private static void DisplayHelpForHuggingFace()
    {
        var foregroundColor = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.DarkYellow;
        Console.WriteLine("  ** Hugging Face: **");
        Console.ForegroundColor = foregroundColor;

        Console.WriteLine($"  {ArgumentOptionConstants.HuggingFace.BaseUrl}           The endpoint URL. Default to 'http://localhost:11434'");
        Console.WriteLine($"  {ArgumentOptionConstants.HuggingFace.Model}              The model name. Default to 'hf.co/google/gemma-3-1b-pt-qat-q4_0-gguf'");
        Console.WriteLine();
    }

    private static void DisplayHelpForOllama()
    {
        var foregroundColor = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.DarkYellow;
        Console.WriteLine("  ** Ollama: **");
        Console.ForegroundColor = foregroundColor;

        Console.WriteLine($"  {ArgumentOptionConstants.Ollama.BaseUrl}           The baseURL. Default to 'http://localhost:11434'");
        Console.WriteLine($"  {ArgumentOptionConstants.Ollama.Model}              The model name. Default to 'llama3.2'");
        Console.WriteLine();
    }

    private static void DisplayHelpForAnthropic()
    {
        var foregroundColor = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.DarkGreen;
        Console.WriteLine("  ** Anthropic: **");
        Console.ForegroundColor = foregroundColor;

        Console.WriteLine($"  {ArgumentOptionConstants.Anthropic.ApiKey}            The Anthropic API key.");
        Console.WriteLine($"  {ArgumentOptionConstants.Anthropic.Model}              The Anthropic model name. Default to 'claude-sonnet-4-0'");
        Console.WriteLine();
    }

    private static void DisplayHelpForLG()
    {
        var foregroundColor = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.DarkGreen;
        Console.WriteLine("  ** LG: **");
        Console.ForegroundColor = foregroundColor;

        Console.WriteLine("  TBD");
        Console.WriteLine();
    }

    private static void DisplayHelpForNaver()
    {
        var foregroundColor = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.DarkGreen;
        Console.WriteLine("  ** Naver: **");
        Console.ForegroundColor = foregroundColor;

        Console.WriteLine("  TBD");
        Console.WriteLine();
    }

    private static void DisplayHelpForOpenAI()
    {
        var foregroundColor = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.DarkGreen;
        Console.WriteLine("  ** OpenAI: **");
        Console.ForegroundColor = foregroundColor;

        Console.WriteLine($"  {ArgumentOptionConstants.OpenAI.ApiKey}            The OpenAI API key. (Env: OPENAI_API_KEY)");
        Console.WriteLine($"  {ArgumentOptionConstants.OpenAI.Model}              The OpenAI model name. Default to 'gpt-4.1-mini'");
        Console.WriteLine();
    }

    private static void DisplayHelpForUpstage()
    {
        var foregroundColor = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.DarkGreen;
        Console.WriteLine("  ** Upstage: **");
        Console.ForegroundColor = foregroundColor;

        Console.WriteLine($"  {ArgumentOptionConstants.Upstage.BaseUrl}           The base URL for Upstage API. Default to 'https://api.upstage.ai/v1/solar'");
        Console.WriteLine($"  {ArgumentOptionConstants.Upstage.ApiKey}            The Upstage API key.");
        Console.WriteLine($"  {ArgumentOptionConstants.Upstage.Model}              The model name. Default to 'solar-mini'");
        Console.WriteLine();
    }
}
