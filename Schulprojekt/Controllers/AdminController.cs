using Microsoft.AspNetCore.Mvc;

namespace Schulprojekt.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
