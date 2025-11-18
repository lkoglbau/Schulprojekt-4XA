using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Schulprojekt.Data;
using Schulprojekt.Models;
using Schulprojekt.Models.ViewModels;
using System.IO;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Schulprojekt.Controllers
{
    public class ProductController : Controller
    {
        private readonly AppDBContext _context;

        public ProductController(AppDBContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string category)
        {
            var categories = await _context.Categories.ToListAsync();

            var products = string.IsNullOrEmpty(category)
                ? await _context.Products.Include(p => p.Category).ToListAsync()
                : await _context.Products
                    .Include(p => p.Category)
                    .Where(p => p.Category.Name == category)
                    .ToListAsync();

            var viewModel = new ProductFilterViewModel
            {
                Categories = categories,
                SelectedCategory = category,
                Products = products
            };

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var product = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Reviews.Where(r => r.IsApproved))
                    .ThenInclude(r => r.User)
                .FirstOrDefaultAsync(p => p.Id == id);
            return View(product);
        }

        [HttpGet]
        public IActionResult Create()
        {
            ViewData["Categories"] = new SelectList(_context.Categories, "Id", "Name");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Product product)
        {
            if (ModelState.IsValid)
            {
                if (product.ImageFile != null)
                {
                    string wwwRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(product.ImageFile.FileName);
                    string path = Path.Combine(wwwRootPath + "/images/", fileName);

                    using (var fileStream = new FileStream(path, FileMode.Create))
                    {
                        await product.ImageFile.CopyToAsync(fileStream);
                    }

                    product.ImageName = fileName;
                }

                await _context.Products.AddAsync(product);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                ViewData["Categories"] = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
                return View(product);
            }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();

            ViewData["Categories"] = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
            return View(product);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Product product)
        {


            if (ModelState.IsValid)
            {
                try
                {
                    if (product.ImageFile != null)
                    {
                        string wwwRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                        string fileName = Guid.NewGuid().ToString() + Path.GetExtension(product.ImageFile.FileName);
                        string path = Path.Combine(wwwRootPath + "/images/", fileName);

                        using (var fileStream = new FileStream(path, FileMode.Create))
                        {
                            await product.ImageFile.CopyToAsync(fileStream);
                        }

                        product.ImageName = fileName;
                    }
                    else
                    {
                        var existingProduct = await _context.Products.AsNoTracking().FirstOrDefaultAsync(p => p.Id == product.Id);
                        if (existingProduct != null)
                        {
                            product.ImageName = existingProduct.ImageName;
                        }
                    }

                    _context.Update(product);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Products.Any(e => e.Id == product.Id))
                        return NotFound();
                    else
                        throw;
                }
            }

            ViewData["Categories"] = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
            return View(product);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null) return NotFound();

            return View(product);
        }


        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Category(string category)
        {
            var products = await _context.Products
                .Include(p => p.Category)
                .Where(p => p.Category.Name == category)
                .ToListAsync();

            var categories = await _context.Categories.ToListAsync();

            var viewModel = new ProductFilterViewModel
            {
                Products = products,
                Categories = categories,
                SelectedCategory = category
            };

            return View("Index", viewModel); // ← View erwartet ViewModel
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddReview(int productId, int rating, string comment)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var review = new Review
            {
                ProductId = productId,
                UserId = userId,
                Rating = rating,
                Comment = comment,
                CreatedAd = DateTime.Now,
                IsApproved = false
            };

            await _context.Reviews.AddAsync(review);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Danke für deine Bewertung! Sie wird nach Freigabe sichtbar.";

            return RedirectToAction("Details", new { id = productId });
        }


        public async Task<IActionResult> Bestseller() {

            var count = _context.OrderItems.Count();

            var bestseller = await _context.OrderItems
                .Include(i => i.Product)
                .GroupBy(i => i.Product)
                .Select(g => new BestsellerViewModel 
                {
                    Product = g.Key,
                    TotalSold = g.Sum(i => i.Quantity)
                })
                .OrderByDescending(x => x.TotalSold)
                .Take(3)
                .ToListAsync();

            return View(bestseller);
        }

    }
}


