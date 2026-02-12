namespace OverDrive.Api.Dtos.Customers;

public class UpdateCustomerRequest
{
    public string? CpfCnpj { get; set; }
    public string? CustomerType { get; set; }
    public string? Name { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public bool? Active { get; set; }
    public int? AddressId { get; set; }
}
