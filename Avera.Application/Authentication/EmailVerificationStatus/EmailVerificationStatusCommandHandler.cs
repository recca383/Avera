using Avera.Application.Abstractions.Authentication;
using Avera.Application.Abstractions.Messaging;
using Avera.Application.Authentication.Common;
using SharedKernel;

namespace Avera.Application.Authentication.EmailVerificationStatus;

internal sealed class GetEmailVerificationStatusQueryHandler(
        IAuthenticationService authenticationService,
        IUserContext userContext
    ) : IQueryHandler<GetEmailVerificationStatusQuery,EmailVerificationStatusResponse>
{
    public async Task<Result<EmailVerificationStatusResponse>> Handle(
        GetEmailVerificationStatusQuery query,
        CancellationToken cancellationToken)
    {
        var result =
            await authenticationService
                .GetEmailVerificationStatusAsync(
                    userContext.UserId,
                    cancellationToken);

        if (result.IsFailure)
        {
            return Result.Failure<EmailVerificationStatusResponse>(
                result.Error);
        }

        return Result.Success(
            new EmailVerificationStatusResponse(
                result.Value));
    }
}