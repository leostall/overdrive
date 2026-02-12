using OverDrive.Api.Dtos.Sales;

namespace OverDrive.Api.Services.Interfaces;

public interface ISaleService
{
    CreateSaleResponse Create(CreateSaleRequest request);
    SaleResponse GetById(int saleId);
    List<SaleResponse> GetAll();
    SaleResponse Update(int saleId, UpdateSaleRequest request);
    void Delete(int saleId);
}