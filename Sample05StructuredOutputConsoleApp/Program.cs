using Azure.AI.OpenAI;
using Azure.Identity;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using System.ComponentModel;
using System.Net;
using System.Text.Json.Serialization;

var endpoint = Environment.GetEnvironmentVariable("AZURE_OPENAI_ENDPOINT") ?? throw new InvalidOperationException("AZURE_OPENAI_ENDPOINT is not set.");
var deploymentName = Environment.GetEnvironmentVariable("AZURE_OPENAI_DEPLOYMENT_NAME") ?? "gpt-4.1-mini";

AIAgent agent = new AzureOpenAIClient(
    new Uri(endpoint),
    new AzureCliCredential())
    .GetChatClient(deploymentName)
    .AsIChatClient()
    .CreateAIAgent(new ChatClientAgentOptions(name: "HumanAssistant",
        instructions: "You are a helpful assistant. Predict each data even roughly from input as possible.")
    {
        ChatOptions = new()
        {
            ResponseFormat = ChatResponseFormat.ForJsonSchema<PersonInfo>()
        }
    });

var human = await agent.RunAsync("I’m Naoki, a middle-aged guy whose hobby is playing Gundam games.");
Console.WriteLine($"Here is the structured data extracted from the text > {human}");

[Description("Information about a person including their name, age, and Hobby")]
public class PersonInfo
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("age")]
    public int? Age { get; set; }

    [JsonPropertyName("Hobby")]
    public string? Hobby { get; set; }
}