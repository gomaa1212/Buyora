using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace BuyoraModels
{
    public class Order
    {
        public int Id { get; set; }
        [Required]
        public string ApplicationUserId { get; set; } = string.Empty;
        [ForeignKey(nameof(ApplicationUserId))]
        public ApplicationUser ApplicationUser { get; set; } = null!;
        public DateTime OrderDate { get; set; }
        public DateTime? ShippingDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string? Status { get; set; }

        public string? TrackingNumber { get; set; }
        public string? Carrier { get; set; }

        public string? SessionId { get; set; }
        public string? PaymentIntentId { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; } = string.Empty;
        [Required]
        public string PhoneNumber { get; set; } = string.Empty;
        [Required, MaxLength(500)]
        public string ShippingAddress { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Notes { get; set; }
        public ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    }
}
