using System.Text.Json.Serialization;

namespace Avera.Application.ML.Process
{
    public sealed record ProcessRequest(
        [property: JsonPropertyName("case_name")] string CaseName,
        [property: JsonPropertyName("questioned_image_id")] string QuestionedImageUrl,
        [property: JsonPropertyName("reference_image_ids")] List<string> ReferenceImageUrls
    ); 
}