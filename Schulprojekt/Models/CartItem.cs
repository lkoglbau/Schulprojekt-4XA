using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Schulprojekt.Models
{
    public class CartItem
    {
        public int Id { get; set; }

        [ForeignKey(nameof(ProductId))]
        public int ProductId { get; set; }

        public Product ?Product { get; set; }

        public int Quantity { get; set; }

        [ForeignKey(nameof(UserId))]
        public required string UserId { get; set; }
        public IdentityUser ?User { get; set; }
    }
}
