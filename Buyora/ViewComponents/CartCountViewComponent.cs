using BuyoraBusiness.Services.Interfaces;
using BuyoraModels;
using BuyoraUtility;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Buyora.ViewComponents
{
    public class CartCountViewComponent : ViewComponent
    {
        private readonly IShoppingCartService _shoppingCartService;
        private readonly UserManager<ApplicationUser> _userManager;
        public CartCountViewComponent(IShoppingCartService shoppingCartService, UserManager<ApplicationUser> userManager)
        {
            _shoppingCartService = shoppingCartService;
            _userManager = userManager;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            if(!User.Identity.IsAuthenticated)
            {
                HttpContext.Session.Remove(SD.SessionCartCount);
                HttpContext.Session.Remove(SD.AppUserName);
                return View(0);
            }

            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if(claim == null)
            {
                HttpContext.Session.Remove(SD.SessionCartCount);
                HttpContext.Session.Remove(SD.AppUserName);
                return View(0);
            }
            
            var sessionAppUserName = HttpContext.Session.GetString(SD.AppUserName);
            if(string.IsNullOrEmpty(sessionAppUserName))
            {
                var user = await _userManager.GetUserAsync((ClaimsPrincipal)User);
                if(user != null)
                {
                    HttpContext.Session.SetString(SD.AppUserName, user.Name);
                }
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
