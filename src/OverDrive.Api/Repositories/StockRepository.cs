using OverDrive.Api.Models;
using OverDrive.Api.Repositories.Interfaces;

namespace OverDrive.Api.Repositories;

public class StockRepository : IStockRepository
{
    private readonly OverDriveDbContext _context;

    public StockRepository(OverDriveDbContext context)
    {
        _context = context;
    }

    public Stock? GetById(int stockId)
    {
        return _context.Stocks.FirstOrDefault(s => s.Id == stockId);
    }

    public List<Stock> GetAll()
    {
        return _context.Stocks.OrderByDescending(s => s.Id).ToList();
    }

    public Address? GetAddressById(int addressId)
    {
        return _context.Addresses.FirstOrDefault(a => a.Id == addressId);
    }

    public Branch? GetBranchById(int branchId)
    {
        return _context.Branches.FirstOrDefault(b => b.Id == branchId);
    }

    public bool HasParts(int stockId)
    {
        return _context.Parts.Any(p => p.FkStock == stockId);
    }

    public void Add(Stock stock)
    {
        _context.Stocks.Add(stock);
    }

    public void Remove(Stock stock)
    {
        _context.Stocks.Remove(stock);
    }

    public void SaveChanges()
    {
        _context.SaveChanges();
    }
}
