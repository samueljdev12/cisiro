using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using cisiro.Models;
using Microsoft.EntityFrameworkCore;

namespace cisiro.Controllers
{
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


    }
}