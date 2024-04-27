using System.Data.SqlClient;
using Microsoft.AspNetCore.Http.HttpResults;
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
    public async Task<IActionResult> AddProductToWarehouse(int productId, int warehouseId, int amount, DateTime createdAt)
    {
        int result = await _warehouseService.AddProductToWarehouse(productId, warehouseId, amount, createdAt);

        switch (result)
        {
            case -1:
                return BadRequest("Product does not exist");
            case -2:
                return BadRequest("Warehouse does not exist");
            case -3:
                return BadRequest("Amount must be greater than 0");
            case -4:
                return BadRequest("No order found for this product, amount and date");
            case -5:
                return BadRequest("Order is already fulfilled");
            default:
                return Ok(result);
                
        }
    }
    
    [HttpPost("procedure")]
    public async Task<IActionResult> AddProductToWarehouseUsingProcedure(int productId, int warehouseId, int amount, DateTime createdAt)
    {
        try
        {
            int result =
                await _warehouseService.AddProductToWarehouseUsingProcedure(productId, warehouseId, amount, createdAt);
            return Ok(result);
        }
        catch (SqlException e)
        {
            Console.WriteLine(e.Number);
            switch (e.Number)
            {
                case 50000:
                    return BadRequest("Product or Warehouse does not exist");
                default:
                    return StatusCode(500, e.Message);
            }
            
        }
        
    }
    
    //2012-04-23T18:25:43.511Z
    //2024-04-26T18:25:43.511Z
}