namespace OverDrive.Api.Dtos.ServiceOrders;

public class CreateServiceOrderRequest
{
    public string Number { get; set; } = string.Empty;
    public DateTime? OpenDate { get; set; }
    public string? Status { get; set; }
    public string? Notes { get; set; }
    public int VehicleId { get; set; }
    public int? BranchId { get; set; }
    public List<CreateServiceOrderItemRequest> Items { get; set; } = new();
}