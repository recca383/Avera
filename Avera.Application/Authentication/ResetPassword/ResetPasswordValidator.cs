using FluentValidation;

namespace Avera.Application.Authentication.ResetPassword
{
    public sealed class ResetPasswordValidator : AbstractValidator<ResetPasswordCommand>
    {
         public ResetPasswordValidator()
         {
                RuleFor(x => x.Token)
                    .NotEmpty().WithMessage("Token is required.");
                RuleFor(x => x.Password)
                    .NotEmpty().WithMessage("Password is required.")
                    .MinimumLength(6).WithMessage("Password must be at least 6 characters long.");
         }
    }
}