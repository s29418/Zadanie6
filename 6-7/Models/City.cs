using System.ComponentModel.DataAnnotations;

namespace _6_7.Models;

public class City
{
    [Required] 
    public int IdCity { get; set; }
    
    [Required] 
    [MaxLength(30)]
    public string CityName { get; set; }
}