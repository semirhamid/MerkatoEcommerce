using AutoMapper;
using ECommerce.DTO;
using ECommerce.Helpers;
using ECommerce.Models;
using ECommerce.Pagination;
using ECommerce.Services;
using ECommerceModels;
using ECommerceServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Headers;
using System.Security.Claims;

namespace ECommerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class ProductController : ControllerBase
    {
        private readonly IProductRepository productRepository;
        private readonly IOrderRepository orderRepository;
        private readonly IWebHostEnvironment hostingEnvironment;
        private readonly IMapper mapper;
        private readonly AppDbContext context;
        private readonly UserManager<ApplicationUser> userManager;

        public ProductController(IProductRepository productRepository,
            IOrderRepository orderRepository,
            IWebHostEnvironment hostingEnvironment,
            IMapper mapper, AppDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            this.productRepository = productRepository;
            this.orderRepository = orderRepository;
            this.hostingEnvironment = hostingEnvironment;
            this.mapper = mapper;
            this.context = context;
            this.userManager = userManager;
        }

        [HttpGet("getbypage")]
        public async Task<ActionResult<List<ProductRating>>> GetProductsByPage([FromQuery] ProductsParameters parameters)
        {
            var products = await productRepository.GetProductByPageAsync(parameters);
            await HttpContext.InsertParamtersPaginationInHeader(context.Products.AsQueryable());
            return Ok(products);
        }
        
        [HttpGet("{id:int}")]
        public  async Task<ActionResult<SingleProductDTO>> GetProduct(int id)
        {
            var product = await productRepository.GetProductById(id);
            if(product == null)
            {
                return BadRequest("Product doesnot exist");
            }
            return Ok(product);
               
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddProduct([FromForm]ProductModel product)
        {
            var user = GetSubject();
            var userObj = await userManager.FindByIdAsync(user);
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid token");
            }
            if(userObj == null)
            {
                return BadRequest("Your must login to upload a product data");
            }
            string uniqueFileName = ProcessUploadedFile(product).Result;
            

            Product newProduct = new Product()
            {
                Name = product.Name,
                Category = product.Category,
                Color = product.Color,
                Price = product.Price,
                Quantity = product.Quantity,
                Size = product.Size,
                Weight = product.Weight,
                PhotoPath = uniqueFileName,
                Description = product.Description,
                SellerFirstName = userObj.FirstName,
                SellerLastName = userObj.LastName,
                SellerId = userObj.Id,
                SellerEmail = userObj.Email
            };



            if(await productRepository.AddProductAsync(newProduct))
            {
                return Ok();
            }
            return NotFound();
            
        }

        [HttpPut("")]
        [Authorize]
        public async Task<IActionResult> EditProduct(int id, [FromForm] ProductUpdateModel product)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var user = GetSubject();

            Product newProduct = new Product()
            {
                Id = id,
                Name = product.Name,
                Category = product.Category,
                Color = product.Color,
                Price = product.Price,
                Quantity = product.Quantity,
                Size = product.Size,
                Weight = product.Weight,
                Description = product.Description,
                
            };

            if (product.PhotoPath != null)
            {
                newProduct.PhotoPath = ProcessUpdateUploadedFile(product).Result;
            }


            var result = await productRepository.UpdateProductAsync(user, newProduct);

            if (!result)
            {
                return Unauthorized();
            }
            return Ok();
        }

        [HttpDelete("")]
        [Authorize(Roles = "Adminstration")]
        public async Task<ActionResult> DeleteProduct(int id)
        {
            if(await productRepository.DeleteProductAsync(id))
            {
                return Ok();
            }
            return NotFound();
        }

        [HttpGet("")]
        public async Task<ActionResult<List<Product>>> GetProductByKeyword([FromHeader]ProductsParameters? parameters,
            string? keyword, string? category)
        {
            
            parameters ??= new ProductsParameters()
                {
                    Page = 1,
                    RecordsPerPage = 50
                };

            if (string.IsNullOrWhiteSpace(keyword))
            {
                if (!string.IsNullOrWhiteSpace(category))
                {
                    var categories = await productRepository.GetProductByCategory(parameters, category);
                    await HttpContext.InsertParamtersPaginationInHeader(context.Products.AsQueryable());
                    return Ok(categories);
                }
                var product = await productRepository.GetProductByPageAsync(parameters);
                await HttpContext.InsertParamtersPaginationInHeader(context.Products.AsQueryable());
                return Ok(product);
            }

            var products = await productRepository.GetProductByKeywordAsync(parameters, keyword, category);
            await HttpContext.InsertParamtersPaginationInHeader(context.Products);

            return Ok(products);
        }

        [HttpGet("related")]
        public async Task<ActionResult<List<Product>>> GetRelatedProducts(string keyword, string category)
        {
            var products = await productRepository.GetRelatedProductsAsync(keyword, category);
            return Ok(products);
        }
        private async static Task<string> ProcessUploadedFile(ProductModel product)
        {
            string uniqueFileName = null;
            if (product.PhotoPath != null)
            {
                string unTrimmedFileName;
                unTrimmedFileName = Guid.NewGuid().ToString() + "_" + product.PhotoPath.FileName;
                uniqueFileName = unTrimmedFileName.Replace(' ', '-');
                uniqueFileName = uniqueFileName.Replace('+', '-');
                string filePath = Path.Combine("StaticFiles/Images", uniqueFileName);
                //product.PhotoPath.CopyTo(new FileStream(filePath, FileMode.Create));
                using (Stream fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await product.PhotoPath.CopyToAsync(fileStream);

                }

                return uniqueFileName;
            }
            return "none";
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
        private async static Task<string> ProcessUpdateUploadedFile(ProductUpdateModel product)
        {
            string uniqueFileName = null;
            if (product.PhotoPath != null)
            {
                string unTrimmedFileName;
                unTrimmedFileName = Guid.NewGuid().ToString() + "_" + product.PhotoPath.FileName;
                uniqueFileName = unTrimmedFileName.Replace(' ', '-');
                uniqueFileName = uniqueFileName.Replace('+', '-');
                string filePath = Path.Combine("StaticFiles/Images", uniqueFileName);
                //product.PhotoPath.CopyTo(new FileStream(filePath, FileMode.Create));
                using (Stream fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await product.PhotoPath.CopyToAsync(fileStream);

                }

                return uniqueFileName;
            }
            return "none";
        }
    }
}
