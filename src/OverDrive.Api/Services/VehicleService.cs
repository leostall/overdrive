using OverDrive.Api.Dtos.Vehicles;
using OverDrive.Api.Exceptions;
using OverDrive.Api.Models;
using OverDrive.Api.Repositories.Interfaces;
using OverDrive.Api.Services.Interfaces;

namespace OverDrive.Api.Services;

public class VehicleService : IVehicleService
{
    private readonly IVehicleRepository _repository;

    public VehicleService(IVehicleRepository repository)
    {
        _repository = repository;
    }

    public VehicleResponse Create(CreateVehicleRequest request)
    {
        ValidateCreateRequest(request);
        ValidateForeignKeys(request.AddressId, request.CustomerId, request.VehicleTypeId);

        Vehicle objVehicle = new()
        {
            Chassi = request.Chassi.Trim(),
            Plate = NormalizeOptionalString(request.Plate),
            Brand = request.Brand.Trim(),
            Model = request.Model.Trim(),
            Year = request.Year,
            Mileage = request.Mileage,
            Condition = NormalizeOptionalString(request.Condition),
            Status = NormalizeOptionalString(request.Status) ?? "Disponível",
            Value = request.Value,
            FkAddress = request.AddressId,
            FkCustomer = request.CustomerId,
            FkVehicleType = request.VehicleTypeId
        };

        _repository.AddVehicle(objVehicle);
        _repository.SaveChanges();

        return MapToResponse(objVehicle);
    }

    public VehicleResponse Update(int vehicleId, UpdateVehicleRequest request)
    {
        Vehicle objVehicle = _repository.GetVehicleById(vehicleId)
            ?? throw new NotFoundException($"Vehicle not found. Id={vehicleId}");

        ValidateUpdateRequest(request);

        if (request.AddressId.HasValue && request.AddressId.Value > 0)
        {
            var objAddress = _repository.GetAddressById(request.AddressId.Value);
            if (objAddress == null)
                throw new NotFoundException($"Address not found. Id={request.AddressId.Value}");

            objVehicle.FkAddress = request.AddressId.Value;
        }

        if (request.CustomerId.HasValue && request.CustomerId.Value > 0)
        {
            var objCustomer = _repository.GetCustomerById(request.CustomerId.Value);
            if (objCustomer == null)
                throw new NotFoundException($"Customer not found. Id={request.CustomerId.Value}");

            if (!objCustomer.Active)
                throw new BusinessException(ErrorCodes.Customer.Inactive);

            objVehicle.FkCustomer = request.CustomerId.Value;
        }

        if (request.VehicleTypeId.HasValue)
        {
            if (request.VehicleTypeId.Value <= 0)
                throw new BusinessException(ErrorCodes.Sale.MissingRequiredFields, "VehicleTypeId must be greater than zero.");

            var objVehicleType = _repository.GetVehicleTypeById(request.VehicleTypeId.Value);
            if (objVehicleType == null)
                throw new NotFoundException($"Vehicle type not found. Id={request.VehicleTypeId.Value}");

            objVehicle.FkVehicleType = request.VehicleTypeId.Value;
        }

        if (!string.IsNullOrWhiteSpace(request.Chassi))
            objVehicle.Chassi = request.Chassi.Trim();

        if (request.Plate != null)
            objVehicle.Plate = NormalizeOptionalString(request.Plate);

        if (!string.IsNullOrWhiteSpace(request.Brand))
            objVehicle.Brand = request.Brand.Trim();

        if (!string.IsNullOrWhiteSpace(request.Model))
            objVehicle.Model = request.Model.Trim();

        if (request.Year.HasValue)
            objVehicle.Year = request.Year.Value;

        if (request.Mileage.HasValue)
            objVehicle.Mileage = request.Mileage.Value;

        if (request.Condition != null)
            objVehicle.Condition = NormalizeOptionalString(request.Condition);

        if (request.Status != null)
            objVehicle.Status = NormalizeOptionalString(request.Status);

        if (request.Value.HasValue)
            objVehicle.Value = request.Value.Value;

        _repository.SaveChanges();

        return MapToResponse(objVehicle);
    }

