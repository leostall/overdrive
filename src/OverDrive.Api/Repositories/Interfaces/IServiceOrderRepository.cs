using OverDrive.Api.Models;

namespace OverDrive.Api.Repositories.Interfaces;

public interface IServiceOrderRepository
{
    Vehicle? GetVehicleById(int vehicleId);
    Branch? GetBranchById(int branchId);
    Part? GetPartById(int partId);
    List<Part> GetPartsByIds(HashSet<int> hsPartIds);

    ServiceOrder? GetServiceOrderById(int serviceOrderId);
    List<ServiceOrder> GetAllServiceOrders();

    void AddServiceOrder(ServiceOrder serviceOrder);
    void SaveChanges();
}