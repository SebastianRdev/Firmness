namespace Firmness.WebAdmin.Validators.Customers;

using FluentValidation;
using Firmness.WebAdmin.Models.Customers;

public class EditCustomerValidator : AbstractValidator<EditCustomerViewModel>
{
    public EditCustomerValidator()
    {
        // Reglas básicas (puedes mover las de DataAnnotations aquí si quieres centralizar todo)
        RuleFor(x => x.UserName)
            .NotEmpty().WithMessage("Username is required.")
            .MinimumLength(3).WithMessage("Username must be at least 3 characters.");

        RuleFor(x => x.FullName).NotEmpty().WithMessage("Full name is required.");
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Address).NotEmpty();
        RuleFor(x => x.PhoneNumber).NotEmpty();

        // --- LÓGICA DE CONTRASEÑA OPCIONAL ---
        
        // Regla 1: Si NewPassword tiene valor, debe tener mínimo 6 caracteres
        // y ConfirmNewPassword debe ser igual.
        When(x => !string.IsNullOrWhiteSpace(x.NewPassword), () =>
        {
            RuleFor(x => x.NewPassword)
                .MinimumLength(6).WithMessage("The password must be at least 6 characters long.");

            RuleFor(x => x.ConfirmNewPassword)
                .NotEmpty().WithMessage("The ConfirmNewPassword field is required.")
                .Equal(x => x.NewPassword).WithMessage("The new password and confirmation password do not match.");
        });

        // Regla 2: Si escribieron ConfirmNewPassword pero dejaron NewPassword vacío
        When(x => !string.IsNullOrWhiteSpace(x.ConfirmNewPassword), () =>
        {
            RuleFor(x => x.NewPassword)
                .NotEmpty().WithMessage("The NewPassword field is required if you are confirming a password.");
        });
    }
}