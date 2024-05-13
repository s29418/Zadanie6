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

public async Task<IEnumerable<PrescriptionDto>> GetPrescriptions(string doctorLastName)
{
    var prescriptions = new List<PrescriptionDto>();

    try
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            await connection.OpenAsync();

            var query = @"SELECT p.IdPrescription, p.Date, p.DueDate, pt.LastName as PatientLastName, d.LastName as DoctorLastName 
                          FROM Prescription p 
                          INNER JOIN Patient pt ON p.IdPatient = pt.IdPatient 
                          INNER JOIN Doctor d ON p.IdDoctor = d.IdDoctor";

            if (!string.IsNullOrEmpty(doctorLastName))
            {
                query += " WHERE d.LastName = @DoctorLastName";
            }

            query += " ORDER BY p.Date DESC";

            using (var command = new SqlCommand(query, connection))
            {
                if (!string.IsNullOrEmpty(doctorLastName))
                {
                    command.Parameters.AddWithValue("@DoctorLastName", doctorLastName);
                }

                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        Prescription prescription = new Prescription
                        {
                            IdPrescription = (int)dr["IdPrescription"],
                            Date = ((DateTime)dr["Date"]).ToString("yyyy-MM-dd"),
                            DueDate = ((DateTime)dr["DueDate"]).ToString("yyyy-MM-dd"),
                            PatientLastName = dr["PatientLastName"].ToString(),
                            DoctorLastName = dr["DoctorLastName"].ToString()
                        };

                        prescriptions.Add(prescription);
                    }
                }
            }
        }
    }
    catch (Exception ex)
    {
        throw new Exception("Błąd podczas pobierania recept: " + ex.Message);
    }

    return prescriptions;
}
public async Task<Prescription> AddPrescription(Prescription prescription)
{
    using (var connection = new SqlConnection(_connectionString))
    {
        await connection.OpenAsync();

        using (var command = new SqlCommand())
        {
            command.Connection = connection;
            command.CommandText = "INSERT INTO Prescription (Date, DueDate, IdPatient, IdDoctor) OUTPUT INSERTED.IdPrescription VALUES (@Date, @DueDate, @IdPatient, @IdDoctor)";
            command.Parameters.AddWithValue("@Date", prescription.Date);
            command.Parameters.AddWithValue("@DueDate", prescription.DueDate);
            command.Parameters.AddWithValue("@IdPatient", prescription.IdPatient);
            command.Parameters.AddWithValue("@IdDoctor", prescription.IdDoctor);

            prescription.IdPrescription = (int)await command.ExecuteScalarAsync();
        }
    }

    return prescription;
}
[HttpPost]
public async Task<IActionResult> AddPrescription([FromBody] Prescription prescription)
{
    try
    {
        var addedPrescription = await _prescriptionService.AddPrescription(prescription);
        return Ok(addedPrescription);
    }
    catch (Exception ex)
    {
        return NotFound(ex.Message);
    }
}