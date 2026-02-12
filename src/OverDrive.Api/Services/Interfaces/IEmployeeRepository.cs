using OverDrive.Api.Models;

namespace OverDrive.Api.Repositories.Interfaces;

public interface IEmployeeRepository
{
    Employee? GetById(int employeeId);
    List<Employee> GetAll();
    Employee? GetByRegistration(string registration);
    Address? GetAddressById(int addressId);
    Branch? GetBranchById(int branchId);
    Department? GetDepartmentById(int departmentId);
    bool HasSales(int employeeId);
    void Add(Employee employee);
    void Remove(Employee employee);
    void SaveChanges();
}
