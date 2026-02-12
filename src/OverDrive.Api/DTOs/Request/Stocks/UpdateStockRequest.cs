namespace OverDrive.Api.Dtos.Stocks;

public class UpdateStockRequest
{
    public string? Name { get; set; }
    public string? Note { get; set; }
    public int? BranchId { get; set; }
    public int? AddressId { get; set; }
}
