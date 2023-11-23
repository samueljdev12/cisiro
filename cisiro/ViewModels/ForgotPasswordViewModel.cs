using System.ComponentModel.DataAnnotations;

namespace cisiro.ViewModels;

public class ForgotPasswordViewModel
{
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    
}