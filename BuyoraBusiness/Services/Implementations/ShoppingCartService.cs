using BuyoraBusiness.Services.Interfaces;
using BuyoraDataAccess.UnitOfWorks;
using BuyoraModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace BuyoraBusiness.Services.Implementations
{
    public class ShoppingCartService : IShoppingCartService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ShoppingCartService> _logger;
        public ShoppingCartService(IUnitOfWork unitOfWork,ILogger<ShoppingCartService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }
        public async Task<string> AddToCartAsync(ShoppingCart cart)
        {
            try
            {
                if(cart == null)
                {
                    throw new ArgumentException("cart is null");
                }
                var existingItem = await _unitOfWork.Repository<ShoppingCart>().GetAsQuerable()
                                                     .FirstOrDefaultAsync(c => c.ApplicationUserId == cart.ApplicationUserId && c.ProductId == cart.ProductId);

                var product = await _unitOfWork.Repository<Product>().GeByIdAsync(cart.ProductId);
                if(product == null)
                {
                    return "Product not found";
                }

                if(existingItem != null)
                {
                    existingItem.Count+=cart.Count;
                    if(product.Quantity < existingItem.Count)
                    {
                        return "Greater than stock";
                    }
                }
                else
                {
                    if(product.Quantity < cart.Count)
                    {
                        return "Greater than stock";
                    }
                    await _unitOfWork.Repository<ShoppingCart>().AddAsync(cart);
                }
                await _unitOfWork.SaveChangesAsync();
                return "Succeeded";
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Failed to add cart");
                return "Failed";
            }
        }

        public async Task<string> ClearCartAsync(string userId)
        {
            try
            {
                if(string.IsNullOrEmpty(userId))
                {
                    throw new ArgumentException("userId is null");
                }
                var userCarts =await _unitOfWork.Repository<ShoppingCart>().GetAsQuerable()
                                                             .Where(c => c.ApplicationUserId == userId).ToListAsync();
                if (userCarts.Any())
                {
                    _unitOfWork.Repository<ShoppingCart>().RemoveRange(userCarts);
                }
                await _unitOfWork.SaveChangesAsync();
                return "Succeeded";
            }
            catch(Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "Failed to delete carts");
                return "Failed";
            }
        }

        public async Task<ShoppingCart?> GetCartByIdAsync(int cartId , string userId)
        {
            return await _unitOfWork.Repository<ShoppingCart>().GetAsQuerable().Include(c => c.Product).FirstOrDefaultAsync(c => c.Id == cartId && c.ApplicationUserId == userId);
        }

        public async Task<int> GetUserCartCountAsync(string userId)
        {
            if(string.IsNullOrEmpty(userId))
            {
                return 0;
            }
            return await _unitOfWork.Repository<ShoppingCart>().GetAsQuerable().Where(c=>c.ApplicationUserId==userId).SumAsync(c=>c.Count);
        }

        public async Task<List<ShoppingCart>> GetUserCartItemsAsync(string userId)
        {
            if(string.IsNullOrEmpty(userId))
            {
                return new List<ShoppingCart>();
            }
            return await _unitOfWork.Repository<ShoppingCart>().GetAsQuerable().Include(p=>p.Product).ThenInclude(p=>p.ProductImages.Where(i=>i.IsMain))
                                                             .Where(c => c.ApplicationUserId == userId).ToListAsync();
        }

        public async Task<string> UpdateCartAsync(ShoppingCart cart)
        {
            try
            {
                if (cart == null)
                {
                    throw new ArgumentException("cart not found");
                }                                                          
                if(cart.Count<=0)
                {
                    _unitOfWork.Repository<ShoppingCart>().Delete(cart);
                }
                else if(cart.Product.Quantity < cart.Count)
                {
                    return "Greater than stock";
                }
                
                await _unitOfWork.SaveChangesAsync();
                return "Succeeded";
            }

            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "Failed to update cart");
                return "Failed";
            }
        }
        public async Task<string> DeleteCartItemAsync(ShoppingCart cart)
        {
            try
            {
                if(cart == null)
                {
                    throw new ArgumentException("cart not found");
                }
                _unitOfWork.Repository<ShoppingCart>().Delete(cart);
                await _unitOfWork.SaveChangesAsync();
                return "Succeeded";
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Failed to update cart");
                return "Failed";
            }
        }
    }
}
