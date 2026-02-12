using OverDrive.Api.Dtos.ServiceOrders;
using OverDrive.Api.Exceptions;
using OverDrive.Api.Models;
using OverDrive.Api.Repositories.Interfaces;
using OverDrive.Api.Services.Interfaces;

namespace OverDrive.Api.Services;

public class ServiceOrderService : IServiceOrderService
{
    private readonly IServiceOrderRepository _repository;
    private readonly OverDriveDbContext _context;

    public ServiceOrderService(IServiceOrderRepository repository, OverDriveDbContext context)
    {
        _repository = repository;
        _context = context;
    }

    public ServiceOrderResponse Create(CreateServiceOrderRequest request)
    {
        ValidateRequiredFields(request);

        var objVehicle = _repository.GetVehicleById(request.VehicleId);
        if (objVehicle == null)
            throw new NotFoundException($"Vehicle not found. Id={request.VehicleId}");

        if (request.BranchId.HasValue)
        {
            var objBranch = _repository.GetBranchById(request.BranchId.Value);
            if (objBranch == null)
                throw new NotFoundException($"Branch not found. Id={request.BranchId.Value}");
        }

        HashSet<int> hsPartIds = GetPartIdsFromRequest(request.Items);
        Dictionary<int, Part> dicParts = _repository.GetPartsByIds(hsPartIds).ToDictionary(p => p.Id, p => p);

        using var tx = _context.Database.BeginTransaction();
        try
        {
            ServiceOrder objServiceOrder = new()
            {
                Number = request.Number.Trim(),
                OpenDate = request.OpenDate ?? DateTime.Now,
                Status = string.IsNullOrWhiteSpace(request.Status) ? "ABERTA" : request.Status.Trim().ToUpperInvariant(),
                Notes = request.Notes,
                FkVehicle = request.VehicleId,
                FkBranch = request.BranchId,
                TotalValue = 0m
            };

            foreach (var objItemReq in request.Items)
            {
                ServiceOrderItem objItem = BuildServiceOrderItem(objItemReq, dicParts);
                objServiceOrder.ServiceOrderItems.Add(objItem);
            }

            objServiceOrder.TotalValue = objServiceOrder.ServiceOrderItems.Sum(i => i.Subtotal);

            _repository.AddServiceOrder(objServiceOrder);
            _repository.SaveChanges();
            tx.Commit();

            return MapToResponse(objServiceOrder);
        }
        catch
        {
            tx.Rollback();
            throw;
        }
    }

    public ServiceOrderResponse GetById(int serviceOrderId)
    {
        var objServiceOrder = _repository.GetServiceOrderById(serviceOrderId);
        if (objServiceOrder == null)
            throw new NotFoundException($"Service order not found. Id={serviceOrderId}");

        return MapToResponse(objServiceOrder);
    }

    public List<ServiceOrderResponse> GetAll()
    {
        return _repository.GetAllServiceOrders().Select(MapToResponse).ToList();
    }

    public ServiceOrderResponse Update(int serviceOrderId, UpdateServiceOrderRequest request)
    {
        var objServiceOrder = _repository.GetServiceOrderById(serviceOrderId);
        if (objServiceOrder == null)
            throw new NotFoundException($"Service order not found. Id={serviceOrderId}");

        using var tx = _context.Database.BeginTransaction();
        try
        {
            if (request.VehicleId.HasValue)
            {
                var objVehicle = _repository.GetVehicleById(request.VehicleId.Value);
                if (objVehicle == null)
                    throw new NotFoundException($"Vehicle not found. Id={request.VehicleId.Value}");
                objServiceOrder.FkVehicle = request.VehicleId.Value;
            }

            if (request.BranchId.HasValue)
            {
                var objBranch = _repository.GetBranchById(request.BranchId.Value);
                if (objBranch == null)
                    throw new NotFoundException($"Branch not found. Id={request.BranchId.Value}");
            }

            objServiceOrder.Number = string.IsNullOrWhiteSpace(request.Number) ? objServiceOrder.Number : request.Number.Trim();
            objServiceOrder.OpenDate = request.OpenDate ?? objServiceOrder.OpenDate;
            objServiceOrder.CloseDate = request.CloseDate ?? objServiceOrder.CloseDate;
            objServiceOrder.Status = string.IsNullOrWhiteSpace(request.Status) ? objServiceOrder.Status : request.Status.Trim().ToUpperInvariant();
            objServiceOrder.Notes = request.Notes ?? objServiceOrder.Notes;
            objServiceOrder.FkBranch = request.BranchId ?? objServiceOrder.FkBranch;

            if (request.Items != null)
            {
                UpdateItems(objServiceOrder, request.Items);
            }

            objServiceOrder.TotalValue = objServiceOrder.ServiceOrderItems.Sum(i => i.Subtotal);

            _repository.SaveChanges();
            tx.Commit();

            return MapToResponse(objServiceOrder);
        }
        catch
        {
            tx.Rollback();
            throw;
        }
    }

