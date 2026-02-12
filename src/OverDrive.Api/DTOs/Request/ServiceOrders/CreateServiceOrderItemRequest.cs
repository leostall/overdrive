namespace OverDrive.Api.Dtos.ServiceOrders;

public class CreateServiceOrderItemRequest
{
    public string Description { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal Price { get; set; }
    public List<ServiceOrderItemPartRequest>? Parts { get; set; }
}