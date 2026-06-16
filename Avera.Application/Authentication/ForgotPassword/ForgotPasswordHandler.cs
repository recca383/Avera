using Avera.Application.Abstractions.Authentication;
using Avera.Application.Abstractions.Messaging;
using SharedKernel;

namespace Avera.Application.Authentication.ForgotPassword
{
    public sealed class ForgotPasswordHandler
    (
        IAuthenticationService authenticationServices
        
    ) : ICommandHandler<ForgotPasswordCommand>
    {
        public async Task<Result> Handle(ForgotPasswordCommand command, CancellationToken cancellationToken)
        {
            try
            {
                await authenticationServices.ForgotPasswordAsync(command.Email, cancellationToken);

                return Result.Success();
            }
            catch (System.Exception)
            {
                return Result.Failure<ForgotPasswordHandler>(ForgotPasswordCommandError.ForgotPasswordError);
                
            }
        }
    }
}