using OverDrive.Api.Models;

namespace OverDrive.Api.Repositories.Interfaces;

public interface IAddressRepository
{
    Address? GetById(int addressId);
    List<Address> GetAll();
    bool HasCustomers(int addressId);
    bool HasBranches(int addressId);
    bool HasEmployees(int addressId);
    bool HasStocks(int addressId);
    bool HasVehicles(int addressId);
    void Add(Address address);
    void Remove(Address address);
    void SaveChanges();
}
