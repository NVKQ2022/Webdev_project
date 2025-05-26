

using Microsoft.AspNetCore.Mvc;
using Webdev_project.Models;
using Webdev_project.Helpers;
using System.Collections.Generic;
using System.Linq;

namespace Webdev_project.Controllers
{
    public class ShoppingCartController : Controller
    {
        public IActionResult Index()
        {
            var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart");
            if (cart == null || cart.Count == 0)
            {
                // Thêm dữ liệu test nếu giỏ hàng rỗng
                cart = new List<CartItem>
        {
            new CartItem
            {
                ProductId = "1",
                ProductName = "Tai nghe Bluetooth",
                ImageUrl = "https://phukiengiare.com/images/detailed/63/tai-nghe-bluetooth-baseus-w04-pro-1.jpg",
                Price = 19000,
                Quantity = 1
            },
            new CartItem
            {
                ProductId = "2",
                ProductName = "Áo thun Shopee",
                ImageUrl = "https://th.bing.com/th/id/R.d5650f1b99876ba4f12cc3e246dd6c30?rik=aUA6PMpxrEryxQ&pid=ImgRaw&r=0",
                Price = 99000,
                Quantity = 2
            }
        };
                HttpContext.Session.SetObjectAsJson("Cart", cart);
            }
            return View(cart);
        }

        [HttpPost]
        public IActionResult Increase(string productId)
        {
            var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart") ?? new List<CartItem>();
            var item = cart.FirstOrDefault(x => x.ProductId == productId);
            if (item != null) item.Quantity++;
            HttpContext.Session.SetObjectAsJson("Cart", cart);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Decrease(string productId)
        {
            var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart") ?? new List<CartItem>();
            var item = cart.FirstOrDefault(x => x.ProductId == productId);
            if (item != null && item.Quantity > 1) item.Quantity--;
            HttpContext.Session.SetObjectAsJson("Cart", cart);
            return RedirectToAction("Index");
        }
        [HttpPost]
        public IActionResult Remove(string productId)
        {
            var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart") ?? new List<CartItem>();
            cart.RemoveAll(x => x.ProductId == productId);
            HttpContext.Session.SetObjectAsJson("Cart", cart);
            return RedirectToAction("Index");
        }

    }
}



