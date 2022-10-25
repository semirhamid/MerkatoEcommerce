using ECommerce.Services;
using ECommerceModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Adminstration")]
    public class AdminstrationController : ControllerBase
    {
        private readonly IRoleService roleService;

        public AdminstrationController(IRoleService roleService)
        {
            this.roleService = roleService;
        }

        [HttpPost("CreateRole")]
        public async Task<IActionResult> CreateRole([FromForm] string RoleName)
        {
            if (!string.IsNullOrWhiteSpace(RoleName))
            {
                var result = await roleService.CreateRoleAsync(RoleName);
                if (result.IsSuccess)
                {


                    return Ok(result);

                }

                return BadRequest(result);
            }

            return BadRequest("Invalid rolename");
        }

        [HttpGet("ManageRoles")]
        public async Task<ActionResult> ManageRoles()
        {
            var result =  await roleService.GetAllRolesAsync();
            return Ok(result);
        }
        
        [HttpGet("roledetail/{id}")]
        public async Task<ActionResult> RoleDetail(string id)
        {
            var result = await roleService.GetRoleDescriptionAsync(id);
            if(result == null)
            {
                return BadRequest("Error Occured");
            }
            return Ok(result);
        }
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteRole(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                if(id== "df1bade8-7ddb-445f-9fa0-e22c516a74f8" || id== "06745278-4acd-4e93-a338-99d2acf12826")
                {
                    return BadRequest("Adminstration and Customer Role Can not be deleted");
                }
                var result = await roleService.DeleteRoleAsync(id);
                return Ok(result);
            }
            return BadRequest("Invalid role id");
            
        }

        [HttpGet("FindRoleUser/{email}")]
        public async Task<ActionResult> FindRoleUser(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return BadRequest();
            }

            var result = await roleService.GetUserByEmailRoleAsync(email);
            if(result == null)
            {
                return BadRequest("The user with this email address doesnot exist");
            }

            return Ok(result);
        }

        [HttpGet("AddUserToRole")]
        public async Task<ActionResult> AddUserToRole(string userId, string roleId)
        {
            if(string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(roleId))
            {
                return BadRequest("Invalid Format");
            }

            var result = await roleService.AddUserToRole(userId, roleId);
            if (result.IsSuccess)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        [HttpGet("changerolename")]
        public async Task<ActionResult> ChangeRoleName(string roleId, string newName)
        {
            if(string.IsNullOrEmpty(roleId) || string.IsNullOrEmpty(newName))
            {
                return BadRequest("invalid request");
            }
            var result = await roleService.ChangeRoleNameAsync(roleId, newName);
            if (!result.IsSuccess)
            {
                return BadRequest("Error Occured");
            }
            return Ok(result);
        }

        [HttpGet("removeuserfromrole")]
        public async Task<ActionResult> RemoveUserFromRole(string roleId, string userId)
        {
            if (string.IsNullOrEmpty(roleId) || string.IsNullOrEmpty(userId))
            {
                return BadRequest("invalid request");
            }
            var result = await roleService.RemoveUserFromRoleAsync(userId, roleId);
            if (!result.IsSuccess)
            {
                return BadRequest("Error Occured");
            }
            return Ok(result);
        }
    }
}
