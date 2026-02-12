using OverDrive.Api.Models;

namespace OverDrive.Api.Repositories.Interfaces;

public interface IPartRepository
{
    Part? GetById(int partId);
    List<Part> GetAll();
    Part? GetByCode(string code);
    Stock? GetStockById(int stockId);
    bool HasSaleItems(int partId);
    bool HasUseParts(int partId);
    void Add(Part part);
    void Remove(Part part);
    void SaveChanges();
}
