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
            var applicationsWithUser = _db.application
                .AsEnumerable() // Convert to client-side evaluation
                .GroupJoin(
                    _db.Users,
                    app => app.candidate.Id,
                    user => user.Id,
                    (app, users) => new
                    {
                        Application = app,
                        Users = users.DefaultIfEmpty(),
                    }
                )
                .SelectMany(
                    x => x.Users,
                    (app, user) => new ApplicationWithUserViewModel
                    {
                        Application = app.Application,
                        CandidateName = user != null ? $"{user.firstName} {user.lastName}" : "Unknown",
                        // Include other properties as needed
                    }
                )
                .ToList();

            return View(applicationsWithUser);
        }


    }
}