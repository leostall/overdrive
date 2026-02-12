namespace OverDrive.Api.Dtos.Sales;

public class UpdateSaleItemRequest
{
    public int? Id { get; set; }
    public string ItemType { get; set; } = null!;
    public int? PartId { get; set; }
    public int? VehicleId { get; set; }
    public int? Quantity { get; set; }
    public decimal? Discount { get; set; }
    public decimal? UnitPrice { get; set; }
}
