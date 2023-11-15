using Microsoft.AspNetCore.Mvc;

namespace cisiro.Controllers;

public class Errors : Controller
{
    // GET
    public IActionResult UnAuthorized()
    {
        return View();
    }
}