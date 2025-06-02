using Microsoft.AspNetCore.Mvc;
using Webdev_project.Interfaces;

namespace Webdev_project.Controllers
{
    public class OrderController : Controller
    {
        private readonly IAuthenticationRepository authenticationRepository;
        private readonly IProductRepository productRepository;
        private readonly IOrderRepository orderRepository;
        public OrderController(IAuthenticationRepository authenticationRepository, IProductRepository productRepository, IOrderRepository orderRepository)
        {
            this.authenticationRepository = authenticationRepository;
            this.productRepository = productRepository;
            this.orderRepository = orderRepository;
        }
        public async Task<IActionResult> MyOrders()
        {
            var user = authenticationRepository.RetrieveFromSession(HttpContext.Request.Cookies["sessionId"]);
            List<Order> orders = await orderRepository.GetOrdersByUserAsync(user.Id.ToString());
             

            ViewBag.Orders = orders;
            return View();
        }
    }
}
