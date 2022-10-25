using AutoMapper;
using ECommerce.DTO;
using ECommerce.DTO.Responses;
using ECommerce.Helpers;
using ECommerce.Models;
using ECommerce.Pagination;
using ECommerce.Services;
using ECommerceModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce
{
    public class ProductRepository : IProductRepository
    {
        private readonly AppDbContext context;
        private readonly IMapper mapper;
        private readonly UserManager<ApplicationUser> userManager;

        public ProductRepository(AppDbContext context, IMapper mapper,
            UserManager<ApplicationUser> userManager)
        {
            this.context = context;
            this.mapper = mapper;
            this.userManager = userManager;
           
        }

        public int TotalProducts()
        {
            return context.Products.Count(); 
        }
        public async Task<List<ProductRating>> GetProductByPageAsync(ProductsParameters parameters)
        {

            var products = await context.Products.OrderBy(x => x.Date).Paginate(parameters).ToListAsync();
            var result = new List<ProductRating>();

            foreach (var product in products)
            {
                var rate = context.Reviews.Where(x => x.ProductId == product.Id );
                var count = rate.Count();
                var sum = rate.Select(t => t.Rating).Sum();
                double avg = 0;
                if (count == 0) avg = 0; 
                else{ 
                    avg = sum / count; 
                }
                var newRatingProduct = mapper.Map<ProductRating>(product);
                newRatingProduct.Rating = avg;

                result.Add(newRatingProduct);
            }
            return result;
        }
        public async Task<List<Product>> GetLastTenProducts()
        {
            return await context.Products.Take(10).ToListAsync();
        }
        public async Task<List<Product>> GetProductByCategory(ProductsParameters parameters, string category)
        {

            var result = await context.Products.Where(x => x.Category == category).Paginate(parameters).ToListAsync();
            return result;
        }
        public async Task<List<Product>> GetProductByKeywordAsync(ProductsParameters parameters, string keyword, string? category)
        {
            var products = context.Products.Where(x => x.Name.ToLower().Contains(keyword.ToLower()) || 
            x.Description.ToLower().Contains(keyword.ToLower()));
            if (products == null)
            {
                return null;
            }

            if (!string.IsNullOrEmpty(category))
            {
                var productCategory = await  products.Where(x => x.Category == category).Paginate(parameters).ToListAsync();
                return productCategory;
            }

            return await products.Paginate(parameters).ToListAsync();
        }

        public async Task<List<Product>> GetRelatedProductsAsync(string keyword, string category)
        {
            var products = context.Products.Where(x => x.Name.ToLower().Contains(keyword.ToLower()) ||
            x.Description.ToLower().Contains(keyword.ToLower())||x.Category == category);
            if (products == null)
            {
                return null;
            }

            return await products.Take(20).ToListAsync();
        }
        public  async Task<SingleProductDTO> GetProductById(int id)
        {
            var product = await context.Products.FirstOrDefaultAsync(x => x.Id == id);
            if (product == null)
            {
                return null;
            }
            var newProduct = mapper.Map<SingleProductDTO>(product);
            var reviews = context.Reviews.Where(x => x.ProductId == id);
            var count = await reviews.CountAsync();
            newProduct.Count = count;
            if(count == 0)
            {
                newProduct.Rating = 0;
            }
            else
            {
                var sum = reviews.Select(t => t.Rating).Sum();
                newProduct.Rating = sum / count;
            }
            return newProduct;
        }
        public async Task<List<Product>> GetProductsByUserId(string id)
        {
            var products =  context.Products.Where(x => x.SellerId == id).ToList();
            if (products == null)
            {
                return null;

            }
            return products;
        }

        public async Task<bool> AddProductAsync(Product product)
        {
            var newProduct = mapper.Map<Product>(product);
            newProduct.Date = DateTime.Now;
            if(newProduct != null)
            {
                context.Products.Add(newProduct);
                await context.SaveChangesAsync();

                return true;
            }
            return false;
        }

        public async Task<bool> UpdateProductAsync(string userId, Product product)
        {
            var oldProduct = await context.Products.FirstOrDefaultAsync(x => x.Id == product.Id);
            var isAdmin = await userManager.FindByIdAsync(userId);
            var role = await userManager.IsInRoleAsync(isAdmin, "Adminstration");
            if (userId == oldProduct.SellerId || role)
            {


                if (oldProduct != null)
                {
                    oldProduct.Name = product.Name;
                    oldProduct.Price = product.Price;
                    if (product.PhotoPath != null)
                    {
                        oldProduct.PhotoPath = product.PhotoPath;
                    }

                    oldProduct.Category = product.Category;
                    oldProduct.Color = product.Color;
                    oldProduct.Date = DateTime.Now;
                    oldProduct.Description = product.Description;
                    oldProduct.Quantity = product.Quantity;
                    oldProduct.Size = product.Size;
                    oldProduct.Weight = product.Weight;

                    var productAttacher = context.Products.Attach(oldProduct);
                    productAttacher.State = EntityState.Modified;
                    await context.SaveChangesAsync();
                    return true;
                }
            }
            
            return false;
            
        }
        public async Task<bool> DecreaseProductQuantityAsync (Product product, int quantity)
        {
            
            if ( product != null)
            {
                int previousQuantity = product.Quantity;
                int newQuantity = previousQuantity - quantity;
                product.Quantity = newQuantity;
                var productAttacher = context.Products.Attach(product);
                productAttacher.State = EntityState.Modified;
                await context.SaveChangesAsync();
                return true;
            }

            return false;

        }

        public async Task<bool> DeleteProductAsync(int productId)
        {
            var product = context.Products.FirstOrDefault(x=> x.Id==productId);
            if(product != null)
            {
                context.Products.Remove(product);
                await context.SaveChangesAsync();
                return true;
            }

            return false;
        }

    }
}



