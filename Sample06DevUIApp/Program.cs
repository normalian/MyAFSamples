using Azure.AI.OpenAI;
using Azure.Identity;
using Microsoft.Agents.AI;
using Microsoft.Agents.AI.DevUI;
using Microsoft.Agents.AI.Hosting;
using Microsoft.Agents.AI.Workflows;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel;

var endpoint = Environment.GetEnvironmentVariable("AZURE_OPENAI_ENDPOINT") ?? throw new InvalidOperationException("AZURE_OPENAI_ENDPOINT is not set.");
var deploymentName = Environment.GetEnvironmentVariable("AZURE_OPENAI_DEPLOYMENT_NAME") ?? "gpt-4.1-mini";

var chatClient = new AzureOpenAIClient(new Uri(endpoint), new AzureCliCredential()).GetChatClient(deploymentName).AsIChatClient();
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddChatClient(chatClient);

builder.AddAIAgent(name: "SpanishAgent", instructions: "You are a translation assistant that translates the provided text to Spanish.");
builder.AddAIAgent(name: "FrenchAgent", instructions: "You are a translation assistant that translates the provided text to French.");
builder.AddAIAgent(name: "JapaneseAgent", instructions: "You are a translation assistant that translates the provided text to Japanese.");

[Description("Get the Community name for a given technology.")]
static string GetTokyoCommunity([Description("You will return Tokyo Technical community name")] string technologyname)
{
    return $"Normalian's community actively engages in events around {technologyname} technology in a fun and interesting way.";
}
builder.AddAIAgent(name: "communityAgent",
    instructions: "You are a helpful assistant")
    .WithAITool(AIFunctionFactory.Create(GetTokyoCommunity, name: "getTokyoCommunity"));

builder.AddWorkflow("review-workflow", (sp, key) =>
{
    var frenchAgent = sp.GetRequiredKeyedService<AIAgent>("FrenchAgent");
    var spanishAgent = sp.GetRequiredKeyedService<AIAgent>("SpanishAgent");
    var japaneseAgent = sp.GetRequiredKeyedService<AIAgent>("JapaneseAgent");
    return AgentWorkflowBuilder.BuildSequential(workflowName: key, agents: [frenchAgent, spanishAgent, japaneseAgent]);
}).AddAsAIAgent();

builder.Services.AddOpenAIResponses();
builder.Services.AddOpenAIConversations();

var app = builder.Build();
app.MapOpenAIResponses();
app.MapOpenAIConversations();
app.MapDevUI();
app.Run();
