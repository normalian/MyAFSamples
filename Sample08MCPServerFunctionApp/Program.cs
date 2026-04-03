using Azure.AI.OpenAI;
using Azure.Identity;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using Microsoft.Agents.AI.Hosting.AzureFunctions;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

class Program
{
    public static async Task Main(string[] args)
    {
        var endpoint = Environment.GetEnvironmentVariable("AZURE_OPENAI_ENDPOINT") ?? throw new InvalidOperationException("AZURE_OPENAI_ENDPOINT is not set.");
        var deploymentName = Environment.GetEnvironmentVariable("AZURE_OPENAI_DEPLOYMENT_NAME") ?? "gpt-4.1-mini";
        AzureOpenAIClient client = new AzureOpenAIClient(new Uri(endpoint), new DefaultAzureCredential());

        // Put following mcp.json file in the root of your function app to enable MCP server. The url should be your function app url plus "/runtime/webhooks/mcp/sse".
        // {
        //        "servers": {
        //        "mine-mcp-server-local": {
        //          "type": "sse",
        //              "url": "https://your-appservice-name.azurewebsites.net/runtime/webhooks/mcp/sse"
        //         }
        //    }
        // }

        AIAgent agent = client.GetChatClient(deploymentName).AsIChatClient().AsAIAgent(
            instructions: @"In this Normalian project, to facilitate management, class names must follow naming conventions based on business categories.
            Class names according to business categories are provided by the `GenerateClassNamingConvention` tool.
            Please define classes using the names provided here. Do not define classes with any other names.

            If you are unsure about the business category from the class name, use the `DetermineBusinessCategory` tool to obtain the business category.",
            name: "NormalianMyProjectAdvisor",
            description: "This tool describes naming rule overview of the Normalian project.");

        using IHost app = FunctionsApplication
            .CreateBuilder(args)
            .ConfigureFunctionsWebApplication()
            .ConfigureDurableAgents(options =>
            {
                options.AddAIAgent(agent, enableHttpTrigger: false, enableMcpToolTrigger: true);
            })
            .Build();
        app.Run();
    }
}