namespace OverDrive.Api.Dtos.Stocks;

public class CreateStockRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Note { get; set; }
    public int? BranchId { get; set; }
    public int AddressId { get; set; }
}
