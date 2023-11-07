using cisiro.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace cisiro.Controllers
{
    public class ApplicationController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly AppDataContext _db;

        public ApplicationController(IConfiguration configuration, UserManager<ApplicationUser> userManager, AppDataContext db)
        {
            _configuration = configuration;
            _userManager = userManager;
            _db = db;
        }

        private void InitializeData(Application application)
        {
            // Initialize the list of universities
            application.universities = new List<SelectListItem>
            {
                new SelectListItem("Harvard University", "Harvard University"),
                new SelectListItem("Stanford University", "Stanford University"),
                // Add the top 100 universities here
                // You can add more universities as needed
            };

            // Initialize the list of degrees
            application.degrees = new List<SelectListItem>
            {
                new SelectListItem("Master of Data Science", "Master of Data Science"),
                new SelectListItem("Master of AI", "Master of AI"),
                new SelectListItem("Master of Information Technology", "Master of Information Technology")
            };

            // Populate user details
            var user = _userManager.FindByEmailAsync("samuel@gmail.com").Result;

            if (user != null)
            {
                application.candidate = new ApplicationUser
                {
                    firstName = user.firstName,
                    lastName = user.lastName,
                    Email = user.Email,
                    mobileNumber = user.mobileNumber
                };
            }
        }

        [HttpGet]
        public async Task<IActionResult> Apply()
        {
            var applicationModel = new Application();
            InitializeData(applicationModel);
            return View(applicationModel);
        }

        [HttpPost]
        public async Task<IActionResult> Apply(Application a)
        {
            try
            {
                _db.application.Add(a);
                int savedChanges = _db.SaveChanges();

                if (savedChanges > 0)
                {
                    // The operation was successful
                    return RedirectToAction("login", "Account");
                }
                else
                {
                    // No changes were saved; handle as an error
                    // Redirect the user back to the Apply get method
                    InitializeData(a); // Reinitialize degrees, universities, and user details
                    return View(a);
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions that might occur during the database operation
                return View("Error");
            }
        }
    }
}
