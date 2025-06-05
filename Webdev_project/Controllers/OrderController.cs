using AspNetCoreGeneratedDocument;
using Microsoft.AspNetCore.Mvc;
using Webdev_project.Interfaces;

namespace Webdev_project.Controllers
{
    public class OrderController : BaseController
    {
        private readonly IAuthenticationRepository authenticationRepository;
        private readonly IProductRepository productRepository;
        private readonly IOrderRepository orderRepository;
        public OrderController(IAuthenticationRepository authenticationRepository, IProductRepository productRepository, IOrderRepository orderRepository) : base(authenticationRepository)
        {
            this.authenticationRepository = authenticationRepository;
            this.productRepository = productRepository;
            this.orderRepository = orderRepository;
        }
        public async Task<IActionResult> Detail(string id)
        {
            var user = authenticationRepository.RetrieveFromSession(HttpContext.Request.Cookies["sessionId"]);
            if (user == null) 
            {
                return RedirectToAction("MyLogin", "Authenticate");
            }
          
             
            var order = await orderRepository.GetOrderByIdAsync(id);
            //var order = await orderRepository.GetOrderByIdAsync("683753ca1d53f57b6a0d1280");
            
            ViewBag.OrderId = id;
            ViewBag.Order = order;
            return View();
        }
    }
}
