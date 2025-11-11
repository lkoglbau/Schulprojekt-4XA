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
        public required Product Product { get; set; }

        [ForeignKey(nameof(UserId))]
        public required string UserId { get; set; }
        public required IdentityUser User { get; set; }

        [Range(1, 5)]
        public int Rating { get; set; }

        [Required]
        public string Comment { get; set; } = string.Empty;

        public DateTime CreatedAd { get; set; }
    }
}
