using OverDrive.Api.Dtos.Customers;

namespace OverDrive.Api.Services.Interfaces;

public interface ICustomerService
{
    CustomerResponse Create(CreateCustomerRequest request);
    CustomerResponse Update(int customerId, UpdateCustomerRequest request);
    void Delete(int customerId);
    CustomerResponse GetById(int customerId);
    List<CustomerResponse> GetAll();
}
