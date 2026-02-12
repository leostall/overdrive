namespace OverDrive.Api.Dtos.Branches;

public class UpdateBranchRequest
{
    public string? CorporateName { get; set; }
    public string? Cnpj { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public bool? Active { get; set; }
    public int? AddressId { get; set; }
}
