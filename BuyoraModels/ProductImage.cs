using System;
using System.Collections.Generic;
using System.Text;

namespace BuyoraModels
{
    public class ProductImage
    {
        public int Id { get; set; }
        public string Url { get; set; } = string.Empty;
        public bool IsMain { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; } = null;
    }
}
