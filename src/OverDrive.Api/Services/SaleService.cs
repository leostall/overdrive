using Microsoft.EntityFrameworkCore;
using OverDrive.Api.Dtos.Sales;
using OverDrive.Api.Exceptions;
using OverDrive.Api.Models;
using OverDrive.Api.Repositories.Interfaces;
using OverDrive.Api.Services.Interfaces;
using System.Linq;

namespace OverDrive.Api.Services;

public class SaleService : ISaleService
{
    private readonly ISaleRepository _repository;
    private readonly OverDriveDbContext _context;

    public SaleService(ISaleRepository repository, OverDriveDbContext context)
    {
        _repository = repository;
        _context = context;
    }

    public CreateSaleResponse Create(CreateSaleRequest request)
    {
        if (request.CustomerId <= 0 || request.BranchId <= 0 || request.EmployeeId <= 0 || request.PaymentId <= 0)
        {
            throw new BusinessException(ErrorCodes.Sale.MissingRequiredFields, "Camps obrigatory not informed.");
        }

        if (request.Items == null || request.Items.Count == 0)
        {
            throw new BusinessException(ErrorCodes.Sale.MissingRequiredFields, "At least one sale item is required.");
        }

        Customer? objCustomer = _repository.GetCustomerById(request.CustomerId);
        if (objCustomer == null)
        {
            throw new NotFoundException("Customer not found.");
        }

        if (objCustomer.Active == false)
        {
            throw new BusinessException(ErrorCodes.Customer.Inactive);
        }

        HashSet<int> hsPartIds = new();
        HashSet<int> hsVehicleIds = new();

        foreach (var objItem in request.Items)
        {
            string strType = (objItem.ItemType ?? string.Empty).Trim().ToUpperInvariant();

            if (strType != "PART" && strType != "VEHICLE")
            {
                throw new BusinessException(ErrorCodes.Item.InvalidType);
            }

            if (objItem.Quantity <= 0)
            {
                throw new BusinessException(ErrorCodes.Item.QuantityInvalid);
            }

            if (objItem.Discount < 0)
            {
                throw new BusinessException(ErrorCodes.Item.DiscountNegative);
            }

            if (strType == "PART")
            {
                if (!objItem.PartId.HasValue || objItem.PartId.Value <= 0)
                {
                    throw new BusinessException(ErrorCodes.Sale.MissingRequiredFields, "PartId is required when ItemType is PART.");
                }

                if (objItem.VehicleId.HasValue)
                {
                    throw new BusinessException(ErrorCodes.Sale.MissingRequiredFields, "VehicleId must be null when ItemType is PART.");
                }

                hsPartIds.Add(objItem.PartId.Value);
                continue;
            }

            if (!objItem.VehicleId.HasValue || objItem.VehicleId.Value <= 0)
            {
                throw new BusinessException(ErrorCodes.Sale.MissingRequiredFields, "VehicleId is required when ItemType is VEHICLE.");
            }

            if (objItem.PartId.HasValue)
            {
                throw new BusinessException(ErrorCodes.Sale.MissingRequiredFields, "PartId must be null when ItemType is VEHICLE.");
            }

            hsVehicleIds.Add(objItem.VehicleId.Value);
        }

        List<Part> lstParts = _repository.GetPartsByIds(hsPartIds);
        Dictionary<int, Part> dicPartById = lstParts.ToDictionary(p => p.Id, p => p);

        List<Vehicle> lstVehicles = _repository.GetVehiclesByIds(hsVehicleIds);
        Dictionary<int, Vehicle> dicVehicleById = lstVehicles.ToDictionary(v => v.Id, v => v);

        using var objTransaction = _context.Database.BeginTransaction();

        try
        {
            Sale objSale = new()
            {
                SaleDate = DateTime.Now,
                FkCustomer = request.CustomerId,
                FkBranch = request.BranchId,
                FkEmployee = request.EmployeeId,
                FkPayment = request.PaymentId,
                FkStatus = request.StatusId ?? 5, // PENDENTE
                Discount = request.Discount ?? 0m,
                AdditionalFee = request.AdditionalFee ?? 0m
            };

            decimal decSubtotal = 0m;
            decimal decItemsDiscount = 0m;

            foreach (var objItemReq in request.Items)
            {
                string strType = objItemReq.ItemType.Trim().ToUpperInvariant();
                decimal unitPrice = 0m;

                if (strType == "PART")
                {
                    int intPartId = objItemReq.PartId!.Value;

                    if (!dicPartById.TryGetValue(intPartId, out Part? objPart))
                    {
                        throw new NotFoundException($"Part not found. Id={intPartId}");
                    }

                    int intAvailable = objPart.Quantity; // VERIFICAR ESSE LÓGICA AQUI

                    if (intAvailable < objItemReq.Quantity)
                    {
                        throw new BusinessException(ErrorCodes.Part.NoStock, $"Insufficient part quantity. PartId={intPartId}");
                    }

                    objPart.Quantity = intAvailable - objItemReq.Quantity;
                    unitPrice = objPart.Price;
                }
                else if (strType == "VEHICLE")
                {
                    int intVehicleId = objItemReq.VehicleId!.Value;

                    if (!dicVehicleById.TryGetValue(intVehicleId, out Vehicle? objVehicle))
                    {
                        throw new NotFoundException($"Vehicle not found. Id={intVehicleId}");
                    }

                    if (objVehicle.FkCustomer > 0 && objVehicle.FkCustomer != objSale.FkBranch && objVehicle.FkCustomer == objCustomer.Id)
                    {
                        throw new BusinessException(ErrorCodes.Vehicle.AlreadyOwned, $"Customer already owns this vehicle. VehicleId={intVehicleId}");
                    }

                    string strStatus = (objVehicle.Status ?? string.Empty).Trim();

                    if (strStatus == "Vendido")
                    {
                        throw new BusinessException(ErrorCodes.Vehicle.AlreadySold, $"Vehicle already sold. VehicleId={intVehicleId}");
                    }

                    objVehicle.Status = "Vendido";
                    objVehicle.FkCustomer = request.CustomerId;
                    objVehicle.FkAddress = objCustomer.FkAddress.HasValue ? objCustomer.FkAddress.Value : 0; // NECESSÁRIO CORREÇÃO DO BANCO PARA CEITAR NULL - PALIATIVO, INSERIR '0' PARA NULL
                    unitPrice = objVehicle.Value;
                }
                else
                {
                    throw new BusinessException(ErrorCodes.Item.InvalidType);
                }

                decimal discountPerUnit = objItemReq.Discount ?? 0m;

                if (discountPerUnit < 0)
                {
                    throw new BusinessException(ErrorCodes.Item.DiscountNegative);
                }

                if (discountPerUnit > unitPrice)
                {
                    throw new BusinessException(ErrorCodes.Item.DiscountTooLarge, $"Item discount cannot exceed unit price. ItemType={strType}");
                }

                decimal decEffectiveUnitPrice = unitPrice - discountPerUnit;
                decimal decItemSubtotal = decEffectiveUnitPrice * objItemReq.Quantity;
                decSubtotal += decItemSubtotal;
                decItemsDiscount += discountPerUnit * objItemReq.Quantity;

                SaleItem objSaleItem = new()
                {
                    ItemType = strType,
                    Quantity = objItemReq.Quantity,
                    UnitPrice = unitPrice,
                    Discount = discountPerUnit,
                    FkPart = objItemReq.PartId,
                    FkVehicle = objItemReq.VehicleId
                };

                objSale.SaleItems.Add(objSaleItem);
            }

            decimal decTotal = decSubtotal - (request.Discount ?? 0m) + (request.AdditionalFee ?? 0m);

            if (decTotal < 0)
            {
                throw new BusinessException(ErrorCodes.Sale.TotalNegative);
            }

            decimal decTotalDiscount = (request.Discount ?? 0m) + decItemsDiscount;

            objSale.Subtotal = decSubtotal;
            objSale.Total = decTotal;

            _repository.AddSale(objSale);
            _repository.SaveChanges();

            objTransaction.Commit();

            return new CreateSaleResponse
            {
                SaleId = objSale.Id,
                Subtotal = decSubtotal,
                TotalDiscount = decTotalDiscount,
                AdditionalFee = request.AdditionalFee ?? 0m,
                Total = decTotal
            };
        }
        catch
        {
            objTransaction.Rollback();
            throw;
        }
    }

