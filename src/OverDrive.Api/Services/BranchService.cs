using OverDrive.Api.Dtos.Branches;
using OverDrive.Api.Exceptions;
using OverDrive.Api.Models;
using OverDrive.Api.Repositories.Interfaces;
using OverDrive.Api.Services.Interfaces;

namespace OverDrive.Api.Services;

public class BranchService : IBranchService
{
    private readonly IBranchRepository _repository;

    public BranchService(IBranchRepository repository)
    {
        _repository = repository;
    }

    public BranchResponse Create(CreateBranchRequest request)
    {
        ValidateCreate(request);

        string strCnpj = request.Cnpj.Trim();
        ValidateUniqueCnpj(strCnpj);

        Address? objAddress = _repository.GetAddressById(request.AddressId);
        if (objAddress == null)
            throw new NotFoundException($"Address not found. Id={request.AddressId}");

        Branch objBranch = new()
        {
            CorporateName = request.CorporateName.Trim(),
            Cnpj = strCnpj,
            Phone = NormalizeOptional(request.Phone),
            Email = NormalizeOptional(request.Email),
            Active = request.Active,
            FkAddress = request.AddressId
        };

        _repository.Add(objBranch);
        _repository.SaveChanges();

        return MapToResponse(objBranch);
    }

    public BranchResponse Update(int branchId, UpdateBranchRequest request)
    {
        Branch objBranch = _repository.GetById(branchId)
            ?? throw new NotFoundException($"Branch not found. Id={branchId}");

        ValidateUpdate(request);

        if (request.AddressId.HasValue)
        {
            Address? objAddress = _repository.GetAddressById(request.AddressId.Value);
            if (objAddress == null)
                throw new NotFoundException($"Address not found. Id={request.AddressId.Value}");

            objBranch.FkAddress = request.AddressId.Value;
        }

        if (!string.IsNullOrWhiteSpace(request.Cnpj))
        {
            string strCnpj = request.Cnpj.Trim();
            Branch? objSame = _repository.GetByCnpj(strCnpj);
            if (objSame != null && objSame.Id != objBranch.Id)
                throw new BusinessException(ErrorCodes.Sale.MissingRequiredFields, "Cnpj already registered.");

            objBranch.Cnpj = strCnpj;
        }

        if (!string.IsNullOrWhiteSpace(request.CorporateName))
            objBranch.CorporateName = request.CorporateName.Trim();

        if (request.Phone != null)
            objBranch.Phone = NormalizeOptional(request.Phone);

        if (request.Email != null)
            objBranch.Email = NormalizeOptional(request.Email);

        if (request.Active.HasValue)
            objBranch.Active = request.Active.Value;

        _repository.SaveChanges();

        return MapToResponse(objBranch);
    }

    public void Delete(int branchId)
    {
        Branch objBranch = _repository.GetById(branchId)
            ?? throw new NotFoundException($"Branch not found. Id={branchId}");

        if (_repository.HasEmployees(branchId)
            || _repository.HasSales(branchId)
            || _repository.HasServiceOrders(branchId)
            || _repository.HasStocks(branchId))
        {
            objBranch.Active = false;
            _repository.SaveChanges();
            return;
        }

        _repository.Remove(objBranch);
        _repository.SaveChanges();
    }

    public BranchResponse GetById(int branchId)
    {
        Branch objBranch = _repository.GetById(branchId)
            ?? throw new NotFoundException($"Branch not found. Id={branchId}");

        return MapToResponse(objBranch);
    }

    public List<BranchResponse> GetAll()
    {
        return _repository.GetAll().Select(MapToResponse).ToList();
    }

    public BranchReportResponse GetReportById(int branchId)
    {
        BranchReportResponse? objReport = _repository.GetReportByBranchId(branchId);
        if (objReport == null)
            throw new NotFoundException($"Branch not found. Id={branchId}");

        return objReport;
    }

    public List<BranchReportResponse> GetReports()
    {
        return _repository.GetReports();
    }

    private void ValidateCreate(CreateBranchRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.CorporateName)
            || string.IsNullOrWhiteSpace(request.Cnpj)
            || request.AddressId <= 0)
        {
            throw new BusinessException(ErrorCodes.Sale.MissingRequiredFields, "Required branch fields not informed.");
        }
    }

    private void ValidateUpdate(UpdateBranchRequest request)
    {
        if (request.AddressId.HasValue && request.AddressId.Value <= 0)
            throw new BusinessException(ErrorCodes.Sale.MissingRequiredFields, "AddressId must be greater than zero.");
    }

    private void ValidateUniqueCnpj(string cnpj)
    {
        Branch? objBranch = _repository.GetByCnpj(cnpj);
        if (objBranch != null)
            throw new BusinessException(ErrorCodes.Sale.MissingRequiredFields, "Cnpj already registered.");
    }

    private static string? NormalizeOptional(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }

    private static BranchResponse MapToResponse(Branch branch)
    {
        return new BranchResponse
        {
            BranchId = branch.Id,
            CorporateName = branch.CorporateName,
            Cnpj = branch.Cnpj,
            Phone = branch.Phone,
            Email = branch.Email,
            Active = branch.Active,
            AddressId = branch.FkAddress
        };
    }
}
