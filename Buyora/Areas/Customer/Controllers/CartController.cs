using AutoMapper;
using Buyora.Areas.Customer.ViewModel;
using BuyoraBusiness.Services.Interfaces;
using BuyoraModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Buyora.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]    
    public class CartController : Controller
    {
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;
        public CartController(IShoppingCartService shoppingCartService,IMapper mapper,UserManager<ApplicationUser> userManager)
        {
            _shoppingCartService = shoppingCartService;
            _mapper = mapper;
            _userManager = userManager;
        }
        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            if(string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }
            var user = await _userManager.FindByIdAsync(userId);
            if(user == null)
            {
                return Unauthorized();
            }

            var userCartItems = await _shoppingCartService.GetUserCartItemsAsync(userId);
            var userCartItemsModel = _mapper.Map<List<DisplayShoppingCartViewModel>>(userCartItems);

            var model = new ShoppingCartViewModel()
            {
                ShoppingCartItems = userCartItemsModel,
                Order = new Order()
            };

            model.Order.ApplicationUser = user;
            model.Order.ApplicationUserId = userId;
            model.Order.Name = user.Name;
            model.Order.ShippingAddress = user.Address;
            model.Order.PhoneNumber = user.PhoneNumber;

            foreach(var item in userCartItems)
            {
                model.Order.TotalAmount += item.Count * item.UnitPrice;
            }
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddToCart(int productId, int count = 1)
        {
            if (productId <= 0 || count <= 0)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Invalid product or quantity."
                });
            }

            var userId =  _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new
                {
                    success = false,
                    message = "Please login first."
                });
            }

            var cart = new ShoppingCart()
            {
                ProductId = productId,
                Count = count,
                ApplicationUserId = userId
            };

            var res = await _shoppingCartService.AddToCartAsync(cart);
            if (res == "Greater than stock")
            {
                return BadRequest(new
                {
                    success = false,
                    message = "You have exceeded the max count."
                });
            }
            if (res != "Succeeded")
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Failed to add product to cart."
                });
            }

            return Ok(new
            {
                success = true,
                message = "Product added to cart successfully."
            });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateCartItemCount(int cartId , int count)
        {
            if(cartId<=0 || count<0)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Invalid cart item or quantity."
                });
            }
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new
                {
                    success = false,
                    message = "Please login first."
                });
            }
            var cartItem = await _shoppingCartService.GetCartByIdAsync(cartId,userId);
            if(cartItem == null)
            {
                return NotFound(new
                {
                    success = false,
                    message = "Cart item not found"
                });
            }
            cartItem.Count = count;

            var res = await _shoppingCartService.UpdateCartAsync(cartItem);
            if(res == "Greater than stock")
            {
                return BadRequest(new
                {
                    success = false,
                    message = "You have exceeded the max count."
                });
            }
            if (res != "Succeeded")
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Failed to update."
                });
            }

            return Ok(new
            {
                success = true,
                message = "Cart item updated successfully."
            });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveCartItem(int cartId)
        {
            if (cartId <= 0)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Invalid cart item or quantity."
                });
            }
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new
                {
                    success = false,
                    message = "Please login first."
                });
            }

            var cartItem = await _shoppingCartService.GetCartByIdAsync(cartId, userId);
            if (cartItem == null)
            {
                return NotFound(new
                {
                    success = false,
                    message = "Cart item not found"
                });
            }

            var res = await _shoppingCartService.DeleteCartItemAsync(cartItem);
            if (res != "Succeeded")
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Failed to delete."
                });
            }
            return Ok(new
            {
                success = true,
                message = "Cart item deleted successfully."
            });
        }
    }


}
