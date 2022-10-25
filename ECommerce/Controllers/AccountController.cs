using ECommerce.DTO.Requests;
using ECommerce.DTO.Responses;
using ECommerce.Helpers;
using ECommerce.Models;
using ECommerce.Pagination;
using ECommerce.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using SendGrid.Helpers.Mail;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Web.Http.Results;

namespace ECommerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AccountController : ControllerBase
    {
        private readonly IUserService userService;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;

        public AccountController(IUserService userService, 
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            this.userService = userService;
            this.userManager = userManager;
            this.signInManager = signInManager;
        }
        [Authorize(Roles = "Adminstration")]
        [HttpGet("getusers")]
        public async Task<IActionResult> GetUser([FromQuery] UsersParameters parameters)
        {
            var user = GetSubject();
            var headers = this.Request.Headers;
            var users = await userService.GetUsersByPageAsync(parameters);
            
            var token = headers.Authorization;


            await HttpContext.InsertParamtersPaginationInHeader(userManager.Users);

            return Ok(users);
        }

        [HttpPost("edituser")]
        public async Task<ActionResult> EditUser(EditUserProfile editUserProfile)
        {
            var user = GetSubject();
            var isAdmin = await userManager.FindByIdAsync(editUserProfile.Id);
            var role = await userManager.IsInRoleAsync(isAdmin, "Adminstration");
            if(user != editUserProfile.Id || !role)
            {
                return Unauthorized();
            }
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid type");
            }

            var result = await userService.EditUserProfileAsync(editUserProfile);
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            return BadRequest(result);

        }

        [HttpDelete("")]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("user id is required");
            }


            var headers = this.Request.Headers;
            var user = GetSubject();
            var requestUser = await userManager.FindByIdAsync(user);
            if(requestUser == null)
            {
                return BadRequest("user does not exist");
            }
            var role = await userManager.IsInRoleAsync(requestUser, "Adminstration");

            if (user == userId || role)
            {
                var result = await userService.DeleteUserAsync(userId);
                if (result.IsSuccess)
                {
                    return Ok(result);
                }
            }
           
            return Unauthorized();

        }

        [Authorize(Roles = "Adminstration")]
        [HttpGet("getuserbyemail")]
        public async Task<IActionResult> GetUserByEmail([FromQuery]string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return BadRequest("email can not be empty");
            }

            var user = await userService.GetUserByEmailAsync( email);
            if (user == null)
            {
                return BadRequest("The user with this email address doesnot exit");
            }
            return Ok(user);
        }


        [HttpGet("getuserbyid")]
        public async Task<IActionResult> GetUserById([FromQuery] string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest("id can not be empty");
            }

            var user = await userService.GetUserByIdAsync(id);
            var userSub = GetSubject();
            var isAdmin = await userManager.FindByIdAsync(id);
            var role = await userManager.IsInRoleAsync(isAdmin, "Adminstration");
            if (userSub != id || !role)
            {
                return Unauthorized();
            }
            if (user == null)
            {
                return BadRequest("The user with this email address doesnot exit");
            }
            return Ok(user);
        }


        [HttpGet("getuserdashboard")]
        public async Task<ActionResult> GetUserDashboard()
        {
            var userId = GetSubject();
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest();
            }

            var result = await userService.GetUserDashboardAsync(userId);

            return Ok(result);
        }

        [Authorize(Roles = "Adminstration")]
        [HttpGet("getadminstratordashboard")]
        //Authorize(Roles = "Adminstration")]
        public async Task<ActionResult> GetAdminstratorDashboard()
        {
            var userId = GetSubject();
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest();
            }


            var result = await userService.GetAdminstratorDashboardAsync();

            return Ok(result);
        }



        [HttpGet("getuserdetail")]
        //is used in adminstration user management
        //only admin can access this
        public async Task<ActionResult> GetUserDetail(string Id)
        {
            if (string.IsNullOrEmpty(Id))
            {
                return null;
            }
            
            var result = await userService.GetUserDetailAsync(Id);

            return Ok(result);
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
    }
}
