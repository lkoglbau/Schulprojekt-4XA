using Microsoft.AspNetCore.Mvc;

namespace Schulprojekt.Controllers
{
    public class ReviewController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
