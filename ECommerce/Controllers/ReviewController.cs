using ECommerce.DTO;
using ECommerce.Helpers;
using ECommerce.Pagination;
using ECommerce.Services;
using ECommerceModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    
    public class ReviewController : ControllerBase
    {
        private readonly IReviewService reviewService;
        private readonly AppDbContext context;

        public ReviewController(IReviewService reviewService, AppDbContext context)
        {
            this.reviewService = reviewService;
            this.context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetReviewOfProduct([FromQuery] int productId, [FromQuery] ReviewParameters? parameters)
        {
            var result = await reviewService.GetReviewByProductIdAsync(productId, parameters);
            await HttpContext.InsertParamtersPaginationInHeader(context.Reviews.Where(x => x.ProductId == productId).AsQueryable());
            return Ok(result);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddReview([FromForm] ReviewDTO model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var result = await reviewService.AddReviewAsync(model);
            if (result) return Ok();

            return BadRequest();
        }

        [HttpPost("edit")]
        [Authorize]
        public async Task<IActionResult> UpdateReview([FromForm] ReviewDTO model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var result = await reviewService.UpdateReviewAsync(model);
            if (result) return Ok();

            return BadRequest();
        }

        [HttpDelete]
        [Authorize]
        public async Task<ActionResult> DeleteReview(int productId , string userId)
        {
            if (await reviewService.DeleteReviewAsync(productId, userId))
            {
                return Ok();
            }
            return NotFound();
        }

        [HttpGet("userproductreview")]
        public async Task<IActionResult> GetReviewOfProduct(int productId, string? userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return Ok();
            }
            var result = await reviewService.GetUserProductReview(productId, userId);
            return Ok(result);
        }
    }
}
