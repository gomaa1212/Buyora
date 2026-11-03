using AutoMapper;
using Buyora.SharedViewModel;
using BuyoraBusiness.Services.Implementations;
using BuyoraBusiness.Services.Interfaces;
using BuyoraModels;
using BuyoraUtility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NuGet.Common;

namespace Buyora.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin,Employee")]
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly IEmailTemplateService _emailTemplateService;
        private readonly IEmailService _emailService;
        private readonly IMapper _mapper;
        private readonly IConfiguration _config;
        private const int PageSize = 10;
        public OrderController(IOrderService orderService, IEmailTemplateService emailTemplateService, IEmailService emailService, IMapper mapper, IConfiguration config    )
        {
            _orderService = orderService;
            _emailTemplateService = emailTemplateService;
            _emailService = emailService;
            _mapper = mapper;
            _config = config;
        }

        private string AbsoluteUrl(string? path)
            => _config["App:BaseUrl"]!.TrimEnd('/') + path;
        public async Task<IActionResult> Index(int page = 1, string? search = null, string? status = null)
        {
            if (page < 1)
            {
                page = 1;
            }
            var orders = await _orderService.GetAllOrderAsync(page: page, pageSize: PageSize, search: search, status: status, includeUser: true);
            var model = _mapper.Map<List<DisplayOrderViewModel>>(orders.Orders);
            ViewBag.Search = search;
            ViewBag.Status = status;
            ViewBag.TotalCount = orders.TotalCount;
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = PageSize;
            return View(model);
        }
        public async Task<IActionResult> Details(int Id)
        {
            var order = await _orderService.GetOrderByIdAsync(Id, includeDetails: true);
            var model = _mapper.Map<DisplayOrderViewModel>(order);
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateOrder(DisplayOrderViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Invalid data";
                return RedirectToAction(nameof(Details), new { id = model.Id });
            }
            var order = _mapper.Map<Order>(model);
            if (order == null)
            {
                return NotFound();
            }
            var oldOrder = await _orderService.GetOrderByIdAsync(model.Id, includeUser: true, withTracking: false);

            var res = await _orderService.UpdateOrderAsync(order);
            if (res == "Succeeded")
            {
                TempData["SuccessMessage"] = "Order updated successfully";

                if (oldOrder.Status != model.Status)
                {
                    var path = Url.Action("Details", "Order",
                    new { area = "Customer", id = model.Id });
                    var subject = $"Your Buyora order #{order.Id} is now {model.Status}";
                    var body = _emailTemplateService.Build(
                        subject,
                        "Order Updated",
                        oldOrder.ApplicationUser.Name,
                        $"Your order status has been updated to {model.Status}.",
                        "View Order Details",
                        AbsoluteUrl(path)
                        ,
                        "If you have any questions about your order, please contact our support team.");
                    await _emailService.SendEmailAsync(oldOrder.ApplicationUser.Email!, subject, body);
                }
            }
            else if (res == "InsufficientStock")
            {
                TempData["ErrorMessage"] = "Insufficient stock";
            }
            else
            {
                TempData["ErrorMessage"] = "Error occurred";
            }
            return RedirectToAction("Details", new { Id = model.Id });
        }
    }

}
