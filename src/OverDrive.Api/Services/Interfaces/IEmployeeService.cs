using OverDrive.Api.Dtos.Employees;

namespace OverDrive.Api.Services.Interfaces;

public interface IEmployeeService
{
    EmployeeResponse Create(CreateEmployeeRequest request);
    EmployeeResponse Update(int employeeId, UpdateEmployeeRequest request);
    void Delete(int employeeId);
    EmployeeResponse GetById(int employeeId);
    List<EmployeeResponse> GetAll();
}
