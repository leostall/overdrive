using OverDrive.Api.Models;
using OverDrive.Api.Repositories.Interfaces;

namespace OverDrive.Api.Repositories;

public class PartRepository : IPartRepository
{
    private readonly OverDriveDbContext _context;

    public PartRepository(OverDriveDbContext context)
    {
        _context = context;
    }

    public Part? GetById(int partId)
    {
        return _context.Parts.FirstOrDefault(p => p.Id == partId);
    }

    public List<Part> GetAll()
    {
        return _context.Parts.OrderByDescending(p => p.Id).ToList();
    }

    public Part? GetByCode(string code)
    {
        return _context.Parts.FirstOrDefault(p => p.Code == code);
    }

    public Stock? GetStockById(int stockId)
    {
        return _context.Stocks.FirstOrDefault(s => s.Id == stockId);
    }

    public bool HasSaleItems(int partId)
    {
        return _context.SaleItems.Any(si => si.FkPart == partId);
    }

    public bool HasUseParts(int partId)
    {
        return _context.UseParts.Any(up => up.FkPart == partId);
    }

    public void Add(Part part)
    {
        _context.Parts.Add(part);
    }

    public void Remove(Part part)
    {
        _context.Parts.Remove(part);
    }

    public void SaveChanges()
    {
        _context.SaveChanges();
    }
}
