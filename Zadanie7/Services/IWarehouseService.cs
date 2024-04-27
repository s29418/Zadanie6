namespace Zadanie7.Services;

public interface IWarehouseService
{
    Task<int> AddProductToWarehouse(int productId, int warehouseId, int amount, DateTime createdAt);
    Task<int> AddProductToWarehouseUsingProcedure(int productId, int warehouseId, int amount, DateTime createdAt);
}