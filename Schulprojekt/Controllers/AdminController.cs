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


        public IActionResult ProductList()
        {
            var products = _context.Products
                .Include(p => p.Category)
                .ToList();
            return View(products);
        }

        public IActionResult ReviewsList()
        {
            return View();
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

