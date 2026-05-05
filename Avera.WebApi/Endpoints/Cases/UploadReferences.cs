
namespace Avera.WebApi.Endpoints.Cases
{
    internal sealed class UploadReferences : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder routeBuilder)
        {
            routeBuilder.MapPost("cases/{id:guid}/upload-references", ()=>
            {
                
            })
           . WithTags(Tags.Cases)
            ;
        }
    }
}