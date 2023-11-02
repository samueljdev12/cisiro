using Microsoft.AspNetCore.Mvc;

namespace cisiro.Controllers
{
    public class ApplicationController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Apply()
        {

            return View();
        }
    }
}
