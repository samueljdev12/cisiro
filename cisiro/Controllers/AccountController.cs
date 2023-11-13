using cisiro.Models;
using cisiro.services;
using cisiro.ViewModels;
using Microsoft.AspNetCore.Authentication;
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
        public readonly string strKey;

        public AccountController(IConfiguration config, UserManager<ApplicationUser> _userManager, SignInManager<ApplicationUser> _signInManager, RoleManager<IdentityRole> _roleManager)
        {
            userManager = _userManager;
            configuration = config;
            signInManger = _signInManager;
            roleManager = _roleManager; 
            role = "Candidate";
            strKey = configuration.GetValue<string>("SendGrikKey");
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
                    var sid = HttpContext.Session.Id;
                    HttpContext.Session.SetString("user_id", sid);

                    return RedirectToAction("apply", "Application");
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
                    UserName = m.email, Email = m.email, firstName = m.firstName, lastName = m.lastName,
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
                    var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
                    var confirmationLink = Url.Action("ConfirmEmail", "Account", new {userId = user.Id, token = token}, Request.Scheme);
                    ViewBag.ErrorMessage = "You are almost there!";

                    new Email(m.email, "Confirmation email", confirmationLink.ToString(), "Samueljonas922@gmail.com", "");
                    ViewBag.ErrorMessage += "\n Check your email " + m.email + "for link";
                    return View("emails");
                }
                
                foreach( var e in result.Errors)
                {
                    
                    ModelState.AddModelError("", e.Description);
                }
                
                
            }

            return View(m);
        }

        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            var user = await userManager.FindByIdAsync(userId);
            if(user == null)
            {
                ViewBag.ErrorMessage = "User does not exists";
                return View("Error");

            }

            var result = await userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
            {

                ViewBag.ErrorMessage = "Success";
                return View("Login");
            }

             ViewBag.ErrorMessage = "Email not confirmed";
            return View("Error");
        }
        
        
        public async Task<IActionResult> Logout()
        {
            
            await signInManger.SignOutAsync();

            // Clear session variables
            HttpContext.Session.Clear();

            // Clear authentication cookies
            await HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            return RedirectToAction("Login", "Account");
        }
        
        [HttpGet]
        public async Task<IActionResult> Edit()
        {
            // Retrieve user information from your data source
            // For example, if you're using ASP.NET Identity:
            var user = await userManager.GetUserAsync(User);

            var editViewModel = new EditViewModel
            {
                FirstName = user.firstName,
                LastName = user.lastName,
                MobileNumber = user.mobileNumber,
                Email = user.Email
            };

            return View(editViewModel);
        }
        
        [HttpPost]
        public async Task<IActionResult> Edit(EditViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Update user information in your data source
                // For example, if you're using ASP.NET Identity:
                var user = await userManager.GetUserAsync(User);

                user.firstName = model.FirstName;
                user.lastName = model.LastName;
                user.mobileNumber = model.MobileNumber;
                user.Email = model.Email;

                await userManager.UpdateAsync(user);

                // Redirect to a success page or another action
                return View(model);
            }

            // If the model state is not valid, return to the edit page with validation errors
            return View(model);
        }
    }
}
