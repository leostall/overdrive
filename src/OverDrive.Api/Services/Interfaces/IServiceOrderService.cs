using OverDrive.Api.Dtos.ServiceOrders;

namespace OverDrive.Api.Services.Interfaces;

public interface IServiceOrderService
{
    ServiceOrderResponse Create(CreateServiceOrderRequest request);
    ServiceOrderResponse GetById(int serviceOrderId);
    List<ServiceOrderResponse> GetAll();
    ServiceOrderResponse Update(int serviceOrderId, UpdateServiceOrderRequest request);
    void Delete(int serviceOrderId);
}