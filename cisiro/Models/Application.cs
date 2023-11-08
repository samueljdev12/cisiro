using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace cisiro.Models;

public class Application
{
    [Key]
    public long applicationId { get; set; }
    public ApplicationUser candidate { get; set;}
    [Required(ErrorMessage = "GPA is required")]
    [Range(3.0, double.MaxValue, ErrorMessage = "GPA must be greater than 3.0")]
    public float? gpa { get; set;}
    [Required(ErrorMessage = "University is required")]
    public string university { get; set; }
    [Required(ErrorMessage = "Degree is required")]
    public string degree { get; set; }
    public List<SelectListItem> universities = new List<SelectListItem>();
    public List<SelectListItem> degrees = new List<SelectListItem>();
    public List<SelectListItem> gpas = new List<SelectListItem>();


}