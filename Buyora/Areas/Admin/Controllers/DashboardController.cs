using Buyora.Areas.Admin.ViewModels.Dashboard;
using BuyoraBusiness.Services.Interfaces;
using BuyoraModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Buyora.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin,Employee")]
    public class DashboardController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly IProductService _productService;
        private readonly IDashboardService _dashboardService;
        private readonly UserManager<ApplicationUser> _userManager;
        public DashboardController(IOrderService orderService, IProductService productService, IDashboardService dashboardService, UserManager<ApplicationUser> userManager)
        {
            _orderService = orderService;
            _productService = productService;
            _dashboardService = dashboardService;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var model = new DashboardViewModel()
            {
                TotalRevenue = await _orderService.GetTotalRevenueAsync(),
                TotalOrders = await _orderService.GetTotalOrdersAsync(),
                TotalProducts = await _productService.GetTotalProductsAsync(),
                TotalUsers = await _userManager.Users.CountAsync()
            };
            return View(model);
        }
        public IActionResult GetChartData()
        {
            var revenueOverview = _dashboardService.GetChartData();

            var productCategoryCount = revenueOverview.ProductCategoryCount
        .Where(x => x.Key != null)
        .Select(x => new
        {
            id = x.Key.Id,
            nameAr = x.Key.NameAr,
            nameEn = x.Key.NameEn,
            count = x.Value
        }).ToList();

            var lowStockProducts = revenueOverview.LowStockProducts
                .Where(x => x.Key != null)
                .Select(x => new
                {
                    id = x.Key.Id,
                    nameAr = x.Key.NameAr,
                    nameEn = x.Key.NameEn,
                    count = x.Value
                }).ToList();

            return Json(new
            {
                revenueByMonth = revenueOverview.RevenueByMonth,
                ordersByMonth = revenueOverview.OrdersByMonth,
                orderStatusCount = revenueOverview.OrderStatusCount,
                productCategoryCount = productCategoryCount,
                lowStockProducts = lowStockProducts
            });

        }
    }
}
