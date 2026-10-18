namespace Buyora.Areas.Admin.ViewModels.Product
{
    public class GetProductsListViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string MainUrl { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public decimal? DiscountPrice { get; set; }
    }
}
