using FluentValidation;

namespace Avera.Application.Authentication.ForgotPassword
{
    public sealed class ForgotPasswordCommandValidator : AbstractValidator<ForgotPasswordCommand>
    {
         public ForgotPasswordCommandValidator()
        {
            RuleFor(f => f.Email).EmailAddress().NotEmpty();
        }
    }
}