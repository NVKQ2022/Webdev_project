using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using MongoDB.Bson; 
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.RegularExpressions; 
using Webdev_project.Models; 

[Route("api/[controller]")]
[ApiController]
public class SearchController : ControllerBase
{
    private readonly IMongoCollection<Product> _productsCollection;

    public SearchController(IMongoCollection<Product> productsCollection)
    {
        _productsCollection = productsCollection;
    }

    public class ProductSuggestionDto
    {
        public string ProductID { get; set; }
        public string Name { get; set; }
        public string Price { get; set; } 
        public string ImageURL { get; set; }
        public string URL { get; set; } 
    }

    [HttpGet("suggestions")] // Route sẽ là: /api/Search/suggestions?query=abc
    public async Task<IActionResult> GetSuggestions([FromQuery] string query)
    {
        if (string.IsNullOrWhiteSpace(query) || query.Length < 2)
        {
            return Ok(new List<ProductSuggestionDto>());
        }
        var filter = Builders<Product>.Filter.Regex(p => p.Name, new BsonRegularExpression(new Regex(query, RegexOptions.IgnoreCase | RegexOptions.Multiline)));

        // Lấy tối đa 7 kết quả gợi ý
        var productsFromDb = await _productsCollection.Find(filter)
                                                     .Limit(7)
                                                     .Project(p => new Product
                                                     {
                                                         ProductID = p.ProductID,
                                                         Name = p.Name,
                                                         Price = p.Price,
                                                         ImageURL = p.ImageURL
                                                     })
                                                     .ToListAsync();

        var suggestions = productsFromDb.Select(p => new ProductSuggestionDto
        {
            ProductID = p.ProductID,
            Name = p.Name,
            Price = $"{p.Price:N0}₫", 
            ImageURL = p.ImageURL?.FirstOrDefault(), // Lấy ảnh đầu tiên nếu có
            URL = Url.Action("Details", "Product", new { id = p.ProductID }) ?? $"/Product/Details/{p.ProductID}"
        }).ToList();

        return Ok(suggestions);
    }
}