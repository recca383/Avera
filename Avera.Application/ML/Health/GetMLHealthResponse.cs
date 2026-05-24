using System.Text.Json.Serialization;

namespace Avera.Application.ML.Health
{
    public record GetMLHealthResponse(
        string Status, 
        [property: JsonPropertyName("model_loaded")] bool ModelLoaded,
        string? Version = null
        );
}