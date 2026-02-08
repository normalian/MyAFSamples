using AdaptiveCards;
using Microsoft.Agents.AI;
using Microsoft.Agents.Builder;
using Microsoft.Agents.Builder.App;
using Microsoft.Agents.Builder.State;
using Microsoft.Agents.Core.Models;
using Microsoft.Extensions.AI;
using Sample09M365Agent.Agents;
using Sample09M365Agent.Models;
using System.Text.Json;
using System.Text.Json.Serialization;
using AdaptiveCard = AdaptiveCards.AdaptiveCard;
using JsonException = System.Text.Json.JsonException;
using JsonSerializer = System.Text.Json.JsonSerializer;


namespace Sample09M365Agent;

public class AFAgentApp : AgentApplication
{
    private readonly AIAgent _agent;
    private readonly string _welcomeMessage;

    public AFAgentApp(AIAgent agent, AgentApplicationOptions options, [FromKeyedServices("AFAgentAppWelcomeMessage")] string welcomeMessage = null) : base(options)
    {
        _agent = agent;
        _welcomeMessage = welcomeMessage;

        OnConversationUpdate(ConversationUpdateEvents.MembersAdded, WelcomeMessageAsync);
        OnActivity(ActivityTypes.Message, MessageActivityAsync, rank: RouteRank.Last);
    }

    protected async Task MessageActivityAsync(ITurnContext turnContext, ITurnState turnState, CancellationToken cancellationToken)
    {
        // Start a Streaming Process 
        await turnContext.StreamingResponse.QueueInformativeUpdateAsync("Working on a response for you", cancellationToken);

        JsonElement threadElement = turnState.GetValue<JsonElement>("conversation.thread");
        AgentThread agentThread = threadElement.ValueKind is not JsonValueKind.Undefined and not JsonValueKind.Null
            ? _agent.DeserializeThread(threadElement, JsonUtilities.DefaultOptions)
            : _agent.GetNewThread();

        ChatMessage chatMessage = HandleUserInput(turnContext);

        AgentRunResponse response = await _agent.RunAsync(chatMessage, agentThread, cancellationToken: cancellationToken);

        List<Attachment> attachments = null;
        HandleUserInputRequests(response, ref attachments);

        var adaptiveCards = response.Messages.SelectMany(x => x.Contents).OfType<AdaptiveCardAIContent>().ToList();

        if (adaptiveCards.Any())
        {
            attachments ??= [];
            foreach (var ac in adaptiveCards)
            {
                attachments.Add(new Attachment()
                {
                    ContentType = AdaptiveCard.ContentType,
                    Content = ac.AdaptiveCardJson
                });
            }
        }
        else if (!string.IsNullOrEmpty(response.Text))
        {
            turnContext.StreamingResponse.QueueTextChunk(response.Text);
        }

        if (attachments is not null)
        {
            turnContext.StreamingResponse.FinalMessage = MessageFactory.Attachment(attachments);
        }

        JsonElement threadElementEnd = agentThread.Serialize(JsonUtilities.DefaultOptions);
        turnState.SetValue("conversation.thread", threadElementEnd);

        await turnContext.StreamingResponse.EndStreamAsync(cancellationToken);
    }

