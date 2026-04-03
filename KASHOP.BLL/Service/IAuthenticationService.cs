using KASHOP.DAL.DTO.Request;
using KASHOP.DAL.DTO.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KASHOP.BLL.Service
{
    public interface IAuthenticationService
    {
        Task<RegisterResponse> RegisterAsync(RegisterRequests request);
        Task<LoginResponse> LoginAsync(LoginRequests request);

        Task<bool> ConfirmEmailAsync(string token, string userId);

        Task<ForgotPasswordRespsonse> RequestPasswordResetAsync(ForgotPasswordRequests request);
        Task<ResetPasswordResponse> ResetPasswordAsync(ResetPasswordRequests request);
    }
}
