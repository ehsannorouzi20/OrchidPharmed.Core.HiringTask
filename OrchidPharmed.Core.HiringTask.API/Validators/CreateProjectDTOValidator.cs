using FluentValidation;

namespace OrchidPharmed.Core.HiringTask.API.Validators
{
    public class CreateProjectDTOValidator : FluentValidation.AbstractValidator<Structure.DTO.CreateProjectDTO>
    {
        public CreateProjectDTOValidator()
        {
            RuleFor(project => project.Title).NotEmpty().MaximumLength(200).WithMessage("Title cannot be empty or too large");

            RuleFor(project => project.Description).MaximumLength(1000).WithMessage("Description is too large");
        }
    }
}
