using System.Text.Json.Serialization;
using Avera.Domain.Application.OverlayImages;

namespace Avera.Application.ML.Process
{
    public sealed record ProcessResponse(
        [property: JsonPropertyName("case_name")] string CaseName,
        [property: JsonPropertyName("confidence_forged")] float ConfidenceForged,
        [property: JsonPropertyName("confidence_genuine")] float ConfidenceGenuine,
        [property: JsonPropertyName("distance")] float Distance,
        [property: JsonPropertyName("gradcam_images")] List<GradCamImageDto> GradcamImages,
        float Threshold,
        string Verdict);

    public sealed record GradCamImageDto(
        [property: JsonPropertyName("slot")] string Slot,
        [property: JsonPropertyName("variant")] string Variant,
        [property: JsonPropertyName("image_id")] Guid ImageId);
}
