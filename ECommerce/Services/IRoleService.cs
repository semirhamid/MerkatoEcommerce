using ECommerce.DTO.Responses;
using ECommerce.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Services
{
    public interface IRoleService
    {
        Task<AdminRoleModel> CreateRoleAsync(string RoleName);
        Task<List<IdentityRole>> GetAllRolesAsync();
        Task<AdminRoleModel> DeleteRoleAsync(string id);
        Task<UserRole> GetUserByEmailRoleAsync(string email);
        Task<AdminRoleModel> AddUserToRole(string userId, string roleId);
        Task<RoleDescription> GetRoleDescriptionAsync(string roleId);
        Task<AdminRoleModel> ChangeRoleNameAsync(string roleId, string newName);
        Task<AdminRoleModel> RemoveUserFromRoleAsync(string userId, string roleId);
    }

    public class RoleManagerService : IRoleService
    {
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly UserManager<ApplicationUser> userManager;

        public RoleManagerService(RoleManager<IdentityRole> roleManager, 
            UserManager<ApplicationUser> userManager)
        {
            this.roleManager = roleManager;
            this.userManager = userManager;
        }

        public async Task<AdminRoleModel> CreateRoleAsync(string RoleName)
        {
            if (RoleName == null)
            {
                return new AdminRoleModel
                {
                    Message = "RoleName must more than one three character",
                    IsSuccess = false
                };
            }

            var answer = await roleManager.FindByNameAsync(RoleName);
            if(answer != null)
            {
                return new AdminRoleModel
                {
                    Message = "The role already exists",
                    IsSuccess = false
                };
            }
            IdentityRole identityRole = new IdentityRole
            {
                Name = RoleName
            };
            var result = await roleManager.CreateAsync(identityRole);

            if (result.Succeeded)
            {
                return new AdminRoleModel
                {
                    Message = "Role created successfully",
                    IsSuccess = true
                };

            }

            return new AdminRoleModel
            {
                Message = "Some error occured",
                IsSuccess = false,
                Errors = result.Errors.Select(e => e.Description)
            };
        }

        public async Task<List<IdentityRole>> GetAllRolesAsync()
        {
            return await roleManager.Roles.ToListAsync();
        }

        public async Task<AdminRoleModel> DeleteRoleAsync(string id)
        {
            IdentityRole role = await roleManager.FindByIdAsync(id);
            if (role == null)
            {
                return new AdminRoleModel
                {
                    Message = "The role does not exist",
                    IsSuccess = false,

                };
            }
            ApplicationUser[] users = null;

            foreach(var user in userManager.Users)
            {
                if(await userManager.IsInRoleAsync(user, role.Name))
                {
                    users.Append(user);
                }
            }
            if(users != null)
            {
                return new AdminRoleModel
                {
                    Message = "You must remove all user from the role",
                    IsSuccess = false,

                };
            }
            var result = await roleManager.DeleteAsync(role);
            if (result.Succeeded)
            {
                return new AdminRoleModel
                {
                    Message = "The has been successfully removed",
                    IsSuccess = true,

                };
            }

            return new AdminRoleModel
            {
                Message = "The role does not exist",
                IsSuccess = false,
                Errors = result.Errors.Select(e => e.Description)

            };

        }

        public async Task<UserRole> GetUserByEmailRoleAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return null;
            }

            var user = await userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return null;
            }
            IList<string> rolesName = await userManager.GetRolesAsync(user);

            return new UserRole
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Id = user.Id,
                RoleName = rolesName
            };
        }

        public async Task<AdminRoleModel> AddUserToRole(string userId, string roleId)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return new AdminRoleModel
                {
                    Message = "the user with this id doesnot exist",
                    IsSuccess = false
                };

            }
            var role = await roleManager.FindByIdAsync(roleId);
            if (role == null)
            {
                return new AdminRoleModel
                {
                    Message = "the role with this id doesnot exist",
                    IsSuccess = false
                };

            }

            var IsInRole = userManager.IsInRoleAsync(user, role.Name);
            if (IsInRole.Result)
            {
                return new AdminRoleModel
                {
                    Message = "the user is already registered for the role",
                    IsSuccess = false
                };
            }

            var result = await userManager.AddToRoleAsync(user, role.Name);

            if (!result.Succeeded)
            {
                return new AdminRoleModel
                {
                    Message = "Could not add the user to the role, Please contact the developer",
                    IsSuccess = false,
                    Errors = result.Errors.Select(e => e.Description)
                };
            }
            return new AdminRoleModel
            {
                Message = "The user is added successfully",
                IsSuccess = true
            };
        }

        public async Task<RoleDescription> GetRoleDescriptionAsync(string roleId)
        {
            var role = await roleManager.FindByIdAsync(roleId);
            if (role == null)
            {
                return null;
            }
            var users = new List<EditRoleUserModel>();

            foreach(var user in userManager.Users)
            {
                if(await userManager.IsInRoleAsync(user, role.Name))
                {
                    users.Add(new EditRoleUserModel
                    {
                        Id = user.Id,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Email = user.Email,
                        PhoneNumber = user.PhoneNumber
                    });
                }
            }

            return new RoleDescription {
                RoleId = role.Id,
                RoleName = role.Name,
                Users = users
            };
        }

        public async Task<AdminRoleModel> ChangeRoleNameAsync(string roleId, string newName)
        {
            var role = await roleManager.FindByIdAsync(roleId);
            if(role== null)
            {
                return new AdminRoleModel
                {
                    Message = "role with this id doesnot exist",
                    IsSuccess = false
                };
            }

            role.Name = newName;
            var result = await roleManager.UpdateAsync(role);

            if (result.Succeeded)
            {
                return new AdminRoleModel
                {
                    Message = "name successfully updated",
                    IsSuccess = true
                };
            }

            return new AdminRoleModel
            {
                Message = "Error occured",
                IsSuccess = false
            };
        }

        public async Task<AdminRoleModel> RemoveUserFromRoleAsync(string userId, string roleId)
        {
            var user = await userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return new AdminRoleModel
                {
                    Message = "user with this id doesnot exist",
                    IsSuccess = false
                };
            }

            var role = await roleManager.FindByIdAsync(roleId);

            if (role == null)
            {
                return new AdminRoleModel
                {
                    Message = "role with this id doesnot exist",
                    IsSuccess = false
                };
            }

            var result = await userManager.RemoveFromRoleAsync(user, role.Name);
            if (result.Succeeded)
            {
                return new AdminRoleModel
                {
                    Message = "user removed successfully",
                    IsSuccess = true
                };
            }
            return new AdminRoleModel
            {
                Message = "Caould not remove the user",
                IsSuccess = false
            };
        }
    }
}
