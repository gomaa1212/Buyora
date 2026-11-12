using AspNetCoreGeneratedDocument;
using AutoMapper;
using Buyora.Areas.Admin.ViewModels.Category;
using Buyora.Areas.Admin.ViewModels.Product;
using Buyora.Resources;
using BuyoraBusiness.Services.Interfaces;
using BuyoraModels;
using BuyoraUtility;
using Microsoft.AspNetCore.Identity;
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
        private readonly IShoppingCartService _shoppingCartService;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IMapper _mapper;
        private readonly IHtmlLocalizer<SharedResource> _localizer;
        public HomeController(IProductService productService, ICategoryService categoryService, IShoppingCartService shoppingCartService, SignInManager<ApplicationUser> signInManager, IMapper mapper, IHtmlLocalizer<SharedResource> localizer)
        {
            _productService = productService;
            _categoryService = categoryService;
            _shoppingCartService = shoppingCartService;
            _signInManager = signInManager;
            _localizer = localizer;
            _mapper = mapper;

        }
    
        public async Task<IActionResult> Index()
        {
            if(_signInManager.IsSignedIn(User))
            {
                var cartCount = HttpContext.Session.GetInt32(SD.SessionCartCount);
                if(!cartCount.HasValue)
                {
                    var userId = _signInManager.UserManager.GetUserId(User);
                    if (!string.IsNullOrEmpty(userId))
                    {
                        var count = await _shoppingCartService.GetUserCartCountAsync(userId);
                        HttpContext.Session.SetInt32(SD.SessionCartCount, count);
                    }
                }
            }
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
