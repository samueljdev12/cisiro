using cisiro.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace cisiro.Controllers
{
    [Authorize]
    public class ApplicationController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly AppDataContext _db;

        public ApplicationController(IConfiguration configuration, UserManager<ApplicationUser> userManager,
            AppDataContext db)
        {
            _configuration = configuration;
            _userManager = userManager;
            _db = db;
        }

        private async void InitializeData(Application application)
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

            //add gpas

            application.gpas = new List<SelectListItem>();

            for (float gpa = 3.0f; gpa <= 4.0f; gpa += 0.1f)
            {
                application.gpas.Add(new SelectListItem(gpa.ToString("0.0"), gpa.ToString("0.0")));
            }
            
        }

       
        [HttpGet]
        public async Task<IActionResult> Apply()
        {
            // Populate user details
            var user = await _userManager.GetUserAsync(User);
            
                var applicationModel = new Application();
                if (user != null)
                {
                    applicationModel.candidate = new ApplicationUser
                    {
                        firstName = user.firstName,
                        lastName = user.lastName,
                        Email = user.Email,
                        mobileNumber = user.mobileNumber
                    };
                }
                
            InitializeData(applicationModel);
                 
                return View(applicationModel); 
        }

      
        [HttpPost]
        public async Task<IActionResult> Apply(Application a)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);

                if (user != null)
                {
                    // Check if the user already has an application
                    var existingApplication = _db.application.FirstOrDefault(app => app.candidate.Id == user.Id);

                    if (existingApplication != null)
                    {
                        // User already has an application; display a message
                        ViewBag.ErrorMessage = "You have already applied.";

                        // Reinitialize degrees, universities, and user details
                        InitializeData(a);
                        return View(a);
                    }

                    // User doesn't have an existing application; proceed to create a new one
                    a.candidate = user;
                    _db.application.Add(a);
                    int savedChanges = _db.SaveChanges();

                    if (savedChanges > 0)
                    {
                        // The operation was successful
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        // No changes were saved; handle as an error
                        // Redirect the user back to the Apply get method
                        InitializeData(a); // Reinitialize degrees, universities, and user details
                        return View(a);
                    }
                }
                else
                {
                    // User not found; handle as an error
                    return View("Error");
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