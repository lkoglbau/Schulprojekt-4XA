using System.ComponentModel.DataAnnotations;

namespace Schulprojekt.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        public ICollection<Product> ?Products { get; set; }


    }
}
