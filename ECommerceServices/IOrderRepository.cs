using ECommerceModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceServices
{
    public interface IOrderRepository
    {
        Task<List<Order>> GetAllOrders();
        Task<Order> GetOrderById(string id);
        Task<List<Order>> GetOrderByUserId(string userId);
        Task<string> AddOrderAsync(Order order);
        Task UpdateOrderAsync(int id, Order order);
        Task DeleteOrderAsync(int orderId);
    }
}
