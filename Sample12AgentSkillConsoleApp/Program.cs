// See https://aka.ms/new-console-template for more information
using Azure.AI.OpenAI;
using Azure.Identity;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using System.ComponentModel;

var endpoint = Environment.GetEnvironmentVariable("AZURE_OPENAI_ENDPOINT") ?? throw new InvalidOperationException("AZURE_OPENAI_ENDPOINT is not set.");
var deploymentName = Environment.GetEnvironmentVariable("AZURE_OPENAI_DEPLOYMENT_NAME") ?? "gpt-4.1-mini";

// ============================================================
// Approach 1: File-based skills (AgentFileSkill)
// Skills are discovered from SKILL.md files in the filesystem.
// Directory structure:
//   skills/
//     seattle-tourism/
//       SKILL.md              <- Frontmatter (name, description) + instructions
//       resources/
//         attractions.md      <- Static resource file
//         restaurants.md      <- Static resource file
//     weather-info/
//       SKILL.md
//       resources/
//         weather-data.md
// ============================================================
Console.WriteLine("========================================");
Console.WriteLine(" Approach 1: File-based Skills");
Console.WriteLine("========================================\n");

var skillsDirectory = Path.Combine(AppContext.BaseDirectory, "skills");

// Copy skills directory to output if running from bin folder
if (!Directory.Exists(skillsDirectory))
{
    // Fallback: try relative to project root
    skillsDirectory = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "skills"));
}

// Build skills provider from the file system directory
var fileSkillsProvider = new AgentSkillsProviderBuilder()
    .UseFileSkills([skillsDirectory])
    .Build();

var chatClient = new AzureOpenAIClient(
    new Uri(endpoint),
    new AzureCliCredential())
    .GetChatClient(deploymentName)
    .AsIChatClient();

ChatClientAgentOptions fileAgentOptions = new ChatClientAgentOptions
{
    ChatOptions = new ChatOptions()
    {
        Instructions = "You are a helpful travel assistant. Use your available skills to answer user questions about Seattle tourism and weather.",
    },
    AIContextProviders = [fileSkillsProvider]
};

AIAgent fileAgent = chatClient.AsAIAgent(fileAgentOptions);

Console.WriteLine("=== Query 1: Seattle Attractions (from file skill) ===");
Console.WriteLine(await fileAgent.RunAsync("What are some outdoor attractions in Seattle?"));
Console.WriteLine();

Console.WriteLine("=== Query 2: Weather Information (from file skill) ===");
Console.WriteLine(await fileAgent.RunAsync("What's the weather like in Tokyo?"));
Console.WriteLine();

// ============================================================
// Approach 2: Inline skills (AgentInlineSkill)
// Skills are defined in code with scripts (C# delegates)
// and static resources.
// ============================================================
Console.WriteLine("========================================");
Console.WriteLine(" Approach 2: Inline Skills with Scripts");
Console.WriteLine("========================================\n");

// Define an inline skill for Seattle tourism with callable scripts
var seattleTourismSkill = new AgentInlineSkill(
    name: "seattle-tourism",
    description: "Provides detailed tourism information about Seattle including attractions, restaurants, and events.",
    instructions: """
        You are a Seattle tourism expert. When asked about Seattle tourism,
        use the available scripts to look up attractions and restaurants.
        Always provide enthusiastic and helpful recommendations.
        """);

// Add a script (callable by LLM) that returns Seattle attractions
seattleTourismSkill.AddScript(
    "get-attractions",
    GetSeattleAttractions,
    "Returns a list of popular Seattle attractions for the given category.");

// Add a script that returns restaurant recommendations
seattleTourismSkill.AddScript(
    "get-restaurants",
    GetSeattleRestaurants,
    "Returns restaurant recommendations near a Seattle landmark.");

// Add a static resource with general Seattle facts
seattleTourismSkill.AddResource(
    "seattle-facts",
    """
    - Seattle is known as the Emerald City
    - Population: approximately 750,000
    - Famous for coffee (Starbucks originated here)
    - Home to the Space Needle, Pike Place Market, and Museum of Pop Culture
    - Average annual rainfall: 37 inches (less than New York City!)
    - Major tech companies: Microsoft, Amazon, Boeing
    """,
    "General facts about Seattle");

// Define another inline skill for weather information
var weatherSkill = new AgentInlineSkill(
    name: "weather-info",
    description: "Provides current weather information for cities.",
    instructions: "Use the get-weather script to retrieve weather data when asked about weather conditions.");

weatherSkill.AddScript(
    "get-weather",
    GetWeather,
    "Returns the current weather for a given city.");

// Build the skills provider using the builder pattern
var inlineSkillsProvider = new AgentSkillsProviderBuilder()
    .UseSkills(seattleTourismSkill, weatherSkill)
    .Build();

ChatClientAgentOptions inlineAgentOptions = new ChatClientAgentOptions
{
    ChatOptions = new ChatOptions()
    {
        Instructions = "You are a helpful travel assistant. Use your available skills to answer user questions about Seattle tourism and weather.",
    },
    AIContextProviders = [inlineSkillsProvider]
};

AIAgent inlineAgent = chatClient.AsAIAgent(inlineAgentOptions);

Console.WriteLine("=== Query 3: Restaurant Recommendations (from inline skill) ===");
Console.WriteLine(await inlineAgent.RunAsync("Can you recommend restaurants near Pike Place Market?"));
Console.WriteLine();

Console.WriteLine("=== Query 4: Weather Information (from inline skill) ===");
Console.WriteLine(await inlineAgent.RunAsync("What's the weather like in Seattle today?"));
Console.WriteLine();

// ============================================================
// Approach 3: Mixed - Combining file-based and inline skills
// using AgentSkillsProviderBuilder
// ============================================================
Console.WriteLine("========================================");
Console.WriteLine(" Approach 3: Mixed (File + Inline)");
Console.WriteLine("========================================\n");

