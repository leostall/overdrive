using OverDrive.Api.Models;

namespace OverDrive.Api.Repositories.Interfaces;

public interface IPaymentRepository
{
    Payment? GetById(int paymentId);
    List<Payment> GetAll();
    Payment? GetByMethod(string paymentMethod);
    bool HasSales(int paymentId);
    void Add(Payment payment);
    void Remove(Payment payment);
    void SaveChanges();
}
