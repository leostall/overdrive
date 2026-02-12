namespace OverDrive.Api.Dtos.Payments;

public class UpdatePaymentRequest
{
    public string? PaymentMethod { get; set; }
    public string? Description { get; set; }
}
