using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption;
using Microsoft.AspNetCore.Mvc;
using Webdev_project.Helpers;
using Webdev_project.Models;
using Webdev_project.Interfaces;
using System.Text.Json;
using MongoDB.Bson.IO;
using Newtonsoft.Json;
using MongoDB.Bson;
using System.Text.Json.Serialization;

namespace Webdev_project.Controllers
{
    public class PaymentController : Controller
    {
        private readonly IAuthenticationRepository authenticationRepository;
        private readonly IUserDetailRepository userDetailRepository; 
        public PaymentController(IAuthenticationRepository authenticationRepository, IUserDetailRepository userDetailRepository)
        {
            this.authenticationRepository = authenticationRepository;
            this.userDetailRepository = userDetailRepository;
        }

        [HttpPost]
        public async Task<IActionResult> BuyItems(string selectedItems)
        {
            var user = authenticationRepository.RetrieveFromSession(HttpContext.Request.Cookies["SessionId"]);
            if (user == null)
            {
                return RedirectToAction("MyLogin", "Authenticate");
            }

            var items = Newtonsoft.Json.JsonConvert.DeserializeObject<List<CartItem>>(selectedItems);
            var address = await userDetailRepository.GetReceiveInfoAsync(user.Id);

            // Calculate total amount
            var total = items.Sum(item => item.UnitPrice * item.Quantity);
            ViewBag.TotalAmount = total;
            ViewBag.User = user;
            ViewBag.ReceiveInfos = address;

            return View("Index", items);
        }

        [HttpPost]
        public async Task<IActionResult> PaymentResult([FromBody] OrderData orderData)
        {
            try
            {
                var user = authenticationRepository.RetrieveFromSession(HttpContext.Request.Cookies["SessionId"]);
                if (user == null)
                {
                    return Unauthorized(new { message = "Vui lòng đăng nhập để tiếp tục" });
                }

                // Validate order data
                if (orderData == null || orderData.Items == null || !orderData.Items.Any())
                {
                    return BadRequest(new { message = "Dữ liệu đơn hàng không hợp lệ" });
                }

                // Create order document for MongoDB
                var order = new Order
                {
                    _id = null,
                    UserID = user.Id,
                    Items = orderData.Items,
                    TotalAmount = orderData.TotalAmount,
                    PaymentMethod = orderData.PaymentMethod,
                    Status = "Pending",
                    CreatedAt = new { date = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.000Z") },
                    ReceiveInfo = orderData.ReceiveInfo
                };

                // Here you would save the order to MongoDB
                // await _orderRepository.CreateOrder(order);

                return Json(new { 
                    success = true,
                    redirectUrl = Url.Action("PaymentResult", "Payment", new { 
                        paymentMethod = orderData.PaymentMethod,
                        totalAmount = orderData.TotalAmount
                    })
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Có lỗi xảy ra khi xử lý đơn hàng", error = ex.Message });
            }
        }

        public class Order
        {
            public string _id { get; set; }
            public int UserID { get; set; }
            public List<OrderItem> Items { get; set; }
            public decimal TotalAmount { get; set; }
            public string PaymentMethod { get; set; }
            public string Status { get; set; }
            public object CreatedAt { get; set; }
            public ReceiveInfo ReceiveInfo { get; set; }
        }

        public class OrderItem
        {
            public string ProductID { get; set; }
            public string ProductName { get; set; }
            public string Image { get; set; }
            public int Quantity { get; set; }
            public decimal UnitPrice { get; set; }
        }

        public class ReceiveInfo
        {
            public string Name { get; set; }
            public string Phone { get; set; }
            public string Address { get; set; }
        }

        public class OrderData
        {
            public List<OrderItem> Items { get; set; }
            public decimal TotalAmount { get; set; }
            public string PaymentMethod { get; set; }
            public ReceiveInfo ReceiveInfo { get; set; }
        }
    }
}
