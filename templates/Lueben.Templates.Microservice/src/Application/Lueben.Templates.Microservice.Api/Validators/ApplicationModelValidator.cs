using FluentValidation;
using Lueben.Templates.Microservice.Api.Contract.Models;

namespace Lueben.Templates.Microservice.Api.Validators
{
    public class ApplicationModelValidator : AbstractValidator<Application>
    {
        public ApplicationModelValidator()
        {
            RuleFor(x => x.PreferredDepotId)
                .MaximumLength(50)
                .When(x => !string.IsNullOrEmpty(x.PreferredDepotId));

            RuleFor(x => x.Comments)
                .MaximumLength(1000)
                .When(x => !string.IsNullOrEmpty(x.Comments));

            RuleFor(x => x.RejectionReason)
                .MaximumLength(100)
                .When(x => !string.IsNullOrEmpty(x.RejectionReason));
        }
    }
}