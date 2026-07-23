using Avera.Application.ML.Health;
using Avera.Application.ML.Process;

namespace Avera.Application.Abstractions.ML
{
    public interface IMLService
    {
         Task<GetMLHealthResponse> GetMLHealthAsync(CancellationToken cancellationToken);

         Task<ProcessMLResponse> ProcessAsync(ProcessRequest request, CancellationToken cancellationToken);
    }
}