using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.Metrics;

namespace Schulprojekt.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        [Required]
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }

        [NotMapped]
        public IFormFile ?ImageFile { get; set; }

        public string? ImageName { get; set; } = "b1cc3620-0949-4228-8433-5eec301d949e.jpg";


        [ForeignKey(nameof(CategoryId))]
        public int CategoryId { get; set; }
        public  Category ?Category { get; set; }

        public ICollection<Review> ?Reviews { get; set; }
    }
}
