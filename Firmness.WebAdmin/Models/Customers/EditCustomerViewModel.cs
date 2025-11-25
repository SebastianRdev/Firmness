namespace Firmness.WebAdmin.Models.Customers;

public class EditCustomerViewModel
{
    public Guid Id { get; set; }
    public string UserName { get; set; }
    public string FullName { get; set; }
    public string Email { get; set; }
    public string Address { get; set; }
    public string PhoneNumber { get; set; }

    // Password fields (Optional)
    public string? NewPassword { get; set; }
    public string? ConfirmNewPassword { get; set; }
}