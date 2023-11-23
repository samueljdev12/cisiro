using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using cisiro.Models;
using cisiro.services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace cisiro.Controllers
{
    public class AdminController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly AppDataContext _db;
        private readonly string strKey;

        public AdminController(IConfiguration configuration, AppDataContext db)
        {
            _configuration = configuration;
            _db = db;
            strKey = configuration.GetValue<string>("ApiKey");
        }

        // GET
         [HttpGet]
        public async Task<IActionResult> Index()
        {
            if (!User.IsInRole("Admin"))
            {
                return View("UnAuthorized");
            }
            var applicationWithUser = (
                from app in _db.application
                join user in _db.Users
                    on app.candidate.Id equals user.Id into userGroup
                from user in userGroup.DefaultIfEmpty()
                orderby app.gpa descending 
                select new ApplicationWithUserViewModel
                {
                    Application = app,
                    CandidateName = user != null ? $"{user.firstName} {user.lastName} " : "Unknown",
                    mobileNumber = user.mobileNumber,
                    Email = user.Email,


                }

            ).ToList();
            

            return View(applicationWithUser);
        }

        [HttpPost]
        public async Task<IActionResult> Index(string Name)
        {
            if (!User.IsInRole("Admin"))
            {
                return View("UnAuthorized");
            }
            // Assuming _db is your database context
            var applicationWithUser = (
                from app in _db.application
                join user in _db.Users
                    on app.candidate.Id equals user.Id into userGroup
                from user in userGroup.DefaultIfEmpty()
                where user != null && user.firstName == Name  // Use the Name parameter for filtering
                orderby app.gpa descending 
                select new ApplicationWithUserViewModel
                {
                    Application = app,
                    CandidateName = $"{user.firstName} {user.lastName}",
                    mobileNumber = user.mobileNumber,
                    Email = user.Email,
                }
            ).ToList();

            // Do something with the result, for example, pass it to the view
            return View(applicationWithUser);
        }


        public async Task<IActionResult> Qualified()
        {
            if (!User.IsInRole("Admin"))
            {
                return View("UnAuthorized");
            }
            var applicationWithUser = (
                    from app in _db.application
                    join user in _db.Users
                        on app.candidate.Id equals user.Id into userGroup
                    from user in userGroup.DefaultIfEmpty()
                    orderby app.gpa descending 
                    select new ApplicationWithUserViewModel
                    {
                        Application = app,
                        CandidateName = user != null ? $"{user.firstName} {user.lastName}" : "Unknown",
                        mobileNumber = user.mobileNumber, // Use the null-conditional operator
                        Email = user.Email, // Use the null-conditional operator
                    }
                ).Take(10) // Take only the first 10 results
                .ToList();

            return View(applicationWithUser);
        }

        [HttpGet]
        public IActionResult SendEmail(string email, string name)
        {
            if (!User.IsInRole("Admin"))
            {
                return View("UnAuthorized");
            }
            var userClaimsPrincipal = User;

            // Retrieve the user's first name from the FirstName claim (assuming you added this claim during user registration or login)
            var firstName = userClaimsPrincipal.FindFirstValue(ClaimTypes.Name);

            ViewBag.send = firstName;
            ViewBag.Email = email;
            ViewBag.Name = name;
            return View();
        }

        [HttpPost]
        public IActionResult SendEmail(string toEmail, string emailContent, string additionalContent)
        {
            if (!User.IsInRole("Admin"))
            {
                return View("UnAuthorized");
            }

            new Email(toEmail, "OutCome of your Application", emailContent.ToString() + "\n" + additionalContent.ToString(), "", strKey);
            return RedirectToAction("Success"); // Redirect to a success page
        }

        public IActionResult Success()
        {
            if (!User.IsInRole("Admin"))
            {
                return View("UnAuthorized");
            }
            return View();
        }
    }
}