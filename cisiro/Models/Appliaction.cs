using System.ComponentModel.DataAnnotations;

namespace cisiro.Models;

public class Appliaction
{
    [Key]
    public long applicationId { get; set; }
    public ApplicationUser candidate { get; set;}
    public float? gpa { get; set;}
    
}