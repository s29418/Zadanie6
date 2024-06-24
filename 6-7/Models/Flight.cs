using System.ComponentModel.DataAnnotations;

namespace _6_7.Models;

public class Flight
{
    [Required] 
    public int FlightId { get; set; }
    
    [Required] 
    public DateTime FlightDate { get; set; }
    
    [MaxLength(200)]
    public string Comments { get; set; }
    
    [Required] 
    public Plane Plane { get; set; }
    
    [Required] 
    public City City { get; set; }
}