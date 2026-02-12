using OverDrive.Api.Dtos.Parts;

namespace OverDrive.Api.Services.Interfaces;

public interface IPartService
{
    PartResponse Create(CreatePartRequest request);
    PartResponse Update(int partId, UpdatePartRequest request);
    void Delete(int partId);
    PartResponse GetById(int partId);
    List<PartResponse> GetAll();
}
