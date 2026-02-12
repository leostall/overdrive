namespace OverDrive.Api.Dtos.Sales;

public class UpdateSaleRequest
{
    public decimal? Discount { get; set; }
    public decimal? AdditionalFee { get; set; }
    public List<UpdateSaleItemRequest>? Items { get; set; }
}