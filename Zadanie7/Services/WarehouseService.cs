using Zadanie7.Models;
using Zadanie7.Repositories;

namespace Zadanie7.Services;

public class WarehouseService : IWarehouseService
{
    private readonly IWarehouseRepository _warehouseRepository;
    
    public WarehouseService(IWarehouseRepository warehouseRepository)
    {
        _warehouseRepository = warehouseRepository;
    }

    public async Task<int> InsertProductWarehouse(int productId, int warehouseId, int amount, DateTime createdAt)
    {
        if (await _warehouseRepository.ProductExists(productId) && await _warehouseRepository.WarehouseExists(warehouseId) && amount > 0)
        {
            Order order = await _warehouseRepository.GetOrder(productId, amount, createdAt);
            if (await _warehouseRepository.IsOrderFulfilled(order.IdOrder))
            {
                await _warehouseRepository.UpdateOrderFulfilledAt(order.IdOrder);
                double price = await _warehouseRepository.CalculateTotalPrice(productId, amount);
                return await _warehouseRepository.InsertProductWarehouse(productId, warehouseId, amount, price, order.IdOrder);
            }

            return -2;
        }
        return -1;
    }
    
}