    public void Delete(int vehicleId)
    {
        Vehicle objVehicle = _repository.GetVehicleById(vehicleId)
            ?? throw new NotFoundException($"Vehicle not found. Id={vehicleId}");

        if (_repository.HasSaleItems(vehicleId))
            throw new BusinessException(ErrorCodes.Vehicle.AlreadySold, "Vehicle cannot be deleted because it is linked to sales.");

        if (_repository.HasServiceOrders(vehicleId))
            throw new BusinessException(ErrorCodes.Vehicle.AlreadyOwned, "Vehicle cannot be deleted because it is linked to service orders.");

        _repository.RemoveVehicle(objVehicle);
        _repository.SaveChanges();
    }

    public VehicleResponse GetById(int vehicleId)
    {
        Vehicle objVehicle = _repository.GetVehicleById(vehicleId)
            ?? throw new NotFoundException($"Vehicle not found. Id={vehicleId}");

        return MapToResponse(objVehicle);
    }

    public List<VehicleResponse> GetAll()
    {
        return _repository.GetAllVehicles().Select(MapToResponse).ToList();
    }

    private void ValidateCreateRequest(CreateVehicleRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Chassi)
            || string.IsNullOrWhiteSpace(request.Brand)
            || string.IsNullOrWhiteSpace(request.Model)
            || request.Year <= 0
            || request.Value < 0
            || request.AddressId <= 0
            || request.CustomerId <= 0)
        {
            throw new BusinessException(ErrorCodes.Sale.MissingRequiredFields, "Required vehicle fields not informed.");
        }

        if (request.Mileage.HasValue && request.Mileage.Value < 0)
            throw new BusinessException(ErrorCodes.Item.QuantityInvalid, "Mileage must be non-negative.");
    }

    private void ValidateUpdateRequest(UpdateVehicleRequest request)
    {
        if (request.Year.HasValue && request.Year.Value <= 0)
            throw new BusinessException(ErrorCodes.Sale.MissingRequiredFields, "Year must be greater than zero.");

        if (request.Value.HasValue && request.Value.Value < 0)
            throw new BusinessException(ErrorCodes.Item.DiscountNegative, "Value must be non-negative.");

        if (request.Mileage.HasValue && request.Mileage.Value < 0)
            throw new BusinessException(ErrorCodes.Item.QuantityInvalid, "Mileage must be non-negative.");

        if (request.AddressId.HasValue && request.AddressId.Value <= 0)
            throw new BusinessException(ErrorCodes.Sale.MissingRequiredFields, "AddressId must be greater than zero.");

        if (request.CustomerId.HasValue && request.CustomerId.Value <= 0)
            throw new BusinessException(ErrorCodes.Sale.MissingRequiredFields, "CustomerId must be greater than zero.");
    }

    private void ValidateForeignKeys(int addressId, int customerId, int? vehicleTypeId)
    {
        var objAddress = _repository.GetAddressById(addressId);
        if (objAddress == null)
            throw new NotFoundException($"Address not found. Id={addressId}");

        var objCustomer = _repository.GetCustomerById(customerId);
        if (objCustomer == null)
            throw new NotFoundException($"Customer not found. Id={customerId}");

        if (!objCustomer.Active)
            throw new BusinessException(ErrorCodes.Customer.Inactive);

        if (vehicleTypeId.HasValue)
        {
            if (vehicleTypeId.Value <= 0)
                throw new BusinessException(ErrorCodes.Sale.MissingRequiredFields, "VehicleTypeId must be greater than zero.");

            var objVehicleType = _repository.GetVehicleTypeById(vehicleTypeId.Value);
            if (objVehicleType == null)
                throw new NotFoundException($"Vehicle type not found. Id={vehicleTypeId.Value}");
        }
    }

    private static string? NormalizeOptionalString(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }

    private static VehicleResponse MapToResponse(Vehicle vehicle)
    {
        return new VehicleResponse
        {
            VehicleId = vehicle.Id,
            Chassi = vehicle.Chassi,
            Plate = vehicle.Plate,
            Brand = vehicle.Brand,
            Model = vehicle.Model,
            Year = vehicle.Year,
            Mileage = vehicle.Mileage,
            Condition = vehicle.Condition,
            Status = vehicle.Status,
            Value = vehicle.Value,
            AddressId = vehicle.FkAddress,
            CustomerId = vehicle.FkCustomer,
            VehicleTypeId = vehicle.FkVehicleType
        };
    }
}
