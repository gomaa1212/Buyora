using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace BuyoraModels
{
    public class OrderDetail
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        [ForeignKey(nameof(OrderId))]
        public Order Order { get; set; } = null!;

        [Required]
        public int ProductId { get; set; }
        [ForeignKey(nameof(ProductId))]
        public Product Product { get; set; } = null!;
        public int Count { get; set; }
        public decimal UnitPrice { get; set; }

        [NotMapped]
        public decimal Total => UnitPrice * Count;
    }
}
