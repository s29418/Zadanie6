using Zadanie7.Models;

namespace Zadanie7.Repositories;

public interface IWarehouseRepository
{
    Task<bool> WarehouseExists(int id);
    Task<bool> ProductExists(int id);
    Task<int> GetOrderId(int productId, int amount, DateTime createdAt);
    Task<bool> IsOrderFulfilled(int orderId);
    Task<int> UpdateOrderFulfilledAt(int orderId);
    Task<decimal> CalculateTotalPrice(int productId, int amount);
    Task<int> AddProductToWarehouse(int productId, int warehouseId, int amount, decimal price, int orderId);
    Task<int> AddProductToWarehouseUsingProcedure(int productId, int warehouseId, int amount, DateTime createdAt);

}