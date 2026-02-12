using Microsoft.EntityFrameworkCore;
using OverDrive.Api.Models;
using OverDrive.Api.Repositories.Interfaces;

namespace OverDrive.Api.Repositories;

public class ServiceOrderRepository : IServiceOrderRepository
{
    private readonly OverDriveDbContext _context;

    public ServiceOrderRepository(OverDriveDbContext context)
    {
        _context = context;
    }

    public Vehicle? GetVehicleById(int vehicleId)
    {
        return _context.Vehicles.FirstOrDefault(v => v.Id == vehicleId);
    }

    public Branch? GetBranchById(int branchId)
    {
        return _context.Branches.FirstOrDefault(b => b.Id == branchId);
    }

    public Part? GetPartById(int partId)
    {
        return _context.Parts.FirstOrDefault(p => p.Id == partId);
    }

    public List<Part> GetPartsByIds(HashSet<int> hsPartIds)
    {
        if (hsPartIds.Count == 0)
        {
            return new List<Part>();
        }

        return _context.Parts.Where(p => hsPartIds.Contains(p.Id)).ToList();
    }

    public ServiceOrder? GetServiceOrderById(int serviceOrderId)
    {
        return _context.ServiceOrders
            .Include(s => s.ServiceOrderItems)
                .ThenInclude(i => i.UseParts)
            .FirstOrDefault(s => s.Id == serviceOrderId);
    }

    public List<ServiceOrder> GetAllServiceOrders()
    {
        return _context.ServiceOrders
            .Include(s => s.ServiceOrderItems)
                .ThenInclude(i => i.UseParts)
            .ToList();
    }

    public void AddServiceOrder(ServiceOrder serviceOrder)
    {
        _context.ServiceOrders.Add(serviceOrder);
    }

    public void SaveChanges()
    {
        _context.SaveChanges();
    }
}