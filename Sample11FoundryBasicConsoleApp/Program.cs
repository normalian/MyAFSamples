using Azure.AI.Projects;
using Azure.AI.Projects.OpenAI;
using Azure.Identity;
using Microsoft.Agents.AI;
class Program
{
    static async Task Main()
    {
        // Foundry endpoint
        var endpoint = Environment.GetEnvironmentVariable("MS_FOUNDRY_ENDPOINT") ?? throw new InvalidOperationException("MS_FOUNDRY_ENDPOINT is not set.");
        var agentname = "your-agent-name";
        var version = "1";
        var prompt = "Please introduce yourself.";

        AIProjectClient aiProjectClient = new(new Uri(endpoint), new DefaultAzureCredential());
        AgentVersion agentVersion = aiProjectClient.Agents.GetAgentVersion(agentname, version);

        // You can use an AIAgent with an already created server side agent version.
        AIAgent agent = aiProjectClient.AsAIAgent(agentVersion);
        var options = new AgentRunOptions();
        AgentResponse response = await agent.RunAsync(prompt, null, options);
        Console.WriteLine(response.Text);
    }
}