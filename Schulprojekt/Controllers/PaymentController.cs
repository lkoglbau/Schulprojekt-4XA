using Microsoft.AspNetCore.Mvc;

namespace Schulprojekt.Controllers
{
    public class PaymentController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
