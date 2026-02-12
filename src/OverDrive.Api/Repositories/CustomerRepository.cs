using OverDrive.Api.Models;
using OverDrive.Api.Repositories.Interfaces;

namespace OverDrive.Api.Repositories;

public class CustomerRepository : ICustomerRepository
{
    private readonly OverDriveDbContext _context;

    public CustomerRepository(OverDriveDbContext context)
    {
        _context = context;
    }

    public Customer? GetById(int customerId)
    {
        return _context.Customers.FirstOrDefault(c => c.Id == customerId);
    }

    public List<Customer> GetAll()
    {
        return _context.Customers.OrderByDescending(c => c.Id).ToList();
    }

    public Address? GetAddressById(int addressId)
    {
        return _context.Addresses.FirstOrDefault(a => a.Id == addressId);
    }

    public Customer? GetByCpfCnpj(string cpfCnpj)
    {
        return _context.Customers.FirstOrDefault(c => c.CpfCnpj == cpfCnpj);
    }

    public bool HasSales(int customerId)
    {
        return _context.Sales.Any(s => s.FkCustomer == customerId);
    }

    public bool HasVehicles(int customerId)
    {
        return _context.Vehicles.Any(v => v.FkCustomer == customerId);
    }

    public void Add(Customer customer)
    {
        _context.Customers.Add(customer);
    }

    public void Remove(Customer customer)
    {
        _context.Customers.Remove(customer);
    }

    public void SaveChanges()
    {
        _context.SaveChanges();
    }
}
