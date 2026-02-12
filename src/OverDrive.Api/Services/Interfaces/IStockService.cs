using OverDrive.Api.Dtos.Stocks;

namespace OverDrive.Api.Services.Interfaces;

public interface IStockService
{
    StockResponse Create(CreateStockRequest request);
    StockResponse Update(int stockId, UpdateStockRequest request);
    void Delete(int stockId);
    StockResponse GetById(int stockId);
    List<StockResponse> GetAll();
}
