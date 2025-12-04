using Azure.AI.OpenAI;
using Azure.Identity;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using System.Net;

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

AgentThread thread2 = agent.GetNewThread();
Console.WriteLine(await agent.RunAsync(
    new Microsoft.Extensions.AI.ChatMessage(ChatRole.User,
    "’m a 41-year-old married man. I want a 2-night hot-spring trip with my wife and our 10-year-old daughter, and I want to make sure the child doesn’t get bored."), thread2));
Console.WriteLine("\n\n\n");

Console.WriteLine(await agent.RunAsync(
    new Microsoft.Extensions.AI.ChatMessage(ChatRole.User,
    "Two women of the same age have been added for our trip. We also want stylish, Instagrammable food."), thread1));
