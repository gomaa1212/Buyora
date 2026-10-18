using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Buyora.Areas.Admin.ViewModels.Product
{
    public class EditProductViewModel : AddProductViewModel
    {
        public int Id { get; set; }
        [BindNever]
        public IEnumerable<ProductImageViewModel> ProductImages { get; set; } = new List<ProductImageViewModel>();
    }
}
