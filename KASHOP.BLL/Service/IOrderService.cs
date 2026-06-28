using KASHOP.DAL.DTO.Request;
using KASHOP.DAL.DTO.Response;
using KASHOP.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KASHOP.BLL.Service
{
    public interface IOrderService
    {
        Task<List<OrderResponse>> GetUserOrders(string userId);
        Task<OrderDetailsResponse?> GetUserOrder(int orderId, string userId);
        Task<bool> CancelOrder (int orderId, string userId);

        Task<List<OrderResponse>> GetAllOrders(OrderStatusEnum status);

        Task<bool> ChangeOrderStatus(int orderId, ChangeOrderStatusRequest request);
    }
}
