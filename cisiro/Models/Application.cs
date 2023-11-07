using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace cisiro.Models;

public class Application
{
    [Key]
    public long applicationId { get; set; }
    public ApplicationUser candidate { get; set;}
    public float? gpa { get; set;}
    public string university { get; set; }
    public string degree { get; set; }
    public List<SelectListItem> universities = new List<SelectListItem>();
    public List<SelectListItem> degrees = new List<SelectListItem>();
    

}