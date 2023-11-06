using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace cisiro.Models;

public class AppDataContext:IdentityDbContext<ApplicationUser>
{
    public DbSet<Appliaction> application { get; set; }
    public AppDataContext(DbContextOptions<AppDataContext> options):base(options)
    {
        Database.EnsureCreated();
    }
}