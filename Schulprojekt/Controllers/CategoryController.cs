using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Schulprojekt.Data;
using Schulprojekt.Models;

namespace Schulprojekt.Controllers
{
    public class CategoryController : Controller
    {

        private readonly AppDBContext _context;

        public CategoryController(AppDBContext context)
        {
        _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var categories = await _context.Categories.ToListAsync();
            return View(categories);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Create(Category category)
        {
            if (ModelState.IsValid)
            {
                await _context.Categories.AddAsync(category);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }
    }
}
