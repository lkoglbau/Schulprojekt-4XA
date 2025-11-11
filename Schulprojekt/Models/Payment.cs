using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Schulprojekt.Models
{
    public class Payment
    {
        public int Id { get; set; }

        [Required]
        public string Method { get; set; } = string.Empty;

        [Required]
        public string Status { get; set; } = string.Empty;

        [Required]
        public string TransactionId { get; set; } = string.Empty;

        [ForeignKey(nameof(OrderId))]
        public int OrderId { get; set; }
        public Order Order { get; set; }
    }
}
