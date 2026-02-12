namespace OverDrive.Api.Dtos.Parts;

public class UpdatePartRequest
{
    public string? Code { get; set; }
    public string? Description { get; set; }
    public int? Quantity { get; set; }
    public decimal? Price { get; set; }
    public int? StockId { get; set; }
}
