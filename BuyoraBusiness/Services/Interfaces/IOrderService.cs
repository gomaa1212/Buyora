using BuyoraModels;
using System;
using System.Collections.Generic;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BuyoraBusiness.Services.Interfaces
{
    public interface IOrderService
    {
        public Task<string> CreateOrderAsync(Order order);
        public Task<Order?> GetOrderByIdAsync(int id, string? userId = null, bool includeUser = false, bool includeDetails = false, bool withTracking = false);
        public Task<(IEnumerable<Order> Orders, int TotalCount)> GetAllOrderAsync(int page, int pageSize,string? userId = null, string? status = null, string? search = null, bool includeUser = false, bool includeDetails = false);
        public Task<string> UpdateOrderAsync(Order order);

    }
}
