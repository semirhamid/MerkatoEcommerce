using AutoMapper;
using ECommerceModels;
using Microsoft.EntityFrameworkCore;


namespace ECommerceServices
{
    public class OrderRepository : IOrderRepository
    {
        private readonly AppDbContext context;
        private readonly IMapper mapper;
        public OrderRepository(AppDbContext context)
        {
            this.context = context;
        }

        public async Task<List<Order>> GetAllOrders()
        {
            var orders = await context.Orders.ToListAsync();
            return mapper.Map<List<Order>>(orders);
        }
        public async Task<List<Order>> GetOrderByUserId(string userId)
        {
            var orders = await context.Orders.Where(c => c.UserId == userId).ToListAsync();
            return mapper.Map<List<Order>>(orders);
        }

        public async Task<Order> GetOrderById(string id)
        {

            var order = await context.Orders.FindAsync(id);

            return mapper.Map<Order>(order);
        }

        public async Task<string> AddOrderAsync(Order order)
        {
            var newOrder = mapper.Map<Order>(order);

            context.Orders.Add(newOrder);
            await context.SaveChangesAsync();

            return newOrder.TrackingId;
        }

        public async Task UpdateOrderAsync(int id, Order order)
        {
            var updatedOrder = mapper.Map<Order>(order);
            updatedOrder.Id = id;
            context.Orders.Update(updatedOrder);
            await context.SaveChangesAsync();
        }

        public async Task DeleteOrderAsync(int orderId)
        {
            var order = await context.Orders.FindAsync(orderId);
            if (order != null)
            {
                context.Orders.Remove(order);
                await context.SaveChangesAsync();
            }
        }

        
    }
}



       



        



        
