using OverDrive.Api.Models;

namespace OverDrive.Api.Repositories.Interfaces;

public interface IVehicleRepository
{
    Vehicle? GetVehicleById(int vehicleId);
    List<Vehicle> GetAllVehicles();
    Customer? GetCustomerById(int customerId);
    Address? GetAddressById(int addressId);
    VehicleType? GetVehicleTypeById(int vehicleTypeId);
    bool HasSaleItems(int vehicleId);
    bool HasServiceOrders(int vehicleId);
    void AddVehicle(Vehicle vehicle);
    void RemoveVehicle(Vehicle vehicle);
    void SaveChanges();
}
