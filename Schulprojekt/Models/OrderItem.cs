using System.ComponentModel.DataAnnotations.Schema;

namespace Schulprojekt.Models
{
    public class OrderItem
    {
        public int Id { get; set; }

        [ForeignKey(nameof(OrderId))]
        public int OrderId { get; set; }
        public Order Order { get; set; }

        [ForeignKey(nameof(ProductId))]
        public int ProductId { get; set; }
        public Product Product { get; set; }

        public int Quantity { get; set; }

        public decimal UnitPrice { get; set; }
    }
}
