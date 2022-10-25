using ECommerceModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceServices
{
    public interface IProductRepository
    {
        Task<List<Product>> GetAllProduct();
        Product GetProductById(int id);
        Task<bool> AddProductAsync(Product product);
        Task<bool> UpdateProductAsync(Product product);
        Task<bool> DeleteProductAsync(int productId);
    }
}
