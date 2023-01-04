using FluentValidation;

namespace RobusAPI.WebApi.Models.Apiv2.Courses.Validators
{
    public class AddCourseRequestValidator : AbstractValidator<AddCourseRequest>
    {
        public AddCourseRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Name is required");
            When(x => !string.IsNullOrWhiteSpace(x.Name), () =>
            {
                RuleFor(o => o.Name)
                .MaximumLength(100)
                .WithMessage("Max length of Name is 100");
            });
        }
    }
}
