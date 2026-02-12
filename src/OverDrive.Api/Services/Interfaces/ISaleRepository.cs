using OverDrive.Api.Models;

namespace OverDrive.Api.Repositories.Interfaces;

public interface ISaleRepository
{
    Customer? GetCustomerById(int customerId);
    Customer? GetCustomerByCnpj(string cnpj);
    Part? GetPartById(int partId);
    Vehicle? GetVehicleById(int vehicleId);
    Branch? GetBranchById(int branchId);
    List<Part> GetPartsByIds(HashSet<int> hsPartIds);
    List<Vehicle> GetVehiclesByIds(HashSet<int> hsVehicleIds);

    Sale? GetSaleById(int saleId);
    List<Sale> GetAllSales();

    void AddSale(Sale objSale);
    void SaveChanges();
}