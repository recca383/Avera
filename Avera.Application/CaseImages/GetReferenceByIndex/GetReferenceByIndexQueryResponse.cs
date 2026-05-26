namespace Avera.Application.CaseImages.GetReferenceByIndex
{
    public record GetReferenceByIndexQueryResponse
    (
        Guid CaseId,
        Stream ImageStream,
        string ContentType
    );
}