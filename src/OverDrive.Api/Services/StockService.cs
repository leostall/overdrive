using OverDrive.Api.Dtos.Stocks;
using OverDrive.Api.Exceptions;
using OverDrive.Api.Models;
using OverDrive.Api.Repositories.Interfaces;
using OverDrive.Api.Services.Interfaces;

namespace OverDrive.Api.Services;

public class StockService : IStockService
{
    private readonly IStockRepository _repository;

    public StockService(IStockRepository repository)
    {
        _repository = repository;
    }

    public StockResponse Create(CreateStockRequest request)
    {
        ValidateCreate(request);
        ValidateForeignKeys(request.AddressId, request.BranchId);

        Stock objStock = new()
        {
            Name = request.Name.Trim(),
            Note = NormalizeOptional(request.Note),
            FkBranch = request.BranchId,
            FkAddress = request.AddressId
        };

        _repository.Add(objStock);
        _repository.SaveChanges();

        return MapToResponse(objStock);
    }

    public StockResponse Update(int stockId, UpdateStockRequest request)
    {
        Stock objStock = _repository.GetById(stockId)
            ?? throw new NotFoundException($"Stock not found. Id={stockId}");

        ValidateUpdate(request);

        if (!string.IsNullOrWhiteSpace(request.Name))
            objStock.Name = request.Name.Trim();

        if (request.Note != null)
            objStock.Note = NormalizeOptional(request.Note);

        if (request.AddressId.HasValue)
        {
            Address? objAddress = _repository.GetAddressById(request.AddressId.Value);
            if (objAddress == null)
                throw new NotFoundException($"Address not found. Id={request.AddressId.Value}");

            objStock.FkAddress = request.AddressId.Value;
        }

        if (request.BranchId.HasValue)
        {
            Branch? objBranch = _repository.GetBranchById(request.BranchId.Value);
            if (objBranch == null)
                throw new NotFoundException($"Branch not found. Id={request.BranchId.Value}");

            objStock.FkBranch = request.BranchId.Value;
        }

        _repository.SaveChanges();

        return MapToResponse(objStock);
    }

    public void Delete(int stockId)
    {
        Stock objStock = _repository.GetById(stockId)
            ?? throw new NotFoundException($"Stock not found. Id={stockId}");

        if (_repository.HasParts(stockId))
            throw new BusinessException(ErrorCodes.Sale.Blocked, "Stock cannot be deleted because it has parts linked.");

        _repository.Remove(objStock);
        _repository.SaveChanges();
    }

    public StockResponse GetById(int stockId)
    {
        Stock objStock = _repository.GetById(stockId)
            ?? throw new NotFoundException($"Stock not found. Id={stockId}");

        return MapToResponse(objStock);
    }

    public List<StockResponse> GetAll()
    {
        return _repository.GetAll().Select(MapToResponse).ToList();
    }

    private static void ValidateCreate(CreateStockRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name) || request.AddressId <= 0)
            throw new BusinessException(ErrorCodes.Sale.MissingRequiredFields, "Required stock fields not informed.");

        if (request.BranchId.HasValue && request.BranchId.Value <= 0)
            throw new BusinessException(ErrorCodes.Sale.MissingRequiredFields, "BranchId must be greater than zero.");
    }

    private static void ValidateUpdate(UpdateStockRequest request)
    {
        if (request.AddressId.HasValue && request.AddressId.Value <= 0)
            throw new BusinessException(ErrorCodes.Sale.MissingRequiredFields, "AddressId must be greater than zero.");

        if (request.BranchId.HasValue && request.BranchId.Value <= 0)
            throw new BusinessException(ErrorCodes.Sale.MissingRequiredFields, "BranchId must be greater than zero.");
    }

    private void ValidateForeignKeys(int addressId, int? branchId)
    {
        Address? objAddress = _repository.GetAddressById(addressId);
        if (objAddress == null)
            throw new NotFoundException($"Address not found. Id={addressId}");

        if (branchId.HasValue)
        {
            Branch? objBranch = _repository.GetBranchById(branchId.Value);
            if (objBranch == null)
                throw new NotFoundException($"Branch not found. Id={branchId.Value}");
        }
    }

    private static string? NormalizeOptional(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }

    private static StockResponse MapToResponse(Stock stock)
    {
        return new StockResponse
        {
            StockId = stock.Id,
            Name = stock.Name,
            Note = stock.Note,
            BranchId = stock.FkBranch,
            AddressId = stock.FkAddress
        };
    }
}
