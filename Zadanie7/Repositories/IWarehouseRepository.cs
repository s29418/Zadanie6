using Zadanie7.Models;

namespace Zadanie7.Repositories;

public interface IWarehouseRepository
{
    Task<bool> WarehouseExists(int id);
    Task<bool> ProductExists(int id);
    Task<Order> GetOrder(int productId, int amount, DateTime createdAt);
    Task<bool> IsOrderFulfilled(int orderId);
    Task<int> UpdateOrderFulfilledAt(int orderId);
    Task<double> CalculateTotalPrice(int productId, int amount);
    Task<int> InsertProductWarehouse(int productId, int warehouseId, int amount, double price, int orderId);

}