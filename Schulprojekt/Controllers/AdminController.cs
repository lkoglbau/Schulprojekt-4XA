using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Schulprojekt.Models.ViewModels;

namespace Schulprojekt.Controllers
{
    public class AdminController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        public AdminController(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }


        public IActionResult Dashboard()
        {
            return View();
        }

        public IActionResult OrderList()
        {
            return View();
        }

        public IActionResult ProductList()
        {
            return View();
        }

        public IActionResult ReviewsList()
        {
            return View();
        }

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
    }
}