// A custom inline skill not available as a file
var transportSkill = new AgentInlineSkill(
    name: "seattle-transport",
    description: "Provides public transportation information for Seattle.",
    instructions: "Help users navigate Seattle's public transportation options.");

transportSkill.AddScript(
    "get-transit-info",
    GetTransitInfo,
    "Returns public transit options for getting between Seattle locations.");

// Combine file-based skills + inline skills in one provider
var mixedSkillsProvider = new AgentSkillsProviderBuilder()
    .UseFileSkills([skillsDirectory])
    .UseSkills(transportSkill)
    .Build();

ChatClientAgentOptions mixedAgentOptions = new ChatClientAgentOptions
{
    ChatOptions = new ChatOptions()
    {
        Instructions = "You are a comprehensive Seattle travel assistant. Use all your available skills to help users plan their trip.",
    },
    AIContextProviders = [mixedSkillsProvider]
};

AIAgent mixedAgent = chatClient.AsAIAgent(mixedAgentOptions);

Console.WriteLine("=== Query 5: Transit + Tourism (mixed skills) ===");
Console.WriteLine(await mixedAgent.RunAsync("How do I get from the airport to Pike Place Market using public transit, and what should I eat there?"));
Console.WriteLine();

Console.WriteLine("Done!");

// --- Skill script implementations ---

[Description("Get Seattle attractions by category (outdoor, cultural, family, food)")]
static string GetSeattleAttractions([Description("The category of attractions")] string category)
{
    return category.ToLower() switch
    {
        "outdoor" => "1. Discovery Park - 534 acres of beaches and trails\n2. Kerry Park - Best city skyline views\n3. Green Lake Park - 2.8 mile walking loop\n4. Alki Beach - Sandy beach with mountain views",
        "cultural" => "1. Museum of Pop Culture (MoPOP) - Music and sci-fi exhibits\n2. Seattle Art Museum - World-class art collection\n3. Chihuly Garden and Glass - Stunning glass sculptures\n4. Wing Luke Museum - Asian Pacific American heritage",
        "family" => "1. Seattle Aquarium - Marine life on the waterfront\n2. Woodland Park Zoo - Over 1,000 animals\n3. Pacific Science Center - Interactive exhibits\n4. Seattle Great Wheel - Ferris wheel on the pier",
        "food" => "1. Pike Place Market - Iconic farmers market\n2. Ballard Farmers Market - Local produce and crafts\n3. International District - Diverse Asian cuisine\n4. Capitol Hill - Trendy restaurants and cafes",
        _ => $"Popular attractions in '{category}' category: Space Needle, Pike Place Market, Museum of Pop Culture, Seattle Aquarium"
    };
}

[Description("Get restaurant recommendations near a Seattle landmark")]
static string GetSeattleRestaurants([Description("The landmark name")] string landmark)
{
    return landmark.ToLower() switch
    {
        var l when l.Contains("pike place") => "1. Pike Place Chowder - Award-winning clam chowder\n2. Beecher's Handmade Cheese - Fresh mac & cheese\n3. Piroshky Piroshky - Russian pastries\n4. The Walrus and the Carpenter - Oyster bar",
        var l when l.Contains("space needle") => "1. SkyCity Restaurant - Rotating restaurant with views\n2. Toulouse Petit - Cajun-Creole brunch\n3. Serious Pie - Artisan pizzas\n4. Poquitos - Creative Mexican cuisine",
        var l when l.Contains("waterfront") => "1. Ivar's Acres of Clams - Iconic seafood\n2. The Crab Pot - Seafood feast experience\n3. Miner's Landing - Casual waterfront dining\n4. Steelhead Diner - Pacific Northwest cuisine",
        _ => $"Near {landmark}: Try exploring local cafes and seafood restaurants in the area. Seattle is known for fresh Pacific Northwest cuisine!"
    };
}

[Description("Get current weather information for a city")]
static string GetWeather([Description("The city name")] string city)
{
    return city.ToLower() switch
    {
        "seattle" => "Seattle: 62°F (17°C), Partly cloudy with a chance of light rain in the evening. Humidity: 72%. Wind: 8 mph from the SW.",
        "tokyo" => "Tokyo: 78°F (26°C), Sunny with high humidity. Humidity: 85%. Wind: 5 mph from the SE.",
        "new york" => "New York: 75°F (24°C), Clear skies. Humidity: 55%. Wind: 12 mph from the W.",
        _ => $"{city}: 68°F (20°C), Conditions vary. Check a local weather service for the most up-to-date information."
    };
}

[Description("Get public transit information between Seattle locations")]
static string GetTransitInfo([Description("The starting location")] string from, [Description("The destination")] string to)
{
    return (from.ToLower(), to.ToLower()) switch
    {
        var (f, t) when f.Contains("airport") && t.Contains("pike place") =>
            "Take the Link Light Rail from SEA-TAC Airport Station to Westlake Station (~38 min, $3.25). Walk 5 min to Pike Place Market.",
        var (f, t) when f.Contains("airport") && t.Contains("space needle") =>
            "Take the Link Light Rail from SEA-TAC Airport Station to Westlake Station (~38 min, $3.25). Then take the Seattle Monorail to Seattle Center (~2 min, $3.50).",
        var (f, t) when f.Contains("pike place") && t.Contains("space needle") =>
            "Walk to Westlake Station (5 min). Take the Seattle Monorail to Seattle Center (~2 min, $3.50). Or walk the entire way (~20 min).",
        _ => $"From {from} to {to}: Use the King County Metro bus system or Link Light Rail. Check OneBusAway app for real-time schedules. Fare: $2.75-$3.25."
    };
}
