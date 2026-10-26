namespace Buyora.Areas.Customer.ViewModel
{
    public class DisplayShoppingCartViewModel
    {
        public int Id { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public int Count { get; set; }
        public decimal UnitPrice { get; set; }
    }
}
