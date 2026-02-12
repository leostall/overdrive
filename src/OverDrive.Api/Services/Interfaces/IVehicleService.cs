using OverDrive.Api.Dtos.Vehicles;

namespace OverDrive.Api.Services.Interfaces;

public interface IVehicleService
{
    VehicleResponse Create(CreateVehicleRequest request);
    VehicleResponse Update(int vehicleId, UpdateVehicleRequest request);
    void Delete(int vehicleId);
    VehicleResponse GetById(int vehicleId);
    List<VehicleResponse> GetAll();
}
