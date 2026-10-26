using BuyoraModels;
using System.ComponentModel.DataAnnotations;

namespace Buyora.Areas.Customer.ViewModel
{
    public class ShoppingCartViewModel
    {
        public IEnumerable<DisplayShoppingCartViewModel> ShoppingCartItems { get; set; } = new List<DisplayShoppingCartViewModel>();
        [Required]
        public Order Order { get; set; } = new Order();
    }
}
