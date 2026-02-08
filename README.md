# MyAFSamples

A collection of sample applications demonstrating the **Microsoft.Agents.AI** framework with Azure OpenAI. These samples showcase various AI agent capabilities, from basic chat interactions to advanced features like function calling, structured outputs, workflows, and Azure Functions integration.

## Prerequisites

- .NET 9.0 SDK
- Azure OpenAI Service endpoint and deployment
- Azure CLI (for authentication)

## Environment Variables

All samples require the following environment variables:

```powershell
$env:AZURE_OPENAI_ENDPOINT = "https://your-openai-endpoint.openai.azure.com/"
$env:AZURE_OPENAI_DEPLOYMENT_NAME = "gpt-4.1-mini"  # or your deployment name
```

## Samples Overview

### Sample01ConsoleApp - Basic AI Agent
**Location**: `Sample01ConsoleApp/`

A simple console application demonstrating the basic setup of an AI agent.

**Key Features**:
- Basic agent creation using `AzureOpenAIClient`
- Azure CLI credential authentication
- Simple prompt/response interaction

**What it does**: Creates an agent that tells jokes about Seattle beer.

**Running**:
```powershell
cd Sample01ConsoleApp
dotnet run
```

---

### Sample02AgentThreadConsoleApp - Multi-Thread Conversations
**Location**: `Sample02AgentThreadConsoleApp/`

Demonstrates managing multiple conversation threads with a single AI agent.

**Key Features**:
- Multiple independent conversation threads
- Context-aware responses based on thread history
- Thread-specific conversation state

**What it does**: Simulates a Seattle tour guide that maintains separate conversations for different family groups, providing tailored recommendations based on group composition.

**Running**:
```powershell
cd Sample02AgentThreadConsoleApp
dotnet run
```

---

### Sample03AgentThreadConsoleApp - Thread Serialization
**Location**: `Sample03AgentThreadConsoleApp/`

Shows how to persist and resume conversation threads.

**Key Features**:
- Thread serialization to JSON
- Thread deserialization and resumption
- Conversation state persistence across sessions

**What it does**: Demonstrates saving a conversation thread to a file and resuming it later, maintaining context continuity.

**Running**:
```powershell
cd Sample03AgentThreadConsoleApp
dotnet run
```

---

### Sample04FunctionCallingConsoleApp - Function Calling
**Location**: `Sample04FunctionCallingConsoleApp/`

Demonstrates how AI agents can call C# functions to retrieve information.

**Key Features**:
- Function calling with `AIFunctionFactory`
- Function descriptions and parameter annotations
- Dynamic function invocation

**What it does**: Creates an agent with a custom function `GetHokkaidoCommunity` that provides information about technical communities based on technology names.

**Running**:
```powershell
cd Sample04FunctionCallingConsoleApp
dotnet run
```

---

### Sample05StructuredOutputConsoleApp - Structured JSON Output
**Location**: `Sample05StructuredOutputConsoleApp/`

Shows how to get structured, typed responses from AI agents.

**Key Features**:
- JSON schema-based responses
- Strong typing with C# classes
- Automatic data extraction and structuring

**What it does**: Extracts structured person information (name, age, hobby) from natural language text.

**Running**:
```powershell
cd Sample05StructuredOutputConsoleApp
dotnet run
```

---

### Sample06DevUIApp - Multi-Agent Workflow with Dev UI
**Location**: `Sample06DevUIApp/`

Advanced sample demonstrating multiple agents, workflows, and a development UI.

**Key Features**:
- Multiple specialized agents (Spanish, French, Japanese translators)
- Sequential workflows
- Function calling integration
- Web-based development UI
- OpenAI-compatible API endpoints

**What it does**: 
- Creates translation agents for different languages
- Builds a sequential workflow that processes text through multiple translation agents
- Provides a community lookup agent with function calling
- Exposes agents through a web UI and OpenAI-compatible endpoints

**Running**:
```powershell
cd Sample06DevUIApp
dotnet run
```

Then navigate to the Dev UI (typically `http://localhost:5000` or as shown in console output).

---

### Sample07FunctionApp - Basic Azure Functions Integration
**Location**: `Sample07FunctionApp/`

A simple Azure Functions app demonstrating the basic structure.

**Key Features**:
- HTTP-triggered function
- Azure Functions Worker runtime
- Basic logging

**What it does**: Provides a simple HTTP endpoint that returns a welcome message.

**Running**:
```powershell
cd Sample07FunctionApp
func start  # or dotnet run
```

---

### Sample08MCPServerFunctionApp - MCP Server as Azure Function
**Location**: `Sample08MCPServerFunctionApp/`

Advanced sample showing how to host an AI agent as an MCP (Model Context Protocol) server using Azure Functions.

**Key Features**:
- Durable Agent hosting in Azure Functions
- MCP tool trigger integration
- HTTP endpoint disabled (MCP-only access)
- Custom business logic agent (Normalian project naming conventions)

**What it does**: Hosts an AI agent that provides guidance on class naming conventions for the "Normalian project," accessible via MCP protocol.

**Running**:
```powershell
cd Sample08MCPServerFunctionApp
func start  # or dotnet run
```

**Testing the MCP Server**:
Configure your MCP client to connect to the function app endpoint and invoke the tools.

---

### Sample09M365Agent - Microsoft Teams Agent App
**Location**: `Sample09M365Agent/`

A Microsoft Teams agent application that combines adaptive cards with Teams app capabilities.

**Key Features**:
- Adaptive Card-based interactive UI
- Teams app integration
- Agent-powered conversational experience

**What it does**: Demonstrates how to build an agent application for Microsoft Teams using adaptive cards for rich, interactive user experiences.

For detailed setup instructions and configuration, please refer to the [Sample09M365Agent README](Sample09M365Agent/README.md).

---

## Key Dependencies

All projects use:
- **Microsoft.Agents.AI** (v1.0.0-preview.251125.1) - Core agent framework
- **Microsoft.Extensions.AI** (v10.0.1) - AI extensions
- **Azure.AI.OpenAI** (v2.7.0-beta.2) - Azure OpenAI client
- **Azure.Identity** (v1.18.0-beta.2) - Azure authentication

Additional dependencies by project:
- **Sample06DevUIApp**: `Microsoft.Agents.AI.DevUI`, `Microsoft.Agents.AI.Workflows`
- **Sample07/08FunctionApp**: `Microsoft.Azure.Functions.Worker`, `Microsoft.Agents.AI.Hosting.AzureFunctions`

## Authentication

All samples use **Azure CLI credential** for authentication:

```bash
az login
```

Ensure your Azure CLI is logged in with an account that has access to the Azure OpenAI Service.

## Solution Structure

The repository includes a Visual Studio solution file (`MyAFSamples.sln`) that contains all sample projects for easy navigation and building.

## Learning Path

Recommended order to explore the samples:

1. **Sample01** - Understand basic agent setup
2. **Sample02** - Learn about conversation threads
3. **Sample03** - Explore thread persistence
4. **Sample04** - Add function calling capabilities
5. **Sample05** - Work with structured outputs
6. **Sample06** - Build complex multi-agent workflows
7. **Sample07** - Deploy to Azure Functions (basic)
8. **Sample08** - Create MCP-enabled agents in Azure Functions
9. **Sample09** - Build Teams agent apps with adaptive cards

## Resources

- [Microsoft.Agents.AI Documentation](https://learn.microsoft.com/azure/ai-services/)
- [Azure OpenAI Service](https://azure.microsoft.com/products/ai-services/openai-service)
- [Azure Functions](https://azure.microsoft.com/services/functions/)

