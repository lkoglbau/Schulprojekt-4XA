using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace Schulprojekt.Models
{
    public class Order
    {
        public int Id { get; set; }

        [ForeignKey(nameof(UserId))]
        public required string UserId { get; set; }
        public IdentityUser ?User { get; set; }

        public DateTime OrderDate { get; set; }

        public decimal TotalAmount { get; set; }

        public string ?Status { get; set; }

        public ICollection<OrderItem> ?OrderItems { get; set; }
        public ShippingInfo ?ShippingInfo { get; set; }

        public required Payment Payment { get; set; }

        public string InvoiceNumber { get; set; } = string.Empty;
    }
}
