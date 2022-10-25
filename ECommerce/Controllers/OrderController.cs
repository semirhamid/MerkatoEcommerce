using AutoMapper;
using ECommerce.DTO.Requests;
using ECommerce.Helpers;
using ECommerce.Models;
using ECommerce.Pagination;
using ECommerce.Services;
using ECommerceModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ECommerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors]
    [Authorize]
    public class OrderController : Controller
    {
        private readonly IOrderRepository orderRepository;
        private readonly AppDbContext context;
        private readonly IMapper mapper;
        private readonly IProductRepository productRepository;
        private readonly UserManager<ApplicationUser> userManager;

        public OrderController(IOrderRepository orderRepository, 
            AppDbContext context, IMapper mapper, 
            IProductRepository productRepository,
            UserManager<ApplicationUser> userManager)
        {
            this.orderRepository = orderRepository;
            this.context = context;
            this.mapper = mapper;
            this.productRepository = productRepository;
            this.userManager = userManager;
        }

        [HttpGet("getorders")]
        
        public async Task<IActionResult> GetOrders([FromQuery] OrdersParameters parameters)
        {
            var orders = await orderRepository.GetAllOrdersByPage(parameters);
            await HttpContext.InsertParamtersPaginationInHeader(context.Orders);

            return Ok(orders);
        }

        [HttpGet("id")]
        public async Task<ActionResult<Order>> GetOrderByOrderId(int id)
        {
            var orders = await orderRepository.GetOrderById(id);
            var userId = GetSubject();
            var isAdmin = await userManager.FindByIdAsync(userId);
            var role = await userManager.IsInRoleAsync(isAdmin, "Adminstration");
            if (userId != orders.UserId || !role)
            {
                return Unauthorized();
            }
            if (orders != null)
            {
                return Ok(orders);

            }
            return BadRequest("order doesnot exist");
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Adminstration")]
        public async Task<ActionResult> DeleteOrderById(int id)
        {
            var orders = await orderRepository.DeleteOrderAsync(id);

            if (orders)
            {
                return Ok();

            }
            return BadRequest();
        }

        [HttpPost("")]
        public async Task<ActionResult> AddOrder(List<UserOrderModel> userOrderModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(); 
            }
            var headers = this.Request.Headers;
            var user = GetSubject();
            

            var result = await orderRepository.AddOrderAsync(userOrderModel,user);
            if (result.IsSuccess)
            {
                return Ok(result);

            }
            else
            {
                return BadRequest();
            }

        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "Adminstration")]
        public async Task<ActionResult> UpdateOrder(int id, Order orderModel)
        {
            if (ModelState.IsValid)
            {
                var order = await orderRepository.GetOrderById(id);

                order.Quantity = orderModel.Quantity; 
                order.TotalPrice = order.Product.Price * orderModel.Quantity;
                order.Country = orderModel.Country;
                order.Region = orderModel.Region;
                order.City = orderModel.City;
                order.Date = DateTime.Now;
                order.ZipCode = orderModel.ZipCode;

                var result = await orderRepository.UpdateOrderAsync(order);
                if (result)
                {
                    return Ok();

                }
                else
                {
                    return BadRequest();
                }
            }
            
            return BadRequest();
        }

        [HttpGet("deliverorder")]
        [Authorize(Roles = "Adminstration")]
        public async Task<ActionResult> DeliverOrder(int id)
        {
            var result = await orderRepository.DeliverOrderAsync(id);
            if (result)
            {
                return Ok(true);
            }
            return BadRequest(false);
        }

        private String GetSubject()
        {
            return GetClaim(ClaimTypes.NameIdentifier);
        }
        private String GetClaim(String type)
        {
            Claim c = User.Claims.FirstOrDefault(c => c.Type == type);
            return c?.Value;
        }

    }
    
}
