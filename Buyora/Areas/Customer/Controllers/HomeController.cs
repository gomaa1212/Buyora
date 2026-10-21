using AspNetCoreGeneratedDocument;
using AutoMapper;
using Buyora.Areas.Admin.ViewModels.Category;
using Buyora.Areas.Admin.ViewModels.Product;
using Buyora.Resources;
using BuyoraBusiness.Services.Interfaces;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.Extensions.Localization;
using System.Diagnostics;

namespace Buyora.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private readonly IMapper _mapper;
        private readonly IHtmlLocalizer<SharedResource> _localizer;
        public HomeController(IProductService productService, ICategoryService categoryService,IMapper mapper, IHtmlLocalizer<SharedResource> localizer)
        {
            _productService = productService;
            _categoryService = categoryService;
            _localizer = localizer;
            _mapper = mapper;

        }
    
        public async Task<IActionResult> Index()
        {
            var products = await _productService.GetProductsListAsync(1, 8);
            var categories = await _categoryService.GetCategoriesListAsync();
            ViewBag.FeaturedProducts = _mapper.Map<List<GetProductsListViewModel>>(products.Items);
            ViewBag.Categories = _mapper.Map<List<GetCategoriesListViewModel>>(categories);
            return View();
        }

        public IActionResult Privacy()
        {
            var names = typeof(SharedResource).Assembly.GetManifestResourceNames();
            var culture = System.Globalization.CultureInfo.CurrentUICulture.Name;
            return Ok(new { culture, names });
        }
        [HttpPost]
        public IActionResult SetLanguage(string culture, string returnUrl)
        {
   
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
            );

            return LocalRedirect(returnUrl);
        }


    }
}
