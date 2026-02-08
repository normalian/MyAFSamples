using System.ComponentModel;
using System.Text.Json.Serialization;

namespace Sample09M365Agent.Models;

public enum MySurveyAgentResponseContentType
{
    [JsonPropertyName("text")]
    Text,

    [JsonPropertyName("adaptive-card")]
    AdaptiveCard
}

public class MySurveyAgentResponse
{
    [JsonPropertyName("contentType")]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public MySurveyAgentResponseContentType ContentType { get; set; }

    /// <summary>
    /// If the agent could not provide a survey response this should contain a textual response.
    /// </summary>
    [Description("If the answer is other agent response, contains the textual agent response.")]
    [JsonPropertyName("otherResponse")]
    public string OtherResponse { get; set; }
}
