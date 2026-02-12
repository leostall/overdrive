using OverDrive.Api.Models;
using OverDrive.Api.Repositories.Interfaces;

namespace OverDrive.Api.Repositories;

public class VehicleRepository : IVehicleRepository
{
    private readonly OverDriveDbContext _context;

    public VehicleRepository(OverDriveDbContext context)
    {
        _context = context;
    }

    public Vehicle? GetVehicleById(int vehicleId)
    {
        return _context.Vehicles.FirstOrDefault(v => v.Id == vehicleId);
    }

    public List<Vehicle> GetAllVehicles()
    {
        return _context.Vehicles
            .OrderByDescending(v => v.Id)
            .ToList();
    }

    public Customer? GetCustomerById(int customerId)
    {
        return _context.Customers.FirstOrDefault(c => c.Id == customerId);
    }

    public Address? GetAddressById(int addressId)
    {
        return _context.Addresses.FirstOrDefault(a => a.Id == addressId);
    }

    public VehicleType? GetVehicleTypeById(int vehicleTypeId)
    {
        return _context.VehicleTypes.FirstOrDefault(vt => vt.Id == vehicleTypeId);
    }

    public bool HasSaleItems(int vehicleId)
    {
        return _context.SaleItems.Any(si => si.FkVehicle == vehicleId);
    }

    public bool HasServiceOrders(int vehicleId)
    {
        return _context.ServiceOrders.Any(so => so.FkVehicle == vehicleId);
    }

    public void AddVehicle(Vehicle vehicle)
    {
        _context.Vehicles.Add(vehicle);
    }

    public void RemoveVehicle(Vehicle vehicle)
    {
        _context.Vehicles.Remove(vehicle);
    }

    public void SaveChanges()
    {
        _context.SaveChanges();
    }
}
