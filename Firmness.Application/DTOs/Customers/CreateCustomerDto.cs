namespace Firmness.Application.DTOs.Customers;

public class CreateCustomerDto
{
    public string UserName { get; set; }
    public string FullName { get; set; }
    public string Email { get; set; } 
    public string Address { get; set; }
    public string PhoneNumber { get; set; }
    public string Password { get; set; }

}
