namespace OverDrive.Api.Dtos.Payments;

public class PaymentResponse
{
    public int PaymentId { get; set; }
    public string PaymentMethod { get; set; } = string.Empty;
    public string? Description { get; set; }
}
