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

        [HttpPost]
        public async Task<IActionResult> CancelOrder([FromBody] CancelOrderRequest request)
        {
            try
            {
                // Kiểm tra user đăng nhập
                var user = authenticationRepository.RetrieveFromSession(HttpContext.Request.Cookies["sessionId"]);
                if (user == null)
                {
                    return Unauthorized(new { success = false, message = "Vui lòng đăng nhập để tiếp tục" });
                }

                // Lấy thông tin đơn hàng
                var order = await orderRepository.GetOrderByIdAsync(request.OrderId);
                if (order == null)
                {
                    return NotFound(new { success = false, message = "Không tìm thấy đơn hàng" });
                }

                // Kiểm tra xem đơn hàng có phải của user này không
                if (order.UserID != user.Id)
                {
                    return Unauthorized(new { success = false, message = "Bạn không có quyền hủy đơn hàng này" });
                }

                // Kiểm tra trạng thái đơn hàng
                if (order.Status != "Pending")
                {
                    return BadRequest(new { success = false, message = "Chỉ có thể hủy đơn hàng ở trạng thái chờ xử lý" });
                }

                // Cập nhật trạng thái đơn hàng
                var success = await orderRepository.UpdateOrderStatusAsync(request.OrderId, "Cancelled");
                if (!success)
                {
                    return StatusCode(500, new { success = false, message = "Không thể hủy đơn hàng. Vui lòng thử lại sau" });
                }

                return Json(new { success = true, message = "Hủy đơn hàng thành công" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Có lỗi xảy ra: " + ex.Message });
            }
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

    public class CancelOrderRequest
    {
        public string OrderId { get; set; }
    }
}
