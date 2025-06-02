

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
        private readonly IUserRepository _userRepository;
        private readonly ICartRepository _cartRepository;
        private readonly IProductRepository _productRepository;

        public async Task<IActionResult> Index()
        {
            var userIdStr = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdStr))
            {
                // Chưa đăng nhập, chuyển hướng về trang đăng nhập
                return RedirectToAction("MyLogin", "Authenticate");
            }
            int userId = int.Parse(userIdStr);

            var user = _userRepository.GetUserById(userId);
            ViewBag.User = user;

            var cart = await _cartRepository.GetCartByUserIdAsync(userId) ?? new Cart { UserId = userId };
            // KHÔNG gán cart.Id = null; hoặc cart.Id = "";

            return View(cart?.Items ?? new List<CartItem>());
        }


        [HttpPost]
        public async Task<IActionResult> Increase(string productId)
        {
            int userId = int.Parse(HttpContext.Session.GetString("UserId"));
            var cart = await _cartRepository.GetCartByUserIdAsync(userId);
            var item = cart?.Items.FirstOrDefault(x => x.ProductId == productId);
            if (item != null)
            {
                item.Quantity++;
                await _cartRepository.AddOrUpdateCartAsync(cart);
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Decrease(string productId)
        {
            int userId = int.Parse(HttpContext.Session.GetString("UserId"));
            var cart = await _cartRepository.GetCartByUserIdAsync(userId);
            var item = cart?.Items.FirstOrDefault(x => x.ProductId == productId);
            if (item != null)
            {
                if (item.Quantity > 1)
                    item.Quantity--;
                else
                    cart.Items.Remove(item);

                await _cartRepository.AddOrUpdateCartAsync(cart);
            }
            return RedirectToAction("Index");
        }


        [HttpPost]
        public async Task<IActionResult> Remove(string productId)
        {
            int userId = int.Parse(HttpContext.Session.GetString("UserId"));
            var cart = await _cartRepository.GetCartByUserIdAsync(userId);
            cart?.Items.RemoveAll(x => x.ProductId == productId);
            if (cart != null)
                await _cartRepository.AddOrUpdateCartAsync(cart);
            return RedirectToAction("Index");
        }

        // ShoppingCartController.cs
        [HttpPost]
        public async Task<IActionResult> AddToCart(string productId)
        {
            int userId = int.Parse(HttpContext.Session.GetString("UserId"));
            var cart = await _cartRepository.GetCartByUserIdAsync(userId) ?? new Cart { UserId = userId };

            var product = await _productRepository.GetByIdAsync(productId);
            if (product == null) return NotFound();

            var existingItem = cart.Items.FirstOrDefault(x => x.ProductId == product.ProductID);
            if (existingItem != null)
                existingItem.Quantity++;
            else
                cart.Items.Add(new CartItem
                {
                    ProductId = product.ProductID,
                    ProductName = product.Name,
                    ImageUrl = product.ImageURL?.FirstOrDefault() ?? "",
                    Price = product.Price,
                    Quantity = 1
                });

            await _cartRepository.AddOrUpdateCartAsync(cart);
            return RedirectToAction("Index");
        }

        public ShoppingCartController(
        IUserRepository userRepository,
        ICartRepository cartRepository,
        IProductRepository productRepository)
        {
            _userRepository = userRepository;
            _cartRepository = cartRepository;
            _productRepository = productRepository;
        }

    }
}



