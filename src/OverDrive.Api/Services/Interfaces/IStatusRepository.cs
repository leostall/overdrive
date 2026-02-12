using OverDrive.Api.Models;

namespace OverDrive.Api.Repositories.Interfaces;

public interface IStatusRepository
{
    Status? GetById(int statusId);
    List<Status> GetAll();
    Status? GetByDescription(string description);
    bool HasSales(int statusId);
    void Add(Status status);
    void Remove(Status status);
    void SaveChanges();
}
