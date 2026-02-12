namespace OverDrive.Api.Dtos.ServiceOrders;

public class UpdateServiceOrderRequest
{
    public string? Number { get; set; }
    public DateTime? OpenDate { get; set; }
    public DateTime? CloseDate { get; set; }
    public string? Status { get; set; }
    public string? Notes { get; set; }
    public int? VehicleId { get; set; }
    public int? BranchId { get; set; }
    public List<UpdateServiceOrderItemRequest>? Items { get; set; }
}