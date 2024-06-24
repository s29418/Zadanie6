using System.ComponentModel.DataAnnotations;

namespace _6_7.Models;

public class Plane
{
    [Required] 
    public int IdPlane { get; set; }
    
    [Required] 
    [MaxLength(50)]
    public string Name { get; set; }
    
    [Required] 
    public int MaxSeat { get; set; }
}