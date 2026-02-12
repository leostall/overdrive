using OverDrive.Api.Models;
using OverDrive.Api.Repositories.Interfaces;

namespace OverDrive.Api.Repositories;

public class PaymentRepository : IPaymentRepository
{
    private readonly OverDriveDbContext _context;

    public PaymentRepository(OverDriveDbContext context)
    {
        _context = context;
    }

    public Payment? GetById(int paymentId)
    {
        return _context.Payments.FirstOrDefault(p => p.Id == paymentId);
    }

    public List<Payment> GetAll()
    {
        return _context.Payments.OrderByDescending(p => p.Id).ToList();
    }

    public Payment? GetByMethod(string paymentMethod)
    {
        return _context.Payments.FirstOrDefault(p => p.PaymentMethod == paymentMethod);
    }

    public bool HasSales(int paymentId)
    {
        return _context.Sales.Any(s => s.FkPayment == paymentId);
    }

    public void Add(Payment payment)
    {
        _context.Payments.Add(payment);
    }

    public void Remove(Payment payment)
    {
        _context.Payments.Remove(payment);
    }

    public void SaveChanges()
    {
        _context.SaveChanges();
    }
}
