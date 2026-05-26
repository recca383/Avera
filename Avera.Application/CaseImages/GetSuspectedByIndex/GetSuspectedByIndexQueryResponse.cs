namespace Avera.Application.CaseImages.GetSuspectedByIndex
{
    public record GetSuspectedByIndexQueryResponse
    (
        Guid CaseId,
        Stream ImageStream,
        string ContentType
    );
}