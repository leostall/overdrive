namespace OverDrive.Api.Dtos.ServiceOrders;

public class UpdateServiceOrderItemRequest
{
    public int? Id { get; set; }
    public string? Description { get; set; }
    public int? Quantity { get; set; }
    public decimal? Price { get; set; }
    public List<ServiceOrderItemPartRequest>? Parts { get; set; }
}