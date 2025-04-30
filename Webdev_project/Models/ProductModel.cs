//using Microsoft.AspNetCore.Mvc.RazorPages;
//using System.Collections.Generic;

//public class ProductModel : PageModel
//{
//    public Product Product { get; set; }

//    public void OnGet()
//    {
//        Product = new Product
//        {
//            Name = "Laptop Asus Vivobook 15 OLED A1505VA",
//            Decription = "tu asus",
//            Price = 18000000,
//            ImageURL = new List<string>
//            {
//                "https://phucanhcdn.com/media/product/55711_laptop_asus_vivobook_15_oled_a1505va_ma469w_6.jpg",
//                "https://cdn.tgdd.vn/Products/Images/44/306196/asus-vivobook-15-oled-a1505va-i9-l1201w-thumb-1-600x600.jpg",
//                "https://cdn.tgdd.vn/Products/Images/44/306196/Slider/vi-vn-asus-vivobook-15-oled-a1505va-i9-l1201w-slider-1.jpg"
//            },
//            Detail = new Dictionary<string, string>
//            {
//                {"Kho", "7"},
//                {"Thương hiệu", "Asus"},
//                {"Kích thước màn hình laptop", "14 Inches"},
//                {"Loại laptop", "Ultrabook"},
//                {"Nhà sản xuất chip đồ họa", "Integrated"},
//                {"Dung lượng lưu trữ", "512GB"},
//                {"Loại lưu trữ", "SSD"},
//                {"Cổng/ Giao diện", "HDMI, USB 3.0, USB Type-C, 3.5mm"},
//                {"Hệ điều hành", "Windows"},
//                {"Tình trạng", "Mới"},
//                {"Hạn bảo hành", "24 tháng"},
//                {"Loại bảo hành", "Bảo hành nhà sản xuất"},
//                {"Tích hợp pin", "Có"},
//                {"Tần số CPU", "4.7Ghz"},
//                {"Bộ xử lý", "Intel Core i5-13500H"},
//                {"Trọng lượng", "1.6kg"},
//                {"Gửi từ", "Hà Nội"}
//            }
//        };
//    }
//    public class Product
//    {
//        public string Name { get; set; }
//        public string Decription { get; set; }
//        public int Price { get; set; }
//        public List<string> ImageURL { get; set; }
//        public Dictionary<string, string> Detail { get; set; }
//    }
//}
