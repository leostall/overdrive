namespace OverDrive.Api.Dtos.Sales;

public class SaleItemResponse
{
    public int Id { get; set; }
    public string ItemType { get; set; } = null!;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Discount { get; set; }
    public int? PartId { get; set; }
    public int? VehicleId { get; set; }
}
