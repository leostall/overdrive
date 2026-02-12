using OverDrive.Api.Models;
using OverDrive.Api.Repositories.Interfaces;

namespace OverDrive.Api.Repositories;

public class StatusRepository : IStatusRepository
{
    private readonly OverDriveDbContext _context;

    public StatusRepository(OverDriveDbContext context)
    {
        _context = context;
    }

    public Status? GetById(int statusId)
    {
        return _context.Statuses.FirstOrDefault(s => s.Id == statusId);
    }

    public List<Status> GetAll()
    {
        return _context.Statuses.OrderByDescending(s => s.Id).ToList();
    }

    public Status? GetByDescription(string description)
    {
        return _context.Statuses.FirstOrDefault(s => s.Description == description);
    }

    public bool HasSales(int statusId)
    {
        return _context.Sales.Any(s => s.FkStatus == statusId);
    }

    public void Add(Status status)
    {
        _context.Statuses.Add(status);
    }

    public void Remove(Status status)
    {
        _context.Statuses.Remove(status);
    }

    public void SaveChanges()
    {
        _context.SaveChanges();
    }
}
