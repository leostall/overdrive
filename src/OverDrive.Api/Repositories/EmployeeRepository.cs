using OverDrive.Api.Models;
using OverDrive.Api.Repositories.Interfaces;

namespace OverDrive.Api.Repositories;

public class EmployeeRepository : IEmployeeRepository
{
    private readonly OverDriveDbContext _context;

    public EmployeeRepository(OverDriveDbContext context)
    {
        _context = context;
    }

    public Employee? GetById(int employeeId)
    {
        return _context.Employees.FirstOrDefault(e => e.Id == employeeId);
    }

    public List<Employee> GetAll()
    {
        return _context.Employees.OrderByDescending(e => e.Id).ToList();
    }

    public Employee? GetByRegistration(string registration)
    {
        return _context.Employees.FirstOrDefault(e => e.Registration == registration);
    }

    public Address? GetAddressById(int addressId)
    {
        return _context.Addresses.FirstOrDefault(a => a.Id == addressId);
    }

    public Branch? GetBranchById(int branchId)
    {
        return _context.Branches.FirstOrDefault(b => b.Id == branchId);
    }

    public Department? GetDepartmentById(int departmentId)
    {
        return _context.Departments.FirstOrDefault(d => d.Id == departmentId);
    }

    public bool HasSales(int employeeId)
    {
        return _context.Sales.Any(s => s.FkEmployee == employeeId);
    }

    public void Add(Employee employee)
    {
        _context.Employees.Add(employee);
    }

    public void Remove(Employee employee)
    {
        _context.Employees.Remove(employee);
    }

    public void SaveChanges()
    {
        _context.SaveChanges();
    }
}
