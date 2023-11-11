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
    [Authorize]
    public class AdminController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly AppDataContext _db;

        public AdminController(IConfiguration configuration, AppDataContext db)
        {
            _configuration = configuration;
            _db = db;
        }

        // GET

        public async Task<IActionResult> Index()
        {
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

        [HttpGet]
        public IActionResult SendEmail(string email, string name)
        {
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

            new Email(toEmail, "OutCome of your Application", emailContent.ToString() + additionalContent.ToString(), "Samueljonas922@gmail.com", "gyrj ykjo gjhw flkk");
            return RedirectToAction("Success"); // Redirect to a success page
        }

        public IActionResult Success()
        {
            return View();
        }
    }
}