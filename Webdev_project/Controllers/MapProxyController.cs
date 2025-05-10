using Microsoft.AspNetCore.Mvc;

namespace Webdev_project.Controllers
{
    public class MapProxyController : Controller
    {
        private readonly IConfiguration _config;  // Interface dùng để truy xuất cấu hình ứng dụng.

        // Constructor injection: ASP.NET Core sẽ tự động cung cấp IConfiguration qua DI container.
        public MapProxyController(IConfiguration config)
        {
            _config = config;
        }

        public IActionResult Index()
        {
            return View();
        }
        // Định nghĩa HTTP GET tại đường dẫn /api/map/embed-url
        [HttpGet("embed-url")]
        public IActionResult GetEmbedUrl([FromQuery] string location = "University+of+Information+Technology+Vietnam") //đặt địa chỉ mặc định là UIT
        {
            // Lấy API key từ cấu hình, tránh hardcode
            var apiKey = _config["GoogleMaps:ApiKey"];

            // Kiểm tra tham số 'location' đầu vào, nếu rỗng thì trả về lỗi BadRequest (HTTP 400)
            if (string.IsNullOrWhiteSpace(location))
                return BadRequest("Location is required");

            // Xây dựng URL nhúng Google Maps Embed API với key bảo mật
            var url = $"https://www.google.com/maps/embed/v1/place?key={apiKey}&q={location}";

            // Trả về đối tượng JSON chứa URL, client sẽ parse và gán vào iframe
            return Ok(new { url });
        }
    }
}
