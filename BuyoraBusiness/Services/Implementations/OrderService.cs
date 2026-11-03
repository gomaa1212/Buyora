using BuyoraBusiness.Services.Interfaces;
using BuyoraDataAccess.UnitOfWorks;
using BuyoraModels;
using BuyoraUtility;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;


namespace BuyoraBusiness.Services.Implementations
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<OrderService> _logger;
        public OrderService(IUnitOfWork unitOfWork,ILogger<OrderService>logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }
        public async Task<string> CreateOrderAsync(Order order)
        {
            try
            {
                if(order == null)
                {
                    throw new ArgumentException("order not found");
                }
                await _unitOfWork.Repository<Order>().AddAsync(order);
                await _unitOfWork.SaveChangesAsync();
                return "Succeeded";
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Failed to create order");
                return "Failed";
            }
        }

        public async Task<(IEnumerable<Order> Orders, int TotalCount)> GetAllOrderAsync(int page, int pageSize, string? userId = null, string? status = null,string? search = null, bool includeUser = false, bool includeDetails = false)
        {
            IQueryable<Order> order = _unitOfWork.Repository<Order>().GetAsQuerable();
            if(includeUser)
            {
                order = order.Include(o => o.ApplicationUser);
            }
            if(includeDetails)
            {
                order = order.Include(o => o.OrderDetails).ThenInclude(p => p.Product);
            }
            if (!string.IsNullOrEmpty(userId))
            {
                order = order.Where(o => o.ApplicationUserId == userId);
            }
            if (!string.IsNullOrEmpty(status))
            {
                order = order.Where(o => o.Status.ToLower() == status.ToLower());
            }
            if(!string.IsNullOrEmpty(search))
            {
                if (int.TryParse(search, out int orderId))
                {
                    order = order.Where(o => o.Id == orderId);
                }
                else
                {
                    order = order.Where(o => o.ApplicationUser.Email.ToLower() == search.ToLower());
                }
            }
            int totalCount = await order.CountAsync();
            order = order.OrderByDescending(x => x.OrderDate).Skip((page - 1) * pageSize).Take(pageSize);
            return (await order.ToListAsync(), totalCount);
        }

        public async Task<Order?> GetOrderByIdAsync(int id,string? userId = null, bool includeUser = false, bool includeDetails = false,bool withTracking = false)
        {
            IQueryable<Order> order = _unitOfWork.Repository<Order>().GetAsQuerable();
            if(!withTracking)
            {
                order = order.AsNoTracking();
            }
            if (includeUser)
            {
                order = order.Include(o => o.ApplicationUser);
            }
            if (includeDetails)
            {
                order = order.Include(o => o.OrderDetails).ThenInclude(p=>p.Product);
            }
            if(string.IsNullOrEmpty(userId))
            {
                return await order.FirstOrDefaultAsync(o => o.Id == id);
            }
            return await order.FirstOrDefaultAsync(o => o.Id == id && o.ApplicationUserId == userId);
        }
        public async Task<string> UpdateOrderAsync(Order order)
        {
            try
            {
                if(order == null)
                {
                    throw new ArgumentException("order not found");
                }
                var oldOrder =await GetOrderByIdAsync(order.Id,includeUser: true, includeDetails: true, withTracking: true);
                if(oldOrder == null)
                {
                    return "Failed";
                }
                if(order.Status==SD.StatusApproved && oldOrder.Status!=SD.StatusApproved)
                {
                    foreach (var detail in oldOrder.OrderDetails)
                    {
                        if (detail.Product.Quantity < detail.Count)
                            return "InsufficientStock";
                    }

                    foreach (var orderDetails in oldOrder.OrderDetails)
                    {
                        orderDetails.Product.Quantity = orderDetails.Product.Quantity - orderDetails.Count;
                    }
                }
                else if ((order.Status == SD.StatusCancelled || order.Status == SD.StatusRefunded)
                    &&
                    (oldOrder.Status == SD.StatusApproved ||
                     oldOrder.Status == SD.StatusInProcess ||
                     oldOrder.Status == SD.StatusShipped))
                    
                {
                    foreach (var orderDetails in oldOrder.OrderDetails)
                    {
                        orderDetails.Product.Quantity = orderDetails.Product.Quantity + orderDetails.Count;
                    }
                }
                oldOrder.Status = order.Status;
                oldOrder.ShippingDate = order.ShippingDate;
                oldOrder.Carrier = order.Carrier;
                oldOrder.TrackingNumber = order.TrackingNumber;
                
                await _unitOfWork.SaveChangesAsync();
                return "Succeeded";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update order");
                return "Failed";
            }
        }
    }
}
