using KASHOP.DAL.DTO.Request;
using KASHOP.DAL.DTO.Response;
using KASHOP.DAL.Models;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace KASHOP.BLL.Service
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _emailSender;
        private readonly IConfiguration _config;

        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthenticationService(UserManager<ApplicationUser> userManager, IEmailSender emailSender,
            IConfiguration configuration, IHttpContextAccessor httpContextAccessor
            )
        {
            _userManager = userManager;
            _emailSender = emailSender;
            _config = configuration;
            _httpContextAccessor = httpContextAccessor;
        }



        public async Task<RegisterResponse> RegisterAsync(RegisterRequests request)
        {
            var user = request.Adapt<ApplicationUser>();
            var result = await _userManager.CreateAsync(user, request.Password);

            if (!result.Succeeded)
                return new RegisterResponse() { Success = false,
                    Message = "Eerror",
                    Errors = result.Errors.Select(p => p.Description).ToList()
                };

            await _userManager.AddToRoleAsync(user, "User");
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            token = Uri.EscapeDataString(token);
            var emailUrl = $"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}/api/account/ConfirmEmail?token={token}&userId={user.Id}";
            await _emailSender.SendEmailAsync(user.Email, "Welcome to KASHOP", $"<h1>Thank you for registering! {request.UserName}</h1>" +
                $"" +
                $"<a href='{emailUrl}'> confirm </a>");

            return new RegisterResponse() { Success = true, Message = "User created successfully" };
        }

        public async Task<LoginResponse> LoginAsync(LoginRequests request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);

            if (user is null)
                return new LoginResponse() { Success = false, Message = "Email not found" };

            if (!await _userManager.IsEmailConfirmedAsync(user))
            {
                return new LoginResponse() { Success = false, Message = "Email not confirmed" };
            }


            var result = await _userManager.CheckPasswordAsync(user, request.Password);
            if (!result)
                return new LoginResponse() { Success = false, Message = "Invalid password" };

            return new LoginResponse() { Success = true, Message = "Login successful", AccessToken = await GenerateAccessAsync(user) };

        }

        private async Task<string> GenerateAccessAsync(ApplicationUser user)
        {
            var userClaims = new List<Claim>() {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.UserName)
            };
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:SecretKey"]!));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
        issuer: _config["Jwt:Issuer"],
        audience: _config["Jwt:Audience"],
        claims: userClaims,
        expires: DateTime.Now.AddDays(5),
        signingCredentials: credentials
    );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<bool> ConfirmEmailAsync(string token, string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user is null) return false;

            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (!result.Succeeded) return false;
            return true;
        }

        public async Task<ForgotPasswordRespsonse> RequestPasswordResetAsync(ForgotPasswordRequests request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user is null)
            {
                return new ForgotPasswordRespsonse()
                {
                    Success = false,
                    Message = "Email not found"
                };
            }
            var random = new Random();
            var code = random.Next(1000, 9999).ToString();

            user.CodeResetPassword = code;
            user.PasswordResetCodeExpiry = DateTime.Now.AddMinutes(15);

            await _userManager.UpdateAsync(user);

            await _emailSender.SendEmailAsync(request.Email, "reset Password", $"<p>Code Is {code}</p>");

            return new ForgotPasswordRespsonse()
            {
                Success = true,
                Message = " code sent to your Email "
            };
        }

        public async Task<ResetPasswordResponse> ResetPasswordAsync(ResetPasswordRequests request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user is null)
            {
                return new ResetPasswordResponse()
                {
                    Success = false,
                    Message = "Email not found"
                };
            }
            if (user.PasswordResetCodeExpiry < DateTime.UtcNow)
            {
                return new ResetPasswordResponse()
                {
                    Success = false,
                    Message = "Code Expired"
                };
            }
            if (user.CodeResetPassword != request.Code)
            {
                return new ResetPasswordResponse()
                {
                    Success = false,
                    Message = "Invalid code"
                };
            }
            var IsSamePassword = await _userManager.CheckPasswordAsync(user, request.NewPassword);
            if (IsSamePassword)
            {
                return new ResetPasswordResponse()
                {
                    Success = false,
                    Message = "New password cannot be the same as the old password"
                };
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, request.NewPassword);
            if (!result.Succeeded)
            {
                return new ResetPasswordResponse()
                {
                    Success = false,
                    Message = "Error resetting password"
                };
            }

            user.CodeResetPassword = null;
            user.PasswordResetCodeExpiry = null;
            await _emailSender.SendEmailAsync(request.Email, "Password Reset Successful", $"<p>Your password has been reset successfully.</p>");
            await _userManager.UpdateAsync(user);
            return new ResetPasswordResponse()
            {
                Success = true,
                Message = "Password reset successful"
            };
        }
    }
}