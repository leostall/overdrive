using OverDrive.Api.Dtos.Statuses;

namespace OverDrive.Api.Services.Interfaces;

public interface IStatusService
{
    StatusResponse Create(CreateStatusRequest request);
    StatusResponse Update(int statusId, UpdateStatusRequest request);
    void Delete(int statusId);
    StatusResponse GetById(int statusId);
    List<StatusResponse> GetAll();
}
