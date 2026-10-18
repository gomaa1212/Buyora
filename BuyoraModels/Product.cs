using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace BuyoraModels
{
    public class Product
    {
        public int Id { get; set; }
        [Required, MaxLength(150)]
        public string NameEn { get; set; } = string.Empty;
        [Required, MaxLength(150)]
        public string NameAr { get; set; } = string.Empty;
        [MaxLength(2000)]
        public string? DescriptionEn { get; set; }
        [MaxLength(2000)]
        public string? DescriptionAr { get; set; }
        [Range(typeof(decimal), "0.01", "999999999")]
        public decimal Price { get; set; }
        [Range(typeof(decimal), "0.01", "999999999")]
        public decimal? DiscountPrice { get; set; }
        [Range(0, int.MaxValue)]
        public int Quantity { get; set; }
        public bool IsActive { get; set; } = false;
        public int CategoryId { get; set; }
        public Category Category { get; set; } = null;
        public ICollection<ProductImage> ProductImages { get; set; } = new List<ProductImage>();
    }
}
