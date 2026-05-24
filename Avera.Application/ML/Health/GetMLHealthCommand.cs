
using Avera.Application.Abstractions.Messaging;
using Avera.Application.ML.Health;

namespace Avera.Application.Abstractions.ML.Health
{
    public sealed record GetMLHealthCommand : ICommand<GetMLHealthResponse>;
}