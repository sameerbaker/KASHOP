using KASHOP.DAL.DTO.Request;
using KASHOP.DAL.Repository;
using KASHOP.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mapster;

namespace KASHOP.BLL.Service
{
    public class ReviewService : IReviewService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IReviewRepository _reviewRepository;

        public ReviewService(IOrderRepository orderRepository, IReviewRepository reviewRepository)
        {
            _orderRepository = orderRepository;
            _reviewRepository = reviewRepository;
        }

        public async Task<bool> AddReview(string userId, AddReviewRequest request)
        {
            var purchasedOrder = await _orderRepository.GetOne(
               filter: o=>o.UserId == userId &&
                o.OrderStatus == OrderStatusEnum.Delivered &&
                o.OrderItems.Any(oi=> oi.ProductId == request.ProductId),
                includes: new[]
                {
                    nameof(Order.OrderItems)
                }
                );
            if (purchasedOrder == null)
            {
                return false; 
            }
            var AlreadyReviewed = await _reviewRepository.GetOne(
                filter: r => r.UserId == userId && r.ProductId == request.ProductId
                );
            if (AlreadyReviewed != null)
            {
                return false;
            }
            var review = request.Adapt<Review>();
            review.UserId = userId;

            await _reviewRepository.CreateAsync(review);

            return true;
        }
    }
}
