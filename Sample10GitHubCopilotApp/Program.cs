using GitHub.Copilot.SDK;
using Microsoft.Agents.AI;
using Microsoft.Agents.AI.GitHub.Copilot;
using Microsoft.Extensions.AI;

// This program requires following components to run
// GitHub Copilot CLI SDKs https://github.com/github/copilot-sdk
// GitHub Copilot CLI https://docs.github.com/en/copilot/how-tos/copilot-cli/install-copilot-cli

// Initialize the Copilot client and the Agent, then establish a new session
using var copilotClient = new CopilotClient();
var config = new SessionConfig { Model = "claude-3.7-sonnet" };
var agent = new GitHubCopilotAgent(copilotClient, config);
await agent.CreateSessionAsync();

// Load file contents
string sourceFilePath = "Program.cs";
string rulesFilePath = "rule.md";

byte[] sourceBytes = await File.ReadAllBytesAsync(sourceFilePath);
byte[] ruleBytes = await File.ReadAllBytesAsync(rulesFilePath);
string mimeType = "text/plain";

var messages = new List<ChatMessage>();
messages.Add(new ChatMessage(ChatRole.System, "You are a code reviewer. Check the provided source code against the provided rules (rule.md)."));

var userMessage = new ChatMessage(ChatRole.User, "Please review the attached source code based on the coding rules in rule.md. List any violations found.");
userMessage.Contents.Add(new DataContent(sourceBytes, mimeType));
userMessage.Contents.Add(new DataContent(ruleBytes, mimeType));
messages.Add(userMessage);

try
{
    Console.WriteLine($"Checking {sourceFilePath} against {rulesFilePath}...");

    var response = await agent.RunAsync(messages);

    Console.WriteLine("--- Review Result ---");
    Console.WriteLine(response);
}
catch (Exception ex)
{
    Console.WriteLine($"Error: {ex.Message}");
    if (ex.InnerException != null)
        Console.WriteLine($"Inner: {ex.InnerException.Message}");
}