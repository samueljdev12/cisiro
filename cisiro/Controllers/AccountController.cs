using Microsoft.AspNetCore.Mvc;

namespace cisiro.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Login()
        {
            return View();
        }
    }
}
