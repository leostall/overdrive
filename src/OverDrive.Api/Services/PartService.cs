using OverDrive.Api.Dtos.Parts;
using OverDrive.Api.Exceptions;
using OverDrive.Api.Models;
using OverDrive.Api.Repositories.Interfaces;
using OverDrive.Api.Services.Interfaces;

namespace OverDrive.Api.Services;

public class PartService : IPartService
{
    private readonly IPartRepository _repository;

    public PartService(IPartRepository repository)
    {
        _repository = repository;
    }

    public PartResponse Create(CreatePartRequest request)
    {
        ValidateCreate(request);

        string strCode = request.Code.Trim();
        Part? objExists = _repository.GetByCode(strCode);
        if (objExists != null)
            throw new BusinessException(ErrorCodes.Sale.MissingRequiredFields, "Part code already registered.");

        Stock? objStock = _repository.GetStockById(request.StockId);
        if (objStock == null)
            throw new NotFoundException($"Stock not found. Id={request.StockId}");

        Part objPart = new()
        {
            Code = strCode,
            Description = request.Description.Trim(),
            Quantity = request.Quantity,
            Price = request.Price,
            FkStock = request.StockId
        };

        _repository.Add(objPart);
        _repository.SaveChanges();

        return MapToResponse(objPart);
    }

    public PartResponse Update(int partId, UpdatePartRequest request)
    {
        Part objPart = _repository.GetById(partId)
            ?? throw new NotFoundException($"Part not found. Id={partId}");

        ValidateUpdate(request);

        if (!string.IsNullOrWhiteSpace(request.Code))
        {
            string strCode = request.Code.Trim();
            Part? objExists = _repository.GetByCode(strCode);
            if (objExists != null && objExists.Id != objPart.Id)
                throw new BusinessException(ErrorCodes.Sale.MissingRequiredFields, "Part code already registered.");

            objPart.Code = strCode;
        }

        if (!string.IsNullOrWhiteSpace(request.Description))
            objPart.Description = request.Description.Trim();

        if (request.Quantity.HasValue)
            objPart.Quantity = request.Quantity.Value;

        if (request.Price.HasValue)
            objPart.Price = request.Price.Value;

        if (request.StockId.HasValue)
        {
            Stock? objStock = _repository.GetStockById(request.StockId.Value);
            if (objStock == null)
                throw new NotFoundException($"Stock not found. Id={request.StockId.Value}");

            objPart.FkStock = request.StockId.Value;
        }

        _repository.SaveChanges();

        return MapToResponse(objPart);
    }

    public void Delete(int partId)
    {
        Part objPart = _repository.GetById(partId)
            ?? throw new NotFoundException($"Part not found. Id={partId}");

        if (_repository.HasSaleItems(partId) || _repository.HasUseParts(partId))
            throw new BusinessException(ErrorCodes.Sale.Blocked, "Part cannot be deleted because it is linked to sales/service orders.");

        _repository.Remove(objPart);
        _repository.SaveChanges();
    }

    public PartResponse GetById(int partId)
    {
        Part objPart = _repository.GetById(partId)
            ?? throw new NotFoundException($"Part not found. Id={partId}");

        return MapToResponse(objPart);
    }

    public List<PartResponse> GetAll()
    {
        return _repository.GetAll().Select(MapToResponse).ToList();
    }

    private static void ValidateCreate(CreatePartRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Code)
            || string.IsNullOrWhiteSpace(request.Description)
            || request.StockId <= 0)
        {
            throw new BusinessException(ErrorCodes.Sale.MissingRequiredFields, "Required part fields not informed.");
        }

        if (request.Quantity < 0)
            throw new BusinessException(ErrorCodes.Item.QuantityInvalid, "Quantity must be non-negative.");

        if (request.Price < 0)
            throw new BusinessException(ErrorCodes.Item.DiscountNegative, "Price must be non-negative.");
    }

    private static void ValidateUpdate(UpdatePartRequest request)
    {
        if (request.StockId.HasValue && request.StockId.Value <= 0)
            throw new BusinessException(ErrorCodes.Sale.MissingRequiredFields, "StockId must be greater than zero.");

        if (request.Quantity.HasValue && request.Quantity.Value < 0)
            throw new BusinessException(ErrorCodes.Item.QuantityInvalid, "Quantity must be non-negative.");

        if (request.Price.HasValue && request.Price.Value < 0)
            throw new BusinessException(ErrorCodes.Item.DiscountNegative, "Price must be non-negative.");
    }

    private static PartResponse MapToResponse(Part part)
    {
        return new PartResponse
        {
            PartId = part.Id,
            Code = part.Code,
            Description = part.Description,
            Quantity = part.Quantity,
            Price = part.Price,
            StockId = part.FkStock
        };
    }
}
