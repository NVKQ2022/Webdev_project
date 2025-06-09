using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using MongoDB.Bson; 
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.RegularExpressions; 
using Webdev_project.Models;
using Webdev_project.Interfaces;

[Route("api/[controller]")]
[ApiController]
public class SearchController : ControllerBase
{
    //private readonly IMongoCollection<Product> _productsCollection;
    private readonly IProductRepository productRepository;
    private readonly IUserDetailRepository userDetailRepository;
    public SearchController( IProductRepository productRepository, IUserDetailRepository userDetailRepository)
    {
        //    _productsCollection = productsCollection;
        this.productRepository = productRepository;
        this.userDetailRepository = userDetailRepository;
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
         var productsFromDb= await productRepository.GetSuggestions(query);




        var suggestions = productsFromDb.Select(p => new ProductSuggestionDto
        {
            ProductID = p.ProductId,
            Name = p.Name,
            Price = $"{p.Price:N0}₫", 
            ImageURL = p.ImageURL?.FirstOrDefault(), // Lấy ảnh đầu tiên nếu có
            URL = Url.Action("Detail", "Product", new { id = p.ProductId }) ?? $"/Product/Detail/{p.ProductId}"
        }).ToList();

        
        return Ok(suggestions);
    }
}