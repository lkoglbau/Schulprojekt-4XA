using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Schulprojekt.Data;
using Schulprojekt.Models.ViewModels;

namespace Schulprojekt.Controllers
{
    public class AdminController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly AppDBContext _context;
        public AdminController(UserManager<IdentityUser> userManager, AppDBContext context)
        {
            _userManager = userManager;
            _context = context;
        }


        public IActionResult Dashboard()
        {
            return View();
        }

        public IActionResult OrderList()
        {
            var orders = _context.Orders
            .Include(o => o.User)
            .Include(o => o.OrderItems)
                .ThenInclude(i => i.Product)
            .OrderByDescending(o => o.OrderDate)
            .ToList();

            return View(orders);
        }

        public async Task<IActionResult> OrderDetails(int id)
        {
            var order = await _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                    .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
                return NotFound();

            return View(order);
        }


        public async Task<IActionResult> ProductList()
        {
            var products = _context.Products
                .Include(p => p.Category)
                .ToListAsync();
            return View(products);
        }

        //---------------------------------------------------------------------REVIEWS VERWALTEN-------------------------------------------------------------------


        public async Task<IActionResult> ReviewsList(string filter = "all")
        {
            var reviewsQuery = _context.Reviews
            .Include(r => r.User)
            .Include(r => r.Product)
            .OrderByDescending(r => r.CreatedAd)
            .AsQueryable();

            switch (filter.ToLower())
            {
                case "approved":
                    reviewsQuery = reviewsQuery.Where(r => r.IsApproved);
                    break;
                case "pending":
                    reviewsQuery = reviewsQuery.Where(r => !r.IsApproved);
                    break;
                default:
                    // "all"
                    break;
            }

            var reviews = await reviewsQuery.ToListAsync();
            ViewBag.Filter = filter;


            return View(reviewsQuery);
        }

        [HttpPost]
        public async Task<IActionResult> ApproveReview(int id)
        {
            var review = _context.Reviews.Find(id);
            if (review != null)
            {
                review.IsApproved = true;
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("ReviewsList");
        }

        [HttpPost]
        public async Task<IActionResult> HideReview(int id)
        {
            var review = _context.Reviews.Find(id);
            if (review != null)
            {
                review.IsApproved = false;
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("ReviewsList");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteReview(int id)
        {
            var review = _context.Reviews.Find(id);
            if (review != null)
            {
                _context.Reviews.Remove(review);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("ReviewsList");
        }





        //---------------------------------------------------------------------USER VERWALTUNG-------------------------------------------------------------------
        public async Task<IActionResult> UsersList()
        {
            var users = _userManager.Users.ToList();
            var userList = new List<AdminUserViewModel>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userList.Add(new AdminUserViewModel
                {
                    Id = user.Id,
                    Email = user.Email,
                    Roles = roles.ToList()
                });
            }

            return View(userList);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateUserRole(string userId, string selectedRole)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();

            var currentRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, currentRoles);
            await _userManager.AddToRoleAsync(user, selectedRole);

            return RedirectToAction("UsersList");
        }

        [HttpGet]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            if (user.Id == _userManager.GetUserId(User))
            {
                ModelState.AddModelError("", "Du kannst dich nicht selbst löschen.");
                return RedirectToAction("UsersList");
            }
            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                // Optional: Fehlerbehandlung oder Logging
                ModelState.AddModelError("", "Benutzer konnte nicht gelöscht werden.");
            }

            return RedirectToAction("UsersList");
        }
    }

}

