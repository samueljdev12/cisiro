using cisiro.Models;
using cisiro.services;
using cisiro.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
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
        private readonly string strKey;
        private readonly AppDataContext _db;

        public AccountController(IConfiguration config, UserManager<ApplicationUser> _userManager, SignInManager<ApplicationUser> _signInManager, RoleManager<IdentityRole> _roleManager, AppDataContext db)
        {
            _db = db;
            userManager = _userManager;
            configuration = config;
            signInManger = _signInManager;
            roleManager = _roleManager; 
            role = "Candidate";
            strKey = configuration.GetValue<string>("ApiKey");
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
                    var role = await userManager.GetRolesAsync(user);

                    if (role.Contains("Admin"))
                    {
                        return RedirectToAction("Index", "Admin");
                    }if (role.Contains("Candidate"))
                    {
                        return RedirectToAction("apply", "Application");
                    }
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

                    new Email(m.email, "Confirmation email", confirmationLink.ToString(), "", strKey);
                    ViewBag.ErrorMessage += "\n Check your email " + m.email + " for confirmation link";
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

            try
            {
                await signInManger.SignOutAsync();

                // Clear session variables
                HttpContext.Session.Clear();

                // Clear authentication cookies
                await HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);
                await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

                return RedirectToAction("Login", "Account");
            }
            catch (Exception e)
            {
                return RedirectToAction("Error", "Home");
            }
        }
        
        [Authorize(Roles = "Candidate")]
        [HttpGet]
        public async Task<IActionResult> Edit()
        {
            // Retrieve user information from your data source
            // For example, if you're using ASP.NET Identity:
            try
            {
                var user = await userManager.GetUserAsync(User);
                if (user == null)
                {
                    return RedirectToAction("Error", "Home");
                }

                var editViewModel = new EditViewModel
                {
                    FirstName = user.firstName,
                    LastName = user.lastName,
                    MobileNumber = user.mobileNumber,
                    Email = user.Email
                };
                return View(editViewModel);
            }
            catch (Exception e)
            {
                return View("Error");
            }
        }
        
        [Authorize(Roles = "Candidate")]
        [HttpPost]
        public async Task<IActionResult> Edit(EditViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Update user information in your data source
                // For example, if you're using ASP.NET Identity:
                try
                {
                    var user = await userManager.GetUserAsync(User);
                    if (user == null)
                    {
                        return RedirectToAction("Error", "Home");
                    }
                    user.firstName = model.FirstName;
                    user.lastName = model.LastName;
                    user.mobileNumber = model.MobileNumber;
                    user.Email = model.Email;

                    await userManager.UpdateAsync(user);

                    // Redirect to a success page or another action
                    return View(model);
                }
                catch (Exception e)
                {
                    return View("Error");
                }
            }

            // If the model state is not valid, return to the edit page with validation errors
            return View(model);
        }
       
        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            if (!User.IsInRole("Candidate"))
            {
                return View("UnAuthorized");
            }
            try
            {
                var user = await userManager.GetUserAsync(User);
                if (user == null)
                {
                    return RedirectToAction("Error", "Home");
                }

                // Assuming _db is your database context
                var userApplication = _db.application.FirstOrDefault(app => app.candidate.Id == user.Id);

                var profileViewModel = new ProfileViewModel
                {
                    User = user,
                    UserApplication = userApplication
                };

                return View(profileViewModel);
            }
            catch (Exception e)
            {
                return View("Error");
            }
        }

        
        [HttpGet]
        public IActionResult forgotPassword()
        {
            return View();
        }
        
        [HttpPost]
        public async Task<IActionResult> forgotPassword(ForgotPasswordViewModel model)
        {
            try
            {
                var user = await userManager.FindByEmailAsync(model.Email);
                if (user == null || !(await userManager.IsEmailConfirmedAsync(user)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return RedirectToAction("Error", "Home");
                }
                
                var Token = await userManager.GeneratePasswordResetTokenAsync(user);
                var callbackUrl = Url.Action("ResetPassword", "Account", new { email = user.Email, Token }, protocol: HttpContext.Request.Scheme);
                new Email(model.Email, "Confirmation email", callbackUrl.ToString(), "Samueljonas922@gmail.com", strKey);
                return View("halfway");
            }
            catch(Exception e)
            {
                return View("Error");
            }
            
            
        }
        
        
        [HttpGet]
        public IActionResult ResetPassword(string email, string token)
        {
            // You may validate the token and email here if needed

            var model = new ResetPasswordViewModel
            {
                Email = email,
                Token = token
            };

            return View(model);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByEmailAsync(model.Email);
                if (user != null)
                {
                    var result = await userManager.ResetPasswordAsync(user, model.Token, model.Password);

                    if (result.Succeeded)
                    {
                        // Password reset successful, you might want to redirect to a success page
                        return View("success", "Account");
                    }
                    else
                    {
                        // Password reset failed, add errors to ModelState
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                    }
                }
                else
                {
                    // User not found, you might want to show a generic message to avoid revealing valid emails
                    ModelState.AddModelError(string.Empty, "Invalid attempt");
                }
            }

            // If ModelState is not valid, return to the same view with the validation errors
            return View(model);
        }
    }
}
