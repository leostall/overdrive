namespace OverDrive.Api.Dtos.Parts;

public class CreatePartRequest
{
    public string Code { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal Price { get; set; }
    public int StockId { get; set; }
}
