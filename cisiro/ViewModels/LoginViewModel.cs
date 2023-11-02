using Microsoft.AspNetCore.Routing.Constraints;

namespace cisiro.ViewModel
{
    public class LoginViewModel
    {
        public string Email { get; set; }   

        public string Password { get; set; }    

        public bool RememberMe { get; set; }
    }
}
