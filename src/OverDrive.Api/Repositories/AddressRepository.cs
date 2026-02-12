using OverDrive.Api.Models;
using OverDrive.Api.Repositories.Interfaces;

namespace OverDrive.Api.Repositories;

public class AddressRepository : IAddressRepository
{
    private readonly OverDriveDbContext _context;

    public AddressRepository(OverDriveDbContext context)
    {
        _context = context;
    }

    public Address? GetById(int addressId)
    {
        return _context.Addresses.FirstOrDefault(a => a.Id == addressId);
    }

    public List<Address> GetAll()
    {
        return _context.Addresses.OrderByDescending(a => a.Id).ToList();
    }

    public bool HasCustomers(int addressId)
    {
        return _context.Customers.Any(c => c.FkAddress == addressId);
    }

    public bool HasBranches(int addressId)
    {
        return _context.Branches.Any(b => b.FkAddress == addressId);
    }

    public bool HasEmployees(int addressId)
    {
        return _context.Employees.Any(e => e.FkAddress == addressId);
    }

    public bool HasStocks(int addressId)
    {
        return _context.Stocks.Any(s => s.FkAddress == addressId);
    }

    public bool HasVehicles(int addressId)
    {
        return _context.Vehicles.Any(v => v.FkAddress == addressId);
    }

    public void Add(Address address)
    {
        _context.Addresses.Add(address);
    }

    public void Remove(Address address)
    {
        _context.Addresses.Remove(address);
    }

    public void SaveChanges()
    {
        _context.SaveChanges();
    }
}
