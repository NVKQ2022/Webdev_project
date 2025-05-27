using Microsoft.AspNetCore.Mvc;
using Webdev_project.Helpers;
using Webdev_project.Models;

namespace Webdev_project.Controllers
{
    public class PaymentController : Controller
    {
        [HttpPost]
        public IActionResult Index()
        {
            // Lấy giỏ hàng từ session
            var cartItems = HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart") ?? new List<CartItem>();

            // Tính tổng tiền đơn hàng
            var total = cartItems.Sum(item => item.Price * item.Quantity);
            ViewBag.TotalAmount = total;

            // Lấy user đang đăng nhập (giả lập ở đây)
            var currentUser = new User
            {
                Id = 1,
                Email = "user1@example.com",
                Username = "user1",
                IsAdmin = false
            };
            ViewBag.User = currentUser;

            return View(cartItems); // Truyền giỏ hàng sang view
        }

        [HttpPost]
        public ActionResult PaymentResult(string paymentMethod, decimal totalAmount)
        {
            ViewBag.PaymentMethod = paymentMethod;
            ViewBag.TotalAmount = totalAmount;
            return View();
        }


        [HttpPost]
        public ActionResult SubmitPayment(string paymentMethod, decimal totalAmount)
        {
            return RedirectToAction("PaymentResult", new {
    paymentMethod = "COD",
    totalAmount = totalAmount
});

        }

    }
}
