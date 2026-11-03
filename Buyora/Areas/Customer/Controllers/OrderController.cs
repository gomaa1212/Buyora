using AutoMapper;
using Buyora.Areas.Customer.ViewModel;
using Buyora.SharedViewModel;
using BuyoraBusiness.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Buyora.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly IMapper _mapper;
        private const int PageSize = 10;
        public OrderController(IOrderService orderService, IMapper mapper)
        {
            _orderService = orderService;
            _mapper = mapper;
        }
        public async Task<IActionResult> Index(int page = 1,string? search = null, string? status = null)
        {
            if(page<1)
            {
                page = 1;
            }
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var orders = await _orderService.GetAllOrderAsync(page: page, pageSize: PageSize, userId: userId, status: status, search: search);
            var models = _mapper.Map<List<DisplayOrderViewModel>>(orders.Orders);
            ViewBag.Search = search;
            ViewBag.Status = status;
            ViewBag.TotalCount = orders.TotalCount;
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = PageSize;
            return View(models);
        }
        public async Task<IActionResult> Details(int Id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var order = await _orderService.GetOrderByIdAsync(Id, userId, includeDetails: true);
            if(order == null)
            {
                return NotFound();
            }
            var model = _mapper.Map<DisplayOrderViewModel>(order);
            return View(model);
        }

    }
}

