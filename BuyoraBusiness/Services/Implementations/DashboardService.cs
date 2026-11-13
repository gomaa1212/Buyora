using BuyoraBusiness.Services.Interfaces;
using BuyoraDataAccess.UnitOfWorks;
using BuyoraModels;
using BuyoraModels.DTO;
using BuyoraUtility;
using System;
using System.Collections.Generic;
using System.Text;

namespace BuyoraBusiness.Services.Implementations
{
    public class DashboardService : IDashboardService
    {
        private readonly IUnitOfWork _unitOfWork;
        public DashboardService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public RevenueOverviewDto GetChartData()
        {
            var chartData = new RevenueOverviewDto();
            var now = DateTime.Now;
            var sixMonthsAgo = now.AddMonths(-5);
            for(int i=0;i<6;i++)
            {
                var month = sixMonthsAgo.AddMonths(i);

                var revenue = _unitOfWork.Repository<Order>().GetAsQuerable()
                    .Where(o => o.OrderDate.Year == month.Year && o.OrderDate.Month == month.Month && o.Status == SD.StatusShipped)
                    .Sum(o => o.TotalAmount);
                chartData.RevenueByMonth.Add(month.ToString("MMMM yyyy"), revenue);

                var orderCount = _unitOfWork.Repository<Order>().GetAsQuerable()
                    .Where(o => o.OrderDate.Year == month.Year && o.OrderDate.Month == month.Month && o.Status == SD.StatusShipped)
                    .Count();
                chartData.OrdersByMonth.Add(month.ToString("MMMM yyyy"), orderCount);
            }

            var orderStatusCounts = _unitOfWork.Repository<Order>().GetAsQuerable()
                .GroupBy(o => o.Status)
                .Select(g => new { Status = g.Key, Count = g.Count() })
                .ToList();
            foreach(var statusCount in orderStatusCounts)
            {
                chartData.OrderStatusCount.Add(statusCount.Status, statusCount.Count);
            }

            var productCategoryCounts = _unitOfWork.Repository<Product>().GetAsQuerable()
                .GroupBy(p => p.Category)
                .Select(g => new { Category = g.Key, Count = g.Count() })
                .ToList();
            foreach(var categoryCount in productCategoryCounts)
            {
                chartData.ProductCategoryCount.Add(categoryCount.Category, categoryCount.Count);
            }

            var lowStockProducts = _unitOfWork.Repository<Product>().GetAsQuerable()
                .Where(p => p.Quantity < 5)
                .Select(p => new { Product = p, Count = p.Quantity })
                .ToList();
            foreach(var lowStockProduct in lowStockProducts)
            {
                chartData.LowStockProducts.Add(lowStockProduct.Product, lowStockProduct.Count);
            }

            return chartData;
        }
    }
}
