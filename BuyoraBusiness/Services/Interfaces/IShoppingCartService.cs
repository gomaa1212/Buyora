using BuyoraModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace BuyoraBusiness.Services.Interfaces
{
    public interface IShoppingCartService
    {
        public Task<ShoppingCart?> GetCartByIdAsync(int cartId,string userId);
        public Task<List<ShoppingCart>> GetUserCartItemsAsync(string userId);
        public Task<int> GetUserCartCountAsync(string userId);
        public Task<string> AddToCartAsync(ShoppingCart cart);
        public Task<string> UpdateCartAsync(ShoppingCart cart);
        public Task<string> ClearCartAsync(string userId);
        public Task<string> DeleteCartItemAsync(ShoppingCart cart);
    }
}