    public SaleResponse GetById(int saleId)
    {
        var objSale = _repository.GetSaleById(saleId);
        if (objSale == null)
            throw new NotFoundException($"Sale not found. Id={saleId}");

        return MapToResponse(objSale);
    }

    public List<SaleResponse> GetAll()
    {
        var lst = _repository.GetAllSales();
        return lst.Select(MapToResponse).ToList();
    }

    private static SaleResponse MapToResponse(Sale sale)
    {
        return new SaleResponse
        {
            SaleId = sale.Id,
            SaleDate = sale.SaleDate,
            Subtotal = sale.Subtotal,
            Discount = sale.Discount,
            AdditionalFee = sale.AdditionalFee,
            Total = sale.Total,
            CustomerId = sale.FkCustomer,
            BranchId = sale.FkBranch,
            EmployeeId = sale.FkEmployee,
            PaymentId = sale.FkPayment,
            StatusId = sale.FkStatus,
            Items = sale.SaleItems.Select(si => new SaleItemResponse
            {
                Id = si.Id,
                ItemType = si.ItemType,
                Quantity = si.Quantity,
                UnitPrice = si.UnitPrice,
                Discount = si.Discount,
                PartId = si.FkPart,
                VehicleId = si.FkVehicle
            }).ToList()
        };
    }

