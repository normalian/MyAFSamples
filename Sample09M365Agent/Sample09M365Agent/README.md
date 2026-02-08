# Overview of the Adaptive Card Survey Agent

This template demonstrates an AI agent that helps users take surveys using adaptive cards. The agent can interact with users in Teams to collect survey information (age, gender, food preferences, hobbies, etc.) through an intuitive conversational interface powered by adaptive cards.

The app template is built using the Microsoft 365 Agents SDK and Semantic Kernel, which provides the capabilities to build AI-based applications with structured outputs and adaptive card responses.

## Features

- **Conversational Survey Experience**: Users can request surveys through natural language
- **Adaptive Card Integration**: Survey forms are presented using adaptive cards for a rich interactive experience
- **Structured AI Responses**: Leverages JSON schema-based responses for reliable data handling
- **Azure OpenAI Integration**: Powered by GPT models for intelligent conversation

## Quick Start

**Prerequisites**
> To run the Adaptive Card Survey Agent template in your local dev machine, you will need:
>
> - [Azure OpenAI](https://aka.ms/oai/access) resource with a deployed GPT model (e.g., gpt-4, gpt-4.1-mini)
> - Visual Studio 2022 or later with .NET 9 SDK

### Debug agent in Microsoft 365 Agents Playground

1. **Configure Azure OpenAI settings** in `appsettings.Playground.json`:
    ```json
    "Azure": {
      "OpenAIApiKey": "<your-azure-openai-api-key>",
      "OpenAIEndpoint": "<your-azure-openai-endpoint>",
      "OpenAIDeploymentName": "<your-azure-openai-deployment-name>"
    }
    ```
    
    Alternatively, you can configure the `AIServices` section:
    ```json
    "AIServices": {
      "AzureOpenAI": {
        "DeploymentName": "<your-deployment-name>",
        "Endpoint": "<your-azure-openai-endpoint>",
        "ApiKey": "<your-azure-openai-api-key>"
      },
      "UseAzureOpenAI": true
    }
    ```

2. Set `Startup Item` as `Microsoft 365 Agents Playground (browser)`.
![image](https://raw.githubusercontent.com/OfficeDev/TeamsFx/dev/docs/images/visualstudio/debug/switch-to-test-tool.png)

3. Press F5, or select the Debug > Start Debugging menu in Visual Studio.

4. In Microsoft 365 Agents Playground from the launched browser, type messages like:
   - "I want to take a survey"
   - "Show me a survey"
   - "I'd like to provide my profile information"

### Debug agent in Teams Web Client

1. **Configure Azure OpenAI settings** in `appsettings.Development.json`:
    ```json
    "Azure": {
      "OpenAIApiKey": "<your-azure-openai-api-key>",
      "OpenAIEndpoint": "<your-azure-openai-endpoint>",
      "OpenAIDeploymentName": "<your-azure-openai-deployment-name>"
    }
    ```

2. **Configure Bot Service Connection** in `appsettings.Development.json`:
    ```json
    "TokenValidation": {
      "Audiences": {
        "ClientId": "<your-microsoft-entra-app-client-id>"
      }
    },
    "Connections": {
      "BotServiceConnection": {
        "Settings": {
          "ClientId": "<your-microsoft-entra-app-client-id>",
          "ClientSecret": "<your-microsoft-entra-app-client-secret>"
        }
      }
    }
    ```

3. In the debug dropdown menu, select Dev Tunnels > Create A Tunnel (set authentication type to Public) or select an existing public dev tunnel.

4. Right-click the 'Sample09M365Agent' project in Solution Explorer and select **Microsoft 365 Agents Toolkit > Select Microsoft 365 Account**.

5. Sign in to Microsoft 365 Agents Toolkit with a **Microsoft 365 work or school account**.

6. Set `Startup Item` as `Microsoft Teams (browser)`.

7. Press F5, or select Debug > Start Debugging menu in Visual Studio to start your app.
</br>![image](https://raw.githubusercontent.com/OfficeDev/TeamsFx/dev/docs/images/visualstudio/debug/debug-button.png)

8. In the opened web browser, select Add button to install the app in Teams.

9. In the chat bar, type messages like "I want to take a survey" to trigger the survey experience.

## Configuration Files

### Required Configuration Files

| File | Purpose | Required Settings |
|------|---------|------------------|
| `appsettings.Playground.json` | Configuration for local testing in Microsoft 365 Agents Playground | Azure OpenAI credentials (ApiKey, Endpoint, DeploymentName) |
| `appsettings.Development.json` | Configuration for debugging in Teams | Azure OpenAI credentials, Bot Service Connection (ClientId, ClientSecret), Token Validation |
| `appsettings.json` | Base configuration file | General app settings (automatically loaded) |

### Configuration Details

**For Playground Testing:**
- File: `appsettings.Playground.json`
- Required values:
  - `Azure.OpenAIApiKey`: Your Azure OpenAI API key
  - `Azure.OpenAIEndpoint`: Your Azure OpenAI endpoint URL
  - `Azure.OpenAIDeploymentName`: Your GPT model deployment name

**For Teams Debugging:**
- File: `appsettings.Development.json`
- Required values:
  - `Azure.OpenAIApiKey`: Your Azure OpenAI API key
  - `Azure.OpenAIEndpoint`: Your Azure OpenAI endpoint URL
  - `Azure.OpenAIDeploymentName`: Your GPT model deployment name
  - `TokenValidation.Audiences.ClientId`: Your Microsoft Entra app client ID
  - `Connections.BotServiceConnection.Settings.ClientId`: Your Microsoft Entra app client ID
  - `Connections.BotServiceConnection.Settings.ClientSecret`: Your Microsoft Entra app client secret

> **Security Note**: Never commit sensitive information like API keys or client secrets to source control. Use environment variables, user secrets, or Azure Key Vault for production deployments.

## Project Structure

- `Agents/MySurveyAgent.cs`: Main agent implementation with survey logic
- `Models/MySurveyAgentResponse.cs`: Response model for structured outputs
- `Models/MySurveyContent.cs`: Survey content model
- `Agents/AdaptiveCardAIContent.cs`: Adaptive card content handling
- `AFAgentApp.cs`: Agent framework application setup
- `Program.cs`: Application entry point

## How It Works

1. User sends a message requesting a survey
2. The AI agent (powered by Azure OpenAI) interprets the request
3. Agent calls the `ShowSurvey` function when appropriate
4. An adaptive card is generated with survey fields (age, gender, food, hobby)
5. User fills out the survey in the adaptive card
6. Responses are processed by the agent

> For local debugging using Microsoft 365 Agents Toolkit CLI, you need to do some extra steps described in [Set up your Microsoft 365 Agents Toolkit CLI for local debugging](https://aka.ms/teamsfx-cli-debugging).

## Additional information and references

- [Microsoft 365 Agents SDK](https://github.com/microsoft/Agents)
- [Microsoft 365 Agents Toolkit Documentations](https://docs.microsoft.com/microsoftteams/platform/toolkit/teams-toolkit-fundamentals)
- [Microsoft 365 Agents Toolkit CLI](https://aka.ms/teamsfx-toolkit-cli)
- [Microsoft 365 Agents Toolkit Samples](https://github.com/OfficeDev/TeamsFx-Samples)
- [Adaptive Cards Documentation](https://adaptivecards.io/)

## Learn more

New to app development or Microsoft 365 Agents Toolkit? Learn more about app manifests, deploying to the cloud, and more in the documentation 
at https://aka.ms/teams-toolkit-vs-docs.

## Report an issue

Select Visual Studio > Help > Send Feedback > Report a Problem. 
Or, you can create an issue directly in our GitHub repository: 
https://github.com/OfficeDev/TeamsFx/issues.
