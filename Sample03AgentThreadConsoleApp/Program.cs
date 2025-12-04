using Azure.AI.OpenAI;
using Azure.Identity;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using System.Net;
using System.Text.Json;

var endpoint = Environment.GetEnvironmentVariable("AZURE_OPENAI_ENDPOINT") ?? throw new InvalidOperationException("AZURE_OPENAI_ENDPOINT is not set.");
var deploymentName = Environment.GetEnvironmentVariable("AZURE_OPENAI_DEPLOYMENT_NAME") ?? "gpt-4.1-mini";

var chatClient = new AzureOpenAIClient(
    new Uri(endpoint),
    new AzureCliCredential())
    .GetChatClient(deploymentName);

ChatClientAgentOptions agentOptions = new ChatClientAgentOptions
{
    ChatOptions = new ChatOptions()
    {
        Instructions = "You are a tour guide with deep knowledge of daily life in Seattle, Washington State. Please tailor your suggestions to the family composition. In every reply, always include the group composition, and keep your response within a total of 6 lines.",
    }
};

AIAgent agent = chatClient.AsIChatClient().CreateAIAgent(agentOptions);
AgentThread thread1 = agent.GetNewThread();
Console.WriteLine(await agent.RunAsync(
    new Microsoft.Extensions.AI.ChatMessage(ChatRole.User,
    "I’m a 25-year-old single male. I’m planning a 3-night hot-spring trip with three friends of the same age."), thread1));
Console.WriteLine("\n\n\n");

// Serialize the thread to JSON and save it to a temporary file
JsonElement serializedThread = thread1.Serialize();
string tempFilePath = Path.GetTempFileName();
await File.WriteAllTextAsync(tempFilePath, JsonSerializer.Serialize(serializedThread));

// Later, reload the thread from the temporary file
JsonElement reloadedSerializedThread = JsonElement.Parse(await File.ReadAllTextAsync(tempFilePath));
AgentThread resumedThread = agent.DeserializeThread(reloadedSerializedThread);

Console.WriteLine(await agent.RunAsync(
    new Microsoft.Extensions.AI.ChatMessage(ChatRole.User,
    "Two women of the same age have been added for our trip. We also want stylish, Instagrammable food."),
    resumedThread));
Console.WriteLine("End!");
