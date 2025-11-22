namespace Firmness.Application.DTOs.Customers;

public class UpdateCustomerDto
{
    public Guid Id { get; set; }
    public string UserName { get; set; }
    public string FullName { get; set; }
    public string Address { get; set; }
    public string PhoneNumber { get; set; }
    
    // The password fields would only be included if the user decides to change them
    public string NewPassword { get; set; }
    public List<string> Roles { get; set; }
}