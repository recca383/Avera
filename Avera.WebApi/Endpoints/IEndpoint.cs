namespace Avera.WebApi.Endpoints
{
    public interface IEndpoint
    {
        void MapEndpoint(IEndpointRouteBuilder routeBuilder);
    }
}
