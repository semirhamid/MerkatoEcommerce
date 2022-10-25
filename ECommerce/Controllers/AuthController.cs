
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ECommerce.DTO.Requests;
using ECommerce.Services;
using ECommerceModels;
using ECommerce.DTO.Responses;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using ECommerce.Models;
using System.IdentityModel.Tokens.Jwt;
using System.IO;

namespace ECommerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly Services.IAuthorizationService authorizationService;
        private readonly IMailService mailService;
        private readonly IConfiguration configuration;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly UserManager<ApplicationUser> userManager;

        public AuthController(Services.IAuthorizationService userService, IMailService mailService, IConfiguration configuration,
            SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager)
        {
            this.authorizationService = userService;
            this.mailService = mailService;
            this.configuration = configuration;
            this.signInManager = signInManager;
            this.userManager = userManager;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromForm] RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await authorizationService.RegisterUserAsync(model);
                if (result.IsSuccess)
                {


                    return Ok(result);

                }

                return Ok(result);
            }

            return BadRequest("Some Properties are not valid");
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromForm] LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await authorizationService.LoginUserAsync(model);
                if (result.IsSuccess)
                {

                    return Ok(result);

                }

                return BadRequest(result.Message);
            }

            return BadRequest("Some Properties are not valid");
        }

        [HttpGet("confirmEmail")]
        public async Task<ActionResult> ConfirmEmail(string userId, string token)
        {
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(token))
            {
                return NotFound();
            }

            var result = await authorizationService.ConfirmEmailAsync(userId, token);

            if (result.IsSuccess)
            {
                return Redirect($"{configuration["frontend_url"]}/account/confirmemail");
            }

            return BadRequest(result);
        }

        [HttpPost("ForgetPassword")]
        public async Task<ActionResult> ForgetPassword(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return NotFound();
            }

            var result = await authorizationService.ForgetPasswordAsync(email);

            if (result.IsSuccess)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        [HttpPost("ResetPassword")]
        public async Task<ActionResult> ResetPassword([FromForm] ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var result = await authorizationService.ResetPasswordAsync(model);

            if (result.IsSuccess)
            {
                return Redirect("http://127.0.0.1:5173/account/login");
            }

            return BadRequest(result);
        }


        [AllowAnonymous]
        [HttpPost("externallogin")]
        public async Task<ActionResult> ExternalLogin(ExternalLoginModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var result = await authorizationService.ExternalLoginAsync(model);
            if (result.IsSuccess)
            {

                return Ok(result);

            }
            return BadRequest(result);
        }

       
    }
}

