namespace OverDrive.Api.Dtos.ServiceOrders;

public class ServiceOrderItemResponse
{
    public int Id { get; set; }
    public string Description { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal Price { get; set; }
    public decimal Subtotal { get; set; }
    public List<ServiceOrderPartResponse> Parts { get; set; } = new();
}