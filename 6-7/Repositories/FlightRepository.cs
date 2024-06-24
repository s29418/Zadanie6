using System.Data.SqlClient;
using _6_7.Models;

namespace _6_7.Repositories;

public class FlightRepository : IFlightRepository
{
    private IConfiguration _configuration;

    public FlightRepository(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<bool> PassengerExists(int idPass)
    {
        await using var con = new SqlConnection(_configuration["ConnectionStrings:DefaultConnection"]);
        await con.OpenAsync();
        
        await using var cmd = new SqlCommand();
        cmd.Connection = con;
        cmd.CommandText = "SELECT COUNT(*) FROM Passenger WHERE IdPassenger = @IdPassenger";
        cmd.Parameters.AddWithValue("@IdPassenger", idPass);
        
        var result = await cmd.ExecuteScalarAsync();
        return (int)result != 0;
    }

    public async Task<IEnumerable<int>> GetFlightsIdsForPassenger(int idPassenger)
    {
        await using var con = new SqlConnection(_configuration["ConnectionStrings:DefaultConnection"]);
        await con.OpenAsync();
        
        await using var cmd = new SqlCommand();
        cmd.Connection = con;
        cmd.CommandText = "SELECT IdFlight FROM Flight_Passenger WHERE IdPassenger=@IdPassenger";
        cmd.Parameters.AddWithValue("@IdPassenger", idPassenger);

        List<int> flightsIds = new List<int>();

        var dr = await cmd.ExecuteReaderAsync();
        while (await dr.ReadAsync())
        {
            flightsIds.Add((int)dr["IdFlight"]);
        }
        
        return flightsIds;
    }

    public async Task<IEnumerable<Flight>> GetFlightsDetailsForPassenger(List<int> ids)
    {
        List<Flight> passengersFlights = new List<Flight>();
        await using var con = new SqlConnection(_configuration["ConnectionStrings:DefaultConnection"]);
        await con.OpenAsync();
        
        await using var cmd = new SqlCommand();
        cmd.Connection = con;
        
        foreach (int id in ids)
        {
            cmd.CommandText = "SELECT IdFlight, FlightDate, Comments, IdPlane, Name, MaxSeat, IdCityDict, City FROM Flight f LEFT JOIN Plane p ON f.IdPlane=p.IdPlane LEFT JOIN CityDict cd ON f.IdCityDict=cd.IdCityDict WHERE IdFlight=@IdFlight";
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@IdFlight", id);
            
            var dr = await cmd.ExecuteReaderAsync();
            while (await dr.ReadAsync())
            {
                Flight flight = new Flight
                {
                    FlightId = (int)dr["IdFlight"],
                    FlightDate = (DateTime)dr["FlightDate"],
                    Comments = dr["Comments"].ToString(),
                    Plane = new Plane
                    {
                        IdPlane = (int)dr["IdPlane"],
                        Name = dr["Name"].ToString(),
                        MaxSeat = (int)dr["MaxSeat"]
                    },
                    City = new City
                    {
                        IdCity = (int)dr["IdCityDict"],
                        CityName = dr["City"].ToString()
                    }
                };
                passengersFlights.Add(flight);
            }
            await dr.CloseAsync();
        }
        return passengersFlights;
    }

    public async Task<bool> FlightInPast(int flightId)
    {
        await using var con = new SqlConnection(_configuration["ConnectionStrings:DefaultConnection"]);
        await con.OpenAsync();
        
        await using var cmd = new SqlCommand();
        cmd.Connection = con;
        cmd.CommandText = "SELECT FlightDate FROM Flight WHERE IdFlight=@IdFlight";
        cmd.Parameters.AddWithValue("@IdFlight", flightId);
        
        var result = await cmd.ExecuteScalarAsync();
        if ((DateTime)result < DateTime.Now || result == null)
        {
            return false;
        }

        return true;
    }
    
    public async Task<int> GetMaxSeatsForFlight(int flightId)
    {
        await using var con = new SqlConnection(_configuration["ConnectionStrings:DefaultConnection"]);
        await con.OpenAsync();
        
        await using var cmd = new SqlCommand();
        cmd.Connection = con;
        cmd.CommandText = "SELECT MaxSeat FROM Plane p LEFT JOIN Flight f ON p.IdPlane=f.IdPlane WHERE IdFlight=@IdFlight";
        cmd.Parameters.AddWithValue("@IdFlight", flightId);
        
        var result = await cmd.ExecuteScalarAsync();
        return (int)result;
    }
    

    public async Task<bool> AnySeatsLeft(int flightId)
    {
        int maxSeat = await GetMaxSeatsForFlight(flightId);
        await using var con = new SqlConnection(_configuration["ConnectionStrings:DefaultConnection"]);
        await con.OpenAsync();
        
        await using var cmd = new SqlCommand();
        cmd.Connection = con;
        cmd.CommandText = "SELECT COUNT(*) FROM Flight_Passenger WHERE IdFlight=@IdFlight";
        cmd.Parameters.AddWithValue("@IdFlight", flightId);
        var result = await cmd.ExecuteScalarAsync();
        if ((int)result < maxSeat)
        {
            return true;
        }
        return false;
    }
    
    public async Task<bool> PassengerAlreadyOnFlight(int flightId, int passengerId)
    {
        await using var con = new SqlConnection(_configuration["ConnectionStrings:DefaultConnection"]);
        await con.OpenAsync();
        
        await using var cmd = new SqlCommand();
        cmd.Connection = con;
        cmd.CommandText = "SELECT COUNT(*) FROM Flight_Passenger WHERE IdFlight=@IdFlight AND IdPassenger=@IdPassenger";
        cmd.Parameters.AddWithValue("@IdFlight", flightId);
        cmd.Parameters.AddWithValue("@IdPassenger", passengerId);
        
        var result = await cmd.ExecuteScalarAsync();
        return (int)result != 0;
    }

    public async Task<int> AddPassengerToFlight(int flightId, int passengerId)
    {
        await using var con = new SqlConnection(_configuration["ConnectionStrings:DefaultConnection"]);
        await con.OpenAsync();
        
        await using var cmd = new SqlCommand();
        cmd.Connection = con;
        cmd.CommandText = "INSERT INTO Flight_Passenger (IdFlight, IdPassenger) VALUES (@IdFlight, @IdPassenger)";
        cmd.Parameters.AddWithValue("@IdFlight", flightId);
        cmd.Parameters.AddWithValue("@IdPassenger", passengerId);
        
        return (int) await cmd.ExecuteScalarAsync();
    }

}