using AutoMapper;
using ECommerce.DTO.Requests;
using ECommerce.DTO.Responses;
using ECommerce.Helpers;
using ECommerce.Pagination;
using ECommerceModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;


namespace ECommerce
{
    public class OrderRepository : IOrderRepository
    {
        private readonly AppDbContext context;
        private readonly IProductRepository productRepository;
        private readonly IMapper mapper;
        public OrderRepository(AppDbContext context, IProductRepository productRepository)
        {
            this.context = context;
            this.productRepository = productRepository;
            
        }
        public double TotalSales()
        {

            var total = 0.0;
            total = context.Orders.Select(i => i.TotalPrice).Sum();
            return total;
        }
        public double TodaysSales()
        {
            var todaysDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day);
            var total = context.Orders.Where(x =>x.Date==todaysDate).Select(i => i.TotalPrice).Sum();
            return total;
        }
        public int TotalOrders()
        {
            return context.Orders.Count();
        }
        public async Task<List<Order>> GetAllOrdersByPage(OrdersParameters parameters)
        {
            var orders = await context.Orders.OrderBy(x => x.Date).Paginate(parameters).ToListAsync();
            return orders;
        }
        public async Task<List<Order>> GetOrderByUserId(string userId)
        {
            var orders = await context.Orders.Where(c => c.UserId == userId).ToListAsync();
            return orders;
        }

        public async Task<List<Order>> GetOrdersByUserIdAsync(string userId)
        {
            return await context.Orders.Where(c => c.UserId == userId).ToListAsync();
        }
        public  List<Order> GetLastSevenDaysOrders()
        {
            var orderByDate =  context.Orders.GroupBy(o => new { EventDate = o.Date })
                .Select(s => new Order(){
                    Date = s.Key.EventDate,
                    Quantity = s.Sum(o => o.Quantity),
                    TotalPrice = s.Sum(o => o.TotalPrice)})
                .OrderBy(o => o.Date);
            var orders =  orderByDate.Take(7).ToList();
            return orders;

        }
        public async Task<Order> GetOrderById(int id)
        {

            var order = await context.Orders.FindAsync(id);

            return order;
        }

        public async Task<OrderResponse> AddOrderAsync(IList<UserOrderModel> orders, string userId)
        {

            var tracking = Guid.NewGuid().ToString();
            var time = DateTime.Now;
            var date = time.Date;
            try
            {
                foreach (var order in orders)
                {
                    var product = await productRepository.GetProductById(order.ProductId);
                    var resp = await productRepository.DecreaseProductQuantityAsync(product, order.Quantity);
                    if (!resp)
                    {
                        return null;
                    }
                    var newOrder = new Order()
                    {
                        UserId = userId,
                        TotalPrice= product.Price * order.Quantity,
                        Product = product,
                        Status = "Pending",
                        ZipCode = order.ZipCode,
                        TrackingId = tracking,
                        Date = date,
                        Country = order.Country, 
                        Region = order.Region,
                        City = order.City,
                        Quantity = order.Quantity,
                        Address = order.Address
                    };
                    await context.Orders.AddAsync(newOrder);
                }
                
            }
            catch (Exception ex) {

                return new OrderResponse
                {
                    IsSuccess = false,
                    TrackingId = null

                };
            }
             var result = await context.SaveChangesAsync();

            return new OrderResponse
            {
                IsSuccess = true,
                TrackingId = tracking

            };
        }

        public async Task<bool> UpdateOrderAsync(Order order)
        {
            context.Orders.Update(order);
            await context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeliverOrderAsync(int id)
        {
            var order = await context.Orders.FirstOrDefaultAsync(x => x.Id == id);
            if(order == null)
            {
                return false;
            }
            order.Status = "Delivered";
            context.Orders.Update(order);
            await context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteOrderAsync(int orderId)
        {
            var order = await context.Orders.FindAsync(orderId);

            if (order != null)
            {
                context.Orders.Remove(order);
                await context.SaveChangesAsync();
                return true;
            }

            return false;
        }


        
    }
}



       



        



        
