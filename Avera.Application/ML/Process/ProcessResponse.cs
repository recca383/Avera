using System.Text.Json.Serialization;

namespace Avera.Application.ML.Process
{
    public sealed record ProcessResponse(
        [property: JsonPropertyName("case_name")] string CaseName,
        [property: JsonPropertyName("confidence_forged")] float ConfidenceForged,
        [property: JsonPropertyName("confidence_genuine")] float ConfidenceGenuine,
        [property: JsonPropertyName("distance")] float Distance,
        [property: JsonPropertyName("gradcam_blob_ids")] List<string> GradcamBlobId,
        float Threshold,
        string Verdict);
    
}