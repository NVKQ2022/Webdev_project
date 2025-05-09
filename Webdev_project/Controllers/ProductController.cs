using Microsoft.AspNetCore.Mvc;
using Webdev_project.Models;

namespace Webdev_project.Controllers
{
    public class ProductController : Controller
    {
        [HttpGet]
        public IActionResult Detail()
        {
            var product = new Product()
            {
                Name = "Laptop Asus Vivobook 15 OLED A1505VA",
                Description = "tu asus",
                Price = 18000000,
                Color = new List<string>
                {
                    "Đỏ", "Cam", "Vàng", "Lục", "Lam", "Chàm", "Tím", "Đen", "Trắng"
                },
                ImageURL = new List<string>
                {
                    "https://phucanhcdn.com/media/product/55711_laptop_asus_vivobook_15_oled_a1505va_ma469w_6.jpg",
                    "https://cdn.tgdd.vn/Products/Images/44/306196/asus-vivobook-15-oled-a1505va-i9-l1201w-thumb-1-600x600.jpg",
                    "https://cdn.tgdd.vn/Products/Images/44/306196/Slider/vi-vn-asus-vivobook-15-oled-a1505va-i9-l1201w-slider-1.jpg",
                    "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcT_6iLj3MAYpKqbw-7cct5cY3x-jwjnUOcmhQ&s",
                    "https://bizweb.dktcdn.net/thumb/1024x1024/100/329/122/products/laptop-asus-vivobook-15-oled-a1505va-ma469w-01.jpg?v=1743907839337",
                    "https://cdn.tgdd.vn/Products/Images/44/310839/asus-vivobook-15-oled-a1505va-i5-l1341w-7-750x500.jpg",
                    "https://cdn.tgdd.vn/Products/Images/44/310839/asus-vivobook-15-oled-a1505va-i5-l1341w-12-750x500.jpg"

                },
                Detail = new Dictionary<string, string>
                {
                    {"Kho", "7"},
                    {"Thương hiệu", "Asus"},
                    {"Kích thước màn hình laptop", "14 Inches"},
                    {"Loại laptop", "Ultrabook"},
                    {"Nhà sản xuất chip đồ họa", "Integrated"},
                    {"Dung lượng lưu trữ", "512GB"},
                    {"Loại lưu trữ", "SSD"},
                    {"Cổng/ Giao diện", "HDMI, USB 3.0, USB Type-C, 3.5mm"},
                    {"Hệ điều hành", "Windows"},
                    {"Tình trạng", "Mới"},
                    {"Hạn bảo hành", "24 tháng"},
                    {"Loại bảo hành", "Bảo hành nhà sản xuất"},
                    {"Tích hợp pin", "Có"},
                    {"Tần số CPU", "4.7Ghz"},
                    {"Bộ xử lý", "Intel Core i5-13500H"},
                    {"Trọng lượng", "1.6kg"},
                    {"Gửi từ", "Hà Nội"}
                }
            };

            ViewBag.Product = product;
            return View();
        }// need to interwith mongodb

    }
}
