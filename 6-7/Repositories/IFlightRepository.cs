using _6_7.Models;

namespace _6_7.Repositories;

public interface IFlightRepository
{
    Task<bool> PassengerExists(int idPass);
    Task<IEnumerable<int>> GetFlightsIdsForPassenger(int idPassenger);
    Task<IEnumerable<Flight>> GetFlightsDetailsForPassenger(List<int> ids);
    Task<bool> FlightInPast(int flightId);
    Task<int> GetMaxSeatsForFlight(int flightId);
    Task<bool> AnySeatsLeft(int flightId);
    Task<bool> PassengerAlreadyOnFlight(int flightId, int passengerId);
    Task<int> AddPassengerToFlight(int flightId, int passengerId);

}