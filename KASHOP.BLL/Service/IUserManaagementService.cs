using KASHOP.DAL.DTO.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KASHOP.BLL.Service
{
    public interface IUserManaagementService
    {
        Task<List<UserListResponse>> GetAllUsers();
        Task<UserDetailsResponse> GetUserDetails(string userId);
        Task<bool> ChangeRole(string userId, string role);
        Task<bool> ToggleBlockUser(string userId);
        //Task<bool> UnblockUser(string userId);
        Task<bool> SoftDeleteUser(string userId);
    }
}
