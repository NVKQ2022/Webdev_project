using MongoDB.Driver;
using MongoDB.Bson;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Webdev_project.Models;
using Webdev_project.Interfaces;

public class CategoryRepository : ICategoryRepository
{
    private readonly IMongoCollection<Product> _products;
    private readonly IMongoCollection<Category> _categories;

    public CategoryRepository(IOptions<MongoDbSettings> settings)
    {
        var client = new MongoClient(settings.Value.ConnectionString);
        var database = client.GetDatabase(settings.Value.DatabaseName);
        _products = database.GetCollection<Product>(settings.Value.ProductCollectionName);
        _categories = database.GetCollection<Category>(settings.Value.CategoryCollectionName);
    }

    public async Task InsertCategoryAsync(string categoryName)
    {
        // Check if category already exists
        var existing = await _categories.Find(c => c.CateName == categoryName).FirstOrDefaultAsync();
        if (existing != null)
        {
            // Category already exists, skip insertion
            return;
        }

        // Get products in this category
        var filter = Builders<Product>.Filter.Eq(p => p.Category, categoryName);
        var productsInCategory = await _products.Find(filter).ToListAsync();

        if (productsInCategory.Count == 0)
        {
            // No products with this category; do not insert empty category
            return;
        }

        // Calculate BuyTime and ProductQuantity
        int buyTime = productsInCategory.Sum(p => p.Sold);
        int quantity = productsInCategory.Count;
        var productIds = productsInCategory.Select(p => p.ProductID).ToList();

        // Create and insert the new Category
        var newCategory = new Category
        {
            CateName = categoryName,
            BuyTime = buyTime,
            ProductQuantity = quantity,
            Products = productIds
        };

        await _categories.InsertOneAsync(newCategory);
    }

}
