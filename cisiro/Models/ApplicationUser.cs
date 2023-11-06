using Microsoft.AspNetCore.Identity;

namespace cisiro.Models;

public class ApplicationUser:IdentityUser
{
    public string firstName { get; set; }
    public string latName { get; set; }
    public string mobileNumber { get; set; }

}