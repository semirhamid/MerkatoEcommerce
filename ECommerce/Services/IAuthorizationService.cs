using ECommerce.Configuration;
using ECommerce.DTO.Responses;
using ECommerce.Models;
using ECommerceModels;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ECommerce.Services
{
    public interface IAuthorizationService
    {
        Task<AuthResponse> RegisterUserAsync(RegisterViewModel model);
        Task<UserManagerResponse> LoginUserAsync(LoginViewModel model);
        Task<UserManagerResponse> ConfirmEmailAsync(string userId, string token);
        Task<UserManagerResponse> ForgetPasswordAsync(string email);
        Task<UserManagerResponse> ExternalLoginAsync(ExternalLoginModel model);
        Task<UserManagerResponse> ResetPasswordAsync(ResetPasswordViewModel model);

    }



    public class AuthorizationService : IAuthorizationService
    {
        private readonly UserManager<ApplicationUser> userManager;


        private readonly IConfiguration configuration;
        private readonly IMailService mailService;
        private readonly SignInManager<ApplicationUser> signInManager;

        public AuthorizationService(UserManager<ApplicationUser> userManager,
            IConfiguration configuration,
            IMailService mailService,
            SignInManager<ApplicationUser> signInManager)
        {
            this.userManager = userManager;
            this.configuration = configuration;
            this.mailService = mailService;
            this.signInManager = signInManager;
        }


        public async Task<AuthResponse> RegisterUserAsync(RegisterViewModel model)
        {

            if (model == null)
            {
                throw new NullReferenceException(nameof(model));
            }
            if (model.Password != model.ConfirmPassword)
            {
                return new AuthResponse()
                {
                    Message = "Password doesnot match with confirm password",
                    IsSuccess = false
                };
            }
            var applicationUser = new ApplicationUser()
            {
                Email = model.Email,
                UserName = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName

            };

            var result = await userManager.CreateAsync(applicationUser, model.Password);
            var roleResult = await userManager.AddToRoleAsync(applicationUser, "Customer");

            if (result.Succeeded)
            {
                var confirmEmailToken = await userManager.GenerateEmailConfirmationTokenAsync(applicationUser);
                var encodedEmailToken = Encoding.UTF8.GetBytes(confirmEmailToken);
                var validEmailToken = WebEncoders.Base64UrlEncode(encodedEmailToken);
                string url = $"{configuration["AppUrl"]}/api/auth/confirmemail?userId={applicationUser.Id}&token={validEmailToken}";
                string content = $"<h1style=\"color:blue;text-align:center;\"> Registered Sucessfully<h1/><p>Click here to confirm your email</p><a href={url}>Confirm Account</a>";


                await mailService.sendEmailAsync(applicationUser.Email, "Thanks for registering", content);

                return new AuthResponse()
                {
                    Message = "Account created successfully",
                    IsSuccess = true
                };
            }
            return new AuthResponse()
            {
                Message = result.Errors.ToList()[0].Code,
                IsSuccess = false,
                Errors = result.Errors.Select(e => e.Description)
            };

        }

        public async Task<UserManagerResponse> LoginUserAsync(LoginViewModel model)
        {
            var user = await userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return new UserManagerResponse
                {
                    Message = "There is no user with this account",
                    IsSuccess = false,
                };
            }

            var result = await userManager.CheckPasswordAsync(user, model.Password);
            if (!result)
            {
                return new UserManagerResponse
                {
                    Message = "Invalid password",
                    IsSuccess = false
                };
            }

            return await TokenGenerator(user);

        }

        private async Task<UserProfile> TokenGenerator(ApplicationUser user)
        {
            var roles = await userManager.GetRolesAsync(user);

            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim("Id", user.Id)

            };
            foreach (var userrole in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, userrole));
            }
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["AuthSettings:Key"]));
            var token = new JwtSecurityToken(
                issuer: configuration["AuthSettings:Issuer"],
                audience: configuration["AuthSettings:Audience"],
                claims: claims,
                expires: DateTime.Now.AddDays(30),
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)

                );
            string tokenAsString = new JwtSecurityTokenHandler().WriteToken(token);

            return new UserProfile()
            {
                Message = tokenAsString,
                IsSuccess = true,
                ExpireDate = token.ValidTo,
                Role = roles,
                FirstName = user.FirstName,
                LastName = user.LastName,
                UserId = user.Id,
            };

        }

        public async Task<UserManagerResponse> ConfirmEmailAsync(string userId, string token)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return new UserManagerResponse
                {
                    Message = "User not found",
                    IsSuccess = false,
                };

            }

            var decodedToken = WebEncoders.Base64UrlDecode(token);
            string normalToken = Encoding.UTF8.GetString(decodedToken);
            var result = await userManager.ConfirmEmailAsync(user, normalToken);

            if (result.Succeeded)
            {
                return new UserManagerResponse
                {
                    Message = "Email confirmed Successfully",
                    IsSuccess = true
                };
                await userManager.AddToRoleAsync(user, "Customer");
            }
            return new UserManagerResponse()
            {
                Message = "Could not confirm email",
                IsSuccess = false,
                Errors = result.Errors.Select(e => e.Description)
            };


        }

        public async Task<UserManagerResponse> ForgetPasswordAsync(string email)
        {
            var user = await userManager.FindByEmailAsync(email);

            if (user == null)
                return new UserManagerResponse
                {
                    Message = "No user associated with this email",
                    IsSuccess = false
                };

            var token = await userManager.GeneratePasswordResetTokenAsync(user);
            var encodedToken = Encoding.UTF8.GetBytes(token);
            string validToken = WebEncoders.Base64UrlEncode(encodedToken);

            string url = $"{configuration["frontend_url"]}/account/resetpassword/{email}/{validToken}";
            string content = $"<h1> Reset password<h1/><p>Click here to reset your password</p><a href={url}>Reset Password</a>";


            await mailService.sendEmailAsync(email, "Reset Password", content);

            return new UserManagerResponse
            {
                Message = "reset url has been sent succesfully",
                IsSuccess = true
            };

        }

        public async Task<UserManagerResponse> ResetPasswordAsync(ResetPasswordViewModel model)
        {
            var user = await userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return new UserManagerResponse
                {
                    Message = "the email doesnot exist",
                    IsSuccess = false
                };
            var decodedToken = WebEncoders.Base64UrlDecode(model.Token);
            string normalToken = Encoding.UTF8.GetString(decodedToken);

            var result = await userManager.ResetPasswordAsync(user, normalToken, model.NewPassword);
            if (result.Succeeded)
            {
                return new UserManagerResponse
                {
                    Message = "Password reset Successfully",
                    IsSuccess = true
                };
            }

            return new UserManagerResponse()
            {
                Message = "something went wrong",
                IsSuccess = false,
                Errors = result.Errors.Select(e => e.Description)
            };
        }

        public async Task<UserManagerResponse> ExternalLoginAsync(ExternalLoginModel model)
        {

            var signInResult = await signInManager.ExternalLoginSignInAsync(model.LoginProvider, model.ProviderKey, isPersistent: false);
            if (signInResult.Succeeded)
            {
                var user = await userManager.FindByEmailAsync(model.Email);
                return await TokenGenerator(user);
            }
            else
            {
                var user = await userManager.FindByEmailAsync(model.Email);

                if (user == null)
                {
                    user = new ApplicationUser {
                        UserName = model.Email,
                        Email = model.Email,
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                    };

                    await userManager.CreateAsync(user);
                    await userManager.AddToRoleAsync(user, "Customer");
                    var confirmToken = await userManager.GenerateEmailConfirmationTokenAsync(user);
                    await userManager.ConfirmEmailAsync(user, confirmToken);
                }
                var logUser = new UserLoginInfoModel(model.ProviderDisplayName, model.ProviderKey, model.ProviderDisplayName);
                await userManager.AddLoginAsync(user, logUser);
                await signInManager.SignInAsync(user, isPersistent: false);

                return await TokenGenerator(user);
            }

            return new UserManagerResponse
            {
                Message = "Error Occurred",
                IsSuccess = false
            };
        }
    }
}

