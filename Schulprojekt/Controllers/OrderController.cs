using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NETCore.MailKit.Core;
using Schulprojekt.Data;
using Schulprojekt.Models;
using System.Security.Claims;

namespace Schulprojekt.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        private readonly AppDBContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly EmailService _emailService;
        private readonly IPdfService _pdf;

        public OrderController(AppDBContext context, UserManager<IdentityUser> userManager, EmailService emailService, IPdfService pdf)
        {
            _context = context;
            _userManager = userManager;
            _emailService = emailService;
            _pdf = pdf;
        }

        [HttpGet]
        public IActionResult CheckOut()
        {
            return View(new CheckoutViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout(CheckoutViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var cartItems = await _context.CartItems
                .Include(c => c.Product)
                .Where(c => c.UserId == userId)
                .ToListAsync();

            if (!cartItems.Any())
            {
                ModelState.AddModelError("", "Dein Warenkorb ist leer.");
                return View(model);
            }

            var order = new Order
            {
                UserId = userId,
                OrderDate = DateTime.Now,
                Status = "Offen",
                TotalAmount = cartItems.Sum(c => c.Product.Price * c.Quantity),
                OrderItems = cartItems.Select(c => new OrderItem
                {
                    ProductId = c.ProductId,
                    Quantity = c.Quantity,
                    UnitPrice = c.Product.Price
                }).ToList(),
                ShippingInfo = new ShippingInfo
                {
                    Address = model.Address,
                    City = model.City,
                    PostalCode = model.PostalCode,
                    Country = model.Country,
                    ShippingStatus = "Offen"
                },
                Payment = new Payment
                {
                    Method = model.PaymentMethod,
                    Status = "Offen"
                }
            };

            _context.Orders.Add(order);
            _context.CartItems.RemoveRange(cartItems);
            await _context.SaveChangesAsync();
            //var user = await _userManager.GetUserAsync(User);
            //await _emailService.SendOrderConfirmationAsync(user.Email, order);

            return RedirectToAction("Confirmation", new { id = order.Id });
        }

        [HttpGet]
        public async Task<IActionResult> Confirmation(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var order = await _context.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .Include(o => o.ShippingInfo)
                .Include(o => o.Payment)
                .FirstOrDefaultAsync(o => o.Id == id && o.UserId == userId);

            if (order == null)
                return NotFound();

            return View(order);
        }

        [HttpGet]
        public async Task<IActionResult> DownloadInvoice(int id)
        {
            var order = await _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderItems).ThenInclude(oi => oi.Product)
                .Include(o => o.ShippingInfo)
                .Include(o => o.Payment)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
                return NotFound();

            // Rechnungsnummer setzen, falls noch leer
            if (string.IsNullOrWhiteSpace(order.InvoiceNumber))
            {
                order.InvoiceNumber = GenerateInvoiceNumber(order.OrderDate);
                await _context.SaveChangesAsync();
            }

            var pdfBytes = _pdf.CreateInvoicePdf(order);
            var fileName = $"Rechnung_{order.InvoiceNumber}.pdf";
            return File(pdfBytes, "application/pdf", fileName);
        }

        private string GenerateInvoiceNumber(DateTime date)
        {
            // Simple Sequenz pro Jahr (ersetzbar durch robustere Logik/DB-Sequence)
            var year = date.Year;
            var count = _context.Orders.Count(o => o.OrderDate.Year == year) + 1;
            return $"{year}-{count.ToString("D6")}";
        }

    }
}
