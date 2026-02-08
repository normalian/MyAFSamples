# Overview of the Adaptive Card Survey Agent template

This template demonstrates an AI agent that helps users take surveys using adaptive cards. Users can interact with the AI agent in Teams to provide survey information such as age, gender, food preferences, and hobbies through an intuitive conversational interface.

The app template is built using the Microsoft 365 Agents SDK and Semantic Kernel, which provides the capabilities to build AI-based applications with structured outputs and adaptive card responses.

![](img/fig01.png) 

## Quick Start

**Prerequisites**
> To run the Adaptive Card Survey Agent template in your local dev machine, you will need:
>
> - [Azure OpenAI](https://aka.ms/oai/access) resource

### Debug agent in Microsoft 365 Agents Playground
1. Ensure your Azure OpenAI settings are filled in `appsettings.Playground.json`.
    ```
    "Azure": {
      "OpenAIApiKey": "<your-azure-openai-api-key>",
      "OpenAIEndpoint": "<your-azure-openai-endpoint>",
      "OpenAIDeploymentName": "<your-azure-openai-deployment-name>"
    }
    ```
1. Set `Startup Item` as `Microsoft 365 Agents Playground (browser)`.
![image](https://raw.githubusercontent.com/OfficeDev/TeamsFx/dev/docs/images/visualstudio/debug/switch-to-test-tool.png)
1. Press F5, or select the Debug > Start Debugging menu in Visual Studio.
1. In Microsoft 365 Agents Playground from the launched browser, type and send messages like "I want to take a survey" or "Show me a survey" to your agent to trigger a response.

### Debug agent in Teams Web Client
1. Ensure your Azure OpenAI settings are filled in `env/.env.local.user`.
    ```
    SECRET_AZURE_OPENAI_API_KEY="<your-azure-openai-api-key>"
    AZURE_OPENAI_ENDPOINT="<your-azure-openai-endpoint>"
    AZURE_OPENAI_DEPLOYMENT_NAME="<your-azure-openai-deployment-name>"
    ```
1. In the debug dropdown menu, select Dev Tunnels > Create A Tunnel (set authentication type to Public) or select an existing public dev tunnel.
2. Right-click the 'M365Agent' project in Solution Explorer and select **Microsoft 365 Agents Toolkit > Select Microsoft 365 Account**
3. Sign in to Microsoft 365 Agents Toolkit with a **Microsoft 365 work or school account**
4. Set `Startup Item` as `Microsoft Teams (browser)`.
5. Press F5, or select Debug > Start Debugging menu in Visual Studio to start your app
</br>![image](https://raw.githubusercontent.com/OfficeDev/TeamsFx/dev/docs/images/visualstudio/debug/debug-button.png)
6. In the opened web browser, select Add button to install the app in Teams
7. In the chat bar, type and send messages like "I want to take a survey" or "I'd like to provide my profile" to your agent to trigger the survey experience.

> For local debugging using Microsoft 365 Agents Toolkit CLI, you need to do some extra steps described in [Set up your Microsoft 365 Agents Toolkit CLI for local debugging](https://aka.ms/teamsfx-cli-debugging).

## Additional information and references
- [Microsoft 365 Agents SDK](https://github.com/microsoft/Agents)
- [Microsoft 365 Agents Toolkit Documentations](https://docs.microsoft.com/microsoftteams/platform/toolkit/teams-toolkit-fundamentals)
- [Microsoft 365 Agents Toolkit CLI](https://aka.ms/teamsfx-toolkit-cli)
- [Microsoft 365 Agents Toolkit Samples](https://github.com/OfficeDev/TeamsFx-Samples)

## Learn more

New to app development or Microsoft 365 Agents Toolkit? Learn more about app manifests, deploying to the cloud, and more in the documentation 
at https://aka.ms/teams-toolkit-vs-docs.

## Report an issue

Select Visual Studio > Help > Send Feedback > Report a Problem. 
Or, you can create an issue directly in our GitHub repository: 
https://github.com/OfficeDev/TeamsFx/issues.
