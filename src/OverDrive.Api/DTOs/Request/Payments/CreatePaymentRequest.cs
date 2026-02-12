namespace OverDrive.Api.Dtos.Payments;

public class CreatePaymentRequest
{
    public string PaymentMethod { get; set; } = string.Empty;
    public string? Description { get; set; }
}
