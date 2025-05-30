using Microsoft.AspNetCore.Mvc;
using Webdev_project.Interfaces;

namespace Webdev_project.Controllers
{
    public class OrderController : Controller
    {
        private readonly IUserRepository userRepository;
        private readonly ISessionRepository sessionRepository;
        private readonly IProductRepository productRepository;
        private readonly IOrderRepository orderRepository;
        public OrderController(IUserRepository userRepository, ISessionRepository sessionRepository, IProductRepository productRepository, IOrderRepository orderRepository)
        {
            this.userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            this.sessionRepository = sessionRepository ?? throw new ArgumentNullException(nameof(sessionRepository));
            this.productRepository = productRepository;
            this.orderRepository = orderRepository;
        }
        public async Task<IActionResult> Index()
        {
            var user = sessionRepository.RetrieveFromSession(HttpContext.Request.Cookies["sessionId"]);
            List<Order> orders = await orderRepository.GetOrdersByUserAsync(user.Id.ToString());
             

            ViewBag.Orders = orders;
            return View();
        }
    }
}