    protected async Task WelcomeMessageAsync(ITurnContext turnContext, ITurnState turnState, CancellationToken cancellationToken)
    {
        foreach (ChannelAccount member in turnContext.Activity.MembersAdded)
        {
            if (member.Id != turnContext.Activity.Recipient.Id)
            {
                await turnContext.SendActivityAsync(MessageFactory.Text(this._welcomeMessage), cancellationToken);
            }
        }
    }

#pragma warning disable MEAI001 // Approve to use preview feature - FunctionApprovalRequestContent
    private static ChatMessage HandleUserInput(ITurnContext turnContext)
    {
        if (turnContext.Activity.Value is JsonElement valueElement)
        {
            // Options to read the string value "25" as the numeric value 25
            var options = new JsonSerializerOptions
            {
                NumberHandling = JsonNumberHandling.AllowReadingFromString,
                PropertyNameCaseInsensitive = true 
            };

            try
            {
                var jsonString = valueElement.GetRawText();
                var result = JsonSerializer.Deserialize<MySurveyContent>(jsonString, options);

                return new ChatMessage(ChatRole.User, $"Survey response: Name={result.Name}, Age={result.Age}, Gender={result.Gender}, FavoriteFood={result.FavoriteFood}, Hobby={result.Hobby}");
            }
            catch (JsonException ex)
            {
                // Log output, etc.
                return new ChatMessage(ChatRole.User, "Failed to parse data.");
            }
        }

        return new ChatMessage(ChatRole.User, turnContext.Activity.Text);
    }

    private static void HandleUserInputRequests(AgentRunResponse response, ref List<Attachment>? attachments)
    {
        // Get requests where the LLM is attempting to call functions
        var toolCalls = response.Messages.SelectMany(m => m.Contents).OfType<FunctionCallContent>().ToList();

        foreach (var call in toolCalls)
        {
            if (call.Name == "ShowSurvey")
            {
                // Generate survey card
                var initialContent = new MySurveyContent { Name = "User", Id = Guid.NewGuid().ToString() };
                var surveyAttachment = CreateSurveyCardFromModel(initialContent);

                attachments ??= [];
                attachments.Add(surveyAttachment);
            }
        }
    }
#pragma warning restore MEAI001

    private static Attachment CreateSurveyCardFromModel(MySurveyContent content)
    {
        var card = new AdaptiveCard("1.5");

        // 1. Header
        card.Body.Add(new AdaptiveTextBlock
        {
            Text = $"Survey for {content.Name}",
            Size = AdaptiveTextSize.Medium,
            Weight = AdaptiveTextWeight.Bolder,
            Wrap = true,
        });

        // 2. Age (add label as an independent TextBlock)
        card.Body.Add(new AdaptiveTextBlock { Text = "Age", Weight = AdaptiveTextWeight.Bolder, Wrap=true, IsVisible=true });
        card.Body.Add(new AdaptiveNumberInput
        {
            Id = "Age",
            Value = content.Age,
            Placeholder = "Input your age",
        });

        // 3. Gender
        card.Body.Add(new AdaptiveTextBlock { Text = "Gender", Weight = AdaptiveTextWeight.Bolder });
        card.Body.Add(new AdaptiveChoiceSetInput
        {
            Id = "Gender",
            Style = AdaptiveChoiceInputStyle.Expanded,
            Value = content.Gender,
            Choices = new List<AdaptiveChoice>
            {
                new AdaptiveChoice { Title = "Male", Value = "Male" },
                new AdaptiveChoice { Title = "Female", Value = "Female" },
                new AdaptiveChoice { Title = "Other", Value = "Other" }
            }
        });

        // 4. Favorite food
        card.Body.Add(new AdaptiveTextBlock { Text = "Your favorite food", Weight = AdaptiveTextWeight.Bolder });
        card.Body.Add(new AdaptiveTextInput
        {
            Id = "FavoriteFood",
            Value = content.FavoriteFood,
            Placeholder = "Example: Sushi, Ramen"
        });

        // 5. Hobby
        card.Body.Add(new AdaptiveTextBlock { Text = "Hobby", Weight = AdaptiveTextWeight.Bolder });
        card.Body.Add(new AdaptiveTextInput
        {
            Id = "Hobby",
            Value = content.Hobby,
            Placeholder = "Example: Reading"
        });

        card.Actions.Add(new AdaptiveSubmitAction { Title = "Submit" });

        // --- Important: Processing when returning ---
        return new Attachment()
        {
            ContentType = AdaptiveCard.ContentType,
            Content = JsonSerializer.Deserialize<object>(card.ToJson())
        };
    }
}