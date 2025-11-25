namespace Firmness.WebAdmin.Validators.Categories;

using FluentValidation;
using Firmness.WebAdmin.Models.Categories;

public class CreateCategoryValidator : AbstractValidator<CreateCategoryViewModel>
{
    public CreateCategoryValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("The name is required")
            .Length(1, 100).WithMessage("The name cannot exceed 100 characters");
    }
}