using FluentValidation.TestHelper;
using Lueben.Templates.Microservice.Api.Validators;
using Xunit;

namespace Lueben.Templates.Microservice.Api.Tests.Validators.Application
{
    public class ApplicationModelValidatorTests
    {
        private readonly ApplicationModelValidator _validator;

        public ApplicationModelValidatorTests()
        {
            _validator = new ApplicationModelValidator();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("1")]
        public void GivenApplicationModel_WhenTheLengthOfPreferredDepotIdIsValid_ThenTheValidatorShouldSucceed(string preferredDepotId)
        {
            var application = new Contract.Models.Application { PreferredDepotId = preferredDepotId };

            _validator.TestValidate(application).ShouldNotHaveValidationErrorFor(x => x.PreferredDepotId);
        }

        [Fact]
        public void GivenApplicationModel_WhenPreferredDepotIdIsTooLong_ThenTheValidatorShouldReturnTheValidationError()
        {
            var application = new Contract.Models.Application { PreferredDepotId = new string('-', 52) };

            _validator.TestValidate(application).ShouldNotHaveValidationErrorFor(x => x.PreferredDepotId);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("comment")]
        public void GivenApplicationModel_WhenTheLengthOfCommentsIsValid_ThenTheValidatorShouldSucceed(string comments)
        {
            var application = new Contract.Models.Application { Comments = comments };

            _validator.TestValidate(application).ShouldNotHaveValidationErrorFor(x => x.Comments);
        }

        [Fact]
        public void GivenApplicationModel_WhenCommentsIsTooLong_ThenTheValidatorShouldReturnTheValidationError()
        {
            var application = new Contract.Models.Application { Comments = new string('-', 1002) };

            _validator.TestValidate(application).ShouldNotHaveValidationErrorFor(x => x.Comments);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("reason")]
        public void GivenApplicationModel_WhenTheLengthOfRejectionReasonIsValid_ThenTheValidatorShouldSucceed(string rejectionReason)
        {
            var application = new Contract.Models.Application { RejectionReason = rejectionReason };

            _validator.TestValidate(application).ShouldNotHaveValidationErrorFor(x => x.RejectionReason);
        }

        [Fact]
        public void GivenApplicationModel_WhenRejectionReasonIsTooLong_ThenTheValidatorShouldReturnTheValidationError()
        {
            var application = new Contract.Models.Application { RejectionReason = new string('-', 102) };

            _validator.TestValidate(application).ShouldNotHaveValidationErrorFor(x => x.RejectionReason);
        }
    }
}
