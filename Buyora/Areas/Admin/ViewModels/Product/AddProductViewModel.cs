using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Buyora.Areas.Admin.ViewModels.Product
{
    public class AddProductViewModel
    {
        [Required]
        public string NameEN { get; set; } = string.Empty;
        [Required]
        public string NameAr { get; set; } = string.Empty;
        public string? DescriptionEn { get; set; } = string.Empty;
        public string? DescriptionAr { get; set; } = string.Empty;
        [Required]
        public decimal Price { get; set; }
        public decimal? DiscountPrice { get; set; }
        [Required]
        public int Quantity { get; set; }
        public bool IsActive { get; set; } = false;
        [Required]
        public int CategoryId { get; set; }
        [NotMapped]
        [Required]
        public IFormFile MainFile { get; set; }
        [NotMapped]
        public List<IFormFile>? Files { get; set; }
    }
}
