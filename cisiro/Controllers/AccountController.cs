using cisiro.Models;
using cisiro.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace cisiro.Controllers
{
    public class AccountController : Controller
    {
        private UserManager<ApplicationUser> userManager { get; }
        private readonly IConfiguration configuration;
        private RoleManager<IdentityRole> roleManager { get; }
        private SignInManager<ApplicationUser> signInManger { get; }
        private string role { get; set; }

        public AccountController(IConfiguration config, UserManager<ApplicationUser> _userManager, SignInManager<ApplicationUser> _signInManager, RoleManager<IdentityRole> _roleManager)
        {
            userManager = _userManager;
            configuration = config;
            signInManger = _signInManager;
            roleManager = _roleManager; 
            role = "Candidate";
        }
        
        
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel m)
        {
            if (ModelState.IsValid)
            {
                var result = await signInManger.PasswordSignInAsync(m.Email, m.Password, m.RememberMe, false);
                if (result.Succeeded)
                {
                    var user = await userManager.FindByEmailAsync(m.Email);
                    var userId = user.Id;

                    return RedirectToAction("index", "Home");
                }
                
                ModelState.AddModelError("", "Invalid Credentials");
            }
            return View();
        }

        [HttpGet]
        public IActionResult Register ()
        {
            return View(new RegisterViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel m)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = m.email, Email = m.email, firstName = m.firstName, latName = m.lastName,
                     mobileNumber = m.mobileNumber
                };
                var result = await userManager.CreateAsync(user, m.password);
                if (result.Succeeded)
                {
                     
                    if (!await roleManager.RoleExistsAsync(role))
                    {
                        await roleManager.CreateAsync(new IdentityRole(role));
                        await roleManager.CreateAsync(new IdentityRole("Admin"));
                    }
                    await userManager.AddToRoleAsync(user, role);
                    // You can also sign the user in after registration if needed
                    // await signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("index", "Home");
                }
                
                foreach( var e in result.Errors)
                {
                    
                    ModelState.AddModelError("", e.Description);
                }
                
                
            }

            return View(m);
        }
    }
}
