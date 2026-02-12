using OverDrive.Api.Dtos.Employees;
using OverDrive.Api.Exceptions;
using OverDrive.Api.Models;
using OverDrive.Api.Repositories.Interfaces;
using OverDrive.Api.Services.Interfaces;

namespace OverDrive.Api.Services;

public class EmployeeService : IEmployeeService
{
    private readonly IEmployeeRepository _repository;

    public EmployeeService(IEmployeeRepository repository)
    {
        _repository = repository;
    }

    public EmployeeResponse Create(CreateEmployeeRequest request)
    {
        ValidateCreate(request);

        string strRegistration = request.Registration.Trim();
        ValidateUniqueRegistration(strRegistration);

        ValidateForeignKeys(request.AddressId, request.BranchId, request.DepartmentId);

        Employee objEmployee = new()
        {
            Registration = strRegistration,
            Name = request.Name.Trim(),
            CommissionRate = request.CommissionRate,
            Active = request.Active,
            BirthDate = request.BirthDate,
            FkAddress = request.AddressId,
            FkBranch = request.BranchId,
            FkDepartment = request.DepartmentId
        };

        _repository.Add(objEmployee);
        _repository.SaveChanges();

        return MapToResponse(objEmployee);
    }

    public EmployeeResponse Update(int employeeId, UpdateEmployeeRequest request)
    {
        Employee objEmployee = _repository.GetById(employeeId)
            ?? throw new NotFoundException($"Employee not found. Id={employeeId}");

        ValidateUpdate(request);

        if (!string.IsNullOrWhiteSpace(request.Registration))
        {
            string strRegistration = request.Registration.Trim();
            var objSameReg = _repository.GetByRegistration(strRegistration);
            if (objSameReg != null && objSameReg.Id != objEmployee.Id)
                throw new BusinessException(ErrorCodes.Sale.MissingRequiredFields, "Registration already registered.");

            objEmployee.Registration = strRegistration;
        }

        if (!string.IsNullOrWhiteSpace(request.Name))
            objEmployee.Name = request.Name.Trim();

        if (request.CommissionRate.HasValue)
            objEmployee.CommissionRate = request.CommissionRate.Value;

        if (request.Active.HasValue)
            objEmployee.Active = request.Active.Value;

        if (request.BirthDate.HasValue)
            objEmployee.BirthDate = request.BirthDate.Value;

        if (request.AddressId.HasValue)
        {
            var objAddress = _repository.GetAddressById(request.AddressId.Value);
            if (objAddress == null)
                throw new NotFoundException($"Address not found. Id={request.AddressId.Value}");

            objEmployee.FkAddress = request.AddressId.Value;
        }

        if (request.BranchId.HasValue)
        {
            var objBranch = _repository.GetBranchById(request.BranchId.Value);
            if (objBranch == null)
                throw new NotFoundException($"Branch not found. Id={request.BranchId.Value}");

            objEmployee.FkBranch = request.BranchId.Value;
        }

        if (request.DepartmentId.HasValue)
        {
            var objDepartment = _repository.GetDepartmentById(request.DepartmentId.Value);
            if (objDepartment == null)
                throw new NotFoundException($"Department not found. Id={request.DepartmentId.Value}");

            if (!objDepartment.Active)
                throw new BusinessException(ErrorCodes.Sale.Blocked, "Department is inactive.");

            objEmployee.FkDepartment = request.DepartmentId.Value;
        }

        _repository.SaveChanges();

        return MapToResponse(objEmployee);
    }

    public void Delete(int employeeId)
    {
        Employee objEmployee = _repository.GetById(employeeId)
            ?? throw new NotFoundException($"Employee not found. Id={employeeId}");

        if (_repository.HasSales(employeeId))
        {
            objEmployee.Active = false;
            _repository.SaveChanges();
            return;
        }

        _repository.Remove(objEmployee);
        _repository.SaveChanges();
    }

    public EmployeeResponse GetById(int employeeId)
    {
        Employee objEmployee = _repository.GetById(employeeId)
            ?? throw new NotFoundException($"Employee not found. Id={employeeId}");

        return MapToResponse(objEmployee);
    }

    public List<EmployeeResponse> GetAll()
    {
        return _repository.GetAll().Select(MapToResponse).ToList();
    }

    private void ValidateCreate(CreateEmployeeRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Registration)
            || string.IsNullOrWhiteSpace(request.Name)
            || request.DepartmentId <= 0)
        {
            throw new BusinessException(ErrorCodes.Sale.MissingRequiredFields, "Required employee fields not informed.");
        }

        if (request.CommissionRate < 0)
            throw new BusinessException(ErrorCodes.Item.DiscountNegative, "CommissionRate must be non-negative.");

        if (request.AddressId.HasValue && request.AddressId.Value <= 0)
            throw new BusinessException(ErrorCodes.Sale.MissingRequiredFields, "AddressId must be greater than zero.");

        if (request.BranchId.HasValue && request.BranchId.Value <= 0)
            throw new BusinessException(ErrorCodes.Sale.MissingRequiredFields, "BranchId must be greater than zero.");
    }

    private void ValidateUpdate(UpdateEmployeeRequest request)
    {
        if (request.CommissionRate.HasValue && request.CommissionRate.Value < 0)
            throw new BusinessException(ErrorCodes.Item.DiscountNegative, "CommissionRate must be non-negative.");

        if (request.AddressId.HasValue && request.AddressId.Value <= 0)
            throw new BusinessException(ErrorCodes.Sale.MissingRequiredFields, "AddressId must be greater than zero.");

        if (request.BranchId.HasValue && request.BranchId.Value <= 0)
            throw new BusinessException(ErrorCodes.Sale.MissingRequiredFields, "BranchId must be greater than zero.");

        if (request.DepartmentId.HasValue && request.DepartmentId.Value <= 0)
            throw new BusinessException(ErrorCodes.Sale.MissingRequiredFields, "DepartmentId must be greater than zero.");
    }

    private void ValidateForeignKeys(int? addressId, int? branchId, int departmentId)
    {
        if (addressId.HasValue)
        {
            var objAddress = _repository.GetAddressById(addressId.Value);
            if (objAddress == null)
                throw new NotFoundException($"Address not found. Id={addressId.Value}");
        }

        if (branchId.HasValue)
        {
            var objBranch = _repository.GetBranchById(branchId.Value);
            if (objBranch == null)
                throw new NotFoundException($"Branch not found. Id={branchId.Value}");

            if (!objBranch.Active)
                throw new BusinessException(ErrorCodes.Sale.Blocked, "Branch is inactive.");
        }

        var objDepartment = _repository.GetDepartmentById(departmentId);
        if (objDepartment == null)
            throw new NotFoundException($"Department not found. Id={departmentId}");

        if (!objDepartment.Active)
            throw new BusinessException(ErrorCodes.Sale.Blocked, "Department is inactive.");
    }

    private void ValidateUniqueRegistration(string registration)
    {
        var objEmployee = _repository.GetByRegistration(registration);
        if (objEmployee != null)
            throw new BusinessException(ErrorCodes.Sale.MissingRequiredFields, "Registration already registered.");
    }

    private static EmployeeResponse MapToResponse(Employee employee)
    {
        return new EmployeeResponse
        {
            EmployeeId = employee.Id,
            Registration = employee.Registration,
            Name = employee.Name,
            CommissionRate = employee.CommissionRate,
            Active = employee.Active,
            BirthDate = employee.BirthDate,
            AddressId = employee.FkAddress,
            BranchId = employee.FkBranch,
            DepartmentId = employee.FkDepartment
        };
    }
}
