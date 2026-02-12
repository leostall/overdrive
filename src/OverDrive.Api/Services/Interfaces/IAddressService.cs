using OverDrive.Api.Dtos.Addresses;

namespace OverDrive.Api.Services.Interfaces;

public interface IAddressService
{
    AddressResponse Create(CreateAddressRequest request);
    AddressResponse Update(int addressId, UpdateAddressRequest request);
    void Delete(int addressId);
    AddressResponse GetById(int addressId);
    List<AddressResponse> GetAll();
}
