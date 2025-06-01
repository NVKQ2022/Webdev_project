

using Microsoft.AspNetCore.Mvc;
using Webdev_project.Models;
using Webdev_project.Helpers;
using System.Collections.Generic;
using System.Linq;
using Webdev_project.Interfaces;

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
            if (item != null)
            {
                item.Quantity++;
                HttpContext.Session.SetObjectAsJson("Cart", cart);
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Decrease(string productId)
        {
            var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart") ?? new List<CartItem>();
            var item = cart.FirstOrDefault(x => x.ProductId == productId);
            if (item != null)
            {
                if (item.Quantity > 1)
                {
                    item.Quantity--;
                }
                else
                {
                    cart.Remove(item);
                }
                HttpContext.Session.SetObjectAsJson("Cart", cart);
            }
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

        // ShoppingCartController.cs
        [HttpPost]
        public async Task<IActionResult> AddToCart(string productId)
        {
            // Lấy sản phẩm từ MongoDB
            var product = await _productRepository.GetByIdAsync(productId);
            if (product == null)
            {
                return NotFound();
            }

            // Lấy giỏ hàng hiện tại
            var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart") ?? new List<CartItem>();

            // Kiểm tra sản phẩm đã có trong giỏ chưa
            var existingItem = cart.FirstOrDefault(x => x.ProductId == product.ProductID);
            if (existingItem != null)
            {
                existingItem.Quantity++;
            }
            else
            {
                cart.Add(new CartItem
                {
                    ProductId = product.ProductID,
                    ProductName = product.Name,
                    ImageUrl = product.ImageURL?.FirstOrDefault() ?? "",
                    Price = product.Price,
                    Quantity = 1
                });
            }

            HttpContext.Session.SetObjectAsJson("Cart", cart);

            // Chuyển hướng về trang giỏ hàng
            return RedirectToAction("Index", "ShoppingCart");
        }

        private readonly IProductRepository _productRepository;

        public ShoppingCartController(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }
    }
}



