using KASHOP.BLL.Service;
using KASHOP.DAL.DTO.Request;
using KASHOP.UI.Resources;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace KASHOP.UI.Controllers
{
    [Route("api/admin")]
    [ApiController]
    [Authorize]
    public class UserManaagementController : ControllerBase
    {
        private readonly IStringLocalizer<SharedResources> _localizer;
        private readonly IUserManaagementService _userManaagementService;
        public UserManaagementController(IUserManaagementService userManaagementService , IStringLocalizer<SharedResources> localizer)
        {
            _userManaagementService = userManaagementService;
            _localizer = localizer;
        }

        [HttpGet("users")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userManaagementService.GetAllUsers();
            return Ok(new { users });
        }

        [HttpGet("users/{userId}")]
        public async Task<IActionResult> GetUserDetails([FromRoute] string userId)
        {
            var user = await _userManaagementService.GetUserDetails(userId);
            return Ok(new { user });
        }

        [HttpPatch("{userId}/change-role")]
        public async Task<IActionResult> ChangeUserRole( string userId,[FromBody] ChangeRoleRequest request)
        {
            var result = await _userManaagementService.ChangeRole(userId, request.newRole);
            if (!result)
            {
                return BadRequest(new { message = _localizer["RoleChangeFailed"] });
            }
            return Ok(new { message = _localizer["RoleChangedSuccessfully"] });
        }

        [HttpPatch("{userId}/toggle-block")]
        public async Task<IActionResult> ToggleBlock(string userId)
        {
            var result = await _userManaagementService.ToggleBlockUser(userId);
            if (!result)
            {
                return BadRequest(new { message = _localizer["User block"] });
            }
            return Ok(new { message = _localizer["User is blocked"] });
        }
    }
}
