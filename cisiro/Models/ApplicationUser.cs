using Microsoft.AspNetCore.Identity;

namespace cisiro.Models;

public class ApplicationUser:IdentityUser
{
    public string firstName { get; set; }
    public string lastName { get; set; }
    public string mobileNumber { get; set; }

}