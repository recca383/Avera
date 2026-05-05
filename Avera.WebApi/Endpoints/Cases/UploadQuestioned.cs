
namespace Avera.WebApi.Endpoints.Cases
{
    internal sealed class UploadQuestioned : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder routeBuilder)
        {
            routeBuilder.MapPost("cases/{id:guid}/upload-questioned", ()=>
            {
                
            })
           . WithTags(Tags.Cases)
            ;
        }
    }
}