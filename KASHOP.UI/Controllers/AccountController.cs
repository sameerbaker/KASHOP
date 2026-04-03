using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using KASHOP.BLL.Service;
using KASHOP.DAL.DTO.Request;

namespace KASHOP.UI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAuthenticationService _authenticationService;
        public AccountController(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequests request)
        {
            var result = await _authenticationService.RegisterAsync(request);
            return Ok(result);
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginRequests request)
        {
            var result = await _authenticationService.LoginAsync(request);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        [HttpGet("ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmail(string token, string userId)
        {
            var isConfirmed = await _authenticationService.ConfirmEmailAsync(token, userId);
            if (isConfirmed) return Ok();
            return BadRequest();

        }

        [HttpPost("SendCode")]
        public async Task<IActionResult> RequestPasswordReset(ForgotPasswordRequests request)
        {
            var result = await _authenticationService.RequestPasswordResetAsync(request);
            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword(ResetPasswordRequests request)
        {
            var result = await _authenticationService.ResetPasswordAsync(request);
            if (!result.Success)
                return BadRequest(result);
            return Ok(result);
        }
    }
}