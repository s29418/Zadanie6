using Microsoft.AspNetCore.Mvc;
using Zadanie7.Services;

namespace Zadanie7.Controllers;

[Route("api/[controller]")]
[ApiController]
public class WarehouseController : ControllerBase
{
    private readonly IWarehouseService _warehouseService;
    
    public WarehouseController(IWarehouseService warehouseService)
    {
        _warehouseService = warehouseService;
    }
    
    
    [HttpPost]
    public async Task<IActionResult> InsertProductWarehouse(int productId, int warehouseId, int amount, DateTime createdAt)
    {
        int result = await _warehouseService.InsertProductWarehouse(productId, warehouseId, amount, createdAt);

        switch (result)
        {
            case -1:
                return BadRequest("Product does not exist");
            case -2:
                return BadRequest("Warehouse does not exist");
            case -3:
                return BadRequest("Amount must be greater than 0");
            case -4:
                return BadRequest("Order is already fulfilled");
            default:
                return Ok(result);
                
        }
    }
    
    
}