using Avera.Domain.Cases;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Avera.Application.Cases.Review
{
    internal class ReviewCaseCommandValidator : AbstractValidator<ReviewCaseCommand>
    {
        public ReviewCaseCommandValidator()
        {
            RuleFor(c => c.CaseId)
                .NotEmpty()
                .WithMessage("Case Id is Required");

            RuleFor(c => c.FinalVerdict)
                .NotEmpty()
                .WithMessage("Final Verdict should not be default")
                .IsInEnum()
                .WithMessage("Final Verdict should be either Genuine or Suspected");

            RuleFor(c => c.ReviewNote)
                .MaximumLength(512)
                .WithMessage("Review Note should not be longer than 512 characters");
        }
    }
}
