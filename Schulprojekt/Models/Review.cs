using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Schulprojekt.Models
{
    public class Review
    {
        public int Id { get; set; }

        [ForeignKey(nameof(ProductId))]
        public int ProductId { get; set; }
        public Product ?Product { get; set; }

        [ForeignKey(nameof(UserId))]
        public required string UserId { get; set; }
        public IdentityUser ?User { get; set; }

        [Range(1, 5)]
        public int Rating { get; set; }

        [Required]
        public string Comment { get; set; } = string.Empty;

        public DateTime CreatedAd { get; set; }

        public bool IsApproved { get; set; } = false;
    }
}
