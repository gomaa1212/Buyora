using AspNetCoreGeneratedDocument;
using AutoMapper;
using Buyora.Areas.Admin.ViewModels.Category;
using Buyora.Areas.Customer.ViewModel.Product;
using Buyora.Resources;
using BuyoraBusiness.Services.Interfaces;
using BuyoraModels;
using BuyoraUtility;
using Microsoft.AspNetCore.Diagnostics;
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
        private readonly IMapper _mapper;
        private readonly IHtmlLocalizer<SharedResource> _localizer;
        private readonly ILogger<HomeController> _logger;

        public HomeController(
            IProductService productService,
            ICategoryService categoryService,
            IMapper mapper,
            IHtmlLocalizer<SharedResource> localizer,
            ILogger<HomeController> logger)
        {
            _productService = productService;
            _categoryService = categoryService;
            _localizer = localizer;
            _mapper = mapper;
            _logger = logger;

        }
    
        public async Task<IActionResult> Index()
        {
            var products = await _productService.GetProductsWithDiscountAsync();
            var categories = await _categoryService.GetCategoriesListAsync();
            ViewBag.FeaturedProducts = _mapper.Map<List<GetProductsListViewModel>>(products);
            ViewBag.Categories = _mapper.Map<List<GetCategoriesListViewModel>>(categories);
            return View();
        }

        [Microsoft.AspNetCore.Authorization.AllowAnonymous]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            var exceptionFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
            var traceId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;

            if (exceptionFeature?.Error is not null)
            {
                _logger.LogError(
                    exceptionFeature.Error,
                    "Unhandled exception while processing {Path}. TraceId: {TraceId}",
                    exceptionFeature.Path,
                    traceId);
            }

            Response.StatusCode = StatusCodes.Status500InternalServerError;
            ViewData["RequestId"] = traceId;
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
