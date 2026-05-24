namespace Avera.Application.Cases.Get
{
    public record class GetCasesQueryResult 
    (
         List<CaseDto> Cases,
         int TotalCount,
         int Page,
         int PageSize
    );
}