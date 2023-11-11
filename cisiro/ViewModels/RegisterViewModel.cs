using System.ComponentModel.DataAnnotations;

namespace cisiro.ViewModels
{
    public class RegisterViewModel
    {
        [Display(Name = "First name")]
        [Required(ErrorMessage = "First name is required")]
        
        public string firstName { get; set; }
        
        [Display(Name = "Last name")]
        [Required(ErrorMessage = "last name is required")]
        public string lastName { get; set; }

        [Required(ErrorMessage = "email is required")]
        [EmailAddress]
        
        [Display(Name = "Email")]
        public string email { get; set; }
        
        
        [Display(Name = "Mobile")]
        [Required(ErrorMessage = "mobile number is required")]
        public string mobileNumber { get; set; }
        
         [Display(Name = "Password")]
        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string password { get; set; }
        
        [Display(Name = "Confirm password")]
        [Required(ErrorMessage = "Password is confirm required")]
        [DataType(DataType.Password)]
        [Compare("password", ErrorMessage ="Passwords do not match")]
        public string confirmPassword { get; set; }
    }
}
