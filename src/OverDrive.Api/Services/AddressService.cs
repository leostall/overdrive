using OverDrive.Api.Dtos.Addresses;
using OverDrive.Api.Exceptions;
using OverDrive.Api.Models;
using OverDrive.Api.Repositories.Interfaces;
using OverDrive.Api.Services.Interfaces;

namespace OverDrive.Api.Services;

public class AddressService : IAddressService
{
    private readonly IAddressRepository _repository;

    public AddressService(IAddressRepository repository)
    {
        _repository = repository;
    }

    public AddressResponse Create(CreateAddressRequest request)
    {
        ValidateCreate(request);

        Address objAddress = new()
        {
            Street = request.Street.Trim(),
            Number = request.Number.Trim(),
            Complement = NormalizeOptional(request.Complement),
            Neighborhood = request.Neighborhood.Trim(),
            City = request.City.Trim(),
            State = request.State.Trim().ToUpperInvariant(),
            Zip = request.Zip.Trim()
        };

        _repository.Add(objAddress);
        _repository.SaveChanges();

        return MapToResponse(objAddress);
    }

    public AddressResponse Update(int addressId, UpdateAddressRequest request)
    {
        Address objAddress = _repository.GetById(addressId)
            ?? throw new NotFoundException($"Address not found. Id={addressId}");

        if (request.Street != null)
            objAddress.Street = NormalizeOptional(request.Street);

        if (request.Number != null)
            objAddress.Number = NormalizeOptional(request.Number);

        if (request.Complement != null)
            objAddress.Complement = NormalizeOptional(request.Complement);

        if (request.Neighborhood != null)
            objAddress.Neighborhood = NormalizeOptional(request.Neighborhood);

        if (request.City != null)
            objAddress.City = NormalizeOptional(request.City);

        if (request.State != null)
            objAddress.State = NormalizeOptional(request.State)?.ToUpperInvariant();

        if (request.Zip != null)
            objAddress.Zip = NormalizeOptional(request.Zip);

        _repository.SaveChanges();

        return MapToResponse(objAddress);
    }

    public void Delete(int addressId)
    {
        Address objAddress = _repository.GetById(addressId)
            ?? throw new NotFoundException($"Address not found. Id={addressId}");

        if (_repository.HasCustomers(addressId)
            || _repository.HasBranches(addressId)
            || _repository.HasEmployees(addressId)
            || _repository.HasStocks(addressId)
            || _repository.HasVehicles(addressId))
        {
            throw new BusinessException(ErrorCodes.Sale.Blocked, "Address cannot be deleted because it is linked to other records.");
        }

        _repository.Remove(objAddress);
        _repository.SaveChanges();
    }

    public AddressResponse GetById(int addressId)
    {
        Address objAddress = _repository.GetById(addressId)
            ?? throw new NotFoundException($"Address not found. Id={addressId}");

        return MapToResponse(objAddress);
    }

    public List<AddressResponse> GetAll()
    {
        return _repository.GetAll().Select(MapToResponse).ToList();
    }

    private static void ValidateCreate(CreateAddressRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Street)
            || string.IsNullOrWhiteSpace(request.Number)
            || string.IsNullOrWhiteSpace(request.Neighborhood)
            || string.IsNullOrWhiteSpace(request.City)
            || string.IsNullOrWhiteSpace(request.State)
            || string.IsNullOrWhiteSpace(request.Zip))
        {
            throw new BusinessException(ErrorCodes.Sale.MissingRequiredFields, "Required address fields not informed.");
        }
    }

    private static string? NormalizeOptional(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }

    private static AddressResponse MapToResponse(Address address)
    {
        return new AddressResponse
        {
            AddressId = address.Id,
            Street = address.Street,
            Number = address.Number,
            Complement = address.Complement,
            Neighborhood = address.Neighborhood,
            City = address.City,
            State = address.State,
            Zip = address.Zip
        };
    }
}
