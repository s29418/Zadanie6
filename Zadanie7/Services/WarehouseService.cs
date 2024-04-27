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

    public async Task<int> AddProductToWarehouse(int productId, int warehouseId, int amount, DateTime createdAt)
    {
        if (!await _warehouseRepository.ProductExists(productId))
        {
            return -1;
        }
        if (!await _warehouseRepository.WarehouseExists(warehouseId))
        {
            return -2;
        }

        if (amount <= 0)
        {
            return -3;
        }
        
        int orderId = await _warehouseRepository.GetOrderId(productId, amount, createdAt);
        if (orderId == -4)
        {
            return -4;
        }
        
        if (!await _warehouseRepository.IsOrderFulfilled(orderId))
        {
            await _warehouseRepository.UpdateOrderFulfilledAt(orderId);
            decimal price = await _warehouseRepository.CalculateTotalPrice(productId, amount);
            return await _warehouseRepository.AddProductToWarehouse(productId, warehouseId, amount, price, orderId);
        }
        return -5;
    }

    public async Task<int> AddProductToWarehouseUsingProcedure(int productId, int warehouseId, int amount,
        DateTime createdAt)
    {
        return await _warehouseRepository.AddProductToWarehouseUsingProcedure(productId, warehouseId, amount,
            createdAt);
    }
    
}