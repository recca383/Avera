namespace Avera.Application.Cases.Get
{
    public sealed class GetCasesQueryResult 
    {
        public IReadOnlyList<CaseDto> Cases { get; set; } = Array.Empty<CaseDto>();
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
    }
}