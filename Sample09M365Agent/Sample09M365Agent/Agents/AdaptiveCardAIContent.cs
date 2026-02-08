using AdaptiveCards;
using Microsoft.Extensions.AI;
using System.Text.Json.Serialization;

namespace Sample09M365Agent.Agents
{
    internal sealed class AdaptiveCardAIContent : AIContent
    {
        public AdaptiveCardAIContent(AdaptiveCard adaptiveCard)
        {
            AdaptiveCard = adaptiveCard ?? throw new ArgumentNullException(nameof(adaptiveCard));
        }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
        [JsonConstructor]
        public AdaptiveCardAIContent(string adaptiveCardJson)
        {
            AdaptiveCardJson = adaptiveCardJson;
        }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

        [JsonIgnore]
        public AdaptiveCard AdaptiveCard { get; private set; }

        public string AdaptiveCardJson
        {
            get => AdaptiveCard.ToJson();
            set => AdaptiveCard = AdaptiveCard.FromJson(value).Card;
        }
    }
}