    public void Delete(int serviceOrderId)
    {
        var objServiceOrder = _repository.GetServiceOrderById(serviceOrderId);
        if (objServiceOrder == null)
            throw new NotFoundException($"Service order not found. Id={serviceOrderId}");

        using var tx = _context.Database.BeginTransaction();
        try
        {
            RestoreAndDeleteItems(objServiceOrder.ServiceOrderItems.ToList());
            _context.ServiceOrders.Remove(objServiceOrder);
            _repository.SaveChanges();
            tx.Commit();
        }
        catch
        {
            tx.Rollback();
            throw;
        }
    }

    private static void ValidateRequiredFields(CreateServiceOrderRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Number) || request.VehicleId <= 0)
            throw new BusinessException(ErrorCodes.Sale.MissingRequiredFields, "Number and VehicleId are required.");

        if (request.Items == null || request.Items.Count == 0)
            throw new BusinessException(ErrorCodes.Sale.MissingRequiredFields, "At least one service order item is required.");
    }

    private static HashSet<int> GetPartIdsFromRequest(IEnumerable<CreateServiceOrderItemRequest> items)
    {
        HashSet<int> hsPartIds = new();

        foreach (var objItem in items)
        {
            if (objItem.Parts == null) continue;

            foreach (var objPart in objItem.Parts)
            {
                if (objPart.PartId > 0)
                    hsPartIds.Add(objPart.PartId);
            }
        }

        return hsPartIds;
    }

    private ServiceOrderItem BuildServiceOrderItem(CreateServiceOrderItemRequest request, Dictionary<int, Part> dicParts)
    {
        if (string.IsNullOrWhiteSpace(request.Description))
            throw new BusinessException(ErrorCodes.Sale.MissingRequiredFields, "Item description is required.");

        if (request.Quantity <= 0)
            throw new BusinessException(ErrorCodes.Item.QuantityInvalid, "Item quantity must be greater than zero.");

        if (request.Price < 0)
            throw new BusinessException(ErrorCodes.Item.DiscountNegative, "Item price must be non-negative.");

        ServiceOrderItem objItem = new()
        {
            Description = request.Description.Trim(),
            Quantity = request.Quantity,
            Price = request.Price,
            Subtotal = request.Quantity * request.Price
        };

        AddOrUpdateParts(objItem, request.Parts, dicParts);

        return objItem;
    }

    private void UpdateItems(ServiceOrder serviceOrder, List<UpdateServiceOrderItemRequest> requests)
    {
        HashSet<int> hsAllPartIds = new();

        foreach (var objReq in requests)
        {
            if (objReq.Parts == null) continue;
            foreach (var objPartReq in objReq.Parts)
            {
                if (objPartReq.PartId > 0)
                    hsAllPartIds.Add(objPartReq.PartId);
            }
        }

        Dictionary<int, Part> dicParts = _repository.GetPartsByIds(hsAllPartIds).ToDictionary(p => p.Id, p => p);

        foreach (var objReq in requests)
        {
            if (objReq.Id.HasValue && objReq.Quantity.HasValue && objReq.Quantity.Value == 0)
            {
                var objExisting = serviceOrder.ServiceOrderItems.FirstOrDefault(i => i.Id == objReq.Id.Value);
                if (objExisting == null)
                    throw new NotFoundException($"Service order item not found. Id={objReq.Id.Value}");

                RestoreParts(objExisting);
                _context.ServiceOrderItems.Remove(objExisting);
                continue;
            }

            if (objReq.Id.HasValue)
            {
                var objExisting = serviceOrder.ServiceOrderItems.FirstOrDefault(i => i.Id == objReq.Id.Value);
                if (objExisting == null)
                    throw new NotFoundException($"Service order item not found. Id={objReq.Id.Value}");

                objExisting.Description = string.IsNullOrWhiteSpace(objReq.Description) ? objExisting.Description : objReq.Description.Trim();

                if (objReq.Quantity.HasValue)
                {
                    if (objReq.Quantity.Value <= 0)
                        throw new BusinessException(ErrorCodes.Item.QuantityInvalid);
                    objExisting.Quantity = objReq.Quantity.Value;
                }

                if (objReq.Price.HasValue)
                {
                    if (objReq.Price.Value < 0)
                        throw new BusinessException(ErrorCodes.Item.DiscountNegative, "Price must be non-negative.");
                    objExisting.Price = objReq.Price.Value;
                }

                objExisting.Subtotal = objExisting.Quantity * objExisting.Price;

                if (objReq.Parts != null)
                {
                    RestoreParts(objExisting);
                    objExisting.UseParts.Clear();
                    AddOrUpdateParts(objExisting, objReq.Parts, dicParts);
                }

                continue;
            }

            if (string.IsNullOrWhiteSpace(objReq.Description) || !objReq.Quantity.HasValue || !objReq.Price.HasValue)
                throw new BusinessException(ErrorCodes.Sale.MissingRequiredFields, "Description, quantity and price are required for new items.");

            CreateServiceOrderItemRequest objCreateReq = new()
            {
                Description = objReq.Description,
                Quantity = objReq.Quantity.Value,
                Price = objReq.Price.Value,
                Parts = objReq.Parts
            };

            var objNewItem = BuildServiceOrderItem(objCreateReq, dicParts);
            serviceOrder.ServiceOrderItems.Add(objNewItem);
        }
    }

    private void AddOrUpdateParts(ServiceOrderItem item, List<ServiceOrderItemPartRequest>? partRequests, Dictionary<int, Part> dicParts)
    {
        if (partRequests == null || partRequests.Count == 0)
            return;

        foreach (var objPartReq in partRequests)
        {
            if (objPartReq.PartId <= 0 || objPartReq.Quantity <= 0)
                throw new BusinessException(ErrorCodes.Sale.MissingRequiredFields, "PartId and quantity must be greater than zero.");

            if (!dicParts.TryGetValue(objPartReq.PartId, out Part? objPart))
                throw new NotFoundException($"Part not found. Id={objPartReq.PartId}");

            if (objPart.Quantity < objPartReq.Quantity)
                throw new BusinessException(ErrorCodes.Part.NoStock, $"Insufficient part quantity. PartId={objPart.Id}");

            objPart.Quantity -= objPartReq.Quantity;

            item.UseParts.Add(new UsePart
            {
                FkPart = objPart.Id,
                Quantity = objPartReq.Quantity
            });
        }
    }

    private void RestoreParts(ServiceOrderItem item)
    {
        foreach (var objUsePart in item.UseParts)
        {
            var objPart = _repository.GetPartById(objUsePart.FkPart);
            if (objPart != null)
            {
                objPart.Quantity += objUsePart.Quantity;
            }
        }

        _context.UseParts.RemoveRange(item.UseParts);
    }

    private void RestoreAndDeleteItems(List<ServiceOrderItem> items)
    {
        foreach (var objItem in items)
        {
            RestoreParts(objItem);
            _context.ServiceOrderItems.Remove(objItem);
        }
    }

    private static ServiceOrderResponse MapToResponse(ServiceOrder serviceOrder)
    {
        return new ServiceOrderResponse
        {
            Id = serviceOrder.Id,
            Number = serviceOrder.Number,
            OpenDate = serviceOrder.OpenDate,
            CloseDate = serviceOrder.CloseDate,
            Status = serviceOrder.Status,
            TotalValue = serviceOrder.TotalValue,
            Notes = serviceOrder.Notes,
            VehicleId = serviceOrder.FkVehicle,
            BranchId = serviceOrder.FkBranch,
            Items = serviceOrder.ServiceOrderItems.Select(item => new ServiceOrderItemResponse
            {
                Id = item.Id,
                Description = item.Description,
                Quantity = item.Quantity,
                Price = item.Price,
                Subtotal = item.Subtotal,
                Parts = item.UseParts.Select(p => new ServiceOrderPartResponse
                {
                    PartId = p.FkPart,
                    Quantity = p.Quantity
                }).ToList()
            }).ToList()
        };
    }
}