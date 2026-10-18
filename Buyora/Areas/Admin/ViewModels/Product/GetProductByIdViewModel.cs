using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Buyora.Areas.Admin.ViewModels.Product
{
    public class GetProductByIdViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public decimal? DiscountPrice { get; set; }
        public IEnumerable<ProductImageViewModel> ProductImages { get; set; } = new List<ProductImageViewModel>();
        
    }
}
