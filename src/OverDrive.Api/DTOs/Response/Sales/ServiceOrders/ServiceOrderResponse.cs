namespace OverDrive.Api.Dtos.ServiceOrders;

public class ServiceOrderResponse
{
    public int Id { get; set; }
    public string Number { get; set; } = string.Empty;
    public DateTime OpenDate { get; set; }
    public DateTime? CloseDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public decimal TotalValue { get; set; }
    public string? Notes { get; set; }
    public int VehicleId { get; set; }
    public int? BranchId { get; set; }
    public List<ServiceOrderItemResponse> Items { get; set; } = new();
}