    public SaleResponse Update(int saleId, UpdateSaleRequest request)
    {
        var objSale = _repository.GetSaleById(saleId);

        if (objSale == null)
            throw new NotFoundException($"Sale not found. Id={saleId}");

        HashSet<int> Blocked = new() { 8, 9, 10, 11, 12 };

        if (objSale.FkStatus.HasValue && Blocked.Contains(objSale.FkStatus.Value))
        {
            throw new BusinessException(ErrorCodes.Sale.Blocked);
        }

        using var tx = _context.Database.BeginTransaction();

        try
        {
            if (request.Items != null)
            {
                foreach (var objItemReq in request.Items)
                {
                    if (!objItemReq.Id.HasValue) continue;

                    string strType = (objItemReq.ItemType ?? string.Empty).Trim().ToUpperInvariant();

                    if (objItemReq.Quantity.HasValue && objItemReq.Quantity == 0)
                    {
                        var objExistingSaleItem = objSale.SaleItems.FirstOrDefault(si => si.Id == objItemReq.Id.Value);

                        if (objExistingSaleItem == null)
                            throw new BusinessException(ErrorCodes.SaleItem.NotFound);

                        if (strType == "VEHICLE" && objExistingSaleItem.FkVehicle.HasValue)
                        {
                            var objVehicle = _repository.GetVehicleById(objExistingSaleItem.FkVehicle.Value);
                            if (objVehicle != null)
                            {
                                objVehicle.Status = "Disponível";

                                if (objSale.FkBranch > 0)
                                {
                                    var objBranch = _repository.GetBranchById(objSale.FkBranch.Value);
                                    if (objBranch != null)
                                    {
                                        var objCustomerBranch = _repository.GetCustomerByCnpj(objBranch.Cnpj);
                                        objVehicle.FkCustomer = objCustomerBranch?.Id ?? 0;
                                        objVehicle.FkAddress = objBranch.FkAddress;
                                    }
                                }
                                else
                                {
                                    objVehicle.FkCustomer = 0;
                                    objVehicle.FkAddress = 0;
                                }
                            }
                        }

                        if (strType == "PART" && objExistingSaleItem.FkPart.HasValue)
                        {
                            var objPart = _repository.GetPartById(objExistingSaleItem.FkPart.Value);
                            if (objPart != null)
                            {
                                objPart.Quantity += objExistingSaleItem.Quantity;
                            }
                        }

                        _context.SaleItems.Remove(objExistingSaleItem);
                    }
                }
            }

            if (objSale.SaleItems.Count == 0)
            {
                objSale.FkStatus = 11;
                objSale.Discount = 0m;
                objSale.AdditionalFee = 0m;
                objSale.Subtotal = 0m;
                objSale.Total = 0m;

                _repository.SaveChanges();
                tx.Commit();

                return MapToResponse(objSale);
            }

            objSale.Discount = request.Discount ?? objSale.Discount;
            objSale.AdditionalFee = request.AdditionalFee ?? objSale.AdditionalFee;

            if (request.Items != null)
            {
                foreach (var objItemReq in request.Items)
                {
                    string strType = (objItemReq.ItemType ?? string.Empty).Trim().ToUpperInvariant();

                    if (objItemReq.Id.HasValue && objItemReq.Quantity.HasValue && objItemReq.Quantity == 0)
                        continue;

                    if (objItemReq.Id.HasValue)
                    {
                        var objExistingSaleItem = objSale.SaleItems.FirstOrDefault(si => si.Id == objItemReq.Id.Value);

                        if (objExistingSaleItem == null)
                            throw new BusinessException(ErrorCodes.SaleItem.NotFound);

                        if (!string.Equals(objExistingSaleItem.ItemType, strType, StringComparison.OrdinalIgnoreCase))
                            throw new BusinessException(ErrorCodes.Item.InvalidType);

                        if (strType == "PART")
                        {
                            if (!objExistingSaleItem.FkPart.HasValue)
                                throw new BusinessException(ErrorCodes.Part.NotFound);

                            var objPart = _repository.GetPartById(objExistingSaleItem.FkPart.Value);

                            if (objPart == null) throw new NotFoundException($"Part not found. Id={objExistingSaleItem.FkPart.Value}");

                            if (objItemReq.Quantity.HasValue)
                            {
                                int intNewQty = objItemReq.Quantity.Value;
                                int intOldQty = objExistingSaleItem.Quantity;

                                if (intNewQty <= 0)
                                    throw new BusinessException(ErrorCodes.Item.QuantityInvalid, "Quantity must be greater than 0.");

                                if (intNewQty > intOldQty)
                                {
                                    int intDelta = intNewQty - intOldQty;
                                    if (objPart.Quantity < intDelta)
                                        throw new BusinessException(ErrorCodes.Part.NoStock, $"Insufficient part quantity in stock. PartId={objPart.Id}");
                                    objPart.Quantity -= intDelta;
                                }
                                else if (intNewQty < intOldQty)
                                {
                                    int intDelta = intOldQty - intNewQty;
                                    objPart.Quantity += intDelta;
                                }

                                objExistingSaleItem.Quantity = intNewQty;
                            }

                            decimal decNewUnitPrice = objItemReq.UnitPrice ?? objExistingSaleItem.UnitPrice;
                            decimal decDiscountPerUnit = objItemReq.Discount ?? objExistingSaleItem.Discount;

                            if (decDiscountPerUnit < 0) throw new BusinessException(ErrorCodes.Item.DiscountNegative);

                            if (decDiscountPerUnit > decNewUnitPrice) throw new BusinessException(ErrorCodes.Item.DiscountTooLarge);

                            objExistingSaleItem.UnitPrice = decNewUnitPrice;
                            objExistingSaleItem.Discount = decDiscountPerUnit;
                        }
                        else if (strType == "VEHICLE")
                        {
                            if (!objExistingSaleItem.FkVehicle.HasValue)
                                throw new BusinessException(ErrorCodes.Vehicle.NotFound);

                            var objVehicle = _repository.GetVehicleById(objExistingSaleItem.FkVehicle.Value);

                            if (objVehicle == null) throw new NotFoundException($"Vehicle not found. Id={objExistingSaleItem.FkVehicle.Value}");

                            if (objItemReq.Quantity.HasValue && objItemReq.Quantity != 1)
                                throw new BusinessException(ErrorCodes.Vehicle.InvalidQuantity, "Vehicle quantity cannot be changed (must remain 1). To remove the vehicle, set quantity to 0.");

                            decimal decDiscountPerUnit = objItemReq.Discount ?? objExistingSaleItem.Discount;

                            if (decDiscountPerUnit < 0) throw new BusinessException(ErrorCodes.Item.DiscountNegative);
                            if (decDiscountPerUnit > objExistingSaleItem.UnitPrice) throw new BusinessException(ErrorCodes.Item.DiscountTooLarge);

                            objExistingSaleItem.Discount = decDiscountPerUnit;
                        }
                        else
                        {
                            throw new BusinessException(ErrorCodes.Item.InvalidType);
                        }
                    }
                    else
                    {
                        if (strType != "PART" && strType != "VEHICLE") throw new BusinessException(ErrorCodes.Item.InvalidType);

                        if (strType == "PART")
                        {
                            if (!objItemReq.PartId.HasValue) throw new BusinessException(ErrorCodes.Sale.MissingRequiredFields, "PartId required for PART item.");

                            if (!objItemReq.Quantity.HasValue || objItemReq.Quantity <= 0)
                                throw new BusinessException(ErrorCodes.Item.QuantityInvalid, "Quantity is required and must be greater than 0 for PART item.");

                            var objPart = _repository.GetPartById(objItemReq.PartId.Value);

                            if (objPart == null) throw new NotFoundException($"Part not found. Id={objItemReq.PartId.Value}");

                            if (objPart.Quantity < objItemReq.Quantity) throw new BusinessException(ErrorCodes.Part.NoStock, $"Insufficient part quantity. PartId={objPart.Id}");

                            objPart.Quantity -= objItemReq.Quantity.Value;

                            decimal decUnitPrice = objItemReq.UnitPrice ?? objPart.Price;
                            decimal decDiscountPerUnit = objItemReq.Discount ?? 0m;

                            if (decDiscountPerUnit < 0) throw new BusinessException(ErrorCodes.Item.DiscountNegative);
                            if (decDiscountPerUnit > decUnitPrice) throw new BusinessException(ErrorCodes.Item.DiscountTooLarge);

                            var objNewSaleItem = new SaleItem
                            {
                                ItemType = "PART",
                                Quantity = objItemReq.Quantity.Value,
                                UnitPrice = decUnitPrice,
                                Discount = decDiscountPerUnit,
                                FkPart = objPart.Id
                            };
                            objSale.SaleItems.Add(objNewSaleItem);
                        }
                        else
                        {
                            if (!objItemReq.VehicleId.HasValue) throw new BusinessException(ErrorCodes.Sale.MissingRequiredFields, "VehicleId required for VEHICLE item.");

                            var objVehicle = _repository.GetVehicleById(objItemReq.VehicleId.Value);
                            if (objVehicle == null) throw new NotFoundException($"Vehicle not found. Id={objItemReq.VehicleId.Value}");

                            int intBranchCustomerId = 0;
                            if (objSale.FkBranch > 0)
                            {
                                var objBranchForValidation = _repository.GetBranchById(objSale.FkBranch.Value);
                                if (objBranchForValidation != null)
                                {
                                    var objCustomerBranch = _repository.GetCustomerByCnpj(objBranchForValidation.Cnpj);
                                    intBranchCustomerId = objCustomerBranch?.Id ?? 0;
                                }
                            }

                            if (objVehicle.FkCustomer > 0 && objVehicle.FkCustomer != intBranchCustomerId && objVehicle.FkCustomer == objSale.FkCustomer)
                                throw new BusinessException(ErrorCodes.Vehicle.AlreadyOwned);

                            string strStatus = (objVehicle.Status ?? string.Empty).Trim().ToUpperInvariant();
                            if (strStatus == "VENDIDO") throw new BusinessException(ErrorCodes.Vehicle.AlreadySold);

                            if (objItemReq.Quantity.HasValue && objItemReq.Quantity != 1)
                                throw new BusinessException(ErrorCodes.Vehicle.InvalidQuantity, "Vehicle quantity must be 1.");

                            objVehicle.Status = "VENDIDO";
                            objVehicle.FkCustomer = objSale.FkCustomer;

                            var objCustomerForVehicle = _repository.GetCustomerById(objSale.FkCustomer);
                            objVehicle.FkAddress = objCustomerForVehicle?.FkAddress ?? 0;

                            decimal decUnitPrice = objItemReq.UnitPrice ?? objVehicle.Value;
                            decimal decDiscountPerUnit = objItemReq.Discount ?? 0m;
                            if (decDiscountPerUnit < 0) throw new BusinessException(ErrorCodes.Item.DiscountNegative);
                            if (decDiscountPerUnit > decUnitPrice) throw new BusinessException(ErrorCodes.Item.DiscountTooLarge);

                            var objNewSaleItem = new SaleItem
                            {
                                ItemType = "VEHICLE",
                                Quantity = 1,
                                UnitPrice = decUnitPrice,
                                Discount = decDiscountPerUnit,
                                FkVehicle = objVehicle.Id
                            };
                            objSale.SaleItems.Add(objNewSaleItem);
                        }
                    }
                }
            }

            decimal decSubtotal = 0m;
            decimal decItemsDiscount = 0m;
            foreach (var objSaleItem in objSale.SaleItems)
            {
                decSubtotal += (objSaleItem.UnitPrice - objSaleItem.Discount) * objSaleItem.Quantity;
                decItemsDiscount += objSaleItem.Discount * objSaleItem.Quantity;
            }

            decimal decTotal = decSubtotal - objSale.Discount + objSale.AdditionalFee;
            if (decTotal < 0) throw new BusinessException(ErrorCodes.Sale.TotalNegative);

            objSale.Subtotal = decSubtotal;
            objSale.Total = decTotal;

            _repository.SaveChanges();
            tx.Commit();

            return MapToResponse(objSale);
        }
        catch
        {
            tx.Rollback();
            throw;
        }
    }

