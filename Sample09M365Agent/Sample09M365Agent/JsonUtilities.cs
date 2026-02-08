using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using Sample09M365Agent.Agents;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace Sample09M365Agent
{
    internal static partial class JsonUtilities
    {
        public static JsonSerializerOptions DefaultOptions { get; } = CreateDefaultOptions();

        [UnconditionalSuppressMessage("ReflectionAnalysis", "IL3050:RequiresDynamicCode", Justification = "Converter is guarded by IsReflectionEnabledByDefault check.")]
        [UnconditionalSuppressMessage("Trimming", "IL2026:Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access", Justification = "Converter is guarded by IsReflectionEnabledByDefault check.")]
        private static JsonSerializerOptions CreateDefaultOptions()
        {
            JsonSerializerOptions options = new(JsonContext.Default.Options)
            {
                TypeInfoResolver = JsonTypeInfoResolver.Combine(AIJsonUtilities.DefaultOptions.TypeInfoResolver, JsonContext.Default),
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            };
            options.AddAIContentType<AdaptiveCardAIContent>(typeDiscriminatorId: "adaptiveCard");

            if (JsonSerializer.IsReflectionEnabledByDefault)
            {
                options.Converters.Add(new JsonStringEnumConverter());
            }

            options.MakeReadOnly();
            return options;
        }

        [JsonSourceGenerationOptions(JsonSerializerDefaults.Web,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            NumberHandling = JsonNumberHandling.AllowReadingFromString)]

        // M365Agent specific types
        [JsonSerializable(typeof(AdaptiveCardAIContent))]

        [ExcludeFromCodeCoverage]
        internal sealed partial class JsonContext : JsonSerializerContext;
    }
}
