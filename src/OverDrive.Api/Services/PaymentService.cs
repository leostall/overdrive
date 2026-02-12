using OverDrive.Api.Dtos.Payments;
using OverDrive.Api.Exceptions;
using OverDrive.Api.Models;
using OverDrive.Api.Repositories.Interfaces;
using OverDrive.Api.Services.Interfaces;

namespace OverDrive.Api.Services;

public class PaymentService : IPaymentService
{
    private readonly IPaymentRepository _repository;

    public PaymentService(IPaymentRepository repository)
    {
        _repository = repository;
    }

    public PaymentResponse Create(CreatePaymentRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.PaymentMethod))
            throw new BusinessException(ErrorCodes.Sale.MissingRequiredFields, "PaymentMethod is required.");

        string strMethod = request.PaymentMethod.Trim();

        Payment? objExists = _repository.GetByMethod(strMethod);
        if (objExists != null)
            throw new BusinessException(ErrorCodes.Sale.MissingRequiredFields, "PaymentMethod already registered.");

        Payment objPayment = new()
        {
            PaymentMethod = strMethod,
            Description = NormalizeOptional(request.Description)
        };

        _repository.Add(objPayment);
        _repository.SaveChanges();

        return MapToResponse(objPayment);
    }

    public PaymentResponse Update(int paymentId, UpdatePaymentRequest request)
    {
        Payment objPayment = _repository.GetById(paymentId)
            ?? throw new NotFoundException($"Payment not found. Id={paymentId}");

        if (!string.IsNullOrWhiteSpace(request.PaymentMethod))
        {
            string strMethod = request.PaymentMethod.Trim();
            Payment? objExists = _repository.GetByMethod(strMethod);
            if (objExists != null && objExists.Id != objPayment.Id)
                throw new BusinessException(ErrorCodes.Sale.MissingRequiredFields, "PaymentMethod already registered.");

            objPayment.PaymentMethod = strMethod;
        }

        if (request.Description != null)
            objPayment.Description = NormalizeOptional(request.Description);

        _repository.SaveChanges();

        return MapToResponse(objPayment);
    }

    public void Delete(int paymentId)
    {
        Payment objPayment = _repository.GetById(paymentId)
            ?? throw new NotFoundException($"Payment not found. Id={paymentId}");

        if (_repository.HasSales(paymentId))
            throw new BusinessException(ErrorCodes.Sale.Blocked, "Payment cannot be deleted because it is linked to sales.");

        _repository.Remove(objPayment);
        _repository.SaveChanges();
    }

    public PaymentResponse GetById(int paymentId)
    {
        Payment objPayment = _repository.GetById(paymentId)
            ?? throw new NotFoundException($"Payment not found. Id={paymentId}");

        return MapToResponse(objPayment);
    }

    public List<PaymentResponse> GetAll()
    {
        return _repository.GetAll().Select(MapToResponse).ToList();
    }

    private static string? NormalizeOptional(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }

    private static PaymentResponse MapToResponse(Payment payment)
    {
        return new PaymentResponse
        {
            PaymentId = payment.Id,
            PaymentMethod = payment.PaymentMethod,
            Description = payment.Description
        };
    }
}
