using System;
using System.Collections.Generic;
using System.Text;

namespace BuyoraModels.DTO
{
    public class RevenueOverviewDto
    {
        public Dictionary<string, decimal> RevenueByMonth { get; set; } = new Dictionary<string, decimal>();
        public Dictionary<string, int> OrdersByMonth { get; set; } = new Dictionary<string, int>();
        public Dictionary<string,int> OrderStatusCount { get; set; } = new Dictionary<string, int>();
        public Dictionary<Category, int> ProductCategoryCount { get; set; } = new Dictionary<Category, int>();
        public Dictionary<Product,int> LowStockProducts { get; set; } = new Dictionary<Product, int>();
    }
}
