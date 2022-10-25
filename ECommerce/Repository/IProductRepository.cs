using ECommerce.DTO;
using ECommerce.Pagination;
using ECommerceModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce
{
    public interface IProductRepository
    {
        Task<bool> DecreaseProductQuantityAsync(Product product, int quantity);
        Task<List<ProductRating>> GetProductByPageAsync(ProductsParameters parameters);
        Task<SingleProductDTO> GetProductById(int id);
        Task<bool> AddProductAsync(Product product);
        Task<bool> UpdateProductAsync(string userId, Product product);
        Task<bool> DeleteProductAsync(int productId);
        Task<List<Product>> GetProductByKeywordAsync(ProductsParameters parameters, string keyword, string? category);
        Task<List<Product>> GetProductByCategory(ProductsParameters parameters, string category);
        Task<List<Product>> GetRelatedProductsAsync(string keyword, string category);
        Task<List<Product>> GetProductsByUserId(string id);
        int TotalProducts();
        Task<List<Product>> GetLastTenProducts();
    }
}
