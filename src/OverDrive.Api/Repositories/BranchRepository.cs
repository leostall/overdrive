using OverDrive.Api.Dtos.Branches;
using OverDrive.Api.Models;
using OverDrive.Api.Repositories.Interfaces;

namespace OverDrive.Api.Repositories;

public class BranchRepository : IBranchRepository
{
    private readonly OverDriveDbContext _context;

    public BranchRepository(OverDriveDbContext context)
    {
        _context = context;
    }

    public Branch? GetById(int branchId)
    {
        return _context.Branches.FirstOrDefault(b => b.Id == branchId);
    }

    public List<Branch> GetAll()
    {
        return _context.Branches.OrderByDescending(b => b.Id).ToList();
    }

    public Address? GetAddressById(int addressId)
    {
        return _context.Addresses.FirstOrDefault(a => a.Id == addressId);
    }

    public Branch? GetByCnpj(string cnpj)
    {
        return _context.Branches.FirstOrDefault(b => b.Cnpj == cnpj);
    }

    public bool HasSales(int branchId)
    {
        return _context.Sales.Any(s => s.FkBranch == branchId);
    }

    public bool HasServiceOrders(int branchId)
    {
        return _context.ServiceOrders.Any(so => so.FkBranch == branchId);
    }

    public bool HasEmployees(int branchId)
    {
        return _context.Employees.Any(e => e.FkBranch == branchId);
    }

    public bool HasStocks(int branchId)
    {
        return _context.Stocks.Any(st => st.FkBranch == branchId);
    }

    public BranchReportResponse? GetReportByBranchId(int branchId)
    {
        Branch? objBranch = _context.Branches.FirstOrDefault(b => b.Id == branchId);
        if (objBranch == null) return null;

        int intEmployeesCount = _context.Employees.Count(e => e.FkBranch == branchId);
        int intSalesCount = _context.Sales.Count(s => s.FkBranch == branchId);
        int intOpenOrders = _context.ServiceOrders.Count(so => so.FkBranch == branchId
            && so.CloseDate == null
            && so.Status != "CLOSED"
            && so.Status != "CANCELED"
            && so.Status != "FECHADA"
            && so.Status != "CANCELADA");

        IQueryable<Part> qryParts = _context.Parts.Where(p => p.FkStockNavigation.FkBranch == branchId);
        int intStockPartsQuantity = qryParts.Sum(p => (int?)p.Quantity) ?? 0;
        decimal decStockValue = qryParts.Sum(p => (decimal?)(p.Quantity * p.Price)) ?? 0m;

        return new BranchReportResponse
        {
            BranchId = objBranch.Id,
            CorporateName = objBranch.CorporateName,
            EmployeesCount = intEmployeesCount,
            SalesCount = intSalesCount,
            OpenServiceOrdersCount = intOpenOrders,
            StockPartsQuantity = intStockPartsQuantity,
            StockTotalValue = decStockValue
        };
    }

    public List<BranchReportResponse> GetReports()
    {
        return _context.Branches
            .Select(b => new BranchReportResponse
            {
                BranchId = b.Id,
                CorporateName = b.CorporateName,
                EmployeesCount = _context.Employees.Count(e => e.FkBranch == b.Id),
                SalesCount = _context.Sales.Count(s => s.FkBranch == b.Id),
                OpenServiceOrdersCount = _context.ServiceOrders.Count(so => so.FkBranch == b.Id
                    && so.CloseDate == null
                    && so.Status != "CLOSED"
                    && so.Status != "CANCELED"
                    && so.Status != "FECHADA"
                    && so.Status != "CANCELADA"),
                StockPartsQuantity = _context.Parts
                    .Where(p => p.FkStockNavigation.FkBranch == b.Id)
                    .Sum(p => (int?)p.Quantity) ?? 0,
                StockTotalValue = _context.Parts
                    .Where(p => p.FkStockNavigation.FkBranch == b.Id)
                    .Sum(p => (decimal?)(p.Quantity * p.Price)) ?? 0m
            })
            .OrderBy(r => r.BranchId)
            .ToList();
    }

    public void Add(Branch branch)
    {
        _context.Branches.Add(branch);
    }

    public void Remove(Branch branch)
    {
        _context.Branches.Remove(branch);
    }

    public void SaveChanges()
    {
        _context.SaveChanges();
    }
}
