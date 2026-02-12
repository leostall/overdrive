namespace OverDrive.Api.Dtos.Customers;

public class CreateCustomerRequest
{
    public string CpfCnpj { get; set; } = string.Empty;
    public string CustomerType { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public bool Active { get; set; } = true;
    public int AddressId { get; set; }
}
