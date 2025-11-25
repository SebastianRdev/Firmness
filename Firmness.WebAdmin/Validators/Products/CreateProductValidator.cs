namespace Firmness.WebAdmin.Validators.Products;

using FluentValidation;
using Firmness.WebAdmin.Models.Products;

public class CreateProductValidator : AbstractValidator<CreateProductViewModel>
{
    public CreateProductValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("The name is mandatory")
            .Length(1, 100).WithMessage("The name cannot exceed 100 characters");

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("The price must be greater than 0");

        RuleFor(x => x.CategoryId)
            .NotEmpty().WithMessage("Select a category");

        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("Code is required")
            .Length(1, 20).WithMessage("The code cannot exceed 20 characters");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("The description is mandatory")
            .Length(1, 100).WithMessage("The description cannot exceed 100 characters");

        RuleFor(x => x.Stock)
            .GreaterThanOrEqualTo(0).WithMessage("The stock cannot be negative");
    }
}