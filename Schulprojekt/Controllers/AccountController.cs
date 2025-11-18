using Microsoft.AspNetCore.Mvc;

namespace Schulprojekt.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
