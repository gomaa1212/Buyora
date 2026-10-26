using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace BuyoraModels
{
    public class ShoppingCart
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        [ForeignKey("ProductId")]
        public Product Product { get; set; } = null!;
        [Range(1,99)]
        public int Count { get; set; }
        [Required]
        public string ApplicationUserId { get; set; } = string.Empty;
        [ForeignKey(nameof(ApplicationUserId))]
        public ApplicationUser ApplicationUser { get; set; } = null!;
        [NotMapped]
        public decimal UnitPrice
        {
            get
            {
                if (Product == null)
                    return 0;
                return Product.FinalPrice;
            }
        }        
    }
}
