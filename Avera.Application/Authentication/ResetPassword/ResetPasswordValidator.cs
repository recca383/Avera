using FluentValidation;

namespace Avera.Application.Authentication.ResetPassword
{
    public sealed class ResetPasswordValidator : AbstractValidator<ResetPasswordCommand>
    {
         public ResetPasswordValidator()
         {
                RuleFor(x => x.Email)
                    .NotEmpty().WithMessage("Email is required.")
                    .EmailAddress().WithMessage("Email must be a valid email address.");
                RuleFor(x => x.Token)
                    .NotEmpty().WithMessage("Token is required.");
                RuleFor(x => x.Password)
                    .NotEmpty().WithMessage("Password is required.")
                    .MinimumLength(8).WithMessage("Password must be at least 8 characters long.")
                    .Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter.")
                    .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
                    .Matches("[0-9]").WithMessage("Password must contain at least one digit.")
                    .Matches("[^a-zA-Z0-9]").WithMessage("Password must contain at least one non-alphanumeric character.");
         }
    }
}