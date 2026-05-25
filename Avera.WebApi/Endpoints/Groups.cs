using Avera.WebApi.Extensions;
using System.Runtime.CompilerServices;

namespace Avera.WebApi.Endpoints
{
    public static class Groups
    {
        private static string AuthEndpointPrefix = "/auth";
        public static IApplicationBuilder MapGroups(this WebApplication app)
        {

            var authGroup = app.MapGroup(AuthEndpointPrefix);
            //authGroup.Map
            app.MapEndpoints(authGroup);

            return app;
        }
    }

}
