using AutoMapper;
using ECommerceModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceServices
{
    public class ProductRepository : IProductRepository
    {
        private readonly AppDbContext context;
        private readonly IMapper mapper;
        public ProductRepository(AppDbContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }


        public async Task<List<Product>> GetAllProduct()
        {
            var result = await context.Products.ToListAsync();

            return result;
        }

        public  Product GetProductById(int id)
        {
            var product = context.Products.FirstOrDefault(x=> x.Id == id);
            if(product == null)
            {
                return null;
            }
            return product;
        }

        public async Task<bool> AddProductAsync(Product product)
        {
            var newProduct = mapper.Map<Product>(product);
            if(newProduct != null)
            {
                context.Products.Add(newProduct);
                await context.SaveChangesAsync();

                return true;
            }
            return false;
            
        }

        public async Task<bool> UpdateProductAsync(Product product)
        {

            if(await context.Products.FirstOrDefaultAsync(x => x.Id == product.Id) != null)
            {
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



