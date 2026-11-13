using AutoMapper;
using Buyora.Areas.Admin.ViewModels.Category;
using Buyora.Areas.Customer.ViewModel.Product;
using Buyora.Helper;
using BuyoraBusiness.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Buyora.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private readonly IMapper _mapper;
        private readonly int pageSize = 20;
        public ProductController(IProductService productService, IMapper mapper, ICategoryService categoryService)
        {
            _productService = productService;
            _mapper = mapper;
            _categoryService = categoryService;
        }
        public async Task<IActionResult> Index(string? search = null, string? categoryName = null, int page = 1)
        {
            var categories = await _categoryService.GetCategoriesListAsync();
            var products = await _productService.GetProductsListAsync(page, pageSize, categoryName, search,withActive:true);
            var model = new PageResult<GetProductsListViewModel>()
            {
                Items = _mapper.Map<List<GetProductsListViewModel>>(products.Items),
                TotalCount = products.TotalCount,
                Page = page,
                PageSize = pageSize,
                CategoryName = categoryName,
                Search = search
            };
            ViewBag.Categories = _mapper.Map<List<GetCategoriesListViewModel>>(categories);
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_DisplayProductsPartial", model);
            }
            return View(model);
        }

        public async Task<IActionResult> Details(int id)
        {
            var product = await _productService.GetProductByIdWithIncludeAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            var productDisplay = _mapper.Map<GetProductByIdViewModel>(product);
            return View(productDisplay);
        }
    }
}
