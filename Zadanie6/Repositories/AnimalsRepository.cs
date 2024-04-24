using System.Data.SqlClient;
using Zadanie6.Animals;

namespace Zadanie6.Repositories;

public class AnimalsRepository : IAnimalsRepository
{
    private IConfiguration _configuration;
    
    public AnimalsRepository(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    public IEnumerable<Animal> GetAnimals(string orderBy)
    {
        using var con = new SqlConnection(_configuration["ConnectionStrings:DefaultConnection"]);
        con.Open();
        
        string query = "SELECT IdAnimal, Name, Description, Category, Area FROM Animal";
        switch (orderBy)
        { 
            case "name":
                query += " ORDER BY Name";
                break;
            case "description":
                query += " ORDER BY Description";
                break;
            case "category":
                query += " ORDER BY Category";
                break;
            case "area":
                query += " ORDER BY Area";
                break;
            default:
                query += " ORDER BY Name";
                break;
        }
    
        
        
        using var cmd = new SqlCommand();
        cmd.Connection = con;
        cmd.CommandText = query;
        
        var dr = cmd.ExecuteReader();
        var animals = new List<Animal>();
        while (dr.Read())
        {
            Animal animal = new Animal
            {
                IdAnimal = (int)dr["IdAnimal"],
                Name = dr["Name"].ToString(),
                Description = dr["Description"].ToString(),
                Category = dr["Category"].ToString(),
                Area = dr["Area"].ToString()
            };
            animals.Add(animal);
        }
        
        return animals;
    }

    public Animal GetAnimalById(int idAnimal)
    {
        using var con = new SqlConnection(_configuration["ConnectionStrings:DefaultConnection"]);
        con.Open();

        using var cmd = new SqlCommand();
        cmd.Connection = con;
        cmd.CommandText = "SELECT IdAnimal, Name, Description, Category, Area FROM Animal WHERE IdAnimal=@IdAnimal";
        cmd.Parameters.AddWithValue("@IdAnimal", idAnimal);

        var dr = cmd.ExecuteReader();
        if (dr.Read())
        {
            Animal animal = new Animal
            {
                IdAnimal = (int)dr["IdAnimal"],
                Name = dr["Name"].ToString(),
                Description = dr["Description"].ToString(),
                Category = dr["Category"].ToString(),
                Area = dr["Area"].ToString()
            };
            return animal;
        }

        return null;
    }

    public int CreateAnimal(Animal animal)
    {
        using var con = new SqlConnection(_configuration["ConnectionStrings:DefaultConnection"]);
        con.Open();
        
        using var cmd = new SqlCommand();
        cmd.Connection = con;
        cmd.CommandText = "INSERT INTO Animal(Name, Description, Category, Area) VALUES(@Name, @Description, @Category, @Area)";
        cmd.Parameters.AddWithValue("@Name", animal.Name);
        cmd.Parameters.AddWithValue("@Description", animal.Description);
        cmd.Parameters.AddWithValue("@Category", animal.Category);
        cmd.Parameters.AddWithValue("@Area", animal.Area);

        return cmd.ExecuteNonQuery();
    }

    public int UpdateAnimal(Animal animal)
    {
        using var con = new SqlConnection(_configuration["ConnectionStrings:DefaultConnection"]);
        con.Open();

        using var cmd = new SqlCommand();
        cmd.Connection = con;
        cmd.CommandText = "UPDATE Animal SET Name=@Name, Description=@Description, Category=@Category, Area=@Area WHERE IdAnimal=@IdAnimal";
        cmd.Parameters.AddWithValue("@Name", animal.Name);
        cmd.Parameters.AddWithValue("@Description", animal.Description);
        cmd.Parameters.AddWithValue("@Category", animal.Category);
        cmd.Parameters.AddWithValue("@Area", animal.Area);
        cmd.Parameters.AddWithValue("@IdAnimal", animal.IdAnimal);
        
        return cmd.ExecuteNonQuery();
    }

    public int DeleteAnimal(int idAnimal)
    {
        using var con = new SqlConnection(_configuration["ConnectionStrings:DefaultConnection"]);
        con.Open();

        using var cmd = new SqlCommand();
        cmd.Connection = con;
        cmd.CommandText = "DELETE FROM Animal WHERE IdAnimal=@IdAnimal";
        cmd.Parameters.AddWithValue("@IdAnimal", idAnimal);

        return cmd.ExecuteNonQuery();
    }

}