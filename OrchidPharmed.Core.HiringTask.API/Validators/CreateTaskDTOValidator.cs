using FluentValidation;

namespace OrchidPharmed.Core.HiringTask.API.Validators
{
    public class CreateTaskDTOValidator : FluentValidation.AbstractValidator<Structure.DTO.CreateTaskDTO>
    {
        public CreateTaskDTOValidator()
        {
            RuleFor(project => project.Title).NotEmpty().MaximumLength(200).WithMessage("Title cannot be empty or too large");

            RuleFor(project => project.Description).MaximumLength(1000).WithMessage("Description is too large");

            RuleFor(project => project.DueDate).GreaterThan(DateTime.Now).WithMessage("DueDate has not correct value");
        }
    }
}
