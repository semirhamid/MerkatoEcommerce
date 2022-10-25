using ECommerce.DTO.Requests;
using ECommerce.DTO.Responses;
using ECommerce.Helpers;
using ECommerce.Models;
using ECommerce.Pagination;
using ECommerceModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Services
{
    public interface IUserService
    {
        Task<List<UserModel>> GetUsersByPageAsync(UsersParameters parameters);
        Task<AdminRoleModel> DeleteUserAsync(string userId);
        Task<List<UserModel>> GetUserByEmailAsync(string email);
        Task<UserProfile> GetUserByIdAsync(string id);
        Task<UserManagerResponse> EditUserProfileAsync(EditUserProfile editUserProfile);
        Task<UserDashboardModel> GetUserDashboardAsync(string userId);
        Task<AdminstratorDashboardModel> GetAdminstratorDashboardAsync();
        Task<UserDetailForAdminstrator> GetUserDetailAsync(string userId);
    }
    //Pause::p
    //    PrtSc::s
    //    ScrollLock::k
    //    Break::b

    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly IProductRepository productRepository;
        private readonly IOrderRepository orderRepository;

        public UserService(UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,IProductRepository productRepository,
            IOrderRepository orderRepository)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.productRepository = productRepository;
            this.orderRepository = orderRepository;
        }

        public async Task<List<UserModel>> GetUsersByPageAsync(UsersParameters parameters)
        {
            var users =  await userManager.Users.OrderBy(x=>x.FirstName).Paginate(parameters).ToListAsync();
            var mapped = users.Select(x => new UserModel
            {
                Id = x.Id,
                FirstName = x.FirstName,
                LastName = x.LastName,
                Email = x.Email
            }).ToList();

            return  mapped;
            
        }

        public async Task<UserDashboardModel> GetUserDashboardAsync(string userId)
        {
            var user = await userManager.FindByIdAsync(userId);
            if(user == null)
            {
                return null;
            }
            var totalProducts = await productRepository.GetProductsByUserId(user.Id);
            var totalOrders = await orderRepository.GetOrderByUserId(user.Id);
            return new UserDashboardModel()
            {
                TotalOrder = totalOrders.Count(),
                TotalProduct = totalProducts.Count(),
                Products = totalProducts,
                Orders = totalOrders
            };

        }

        public async Task<AdminstratorDashboardModel> GetAdminstratorDashboardAsync()
        {
            try
            {
                return new AdminstratorDashboardModel()
                {
                    TotalOrder = orderRepository.TotalOrders(),
                    TotalProduct = productRepository.TotalProducts(),
                    LastSeveDaysOrders = orderRepository.GetLastSevenDaysOrders(),
                    LastTenProducts = await productRepository.GetLastTenProducts(),
                    TotalUsers = userManager.Users.Count(),
                    Totalsales = orderRepository.TotalSales(),
                    TodaysSales = orderRepository.TodaysSales()
                };
            }catch(Exception e)
            {
                return null;
            }
            
            

        }
        public async Task<List<UserModel>> GetUserByEmailAsync(string email)
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return null;
            }
            var role = await userManager.GetRolesAsync(user);
            return new List<UserModel>
            {
                new UserModel
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Roles = role.ToList()
            }
            };
        }
        
        public async Task<UserProfile> GetUserByIdAsync(string id)
        {
            var user = await userManager.FindByIdAsync(id);
            if (user == null)
            {
                return null;
            }
            var role = await userManager.GetRolesAsync(user);
            return 
                new UserProfile
                {
                UserId = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Role = role.ToList(),
                Birthday = user.Birthday,
                Gender = user.Gender,
                PhoneNumber= user.PhoneNumber,
                Address = user.Address,
                Country = user.Country,
                Region = user.Region,
                City = user.City
            };
        }

        public async Task<AdminRoleModel> DeleteUserAsync(string userId)
        {
            var user = await userManager.FindByIdAsync(userId);
            if(user == null)
            {
                return new AdminRoleModel
                {
                    Message = "User Does not exist",
                    IsSuccess = false
                };
            }
            var result = await userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                return new AdminRoleModel
                {
                    Message = "User Successfully Deleted",
                    IsSuccess = true
                };
            }

            return new AdminRoleModel
            {
                Message = "Error occured : Couldn't Delete the user",
                IsSuccess = false,
                Errors = result.Errors.Select(x => x.Description)
            };



        }

        public async Task<UserManagerResponse> EditUserProfileAsync(EditUserProfile editUserProfile)
        {
            var user = await userManager.FindByIdAsync(editUserProfile.Id);
            user.PhoneNumber = editUserProfile.PhoneNumber;
            user.Address = editUserProfile.Address;
            user.PhoneNumber = editUserProfile.PhoneNumber;
            user.LastName = editUserProfile.LastName;
            user.FirstName = editUserProfile.FirstName;
            user.Birthday = editUserProfile.Birthday;
            user.City = editUserProfile.City;
            user.Region = editUserProfile.Region;
            user.Country = editUserProfile.Country;
            user.Gender = editUserProfile.Gender;

            var result = await userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                return new UserManagerResponse
                {
                    Message = "data updated successfully",
                    IsSuccess = true,

                };
            }
            return new UserManagerResponse
            {
                Message = "error occurred",
                IsSuccess = false,
                Errors = result.Errors.Select(x => x.Description)

            };


        }
        public async Task<UserDetailForAdminstrator> GetUserDetailAsync(string userId)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return null;
            }
            var totalProducts = await productRepository.GetProductsByUserId(user.Id);
            var totalOrders = await orderRepository.GetOrderByUserId(user.Id);
            return new UserDetailForAdminstrator()
            {
                TotalOrder = totalOrders.Count(),
                TotalProduct = totalProducts.Count(),
                Products = totalProducts,
                Orders = totalOrders,
                User = new UserProfile()
                {
                    UserId = user.Id,
                    Address = user.Address,
                    Birthday = user.Birthday,
                    City = user.City,
                    Country = user.Country,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    Gender = user.Gender ,
                    Region = user.Region,
                    Role = await userManager.GetRolesAsync(user),
                    LastName = user.LastName,
                    PhoneNumber = user.PhoneNumber
                }
            };

        }
    }
}
