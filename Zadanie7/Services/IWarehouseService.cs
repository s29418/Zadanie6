namespace Zadanie7.Services;

public interface IWarehouseService
{
    Task<int> InsertProductWarehouse(int productId, int warehouseId, int amount, DateTime createdAt);
}