using ECommerce.DTO.Requests;
using ECommerce.DTO.Responses;
using ECommerce.Pagination;
using ECommerceModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce
{
    public interface IOrderRepository
    {
        Task<bool> DeliverOrderAsync(int id);
        Task<List<Order>> GetAllOrdersByPage(OrdersParameters parameters);
        Task<Order> GetOrderById(int id);
        Task<List<Order>> GetOrderByUserId(string userId);
        Task<OrderResponse> AddOrderAsync(IList<UserOrderModel> orders, string userId);
        Task<bool> UpdateOrderAsync(Order order);
        Task<bool> DeleteOrderAsync(int orderId);
        Task<List<Order>> GetOrdersByUserIdAsync(string userId);
        List<Order> GetLastSevenDaysOrders();
        int TotalOrders();
        double TotalSales();
        double TodaysSales();
    }
}
