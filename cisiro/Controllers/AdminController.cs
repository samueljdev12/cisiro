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
                .Join(
                    _db.Users,
                    app => app.candidate.Id,
                    user => user.Id,
                    (app, user) => new ApplicationWithUserViewModel
                    {
                        Application = app,
                        CandidateName = $"{user.firstName} {user.lastName}",
                        // Include other properties as needed
                    }
                )
                .ToList();

            return View(applicationsWithUser);
        }
    }
}