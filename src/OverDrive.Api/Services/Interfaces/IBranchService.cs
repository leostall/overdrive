using OverDrive.Api.Dtos.Branches;

namespace OverDrive.Api.Services.Interfaces;

public interface IBranchService
{
    BranchResponse Create(CreateBranchRequest request);
    BranchResponse Update(int branchId, UpdateBranchRequest request);
    void Delete(int branchId);
    BranchResponse GetById(int branchId);
    List<BranchResponse> GetAll();
    BranchReportResponse GetReportById(int branchId);
    List<BranchReportResponse> GetReports();
}
