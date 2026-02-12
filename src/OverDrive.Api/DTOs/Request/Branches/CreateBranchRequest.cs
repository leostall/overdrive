namespace OverDrive.Api.Dtos.Branches;

public class CreateBranchRequest
{
    public string CorporateName { get; set; } = string.Empty;
    public string Cnpj { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public bool Active { get; set; } = true;
    public int AddressId { get; set; }
}
