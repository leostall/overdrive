using Microsoft.EntityFrameworkCore;
using OverDrive.Api.Models;
using OverDrive.Api.Repositories.Interfaces;

namespace OverDrive.Api.Repositories;

public class SaleRepository : ISaleRepository
{
    private readonly OverDriveDbContext _context;

    public SaleRepository(OverDriveDbContext context)
    {
        _context = context;
    }

    public Customer? GetCustomerById(int customerId)
    {
        return _context.Customers.FirstOrDefault(c => c.Id == customerId);
    }

    public Customer? GetCustomerByCnpj(string cnpj)
    {
        return _context.Customers.FirstOrDefault(c => c.CpfCnpj == cnpj);
    }

    public Part? GetPartById(int partId)
    {
        return _context.Parts.FirstOrDefault(p => p.Id == partId);
    }

    public Vehicle? GetVehicleById(int vehicleId)
    {
        return _context.Vehicles.FirstOrDefault(v => v.Id == vehicleId);
    }

    public Branch? GetBranchById(int branchId)
    {
        return _context.Branches.FirstOrDefault(b => b.Id == branchId);
    }

    public List<Part> GetPartsByIds(HashSet<int> hsPartIds)
    {
        if (hsPartIds.Count == 0)
        {
            return new List<Part>();
        }

        return _context.Parts.Where(p => hsPartIds.Contains(p.Id)).ToList();
    }

    public List<Vehicle> GetVehiclesByIds(HashSet<int> hsVehicleIds)
    {
        if (hsVehicleIds.Count == 0)
        {
            return new List<Vehicle>();
        }

        return _context.Vehicles.Where(v => hsVehicleIds.Contains(v.Id)).ToList();
    }

    public Sale? GetSaleById(int saleId)
    {
        return _context.Sales
            .Include(s => s.SaleItems)
                .ThenInclude(si => si.FkPartNavigation)
            .Include(s => s.SaleItems)
                .ThenInclude(si => si.FkVehicleNavigation)
            .FirstOrDefault(s => s.Id == saleId);
    }

    public List<Sale> GetAllSales()
    {
        return _context.Sales
            .Include(s => s.SaleItems)
                .ThenInclude(si => si.FkPartNavigation)
            .Include(s => s.SaleItems)
                .ThenInclude(si => si.FkVehicleNavigation)
            .ToList();
    }

    public void AddSale(Sale objSale)
    {
        _context.Sales.Add(objSale);
    }

    public void SaveChanges()
    {
        _context.SaveChanges();
    }
}