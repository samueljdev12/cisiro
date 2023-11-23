using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Routing.Constraints;

namespace cisiro.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Email is reuired")]
        public string Email { get; set; }   

        [Required(ErrorMessage="Password is required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }    

        public bool RememberMe { get; set; }
    }
}
