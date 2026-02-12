using OverDrive.Api.Dtos.Branches;
using OverDrive.Api.Models;

namespace OverDrive.Api.Repositories.Interfaces;

public interface IBranchRepository
{
    Branch? GetById(int branchId);
    List<Branch> GetAll();
    Address? GetAddressById(int addressId);
    Branch? GetByCnpj(string cnpj);
    bool HasSales(int branchId);
    bool HasServiceOrders(int branchId);
    bool HasEmployees(int branchId);
    bool HasStocks(int branchId);
    BranchReportResponse? GetReportByBranchId(int branchId);
    List<BranchReportResponse> GetReports();
    void Add(Branch branch);
    void Remove(Branch branch);
    void SaveChanges();
}
