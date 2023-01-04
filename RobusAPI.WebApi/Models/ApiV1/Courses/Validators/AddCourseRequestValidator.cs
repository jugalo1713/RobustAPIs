using FluentValidation;
using RobusAPI.WebApi.Models.ApiV1.Courses;

namespace RobusAPI.WebApi.Models.Courses.ApiV1.Validators
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
