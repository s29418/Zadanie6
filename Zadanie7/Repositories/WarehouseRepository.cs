using System.Data;
using System.Data.SqlClient;
using Zadanie7.Models;

namespace Zadanie7.Repositories;

public class WarehouseRepository : IWarehouseRepository
{
    private IConfiguration _configuration;
    
    public WarehouseRepository(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<bool> WarehouseExists(int id)
    {
        await using var con = new SqlConnection(_configuration["ConnectionStrings:DefaultConnection"]);
        await con.OpenAsync();
        
        await using var cmd = new SqlCommand();
        cmd.Connection = con;
        cmd.CommandText = "SELECT COUNT(*) FROM Warehouse WHERE IdWarehouse = @IdWarehouse";
        cmd.Parameters.AddWithValue("@IdWarehouse", id);
        
        var result = await cmd.ExecuteScalarAsync();
        return (int)result != 0;
    }

    public async Task<bool> ProductExists(int id)
    {
        await using var con = new SqlConnection(_configuration["ConnectionStrings:DefaultConnection"]);
        await con.OpenAsync();
        
        await using var cmd = new SqlCommand();
        cmd.Connection = con;
        cmd.CommandText = "SELECT COUNT(*) FROM Product WHERE IdProduct = @IdProduct";
        cmd.Parameters.AddWithValue("@IdProduct", id);

        var result = await cmd.ExecuteScalarAsync();
        return (int)result != 0;
    }

    public async Task<int> GetOrderId(int productId, int amount, DateTime createdAt)
    {
        await using var con = new SqlConnection(_configuration["ConnectionStrings:DefaultConnection"]);
        await con.OpenAsync();

        await using var cmd = new SqlCommand();
        cmd.Connection = con;
        cmd.CommandText =
            "SELECT TOP 1 o.IdOrder FROM [Order] o LEFT JOIN Product_Warehouse pw ON o.IdOrder=pw.IdOrder WHERE o.IdProduct=@IdProduct AND o.Amount=@Amount AND pw.IdProductWarehouse IS NULL AND o.CreatedAt<@CreatedAt";
        cmd.Parameters.AddWithValue("@IdProduct", productId);
        cmd.Parameters.AddWithValue("@Amount", amount);
        cmd.Parameters.AddWithValue("@CreatedAt", createdAt);

        await using var reader = await cmd.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return (int)reader["IdOrder"];
        }

        return -4;
    }

    public async Task<bool> IsOrderFulfilled(int orderId)
    {
        await using var con = new SqlConnection(_configuration["ConnectionStrings:DefaultConnection"]);
        await con.OpenAsync();
        
        await using var cmd = new SqlCommand();
        cmd.Connection = con;
        cmd.CommandText = "SELECT COUNT(*) FROM Product_Warehouse WHERE IdOrder = @IdOrder";
        cmd.Parameters.AddWithValue("@IdOrder", orderId);
        
        var result = await cmd.ExecuteScalarAsync();

        return (int)result != 0;
    }

    public async Task<int> UpdateOrderFulfilledAt(int orderId)
    {
        await using var con = new SqlConnection(_configuration["ConnectionStrings:DefaultConnection"]);
        await con.OpenAsync();
        
        await using var cmd = new SqlCommand();
        cmd.Connection = con;
        cmd.CommandText = "UPDATE [Order] SET FulfilledAt = @FulfilledAt WHERE IdOrder = @IdOrder";
        cmd.Parameters.AddWithValue("@FulfilledAt", DateTime.Now);
        cmd.Parameters.AddWithValue("@IdOrder", orderId);

        return await cmd.ExecuteNonQueryAsync();
    }

    public async Task<decimal> CalculateTotalPrice(int productId, int amount)
    {
        await using var con = new SqlConnection(_configuration["ConnectionStrings:DefaultConnection"]);
        await con.OpenAsync();
        
        await using var cmd = new SqlCommand();
        cmd.Connection = con;
        cmd.CommandText = "SELECT Price FROM Product WHERE IdProduct = @IdProduct";
        cmd.Parameters.AddWithValue("@IdProduct", productId);
        
        
        var price = await cmd.ExecuteScalarAsync();
        decimal result = (decimal)price * amount;
        return result;
    }
    
    public async Task<int> AddProductToWarehouse(int productId, int warehouseId, int amount, decimal price, int orderId)
    {
        await using var con = new SqlConnection(_configuration["ConnectionStrings:DefaultConnection"]);
        await con.OpenAsync();
        
        await using var cmd = new SqlCommand();
        cmd.Connection = con;
        cmd.CommandText = "INSERT INTO Product_Warehouse(IdWarehouse, IdProduct, IdOrder, Amount, Price, CreatedAt) OUTPUT INSERTED.IdProductWarehouse VALUES (@IdWarehouse, @IdProduct, @IdOrder, @Amount, @Price, @CreatedAt)";
        cmd.Parameters.AddWithValue("@IdWarehouse", warehouseId);
        cmd.Parameters.AddWithValue("@IdProduct", productId);
        cmd.Parameters.AddWithValue("@IdOrder", orderId);
        cmd.Parameters.AddWithValue("@Amount", amount);
        cmd.Parameters.AddWithValue("@Price", price);
        cmd.Parameters.AddWithValue("@CreatedAt", DateTime.Now);

        return (int)await cmd.ExecuteScalarAsync();
    }

    public async Task<int> AddProductToWarehouseUsingProcedure(int productId, int warehouseId, int amount, DateTime createdAt)
    {
        await using var con = new SqlConnection(_configuration["ConnectionStrings:DefaultConnection"]);
        await using var cmd = new SqlCommand("AddProductToWarehouse", con)
        {
            CommandType = CommandType.StoredProcedure
        };

        await con.OpenAsync();
        
        cmd.Parameters.AddWithValue("@IdProduct", productId);
        cmd.Parameters.AddWithValue("@IdWarehouse", warehouseId);
        cmd.Parameters.AddWithValue("@Amount", amount);
        cmd.Parameters.AddWithValue("@CreatedAt", createdAt);

        return (int) await cmd.ExecuteScalarAsync();


    }
    
}