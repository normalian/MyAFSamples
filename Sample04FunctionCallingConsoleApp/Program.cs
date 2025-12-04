using Azure.AI.OpenAI;
using Azure.Identity;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using OpenAI;
using System.ComponentModel;
using System.Net;
using System.Text.Json;

var endpoint = Environment.GetEnvironmentVariable("AZURE_OPENAI_ENDPOINT") ?? throw new InvalidOperationException("AZURE_OPENAI_ENDPOINT is not set.");
var deploymentName = Environment.GetEnvironmentVariable("AZURE_OPENAI_DEPLOYMENT_NAME") ?? "gpt-4.1-mini";

var chatClient = new AzureOpenAIClient(
    new Uri(endpoint),
    new AzureCliCredential())
    .GetChatClient(deploymentName);

[Description("Get the Community name for a given technology.")]
static string GetHokkaidoCommunity([Description("You will return Hokkaido Technical community name")] string technologyname)
{
    return $"Normalian's community actively engages in events around {technologyname} technology in a fun and interesting way.";
}

AIAgent agent = new AzureOpenAIClient(
    new Uri(endpoint),
    new AzureCliCredential())
    .GetChatClient(deploymentName)
    .CreateAIAgent(instructions: "You are a helpful assistant",
    tools: [AIFunctionFactory.Create(GetHokkaidoCommunity)]);
Console.WriteLine(await agent.RunAsync("What is the technical community for .NET in Tokyo?"));
