using cisiro.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);
IConfiguration configuration;
configuration = new ConfigurationBuilder().AddJsonFile("./config.json").Build();

// Add services to the container.
builder.Services.AddControllersWithViews();
// add our app context
builder.Services.AddDbContext<AppDataContext>(options =>
{
    var connectionString = configuration.GetConnectionString("DBConnection");
    options.UseSqlServer(connectionString);
});

// builder.Services.AddIdentity<ApplicationUser, IdentityRole>();
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
    {
        options.SignIn.RequireConfirmedEmail = false;
    })
    .AddEntityFrameworkStores<AppDataContext>()
    .AddDefaultTokenProviders();

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Application}/{action=Apply}/{id?}");

app.Run();
