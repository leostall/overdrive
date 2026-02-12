namespace OverDrive.Api.Dtos.Sales;

public class CreateSaleRequest
{
    public int CustomerId { get; set; }
    public int BranchId { get; set; }
    public int EmployeeId { get; set; }
    public int PaymentId { get; set; }
    public int? StatusId { get; set; }
    public decimal? Discount { get; set; }
    public decimal? AdditionalFee { get; set; }
    public List<CreateSaleItemRequest> Items { get; set; } = new();
}