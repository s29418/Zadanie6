using _6_7.Models;
using _6_7.Repositories;

namespace _6_7.Services;

public class FlightService : IFlightService
{
    private readonly IFlightRepository _flightRepository;

    public FlightService(IFlightRepository repository)
    {
        _flightRepository = repository;
    }
    
    public async Task<IEnumerable<Flight>> GetFlightsForPassenger(int idPassenger)
    {
        if (!await _flightRepository.PassengerExists(idPassenger))
        {
            throw new ArgumentException("Passenger does not exist");
        }

        var flightsIds = await _flightRepository.GetFlightsIdsForPassenger(idPassenger);
        return await _flightRepository.GetFlightsDetailsForPassenger(flightsIds.ToList());
    }
    
    public async Task<int> AssignPassengerToFlight(int idPassenger, int idFlight)
    {
        if (!await _flightRepository.PassengerExists(idPassenger))
        {
            return -1;
        }
        if (!await _flightRepository.FlightInPast(idFlight))
        {
            return -2;        
        }
        if (await _flightRepository.PassengerAlreadyOnFlight(idPassenger, idFlight))
        {
            return -3;        
        }
        if (await _flightRepository.AnySeatsLeft(idFlight))
        {
            return -4;
        }
        
        return await _flightRepository.AddPassengerToFlight(idFlight, idPassenger);
        
    }
    
}