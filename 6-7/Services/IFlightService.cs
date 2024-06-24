using _6_7.Models;

namespace _6_7.Services;

public interface IFlightService
{
    Task<int> AssignPassengerToFlight(int idPassenger, int idFlight);
    Task<IEnumerable<Flight>> GetFlightsForPassenger(int idPassenger);
}