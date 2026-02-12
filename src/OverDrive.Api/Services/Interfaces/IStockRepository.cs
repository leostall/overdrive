using OverDrive.Api.Models;

namespace OverDrive.Api.Repositories.Interfaces;

public interface IStockRepository
{
    Stock? GetById(int stockId);
    List<Stock> GetAll();
    Address? GetAddressById(int addressId);
    Branch? GetBranchById(int branchId);
    bool HasParts(int stockId);
    void Add(Stock stock);
    void Remove(Stock stock);
    void SaveChanges();
}
