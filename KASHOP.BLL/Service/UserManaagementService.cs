using KASHOP.DAL.DTO.Response;
using KASHOP.DAL.Models;
using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KASHOP.BLL.Service
{
    public class UserManaagementService : IUserManaagementService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserManaagementService(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager) 
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }
        public async Task<List<UserListResponse>> GetAllUsers()
        {
            var users = await _userManager.Users.ToListAsync();
            return users.Adapt<List<UserListResponse>>();
        }

        public async Task<UserDetailsResponse?> GetUserDetails(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            var roles = await _userManager.GetRolesAsync(user);
            var result = user.Adapt<UserDetailsResponse>();
            result.Role = roles.FirstOrDefault();
            return result;
        }

        public async Task<bool> ChangeRole(string userId, string role)
        {
            var user = await _userManager.FindByIdAsync(userId);
            var roleExists = await _roleManager.RoleExistsAsync(role);
            if(!roleExists) {              
                return false;
            }
            var currentRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, currentRoles);
            var result = await _userManager.AddToRoleAsync(user, role);
            return result.Succeeded;
        }

        public Task<bool> BlockUser(string userId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UnblockUser(string userId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> SoftDeleteUser(string userId)
        {
            throw new NotImplementedException();
        }
    }
}
