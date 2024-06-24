using _6_7.Services;
using Microsoft.AspNetCore.Mvc;

namespace _6_7.Controllers;

[Route("api/[controller]")]
[ApiController]
public class FlightController : ControllerBase
{
    private IFlightService _flightService;

    public FlightController(IFlightService flightService)
    {
        _flightService = flightService;
    }
    
    [HttpGet]
    public async Task<IActionResult> GetFlightsForPassenger([FromQuery] int idPassenger)
    {
        var flights = await _flightService.GetFlightsForPassenger(idPassenger);
        return Ok(flights);
    }
    
    [HttpPost]
    public async Task<IActionResult> AssignPassengerToFlight([FromQuery] int idPassenger, [FromQuery] int idFlight)
    {
        var result = await _flightService.AssignPassengerToFlight(idPassenger, idFlight);
        if (result == -1)
        {
            return NotFound("Passenger does not exist");
        }
        if (result == -2)
        {
            return BadRequest("Flight is in the past");
        }
        if (result == -3)
        {
            return BadRequest("Passenger is already on this flight");
        }
        if (result == -4)
        {
            return BadRequest("No seats left on this flight");
        }
        return Ok(result);
    }

}