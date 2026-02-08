using AdaptiveCards;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using Sample09M365Agent.Models;
using System.ComponentModel;
using System.Text;
using System.Text.Json;

namespace Sample09M365Agent.Agents;

public class MySurveyAgent : DelegatingAIAgent
{
    private const string AgentName = "MySurveyAgent";
    private const string AgentInstructions = """
        You are a friendly assistant that helps people take surveys.
        You may ask follow up questions until you have enough information to answer the customers question.
        When answering with a survey, fill out the surveyCard property with an adaptive card containing the survey information and
        add some emojis to indicate the type of survey.
        When answering with just text, fill out the context property with a friendly response.

        If a user says they want to answer a survey or provide their profile (age, gender, etc.), 
        call the 'ShowSurvey' function to provide the survey card.
        """;

    /// <summary>
    /// Initializes a new instance of the <see cref="MySurveyAgent"/> class.
    /// </summary>
    /// <param name="kernel">An instance of <see cref="Kernel"/> for interacting with an LLM.</param>
    public MySurveyAgent(IChatClient chatClient) : base(
        new ChatClientAgent(chatClient,
            new ChatClientAgentOptions()
            {
                Name = AgentName,
                ChatOptions = new ChatOptions()
                {
                    Instructions = AgentInstructions,
                    Tools = [
                        AIFunctionFactory.Create(ShowSurvey)
                    ],
                    // We want the agent to return structured output in a known format
                    // so that we can easily create adaptive cards from the response.
                    ResponseFormat = ChatResponseFormat.ForJsonSchema(
                        schema: AIJsonUtilities.CreateJsonSchema(typeof(MySurveyAgentResponse)),
                        schemaName: "MySurveyAgentResponse",
                        schemaDescription: "Response to a query about the survey in a specified location"),
                }
            })
        )
    {
    }

    [Description("Show a survey card to collect user information like age, gender, food, and hobby.")]
    private static string ShowSurvey()
    {
        return "SURVEY_REQUESTED";
    }

    protected override async Task<AgentRunResponse> RunCoreAsync(IEnumerable<ChatMessage> messages, AgentThread thread = null, AgentRunOptions options = null, CancellationToken cancellationToken = default)
    {
        var response = await base.RunCoreAsync(messages, thread, options, cancellationToken);

        // If the agent returned a valid structured output response
        // we might be able to enhance the response with an adaptive card.
        if (response.TryDeserialize<MySurveyAgentResponse>(JsonSerializerOptions.Web, out var structuredOutput))
        {
            var textContentMessage = response.Messages.FirstOrDefault(x => x.Contents.OfType<TextContent>().Any());
            if (textContentMessage is not null)
            {
                if (structuredOutput.ContentType == MySurveyAgentResponseContentType.Text)
                {
                    var textContent = textContentMessage.Contents.OfType<TextContent>().First();
                    textContent.Text = structuredOutput.OtherResponse;
                }
            }
        }

        return response;
    }
}
