using KASHOP.DAL.DTO.Request;
using KASHOP.DAL.DTO.Response;
using KASHOP.DAL.Models;
using KASHOP.DAL.Repository;
using Microsoft.AspNetCore.Identity;
using Stripe.BillingPortal;
using Stripe.Checkout;
using Stripe;
using Microsoft.AspNetCore.Http;
using Product = KASHOP.DAL.Models.Product;


namespace KASHOP.BLL.Service
{


    public class CheckoutService : ICheckoutService
    {
        private readonly ICartRepository _cartRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IOrderRepository _orderRepository;
        private readonly ICartService _cartService;
        private readonly IProductRepository _productRepository;


        public CheckoutService(ICartRepository cartRepository, UserManager<ApplicationUser> userManager,
            IHttpContextAccessor httpContextAccessor,
                IOrderRepository orderRepository,
                ICartService cartService
, IProductRepository productRepository
            )
        {
            _cartRepository = cartRepository;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
            _orderRepository = orderRepository;
            _cartService = cartService;
            _productRepository = productRepository;
        }
        public async Task<CheckoutResponse> ProcessCheckout(string userId, CheckoutRequest request)
        {
            var cartItems = await _cartRepository.GetAllAsync(
                filter: c => c.UserId == userId,
                includes: new[] { nameof(Cart.Product),
                      $"{nameof(Cart.Product)}.{nameof(Product.Translations)}"
                }
                );
            if (!cartItems.Any())
                return new CheckoutResponse
                {
                    Success = false,
                    ErrorMessage = "Your cart is empty."
                };
            var user = await _userManager.FindByIdAsync(userId);
            var phoneNumber = request.PhoneNumber ?? user.PhoneNumber;
            if (phoneNumber is null)
                return new CheckoutResponse
                {
                    Success = false,
                    ErrorMessage = "Phone number is required"
                };
            var street = request.Street ?? user.Street;
            if (street is null)
                return new CheckoutResponse
                {
                    Success = false,
                    ErrorMessage = "Street is required"
                };
            var city = request.City ?? user.City;
            if (city is null)
                return new CheckoutResponse
                {
                    Success = false,
                    ErrorMessage = "City is required"
                };

            foreach (var item in cartItems)
            {
                if (item.Count > item.Product.Quantity)
                    return new CheckoutResponse
                    {
                        Success = false,
                        ErrorMessage = $"Not enough quantity for product "
                    };
            }

           
            if (request.PaymentMethod == PaymentMethodEnum.Cash)
            {
                return new CheckoutResponse
                {
                    Success = true,

                };

            }
            if (request.PaymentMethod == PaymentMethodEnum.Visa)
            {
                var options = new Stripe.Checkout.SessionCreateOptions
                {
                    PaymentMethodTypes = new List<string> { "card" },
                    Mode = "payment",
                    SuccessUrl = $"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}/checkout/success?sessionId={{CHECKOUT_SESSION_ID}}",
                    CancelUrl = $"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}/checkout/cancel",
                    LineItems = new List<SessionLineItemOptions>()
                };

                foreach (var item in cartItems)
                {
                    options.LineItems.Add(new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {

                            Currency = "usd",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = item.Product.Translations.FirstOrDefault(t => t.Language == "en").Name,

                            },
                            UnitAmount = (long)(item.Product.Price * 100),
                        },
                        Quantity = item.Count,
                    });
                }
                var order = new Order()
                {
                    UserId = userId,
                    PaymentMethod = request.PaymentMethod,
                    City = city,
                    Street = street,
                    PhoneNumber = phoneNumber,
                    AmoundPaid = cartItems.Sum(c => c.Count * c.Product.Price),
                    OrderItems = cartItems.Select(c => new OrderItem
                    {
                        ProductId = c.ProductId,
                        Quantity = c.Count,
                        UnitPrice = c.Product.Price,
                        TotalPrice = c.Count * c.Product.Price
                    }).ToList()
                };

                await _orderRepository.CreateAsync(order);


                var service = new Stripe.Checkout.SessionService();
                var session = service.Create(options);
                order.StipeSessionId = session.Id;
                await _orderRepository.UpdateAsync(order);

                return new CheckoutResponse
                {
                    Success = true,
                    StriprUrl = session.Url
                };
            }

            return new CheckoutResponse
            {
                Success = false,
                ErrorMessage = "Invalid payment method."
            };
        }


        public async Task<CheckoutResponse> HanldeSuccess(string sessionId)
        {
            var order = await _orderRepository.GetOne(o => o.StipeSessionId == sessionId,
                includes: new[] {
                          nameof(Order.OrderItems),
                        $"{nameof(Order.OrderItems)}.{nameof(OrderItem.Product)}",
                         $"{nameof(Order.OrderItems)}.{nameof(OrderItem.Product)}.{nameof(Product.Translations)}"

                }
                );
            order.OrderStatus = OrderStatusEnum.Paid;
            await _orderRepository.UpdateAsync(order);

            await _cartService.ClearCart(order.UserId);
            foreach (var item in order.OrderItems) 
            {
                var isLowQuantity = await _productRepository.DecreaseQuantityAsync(item.ProductId, item.Quantity);
                if (isLowQuantity)
                {
                    // Handle low quantity scenario, e.g., notify admin or user
                }
            }

            return new CheckoutResponse
            {
                Success = true,
                OrderId = order.Id
            };

        }
    }
}