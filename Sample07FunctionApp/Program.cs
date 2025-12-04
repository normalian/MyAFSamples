using Azure.AI.OpenAI;
using Azure.Identity;
using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Hosting.AzureFunctions;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Hosting;

class Program
{
    // Run the following command to test this agent after starting the function app:
    // Invoke-RestMethod -Uri http://localhost:7144/api/agents/TranslateAgent/run  -Method POST   -Headers @{ "Content-Type" = "text/plain" }   -Body "私を英語にしてください" 
    public static async Task Main(string[] args)
    {
        string endpoint = "https://your-openai-endpoint.openai.azure.com/";
        string deploymentName = "gpt-4.1-mini";
        var chatClient = new AzureOpenAIClient(new Uri(endpoint), new AzureCliCredential()).GetChatClient(deploymentName).AsIChatClient();

        AIAgent agent = new ChatClientAgent(
            chatClient,
            $"You are a translation assistant that translates the provided text to English.",
            "TranslateAgent",
            "This Agent is used to translate into English."
        );

        using IHost app = FunctionsApplication
            .CreateBuilder(args)
            .ConfigureFunctionsWebApplication()
            .ConfigureDurableAgents(options => options.AddAIAgent(agent))
            .Build();
        await app.RunAsync();
    }
}