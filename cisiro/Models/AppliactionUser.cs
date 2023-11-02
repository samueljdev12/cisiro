using Microsoft.AspNetCore.Identity;

namespace cisiro.Models;

public class AppliactionUser:IdentityUser
{
    public string firstName { get; set; }
    public string latName { get; set; }
    public string qualification { get; set;}
    public string mobileNumber { get; set; }

}