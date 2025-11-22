namespace Firmness.WebAdmin.Models;

using Microsoft.AspNetCore.Mvc.Rendering;

public class EditCustomerViewModel
{
    public Guid Id { get; set; }
    public string UserName { get; set; }
    public string FullName { get; set; }
    public string Address { get; set; }
    public string PhoneNumber { get; set; }
    public string SelectedRole { get; set; }
    public List<SelectListItem> Roles { get; set; }
    
    // The password fields would only be included if the user decides to change them
    public string NewPassword { get; set; }
    public string ConfirmNewPassword { get; set; }
}