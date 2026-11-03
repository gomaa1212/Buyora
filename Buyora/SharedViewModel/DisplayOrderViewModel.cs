
namespace Buyora.SharedViewModel
{
    public class DisplayOrderViewModel
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string? Status { get; set; }        
        public string Name { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string UserEmail { get; set; } = string.Empty;
        public string ShippingAddress { get; set; } = string.Empty;
        public string? Notes { get; set; }
        public string? TrackingNumber { get; set; }
        public string? Carrier { get; set; }
        public DateTime? ShippingDate { get; set; }
        string? Search { get; set; }
        public List<DisplayOrderDetailsViewModel> Items { get; set; } = new List<DisplayOrderDetailsViewModel>();
    }
}