    public void Delete(int saleId)
    {
        var objSale = _repository.GetSaleById(saleId);

        if (objSale == null)
            throw new NotFoundException($"Sale not found. Id={saleId}");

        HashSet<int> Blocked = new() { 8, 9, 10, 11, 12 };

        if (objSale.FkStatus.HasValue && Blocked.Contains(objSale.FkStatus.Value))
        {
            throw new BusinessException(ErrorCodes.Sale.Blocked);
        }

        using var tx = _context.Database.BeginTransaction();

        try
        {
            foreach (var objSaleItem in objSale.SaleItems)
            {
                string strType = (objSaleItem.ItemType ?? string.Empty).Trim().ToUpperInvariant();

                if (strType == "VEHICLE" && objSaleItem.FkVehicle.HasValue)
                {
                    var objVehicle = _repository.GetVehicleById(objSaleItem.FkVehicle.Value);
                    if (objVehicle != null)
                    {
                        objVehicle.Status = "Disponível";

                        if (objSale.FkBranch > 0)
                        {
                            var objBranch = _repository.GetBranchById(objSale.FkBranch.Value);
                            if (objBranch != null)
                            {
                                var objCustomerBranch = _repository.GetCustomerByCnpj(objBranch.Cnpj);
                                objVehicle.FkCustomer = objCustomerBranch?.Id ?? 0;
                                objVehicle.FkAddress = objBranch.FkAddress;
                            }
                        }
                        else
                        {
                            objVehicle.FkCustomer = 0;
                            objVehicle.FkAddress = 0;
                        }
                    }
                }

                if (strType == "PART" && objSaleItem.FkPart.HasValue)
                {
                    var objPart = _repository.GetPartById(objSaleItem.FkPart.Value);
                    if (objPart != null)
                    {
                        objPart.Quantity += objSaleItem.Quantity;
                    }
                }

                _context.SaleItems.Remove(objSaleItem);
            }

            objSale.FkStatus = 11;
            objSale.Discount = 0m;
            objSale.AdditionalFee = 0m;
            objSale.Subtotal = 0m;
            objSale.Total = 0m;

            _repository.SaveChanges();
            tx.Commit();
        }
        catch
        {
            tx.Rollback();
            throw;
        }
    }
}