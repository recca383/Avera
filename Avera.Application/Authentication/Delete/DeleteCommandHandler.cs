using Avera.Application.Abstractions.Authentication;
using Avera.Application.Abstractions.Messaging;
using SharedKernel;

namespace Avera.Application.Authentication.Delete
{
    public sealed class DeleteCommandHandler
    (
        IAuthenticationService authenticationService,
        IUserContext userContext
    ) : ICommandHandler<DeleteCommand>
    {
        public async Task<Result> Handle(DeleteCommand command, CancellationToken cancellationToken)
        {
            return await authenticationService.DeleteUserAsync(userContext.UserId, cancellationToken);
        }
    }
}