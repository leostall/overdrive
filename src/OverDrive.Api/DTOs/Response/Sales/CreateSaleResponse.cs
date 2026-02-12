namespace OverDrive.Api.Dtos.Sales;

public class CreateSaleResponse
{
    public int SaleId { get; set; }
    public decimal Subtotal { get; set; }
    public decimal TotalDiscount { get; set; }
    public decimal AdditionalFee { get; set; }
    public decimal Total { get; set; }
}