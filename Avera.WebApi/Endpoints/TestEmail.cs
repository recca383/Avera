using Avera.Application.Abstractions.Services;
using Avera.Domain.Identity.Users;
using Avera.Infrastructure.Configuration;
using Avera.Infrastructure.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Razor.Templating.Core;
using SharedKernel;
using System.Web;

namespace Avera.WebApi.Endpoints
{
    public class TestEmail(IConfiguration configuration) : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder routeBuilder)
        {
            routeBuilder.MapPost("test", async (
                IEmailService service,
                UserManager<User> _userManager,
                IDateTimeProvider dateTime,
                [FromServices] IOptions<AppOptions> appOptions,
                CancellationToken cancellationToken) =>
            {

                var html = await service.SendVerifiedFallback(cancellationToken);
                return Results.Content(html.Value, "text/html; charset=utf-8");


            });
            
        }
    }
}