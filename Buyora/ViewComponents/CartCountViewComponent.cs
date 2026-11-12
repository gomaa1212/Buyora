using BuyoraBusiness.Services.Interfaces;
using BuyoraUtility;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Buyora.ViewComponents
{
    public class CartCountViewComponent : ViewComponent
    {
        private readonly IShoppingCartService _shoppingCartService;
        public CartCountViewComponent(IShoppingCartService shoppingCartService)
        {
            _shoppingCartService = shoppingCartService;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            if(!User.Identity.IsAuthenticated)
            {
                HttpContext.Session.Remove(SD.SessionCartCount);
                return View(0);
            }

            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if(claim == null)
            {
                HttpContext.Session.Remove(SD.SessionCartCount);
                return View(0);
            }

            var sessionCartCount = HttpContext.Session.GetInt32(SD.SessionCartCount);
            if(!sessionCartCount.HasValue)
            {
                var count = await _shoppingCartService.GetUserCartCountAsync(claim.Value);
                HttpContext.Session.SetInt32(SD.SessionCartCount, count);
                return View(count);
            }

            return View(sessionCartCount.Value);
        }
    }
}
