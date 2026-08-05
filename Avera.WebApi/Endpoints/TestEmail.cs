using Avera.Application.Abstractions.Services;
using Microsoft.AspNetCore.Identity;

namespace Avera.WebApi.Endpoints
{
    public class TestEmail : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder routeBuilder)
        {
            routeBuilder.MapPost("test", async (IEmailService service, CancellationToken cancellationToken)=>
            {
                //await service.SendRequestApprovedAsync("sirpatrick121402@gmail.com", "Patrick", "Philippine National Police", cancellationToken);
            });
            
        }
    }
}