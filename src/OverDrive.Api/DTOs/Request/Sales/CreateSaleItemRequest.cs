namespace OverDrive.Api.Dtos.Sales;

public class CreateSaleItemRequest
{
    public string ItemType { get; set; } = null!;
    public int? PartId { get; set; }
    public int? VehicleId { get; set; }
    public int Quantity { get; set; }
    public decimal? Discount { get; set; }
}