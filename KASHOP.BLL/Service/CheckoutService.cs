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
        private readonly IEmailSender _emailSender;


        public CheckoutService(ICartRepository cartRepository, UserManager<ApplicationUser> userManager,
            IHttpContextAccessor httpContextAccessor,
                IOrderRepository orderRepository,
                ICartService cartService
                , IProductRepository productRepository
                , IEmailSender emailSender)
        {
            _cartRepository = cartRepository;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
            _orderRepository = orderRepository;
            _cartService = cartService;
            _productRepository = productRepository;
            _emailSender = emailSender;
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
                var order = new Order()
                {
                    UserId = userId,
                    PaymentMethod = request.PaymentMethod,
                    City = city,
                    Street = street,
                    PhoneNumber = phoneNumber,
                    OrderStatus = OrderStatusEnum.Paid, 
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

                
                foreach (var item in cartItems)
                {
                    await _productRepository.DecreaseQuantityAsync(new List<OrderItem> { new OrderItem { ProductId = item.ProductId, Quantity = item.Count } }); //here i think there is a error
                }

                
                await _cartService.ClearCart(userId);

                return new CheckoutResponse
                {
                    Success = true,
                    OrderId = order.Id
                };

            }
            if (request.PaymentMethod == PaymentMethodEnum.Visa)
            {
                var options = new Stripe.Checkout.SessionCreateOptions
                {
                    PaymentMethodTypes = new List<string> { "card" },
                    Mode = "payment",
                    SuccessUrl = $"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}/api/checkout/success?sessionId={{CHECKOUT_SESSION_ID}}",
                    CancelUrl = $"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}/api/checkout/cancel",
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

            var user = await _userManager.FindByIdAsync(order.UserId);
            await _emailSender.SendEmailAsync(user.Email, "Order Confirmation", $"Your order with ID {order.Id} has been successfully placed.");


            var LowStockProducts = await _productRepository.DecreaseQuantityAsync(order.OrderItems);


            foreach (var item in LowStockProducts) 
            {
                if(LowStockProducts != null)
                {
                    await _emailSender.SendEmailAsync($"rakan.sameer1@gmail.com",
                    "Low stock alert",
                    $"<h2>Product {item.Translations.FirstOrDefault(t=>t.Language=="en").Name} current quantity : {item.Quantity}</h2>");
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