namespace Avera.Application.ML.GetResults
{
    public record GetMLResultsResponse
    (
        Guid CaseId,
        Stream ResultsStream
    );  

}