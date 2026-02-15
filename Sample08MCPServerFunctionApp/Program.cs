using Azure.AI.OpenAI;
using Azure.Identity;
using Microsoft.Agents.AI;
using Microsoft.Agents.AI.DurableTask;
using Microsoft.Agents.AI.Hosting.AzureFunctions;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Hosting;

class Program
{
    public static async Task Main(string[] args)
    {
        var endpoint = Environment.GetEnvironmentVariable("AZURE_OPENAI_ENDPOINT") ?? throw new InvalidOperationException("AZURE_OPENAI_ENDPOINT is not set.");
        var deploymentName = Environment.GetEnvironmentVariable("AZURE_OPENAI_DEPLOYMENT_NAME") ?? "gpt-4.1-mini";
        AzureOpenAIClient client = new AzureOpenAIClient(new Uri(endpoint), new AzureCliCredential());

        AIAgent agent = client.GetChatClient(deploymentName).AsIChatClient().AsAIAgent(
            instructions: @"In this Normalian project, to facilitate management, class names must follow naming conventions based on business categories.
            Class names according to business categories are provided by the `GenerateClassNamingConvention` tool.
            Please define classes using the names provided here. Do not define classes with any other names.

            If you are unsure about the business category from the class name, use the `DetermineBusinessCategory` tool to obtain the business category.",
            name: "NormalianProjectAdvisor",
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