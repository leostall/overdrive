using OverDrive.Api.Models;

namespace OverDrive.Api.Repositories.Interfaces;

public interface ICustomerRepository
{
    Customer? GetById(int customerId);
    List<Customer> GetAll();
    Address? GetAddressById(int addressId);
    Customer? GetByCpfCnpj(string cpfCnpj);
    bool HasSales(int customerId);
    bool HasVehicles(int customerId);
    void Add(Customer customer);
    void Remove(Customer customer);
    void SaveChanges();
}
