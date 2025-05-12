
using Microsoft.AspNetCore.Mvc;

namespace Webdev_project.Controllers
{
    public class ShoppingCartController : Controller
    {
        public IActionResult Index()
        {
            // Ví dụ: Lấy dữ liệu giỏ hàng từ cơ sở dữ liệu hoặc session
            var cartItems = new List<string> { "Sản phẩm 1", "Sản phẩm 2", "Sản phẩm 3" };

            // Truyền dữ liệu sang view
            return View(cartItems);
        }
    }
}
