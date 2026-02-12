using OverDrive.Api.Dtos.Payments;

namespace OverDrive.Api.Services.Interfaces;

public interface IPaymentService
{
    PaymentResponse Create(CreatePaymentRequest request);
    PaymentResponse Update(int paymentId, UpdatePaymentRequest request);
    void Delete(int paymentId);
    PaymentResponse GetById(int paymentId);
    List<PaymentResponse> GetAll();
}
