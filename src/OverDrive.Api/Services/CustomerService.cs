using OverDrive.Api.Dtos.Customers;
using OverDrive.Api.Exceptions;
using OverDrive.Api.Models;
using OverDrive.Api.Repositories.Interfaces;
using OverDrive.Api.Services.Interfaces;

namespace OverDrive.Api.Services;

public class CustomerService : ICustomerService
{
    private readonly ICustomerRepository _repository;

    public CustomerService(ICustomerRepository repository)
    {
        _repository = repository;
    }

    public CustomerResponse Create(CreateCustomerRequest request)
    {
        ValidateCreate(request);

        string strCpfCnpj = request.CpfCnpj.Trim();
        ValidateUniqueCpfCnpj(strCpfCnpj);

        var objAddress = _repository.GetAddressById(request.AddressId);
        if (objAddress == null)
            throw new NotFoundException($"Address not found. Id={request.AddressId}");

        Customer objCustomer = new()
        {
            CpfCnpj = strCpfCnpj,
            CustomerType = request.CustomerType.Trim().ToUpperInvariant(),
            Name = request.Name.Trim(),
            Phone = NormalizeOptional(request.Phone),
            Email = NormalizeOptional(request.Email),
            Active = request.Active,
            FkAddress = request.AddressId
        };

        _repository.Add(objCustomer);
        _repository.SaveChanges();

        return MapToResponse(objCustomer);
    }

    public CustomerResponse Update(int customerId, UpdateCustomerRequest request)
    {
        Customer objCustomer = _repository.GetById(customerId)
            ?? throw new NotFoundException($"Customer not found. Id={customerId}");

        ValidateUpdate(request);

        if (request.AddressId.HasValue)
        {
            var objAddress = _repository.GetAddressById(request.AddressId.Value);
            if (objAddress == null)
                throw new NotFoundException($"Address not found. Id={request.AddressId.Value}");

            objCustomer.FkAddress = request.AddressId.Value;
        }

        if (!string.IsNullOrWhiteSpace(request.CpfCnpj))
        {
            string strCpfCnpj = request.CpfCnpj.Trim();
            Customer? objSameCpf = _repository.GetByCpfCnpj(strCpfCnpj);
            if (objSameCpf != null && objSameCpf.Id != objCustomer.Id)
                throw new BusinessException(ErrorCodes.Sale.MissingRequiredFields, "CpfCnpj already registered.");

            objCustomer.CpfCnpj = strCpfCnpj;
        }

        if (!string.IsNullOrWhiteSpace(request.CustomerType))
            objCustomer.CustomerType = request.CustomerType.Trim().ToUpperInvariant();

        if (!string.IsNullOrWhiteSpace(request.Name))
            objCustomer.Name = request.Name.Trim();

        if (request.Phone != null)
            objCustomer.Phone = NormalizeOptional(request.Phone);

        if (request.Email != null)
            objCustomer.Email = NormalizeOptional(request.Email);

        if (request.Active.HasValue)
            objCustomer.Active = request.Active.Value;

        _repository.SaveChanges();

        return MapToResponse(objCustomer);
    }

    public void Delete(int customerId)
    {
        Customer objCustomer = _repository.GetById(customerId)
            ?? throw new NotFoundException($"Customer not found. Id={customerId}");

        if (_repository.HasSales(customerId) || _repository.HasVehicles(customerId))
        {
            objCustomer.Active = false;
            _repository.SaveChanges();
            return;
        }

        _repository.Remove(objCustomer);
        _repository.SaveChanges();
    }

    public CustomerResponse GetById(int customerId)
    {
        Customer objCustomer = _repository.GetById(customerId)
            ?? throw new NotFoundException($"Customer not found. Id={customerId}");

        return MapToResponse(objCustomer);
    }

    public List<CustomerResponse> GetAll()
    {
        return _repository.GetAll().Select(MapToResponse).ToList();
    }

    private void ValidateCreate(CreateCustomerRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.CpfCnpj)
            || string.IsNullOrWhiteSpace(request.CustomerType)
            || string.IsNullOrWhiteSpace(request.Name)
            || request.AddressId <= 0)
        {
            throw new BusinessException(ErrorCodes.Sale.MissingRequiredFields, "Required customer fields not informed.");
        }
    }

    private void ValidateUpdate(UpdateCustomerRequest request)
    {
        if (request.AddressId.HasValue && request.AddressId <= 0)
            throw new BusinessException(ErrorCodes.Sale.MissingRequiredFields, "AddressId must be greater than zero.");
    }

    private void ValidateUniqueCpfCnpj(string cpfCnpj)
    {
        var objCustomer = _repository.GetByCpfCnpj(cpfCnpj);
        if (objCustomer != null)
            throw new BusinessException(ErrorCodes.Sale.MissingRequiredFields, "CpfCnpj already registered.");
    }

    private static string? NormalizeOptional(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }

    private static CustomerResponse MapToResponse(Customer customer)
    {
        return new CustomerResponse
        {
            CustomerId = customer.Id,
            CpfCnpj = customer.CpfCnpj,
            CustomerType = customer.CustomerType,
            Name = customer.Name,
            Phone = customer.Phone,
            Email = customer.Email,
            Active = customer.Active,
            AddressId = customer.FkAddress
        };
    }
}
