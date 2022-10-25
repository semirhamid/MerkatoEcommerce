using AutoMapper;
using ECommerce.DTO;
using ECommerce.Helpers;
using ECommerce.Models;
using ECommerce.Pagination;
using ECommerceModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Services
{
    public interface IReviewService
    {
        public Task<List<Review>> GetReviewByProductIdAsync(int productId, ReviewParameters? parameters);
        Task<bool> AddReviewAsync(ReviewDTO review);
        Task<bool> UpdateReviewAsync(ReviewDTO review);
        Task<bool> DeleteReviewAsync(int productId, string userId);
        Task<Review> GetUserProductReview(int productId, string userId);
    }

    public class ReviewService : IReviewService
    {
        private readonly AppDbContext context;
        private readonly UserManager<ApplicationUser> userManager;

        public ReviewService(AppDbContext context, UserManager<ApplicationUser> userManager)
        {
            this.context = context;
            this.userManager = userManager;
        }

        public async Task<bool> AddReviewAsync(ReviewDTO review)
        {
            var doesExist = await context.Reviews.FirstOrDefaultAsync(x => x.ProductId == review.ProductId &&
            x.UserId == review.UserId);
            if (doesExist != null)
            {
                var updateResult = await this.UpdateReviewAsync(review);
                return updateResult;
            }
            var user = await userManager.FindByIdAsync(review.UserId);

            var newReview = new Review()
            {
                Date = DateTime.Now,
                Comment = review.Comment,
                UserId = user.Id,
                ProductId = review.ProductId,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Rating = review.Rating

            };
            try
            {
                context.Reviews.Add(newReview);
                await context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }

        public async Task<bool> DeleteReviewAsync(int productId, string userId)
        {
            var review = await context.Reviews.FirstOrDefaultAsync(x => x.ProductId == productId &&
            x.UserId == userId);
            if (review != null)
            {
                context.Reviews.Remove(review);
                await context.SaveChangesAsync();
                return true;
            }

            return false;
        }

        public async Task<List<Review>> GetReviewByProductIdAsync(int productId, ReviewParameters? parameters)
        {
            var review = context.Reviews.Where(x => x.ProductId == productId);
            var result = await review.OrderBy(x => x.Date).Paginate(parameters).ToListAsync();
            return result;
        }

        public async Task<bool> UpdateReviewAsync(ReviewDTO review)
        {
            var newReview = await context.Reviews.FirstOrDefaultAsync(x => x.ProductId == review.ProductId &&
            x.UserId == review.UserId);
            if (newReview != null)
            {
                newReview.Date = DateTime.Now;
                newReview.Comment = review.Comment;
                newReview.Rating = review.Rating;

                var reviewAttacher = context.Reviews.Attach(newReview);
                reviewAttacher.State = EntityState.Modified;
                await context.SaveChangesAsync();
                return true;
            }

            return false;

        }

        public async Task<Review> GetUserProductReview(int productId, string userId)
        {
            var result = await context.Reviews.FirstOrDefaultAsync(x => x.ProductId == productId &&
            x.UserId == userId);

            return result;
        }
    }
}
