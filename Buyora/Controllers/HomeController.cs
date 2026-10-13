
using Buyora.Resources;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.Extensions.Localization;
using System.Diagnostics;

namespace Buyora.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHtmlLocalizer<SharedResource> _localizer;
        public HomeController(IHtmlLocalizer<SharedResource> localizer)
        {
            _localizer=localizer;
        }
    
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            var names = typeof(Buyora.Resources.SharedResource).Assembly.GetManifestResourceNames();
            var culture = System.Globalization.CultureInfo.CurrentUICulture.Name;
            return Ok(new { culture, names });
        }

       
    }
}
