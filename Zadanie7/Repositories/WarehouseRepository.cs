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

    public async Task<Order> GetOrder(int productId, int amount, DateTime createdAt)
    {
        await using var con = new SqlConnection(_configuration["ConnectionStrings:DefaultConnection"]);
        await con.OpenAsync();
        
        await using var cmd = new SqlCommand();
        cmd.Connection = con;
        cmd.CommandText = "SELECT * FROM Order WHERE IdProduct = @IdProduct AND Amount = @Amount AND CreatedAt < @CreatedAt";
        cmd.Parameters.AddWithValue("@IdProduct", productId);
        cmd.Parameters.AddWithValue("@Amount", amount);
        cmd.Parameters.AddWithValue("@CreatedAt", createdAt);
        
        await using var reader = await cmd.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            Order order = new Order
            {
                IdOrder = (int)reader["IdOrder"],
                IdProduct = (int)reader["IdProduct"],
                Amount = (int)reader["Amount"],
                CreatedAt = (DateTime)reader["CreatedAt"],
                FulfilledAt = (DateTime)reader["FulfilledAt"]
            };
            return order;
        }
        return null;
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
        cmd.CommandText = "UPDATE Order SET FulfilledAt = @FulfilledAt WHERE IdOrder = @IdOrder";
        cmd.Parameters.AddWithValue("@FulfilledAt", DateTime.Now);
        cmd.Parameters.AddWithValue("@IdOrder", orderId);

        return await cmd.ExecuteNonQueryAsync();
    }

    public async Task<double> CalculateTotalPrice(int productId, int amount)
    {
        await using var con = new SqlConnection(_configuration["ConnectionStrings:DefaultConnection"]);
        await con.OpenAsync();
        
        await using var cmd = new SqlCommand();
        cmd.Connection = con;
        cmd.CommandText = "SELECT Price FROM Product WHERE IdProduct = @IdProduct";
        cmd.Parameters.AddWithValue("@IdProduct", productId);
        
        
        var price = await cmd.ExecuteScalarAsync();
        double result = (double)price * amount;
        return result;
    }
    
    public async Task<int> InsertProductWarehouse(int productId, int warehouseId, int amount, double price, int orderId)
    {
        await using var con = new SqlConnection(_configuration["ConnectionStrings:DefaultConnection"]);
        await con.OpenAsync();
        
        await using var cmd = new SqlCommand();
        cmd.Connection = con;
        cmd.CommandText = "INSERT INTO Product_Warehouse(IdWarehouse, IdProduct, IdOrder, Amount, Price, CreatedAt) OUTPUT INSERTED.IdProduct_Warehouse VALUES (@IdWarehouse, @IdProduct, @IdOrder, @Amount, @Price, @CreatedAt)";
        cmd.Parameters.AddWithValue("@IdWarehouse", warehouseId);
        cmd.Parameters.AddWithValue("@IdProduct", productId);
        cmd.Parameters.AddWithValue("@IdOrder", orderId);
        cmd.Parameters.AddWithValue("@Amount", amount);
        cmd.Parameters.AddWithValue("@Price", price);
        cmd.Parameters.AddWithValue("@CreatedAt", DateTime.Now);

        return (int)await cmd.ExecuteScalarAsync();
    }
}