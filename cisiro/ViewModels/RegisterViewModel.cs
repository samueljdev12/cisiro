using System.ComponentModel.DataAnnotations;

namespace cisiro.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "First name is required")]
        public string firstName { get; set; }
        [Required(ErrorMessage = "last name is required")]
        public string lastName { get; set; }

        [Required(ErrorMessage = "Qualification is required")]
        public string qualification { get; set; }

        [Required(ErrorMessage = "email is required")]
        [EmailAddress]
        public string email { get; set; }
        [Required(ErrorMessage = "mobile number is required")]
        public string mobileNumber { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string password { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        [Compare("password", ErrorMessage ="Passwords do not match")]
        public string confirmPassword { get; set; }
    }
}
